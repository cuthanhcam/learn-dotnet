using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Users;
using Learning.Auth.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Learning.Auth.IntegrationTests;

public sealed class AuthorizationTests : IClassFixture<AuthApiFactory>
{
    private readonly AuthApiFactory _factory;
    private readonly HttpClient _client;

    public AuthorizationTests(AuthApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AdministratorPolicy_DistinguishesChallengeForbidAndSuccess()
    {
        using HttpResponseMessage anonymous = await _client.GetAsync("/auth/admin");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        string memberToken = await RegisterAndSignInAsync($"member-{Guid.NewGuid():N}@example.com");
        using HttpResponseMessage member = await SendAsync(HttpMethod.Get, "/auth/admin", memberToken);
        Assert.Equal(HttpStatusCode.Forbidden, member.StatusCode);

        using HttpResponseMessage administrator = await SendAsync(
            HttpMethod.Get, "/auth/admin", CreateAdministratorToken());
        Assert.Equal(HttpStatusCode.OK, administrator.StatusCode);
    }

    [Fact]
    public async Task DocumentAuthorization_UsesAuthoritativeOwnerPublishedAndAdministratorState()
    {
        string ownerToken = await RegisterAndSignInAsync($"owner-{Guid.NewGuid():N}@example.com");
        string otherToken = await RegisterAndSignInAsync($"other-{Guid.NewGuid():N}@example.com");

        using HttpResponseMessage created = await SendAsync(HttpMethod.Post, "/documents", ownerToken,
            new CreateDocumentRequest("Authorization notes"));
        DocumentResponse document = (await created.Content.ReadFromJsonAsync<DocumentResponse>())!;
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        using HttpResponseMessage draftDenied = await SendAsync(
            HttpMethod.Get, $"/documents/{document.Id}", otherToken);
        Assert.Equal(HttpStatusCode.Forbidden, draftDenied.StatusCode);

        using HttpResponseMessage updateDenied = await SendAsync(HttpMethod.Put,
            $"/documents/{document.Id}", otherToken, new UpdateDocumentRequest("Stolen edit"));
        Assert.Equal(HttpStatusCode.Forbidden, updateDenied.StatusCode);

        using HttpResponseMessage published = await SendAsync(HttpMethod.Post,
            $"/documents/{document.Id}/publish", ownerToken);
        Assert.Equal(HttpStatusCode.OK, published.StatusCode);

        using HttpResponseMessage publishedRead = await SendAsync(
            HttpMethod.Get, $"/documents/{document.Id}", otherToken);
        Assert.Equal(HttpStatusCode.OK, publishedRead.StatusCode);

        using HttpResponseMessage adminUpdate = await SendAsync(HttpMethod.Put,
            $"/documents/{document.Id}", CreateAdministratorToken(),
            new UpdateDocumentRequest("Administrator correction"));
        Assert.Equal(HttpStatusCode.OK, adminUpdate.StatusCode);
    }

    private async Task<string> RegisterAndSignInAsync(string email)
    {
        const string password = "correct horse battery staple";
        using HttpResponseMessage registration = await _client.PostAsJsonAsync(
            "/auth/register", new RegisterRequest(email, password));
        registration.EnsureSuccessStatusCode();
        using HttpResponseMessage signIn = await _client.PostAsJsonAsync(
            "/auth/sign-in", new SignInRequest(email, password));
        signIn.EnsureSuccessStatusCode();
        return (await signIn.Content.ReadFromJsonAsync<TokenResponse>())!.AccessToken;
    }

    private string CreateAdministratorToken()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        IAccessTokenIssuer issuer = scope.ServiceProvider.GetRequiredService<IAccessTokenIssuer>();
        UserAccount account = UserAccount.Register(Guid.NewGuid(),
            EmailAddress.Create($"administrator-{Guid.NewGuid():N}@example.com"),
            "not-used-by-this-token-focused-test", DateTimeOffset.UtcNow);
        account.GrantRole(RoleNames.Administrator);
        return issuer.Issue(account).Value;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, string token,
        object? body = null)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null)
            request.Content = JsonContent.Create(body);
        return await _client.SendAsync(request);
    }
}
