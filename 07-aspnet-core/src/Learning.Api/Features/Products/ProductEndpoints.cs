using Microsoft.AspNetCore.Http.HttpResults;

namespace Learning.Api.Features.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", ListAsync).WithName("ListProducts");
        group.MapGet("/{id:guid}", GetByIdAsync).WithName("GetProductById");
        group.MapPost("/", CreateAsync).WithName("CreateProduct");
        return endpoints;
    }

    private static async Task<Ok<IReadOnlyList<Product>>> ListAsync(
        IProductRepository repository,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await repository.ListAsync(cancellationToken));
    }

    private static async Task<Results<Ok<Product>, NotFound>> GetByIdAsync(
        Guid id,
        IProductRepository repository,
        CancellationToken cancellationToken)
    {
        Product? product = await repository.FindAsync(id, cancellationToken);
        return product is null ? TypedResults.NotFound() : TypedResults.Ok(product);
    }

    private static async Task<Results<CreatedAtRoute<Product>, ValidationProblem>> CreateAsync(
        CreateProductRequest request,
        IProductRepository repository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        Dictionary<string, string[]> errors = request.Validate();
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var product = new Product(
            Guid.NewGuid(),
            request.Name!.Trim(),
            request.Price,
            timeProvider.GetUtcNow());
        await repository.AddAsync(product, cancellationToken);

        return TypedResults.CreatedAtRoute(
            product,
            routeName: "GetProductById",
            routeValues: new { id = product.Id });
    }
}
