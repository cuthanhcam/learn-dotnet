using System.Net;
using System.Net.Http.Json;
using Learning.Api.Features.Products;
using Learning.Api.Operations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Learning.Api.Tests;

public sealed class OperationalEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OperationalEndpointsTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task HealthyApplication_IsLiveAndReady()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage live = await client.GetAsync("/health/live");
        using HttpResponseMessage ready = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        Assert.Equal(HttpStatusCode.OK, ready.StatusCode);
        Assert.Contains("Healthy", await ready.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task FailedDependency_MakesReadinessUnhealthyButLeavesLivenessHealthy()
    {
        using WebApplicationFactory<Program> factory = WithUnavailableCatalog();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage live = await client.GetAsync("/health/live");
        using HttpResponseMessage ready = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, ready.StatusCode);
        Assert.DoesNotContain("database password", await ready.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task KnownDependencyFailure_BecomesSafeProblemDetails()
    {
        using WebApplicationFactory<Program> factory = WithUnavailableCatalog();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/api/products");
        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("The product catalog is temporarily unavailable.", problem!.Title);
        Assert.True(problem.Extensions.ContainsKey("traceId"));
        Assert.DoesNotContain("database password", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    private WebApplicationFactory<Program> WithUnavailableCatalog() =>
        _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IProductRepository>();
            services.AddSingleton<IProductRepository, UnavailableProductRepository>();
        }));

    private sealed class UnavailableProductRepository : IProductRepository
    {
        private static CatalogUnavailableException Failure() =>
            new("database password=must-never-cross-the-HTTP-boundary");

        public Task<(IReadOnlyList<Product> Items, int TotalCount)> PageAsync(
            int skip, int take, CancellationToken cancellationToken) => throw Failure();

        public Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken) => throw Failure();
        public Task<bool> TryAddAsync(Product product, CancellationToken cancellationToken) => throw Failure();
        public Task<ProductMutationResult> TryUpdateAsync(
            Product product, long expectedVersion, CancellationToken cancellationToken) => throw Failure();
        public Task<ProductMutationResult> TryDeleteAsync(
            Guid id, long expectedVersion, CancellationToken cancellationToken) => throw Failure();
    }
}
