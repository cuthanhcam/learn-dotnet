namespace Learning.Api.Features.Products;

public interface IProductRepository
{
    Task<(IReadOnlyList<Product> Items, int TotalCount)> PageAsync(
        int skip,
        int take,
        CancellationToken cancellationToken);

    Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> TryAddAsync(Product product, CancellationToken cancellationToken);
    Task<ProductMutationResult> TryUpdateAsync(Product product, long expectedVersion, CancellationToken cancellationToken);
    Task<ProductMutationResult> TryDeleteAsync(Guid id, long expectedVersion, CancellationToken cancellationToken);
}

public enum ProductMutationStatus
{
    Success,
    NotFound,
    VersionMismatch,
    DuplicateName
}

public sealed record ProductMutationResult(ProductMutationStatus Status, Product? Product = null);
