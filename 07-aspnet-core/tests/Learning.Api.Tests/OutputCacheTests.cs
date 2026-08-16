using System.Net.Http.Json;
using Learning.Api.Features.Products;
using Learning.Api.Middleware;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Learning.Api.Tests;

public sealed class OutputCacheTests
{
    [Fact]
    public async Task Collection_IsCachedButSuccessfulMutationEvictsTag()
    {
        var repository = new CountingProductRepository();
        await using WebApplicationFactory<Program> factory = CreateFactory(repository);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage first = await client.GetAsync("/api/products?page=1&pageSize=20");
        using HttpResponseMessage second = await client.GetAsync("/api/products?page=1&pageSize=20");
        Assert.Equal(1, repository.PageCalls);

        using HttpResponseMessage created = await client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("Cache Invalidation Example", 10m));
        created.EnsureSuccessStatusCode();

        using HttpResponseMessage afterMutation =
            await client.GetAsync("/api/products?page=1&pageSize=20");
        Assert.Equal(2, repository.PageCalls);
    }

    [Fact]
    public async Task CacheHit_StillReceivesFreshCorrelationIdentifier()
    {
        var repository = new CountingProductRepository();
        await using WebApplicationFactory<Program> factory = CreateFactory(repository);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage first = await client.GetAsync("/api/products?page=1&pageSize=20");
        using HttpResponseMessage second = await client.GetAsync("/api/products?page=1&pageSize=20");

        string firstId = first.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single();
        string secondId = second.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single();
        Assert.NotEqual(firstId, secondId);
        Assert.Equal(1, repository.PageCalls);
    }

    private static WebApplicationFactory<Program> CreateFactory(IProductRepository repository) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IProductRepository>();
                services.AddSingleton(repository);
            }));

    private sealed class CountingProductRepository : IProductRepository
    {
        private readonly InMemoryProductRepository _inner = new();
        private int _pageCalls;

        public int PageCalls => Volatile.Read(ref _pageCalls);

        public Task<(IReadOnlyList<Product> Items, int TotalCount)> PageAsync(
            int skip, int take, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _pageCalls);
            return _inner.PageAsync(skip, take, cancellationToken);
        }

        public Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken) =>
            _inner.FindAsync(id, cancellationToken);

        public Task<bool> TryAddAsync(Product product, CancellationToken cancellationToken) =>
            _inner.TryAddAsync(product, cancellationToken);

        public Task<ProductMutationResult> TryUpdateAsync(
            Product product, long expectedVersion, CancellationToken cancellationToken) =>
            _inner.TryUpdateAsync(product, expectedVersion, cancellationToken);

        public Task<ProductMutationResult> TryDeleteAsync(
            Guid id, long expectedVersion, CancellationToken cancellationToken) =>
            _inner.TryDeleteAsync(id, expectedVersion, cancellationToken);
    }
}
