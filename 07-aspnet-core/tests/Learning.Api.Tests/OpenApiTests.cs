using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Learning.Api.Tests;

public sealed class OpenApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OpenApiTests(WebApplicationFactory<Program> factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task JsonDocument_DescribesMinimalAndControllerEndpoints()
    {
        using HttpResponseMessage response = await _client.GetAsync("/openapi/v1.json");
        using JsonDocument document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.StartsWith("3.1", document.RootElement.GetProperty("openapi").GetString());
        JsonElement paths = document.RootElement.GetProperty("paths");
        Assert.True(paths.TryGetProperty("/api/products", out _));
        Assert.True(paths.TryGetProperty("/api/products/{id}", out _));
        Assert.True(paths.TryGetProperty("/api/order-quotes", out _));
    }

    [Fact]
    public async Task YamlDocument_UsesAspNetCore10Serialization()
    {
        using HttpResponseMessage response = await _client.GetAsync("/openapi/v1.yaml");
        string content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("openapi: '3.1", content, StringComparison.Ordinal);
        Assert.Contains("/api/products", content, StringComparison.Ordinal);
    }
}
