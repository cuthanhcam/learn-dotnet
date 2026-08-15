using System.Net;
using System.Net.Http.Json;
using Learning.Api.Features.Products;
using Learning.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Learning.Api.Tests;

public sealed class ProductApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsSuccessAndGeneratedCorrelationId()
    {
        using HttpResponseMessage response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out IEnumerable<string>? values));
        Assert.Single(values);
    }

    [Fact]
    public async Task CorrelationId_ValidCallerValueIsReturned()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add(CorrelationIdMiddleware.HeaderName, "request-123");

        using HttpResponseMessage response = await _client.SendAsync(request);

        Assert.Equal("request-123", response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single());
    }

    [Fact]
    public async Task CorrelationId_InvalidCallerValueIsNotReflected()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, "unsafe value");

        using HttpResponseMessage response = await _client.SendAsync(request);
        string returned = response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single();

        Assert.NotEqual("unsafe value", returned);
        Assert.Matches("^[a-f0-9]{32}$", returned);
    }

    [Fact]
    public async Task CreateThenGet_RoundTripsResourceAndLocation()
    {
        using HttpResponseMessage createdResponse = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("  Mechanical Keyboard  ", 129.99m));

        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        Product created = (await createdResponse.Content.ReadFromJsonAsync<Product>())!;
        Assert.Equal("Mechanical Keyboard", created.Name);
        Assert.Equal(129.99m, created.Price);
        Assert.NotNull(createdResponse.Headers.Location);

        Product? loaded = await _client.GetFromJsonAsync<Product>(createdResponse.Headers.Location);
        Assert.Equal(created, loaded);
    }

    [Fact]
    public async Task Create_InvalidRequestReturnsValidationProblemDetails()
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest(" ", 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        HttpValidationProblemDetails? problem =
            await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains(nameof(CreateProductRequest.Name), problem.Errors.Keys);
        Assert.Contains(nameof(CreateProductRequest.Price), problem.Errors.Keys);
    }

    [Fact]
    public async Task Get_UnknownProductReturnsNotFound()
    {
        using HttpResponseMessage response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
