using Learning.Api.Features.Products;

namespace Learning.Api.Tests;

public sealed class InMemoryProductRepositoryTests
{
    private readonly InMemoryProductRepository _repository = new();

    [Fact]
    public async Task Update_ChangesNameIndexAtomically()
    {
        Product original = ProductNamed("Keyboard");
        Assert.True(await _repository.TryAddAsync(original, CancellationToken.None));

        Product renamed = original with { Name = "Ergonomic Keyboard", Version = 2 };
        ProductMutationResult result =
            await _repository.TryUpdateAsync(renamed, expectedVersion: 1, CancellationToken.None);

        Assert.Equal(ProductMutationStatus.Success, result.Status);
        Assert.True(await _repository.TryAddAsync(ProductNamed("Keyboard"), CancellationToken.None));
        Assert.False(await _repository.TryAddAsync(ProductNamed("ERGONOMIC KEYBOARD"), CancellationToken.None));
    }

    [Fact]
    public async Task Update_WithStaleVersionDoesNotChangeStoredProduct()
    {
        Product original = ProductNamed("Keyboard");
        await _repository.TryAddAsync(original, CancellationToken.None);

        Product attempted = original with { Price = 1m, Version = 2 };
        ProductMutationResult result =
            await _repository.TryUpdateAsync(attempted, expectedVersion: 99, CancellationToken.None);
        Product? stored = await _repository.FindAsync(original.Id, CancellationToken.None);

        Assert.Equal(ProductMutationStatus.VersionMismatch, result.Status);
        Assert.Equal(original, stored);
    }

    [Fact]
    public async Task Operations_HonorPreCancelledTokenBeforeChangingState()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.TryAddAsync(ProductNamed("Keyboard"), source.Token));
        (IReadOnlyList<Product> items, int count) =
            await _repository.PageAsync(0, 10, CancellationToken.None);

        Assert.Empty(items);
        Assert.Equal(0, count);
    }

    private static Product ProductNamed(string name)
    {
        DateTimeOffset now = DateTimeOffset.Parse("2026-08-15T00:00:00Z");
        return new Product(Guid.NewGuid(), name, 10m, now, now, Version: 1);
    }
}
