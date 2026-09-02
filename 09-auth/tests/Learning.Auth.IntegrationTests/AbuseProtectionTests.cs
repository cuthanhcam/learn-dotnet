using System.Net;
using System.Net.Http.Json;
using Learning.Auth.IntegrationTests.Infrastructure;

namespace Learning.Auth.IntegrationTests;

public sealed class AbuseProtectionTests : IClassFixture<AuthApiFactory>
{
    private readonly HttpClient _client;

    public AbuseProtectionTests(AuthApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task SignIn_ExceedingNetworkBudgetReturnsProblemDetailsAndRetryAfter()
    {
        // A dedicated fixture isolates this limiter window from the functional authentication tests.
        for (int attempt = 0; attempt < 10; attempt++)
        {
            using HttpResponseMessage allowed = await _client.PostAsJsonAsync(
                "/auth/sign-in", new SignInRequest("absent@example.com", "wrong password"));
            Assert.Equal(HttpStatusCode.Unauthorized, allowed.StatusCode);
        }

        using HttpResponseMessage rejected = await _client.PostAsJsonAsync(
            "/auth/sign-in", new SignInRequest("absent@example.com", "wrong password"));
        string body = await rejected.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        Assert.True(rejected.Headers.RetryAfter is not null || rejected.Headers.Contains("Retry-After"));
        Assert.Contains("Too many authentication requests", body, StringComparison.Ordinal);
    }
}
