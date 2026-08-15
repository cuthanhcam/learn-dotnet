namespace Learning.Api.Features.Products;

/// <summary>
/// A deterministic learning repository. One lock protects the product dictionary and the
/// case-insensitive name index as one compound invariant.
/// </summary>
public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly object _gate = new();
    private readonly Dictionary<Guid, Product> _products = [];
    private readonly Dictionary<string, Guid> _idByName = new(StringComparer.OrdinalIgnoreCase);

    public Task<(IReadOnlyList<Product> Items, int TotalCount)> PageAsync(
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(skip);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(take);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_gate)
        {
            // Materialize inside the lock so Count and Items describe one consistent snapshot.
            Product[] items = _products.Values
                .OrderBy(product => product.CreatedAt)
                .ThenBy(product => product.Id)
                .Skip(skip)
                .Take(take)
                .ToArray();
            return Task.FromResult(((IReadOnlyList<Product>)items, _products.Count));
        }
    }

    public Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            _products.TryGetValue(id, out Product? product);
            return Task.FromResult(product);
        }
    }

    public Task<bool> TryAddAsync(Product product, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(product);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_gate)
        {
            if (_products.ContainsKey(product.Id) || _idByName.ContainsKey(product.Name))
            {
                return Task.FromResult(false);
            }

            _products.Add(product.Id, product);
            _idByName.Add(product.Name, product.Id);
            return Task.FromResult(true);
        }
    }

    public Task<ProductMutationResult> TryUpdateAsync(
        Product product,
        long expectedVersion,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(product);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_gate)
        {
            if (!_products.TryGetValue(product.Id, out Product? current))
            {
                return Task.FromResult(new ProductMutationResult(ProductMutationStatus.NotFound));
            }

            if (current.Version != expectedVersion)
            {
                return Task.FromResult(new ProductMutationResult(ProductMutationStatus.VersionMismatch, current));
            }

            if (_idByName.TryGetValue(product.Name, out Guid ownerId) && ownerId != product.Id)
            {
                return Task.FromResult(new ProductMutationResult(ProductMutationStatus.DuplicateName, current));
            }

            _idByName.Remove(current.Name);
            _idByName[product.Name] = product.Id;
            _products[product.Id] = product;
            return Task.FromResult(new ProductMutationResult(ProductMutationStatus.Success, product));
        }
    }

    public Task<ProductMutationResult> TryDeleteAsync(
        Guid id,
        long expectedVersion,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            if (!_products.TryGetValue(id, out Product? current))
            {
                return Task.FromResult(new ProductMutationResult(ProductMutationStatus.NotFound));
            }

            if (current.Version != expectedVersion)
            {
                return Task.FromResult(new ProductMutationResult(ProductMutationStatus.VersionMismatch, current));
            }

            _products.Remove(id);
            _idByName.Remove(current.Name);
            return Task.FromResult(new ProductMutationResult(ProductMutationStatus.Success, current));
        }
    }
}
