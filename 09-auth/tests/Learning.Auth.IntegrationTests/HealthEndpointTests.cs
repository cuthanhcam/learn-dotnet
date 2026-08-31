using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Learning.Auth.IntegrationTests;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsSuccess()
    {
        using HttpResponseMessage response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
