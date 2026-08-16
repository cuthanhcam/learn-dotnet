namespace Learning.Api.Features.Products;

public sealed record Product(
    Guid Id,
    string Name,
    decimal Price,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    long Version);

public sealed record CreateProductRequest(string? Name, decimal Price) : ProductWriteRequest(Name, Price);

public sealed record UpdateProductRequest(string? Name, decimal Price) : ProductWriteRequest(Name, Price);

public abstract record ProductWriteRequest(string? Name, decimal Price)
{
    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(Name))
        {
            errors[nameof(Name)] = ["Name is required."];
        }
        else if (Name.Trim().Length > 120)
        {
            errors[nameof(Name)] = ["Name cannot exceed 120 characters."];
        }

        if (Price <= 0)
        {
            errors[nameof(Price)] = ["Price must be greater than zero."];
        }

        return errors;
    }
}

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => TotalCount == 0
        ? 0
        : (int)Math.Ceiling((double)TotalCount / PageSize);
}
