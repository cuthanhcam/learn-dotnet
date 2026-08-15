using System.Net;
using System.Net.Http.Json;
using Learning.Api.Features.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Learning.Api.Tests;

public sealed class ProductLifecycleTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductLifecycleTests(WebApplicationFactory<Program> factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task Create_DuplicateNameIgnoringCaseReturnsConflict()
    {
        string uniqueName = $"Monitor-{Guid.NewGuid():N}";
        using HttpResponseMessage first = await CreateAsync(uniqueName);
        using HttpResponseMessage duplicate = await CreateAsync(uniqueName.ToUpperInvariant());

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        ProblemDetails? problem = await duplicate.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("A product with the same name already exists.", problem?.Title);
    }

    [Fact]
    public async Task List_UsesPaginationEnvelopeAndEnforcesConfiguredMaximum()
    {
        string uniqueName = $"Mouse-{Guid.NewGuid():N}";
        using HttpResponseMessage created = await CreateAsync(uniqueName);
        created.EnsureSuccessStatusCode();

        PagedResponse<Product>? page =
            await _client.GetFromJsonAsync<PagedResponse<Product>>("/api/products?page=1&pageSize=100");

        Assert.NotNull(page);
        Assert.Contains(page.Items, product => product.Name == uniqueName);
        Assert.Equal(1, page.Page);
        Assert.Equal(100, page.PageSize);
        Assert.True(page.TotalCount >= page.Items.Count);

        using HttpResponseMessage invalid = await _client.GetAsync("/api/products?page=1&pageSize=101");
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        HttpValidationProblemDetails? problem =
            await invalid.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.Contains("pageSize", problem!.Errors.Keys);
    }

    [Fact]
    public async Task Update_WithCurrentETagSucceedsAndStaleETagFails()
    {
        using HttpResponseMessage createdResponse = await CreateAsync($"Desk-{Guid.NewGuid():N}");
        Product created = (await createdResponse.Content.ReadFromJsonAsync<Product>())!;
        string originalTag = createdResponse.Headers.ETag!.Tag;

        using var update = new HttpRequestMessage(HttpMethod.Put, $"/api/products/{created.Id}")
        {
            Content = JsonContent.Create(new UpdateProductRequest("Standing Desk", 499m))
        };
        update.Headers.TryAddWithoutValidation("If-Match", originalTag);
        using HttpResponseMessage updatedResponse = await _client.SendAsync(update);

        Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);
        Product updated = (await updatedResponse.Content.ReadFromJsonAsync<Product>())!;
        Assert.Equal(created.Version + 1, updated.Version);
        Assert.Equal("Standing Desk", updated.Name);
        Assert.NotEqual(originalTag, updatedResponse.Headers.ETag!.Tag);

        // This represents a second client that still holds the old representation.
        using var staleUpdate = new HttpRequestMessage(HttpMethod.Put, $"/api/products/{created.Id}")
        {
            Content = JsonContent.Create(new UpdateProductRequest("Overwritten Desk", 1m))
        };
        staleUpdate.Headers.TryAddWithoutValidation("If-Match", originalTag);
        using HttpResponseMessage staleResponse = await _client.SendAsync(staleUpdate);

        Assert.Equal(HttpStatusCode.PreconditionFailed, staleResponse.StatusCode);
    }

    [Fact]
    public async Task Mutation_RequiresOneValidStrongIfMatchHeader()
    {
        using HttpResponseMessage createdResponse = await CreateAsync($"Headset-{Guid.NewGuid():N}");
        Product product = (await createdResponse.Content.ReadFromJsonAsync<Product>())!;

        using HttpResponseMessage missing = await _client.PutAsJsonAsync(
            $"/api/products/{product.Id}",
            new UpdateProductRequest(product.Name, product.Price));
        Assert.Equal((HttpStatusCode)428, missing.StatusCode);

        using var malformedRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/products/{product.Id}");
        malformedRequest.Headers.TryAddWithoutValidation("If-Match", "W/\"1\"");
        using HttpResponseMessage malformed = await _client.SendAsync(malformedRequest);
        Assert.Equal(HttpStatusCode.BadRequest, malformed.StatusCode);
    }

    [Fact]
    public async Task Delete_WithCurrentETagRemovesResource()
    {
        using HttpResponseMessage createdResponse = await CreateAsync($"Webcam-{Guid.NewGuid():N}");
        Product product = (await createdResponse.Content.ReadFromJsonAsync<Product>())!;

        using var delete = new HttpRequestMessage(HttpMethod.Delete, $"/api/products/{product.Id}");
        delete.Headers.TryAddWithoutValidation("If-Match", createdResponse.Headers.ETag!.Tag);
        using HttpResponseMessage deleted = await _client.SendAsync(delete);
        using HttpResponseMessage loaded = await _client.GetAsync($"/api/products/{product.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, loaded.StatusCode);
    }

    private Task<HttpResponseMessage> CreateAsync(string name) =>
        _client.PostAsJsonAsync("/api/products", new CreateProductRequest(name, 25m));
}
