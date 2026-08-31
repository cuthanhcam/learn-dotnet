using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Learning.Auth.IntegrationTests.Infrastructure;

namespace Learning.Auth.IntegrationTests;

public sealed class AuthenticationFlowTests : IClassFixture<AuthApiFactory>
{
    private readonly HttpClient _client;

    public AuthenticationFlowTests(AuthApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task RegisterSignInAndMe_CompletesAuthenticatedFlow()
    {
        string email = $"learner-{Guid.NewGuid():N}@example.com";
        using HttpResponseMessage registration = await _client.PostAsJsonAsync(
            "/auth/register", new RegisterRequest(email, "correct horse battery staple"));
        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);

        TokenResponse token = (await SignInAsync(email, "correct horse battery staple"))!;
        using var request = new HttpRequestMessage(HttpMethod.Get, "/auth/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        using HttpResponseMessage response = await _client.SendAsync(request);
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(email, body, StringComparison.Ordinal);
        Assert.Contains("member", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Me_WithoutTokenReturnsChallenge() =>
        Assert.Equal(HttpStatusCode.Unauthorized, (await _client.GetAsync("/auth/me")).StatusCode);

    [Fact]
    public async Task Me_WithTamperedTokenReturnsChallenge()
    {
        string email = $"learner-{Guid.NewGuid():N}@example.com";
        await _client.PostAsJsonAsync("/auth/register", new RegisterRequest(email, "correct horse battery staple"));
        TokenResponse token = (await SignInAsync(email, "correct horse battery staple"))!;
        string tampered = token.AccessToken[..^1] + (token.AccessToken[^1] == 'a' ? 'b' : 'a');

        using var request = new HttpRequestMessage(HttpMethod.Get, "/auth/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tampered);
        using HttpResponseMessage response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_UnknownCredentialsUsesGenericFailure()
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/auth/sign-in", new SignInRequest("unknown@example.com", "correct horse battery staple"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("{\"message\":\"Invalid credentials.\"}", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Refresh_RotatesOnceAndReplayRevokesReplacementFamily()
    {
        string email = $"learner-{Guid.NewGuid():N}@example.com";
        await _client.PostAsJsonAsync("/auth/register", new RegisterRequest(email, "correct horse battery staple"));
        TokenResponse original = (await SignInAsync(email, "correct horse battery staple"))!;

        using HttpResponseMessage rotatedResponse = await _client.PostAsJsonAsync(
            "/auth/refresh", new RefreshTokenRequest(original.RefreshToken));
        TokenResponse? rotated = await rotatedResponse.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.Equal(HttpStatusCode.OK, rotatedResponse.StatusCode);
        Assert.NotNull(rotated);
        Assert.NotEqual(original.RefreshToken, rotated.RefreshToken);
        Assert.NotEqual(original.AccessToken, rotated.AccessToken);

        using HttpResponseMessage replay = await _client.PostAsJsonAsync(
            "/auth/refresh", new RefreshTokenRequest(original.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, replay.StatusCode);

        using HttpResponseMessage familyRevoked = await _client.PostAsJsonAsync(
            "/auth/refresh", new RefreshTokenRequest(rotated.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, familyRevoked.StatusCode);
    }

    [Fact]
    public async Task Revoke_IsIdempotentAndPreventsRefresh()
    {
        string email = $"learner-{Guid.NewGuid():N}@example.com";
        await _client.PostAsJsonAsync("/auth/register", new RegisterRequest(email, "correct horse battery staple"));
        TokenResponse tokens = (await SignInAsync(email, "correct horse battery staple"))!;

        using HttpResponseMessage first = await _client.PostAsJsonAsync(
            "/auth/revoke", new RefreshTokenRequest(tokens.RefreshToken));
        using HttpResponseMessage second = await _client.PostAsJsonAsync(
            "/auth/revoke", new RefreshTokenRequest(tokens.RefreshToken));
        using HttpResponseMessage refresh = await _client.PostAsJsonAsync(
            "/auth/refresh", new RefreshTokenRequest(tokens.RefreshToken));

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, second.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    private async Task<TokenResponse?> SignInAsync(string email, string password)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/auth/sign-in", new SignInRequest(email, password));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenResponse>();
    }
}
