using System.Collections.Concurrent;

namespace Learning.Api.Features.Products;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Product> snapshot = _products.Values
            .OrderBy(product => product.CreatedAt)
            .ThenBy(product => product.Id)
            .ToArray();
        return Task.FromResult(snapshot);
    }

    public Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _products.TryGetValue(id, out Product? product);
        return Task.FromResult(product);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(product);
        cancellationToken.ThrowIfCancellationRequested();
        if (!_products.TryAdd(product.Id, product))
        {
            throw new InvalidOperationException($"A product with ID '{product.Id}' already exists.");
        }

        return Task.CompletedTask;
    }
}
