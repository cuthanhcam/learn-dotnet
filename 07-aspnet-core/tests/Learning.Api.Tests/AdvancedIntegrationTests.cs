using System.Net;
using System.Net.Http.Json;
using Learning.Api.Features.Products;
using Learning.Api.Tests.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Learning.Api.Tests;

public sealed class AdvancedIntegrationTests
{
    [Fact]
    public async Task IsolatedFactory_UsesDeterministicClockAndTestingEnvironment()
    {
        await using var factory = new LearningApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage createdResponse = await client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("Deterministic Product", 10m));
        Product created = (await createdResponse.Content.ReadFromJsonAsync<Product>())!;
        using HttpResponseMessage openApi = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(factory.Clock.GetUtcNow(), created.CreatedAt);
        Assert.Equal(HttpStatusCode.NotFound, openApi.StatusCode);
    }

    [Theory]
    [InlineData("application/json", "{ not-valid-json", HttpStatusCode.BadRequest)]
    [InlineData("text/plain", "plain text", HttpStatusCode.UnsupportedMediaType)]
    public async Task BodyBoundary_RejectsMalformedOrUnsupportedRepresentations(
        string mediaType,
        string body,
        HttpStatusCode expected)
    {
        await using var factory = new LearningApiFactory();
        using HttpClient client = factory.CreateClient();
        using var content = new StringContent(body);
        content.Headers.ContentType = new(mediaType);

        using HttpResponseMessage response = await client.PostAsync("/api/products", content);

        Assert.Equal(expected, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task RoutingBoundary_DistinguishesConstraintAndMethodFailures()
    {
        await using var factory = new LearningApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage invalidRoute = await client.GetAsync("/api/products/not-a-guid");
        using HttpResponseMessage wrongMethod = await client.PatchAsync(
            $"/api/products/{Guid.NewGuid()}",
            JsonContent.Create(new { }));

        Assert.Equal(HttpStatusCode.NotFound, invalidRoute.StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, wrongMethod.StatusCode);
        Assert.Contains("GET", wrongMethod.Content.Headers.Allow);
    }
}
