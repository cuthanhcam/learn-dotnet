using Learning.Api.Features.Products;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Learning.Api.Tests;

public sealed class CancellationPropagationTests
{
    [Fact]
    public async Task AbortedHttpRequest_CancelsRepositoryOperation()
    {
        var repository = new CancellationAwareRepository();
        await using WebApplicationFactory<Program> factory =
            new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IProductRepository>();
                    services.AddSingleton<IProductRepository>(repository);
                }));
        using HttpClient client = factory.CreateClient();
        using var cancellation = new CancellationTokenSource();

        Task<HttpResponseMessage> request = client.GetAsync(
            "/api/products?page=1&pageSize=20",
            cancellation.Token);
        await repository.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => request);
        await repository.CancellationObserved.Task.WaitAsync(TimeSpan.FromSeconds(5));
    }

    private sealed class CancellationAwareRepository : IProductRepository
    {
        public TaskCompletionSource Started { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource CancellationObserved { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<(IReadOnlyList<Product> Items, int TotalCount)> PageAsync(
            int skip,
            int take,
            CancellationToken cancellationToken)
        {
            Started.TrySetResult();
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                CancellationObserved.TrySetResult();
                throw;
            }

            throw new InvalidOperationException("The infinite delay completed without cancellation.");
        }

        public Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<bool> TryAddAsync(Product product, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<ProductMutationResult> TryUpdateAsync(
            Product product, long expectedVersion, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
        public Task<ProductMutationResult> TryDeleteAsync(
            Guid id, long expectedVersion, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
