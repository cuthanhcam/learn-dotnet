namespace Learning.Api.Features.Products;

public sealed class ProductCatalog(IProductRepository repository, TimeProvider timeProvider)
{
    public async Task<PagedResponse<Product>> ListAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        int skip = checked((page - 1) * pageSize);
        (IReadOnlyList<Product> items, int totalCount) =
            await repository.PageAsync(skip, pageSize, cancellationToken);
        return new PagedResponse<Product>(items, page, pageSize, totalCount);
    }

    public Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        repository.FindAsync(id, cancellationToken);

    public async Task<Product?> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        DateTimeOffset now = timeProvider.GetUtcNow();
        var product = new Product(
            Guid.NewGuid(),
            request.Name!.Trim(),
            request.Price,
            now,
            now,
            Version: 1);

        return await repository.TryAddAsync(product, cancellationToken) ? product : null;
    }

    public async Task<ProductMutationResult> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        long expectedVersion,
        CancellationToken cancellationToken)
    {
        Product? current = await repository.FindAsync(id, cancellationToken);
        if (current is null)
        {
            return new ProductMutationResult(ProductMutationStatus.NotFound);
        }

        var replacement = current with
        {
            Name = request.Name!.Trim(),
            Price = request.Price,
            UpdatedAt = timeProvider.GetUtcNow(),
            Version = checked(current.Version + 1)
        };
        return await repository.TryUpdateAsync(replacement, expectedVersion, cancellationToken);
    }

    public Task<ProductMutationResult> DeleteAsync(
        Guid id,
        long expectedVersion,
        CancellationToken cancellationToken) =>
        repository.TryDeleteAsync(id, expectedVersion, cancellationToken);
}
