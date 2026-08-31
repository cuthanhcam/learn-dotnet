using System.Net;
using Learning.Auth.IntegrationTests.Infrastructure;

namespace Learning.Auth.IntegrationTests;

public sealed class HealthEndpointTests : IClassFixture<AuthApiFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(AuthApiFactory factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsSuccess()
    {
        using HttpResponseMessage response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
