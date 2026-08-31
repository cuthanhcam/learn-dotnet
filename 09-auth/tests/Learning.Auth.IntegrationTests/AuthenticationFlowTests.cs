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

        AccessTokenResponse token = (await SignInAsync(email, "correct horse battery staple"))!;
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
        AccessTokenResponse token = (await SignInAsync(email, "correct horse battery staple"))!;
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

    private async Task<AccessTokenResponse?> SignInAsync(string email, string password)
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/auth/sign-in", new SignInRequest(email, password));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
    }
}
