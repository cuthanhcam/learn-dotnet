using Learning.Api.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Learning.Api.Features.Products;

public static class ProductEndpoints
{
    private const int DefaultPageSize = 20;

    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", ListAsync)
            .WithName("ListProducts")
            .WithSummary("List products using bounded page-number pagination")
            .Produces<PagedResponse<Product>>()
            .ProducesValidationProblem();
        group.MapGet("/{id:guid}", GetAsync)
            .WithName("GetProduct")
            .WithSummary("Get a product and its current strong ETag")
            .Produces<Product>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", CreateAsync)
            .WithName("CreateProduct")
            .WithSummary("Create a uniquely named product")
            .Produces<Product>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateProduct")
            .WithSummary("Replace a product when its If-Match precondition succeeds")
            .Produces<Product>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status412PreconditionFailed)
            .ProducesProblem(StatusCodes.Status428PreconditionRequired);
        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product when its If-Match precondition succeeds")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status412PreconditionFailed)
            .ProducesProblem(StatusCodes.Status428PreconditionRequired);

        return endpoints;
    }

    private static async Task<IResult> ListAsync(
        int page,
        int pageSize,
        ProductCatalog catalog,
        IOptions<LearningOptions> options,
        CancellationToken cancellationToken)
    {
        // Query binding supplies zero for omitted integers. Translate that representation into
        // documented defaults while still rejecting explicit negative values.
        page = page == 0 ? 1 : page;
        pageSize = pageSize == 0 ? DefaultPageSize : pageSize;

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (page < 1)
        {
            errors[nameof(page)] = ["Page must be greater than zero."];
        }

        if (pageSize < 1 || pageSize > options.Value.MaximumPageSize)
        {
            errors[nameof(pageSize)] =
            [$"Page size must be between 1 and {options.Value.MaximumPageSize}."];
        }

        return errors.Count > 0
            ? Results.ValidationProblem(errors)
            : Results.Ok(await catalog.ListAsync(page, pageSize, cancellationToken));
    }

    private static async Task<IResult> GetAsync(
        Guid id,
        ProductCatalog catalog,
        HttpResponse response,
        CancellationToken cancellationToken)
    {
        Product? product = await catalog.FindAsync(id, cancellationToken);
        if (product is null)
        {
            return ProductProblem.NotFound(id);
        }

        response.Headers.ETag = ProductEntityTag.Format(product.Version);
        return Results.Ok(product);
    }

    private static async Task<IResult> CreateAsync(
        CreateProductRequest request,
        ProductCatalog catalog,
        HttpResponse response,
        CancellationToken cancellationToken)
    {
        Dictionary<string, string[]> errors = request.Validate();
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        Product? product = await catalog.CreateAsync(request, cancellationToken);
        if (product is null)
        {
            return ProductProblem.DuplicateName(request.Name!);
        }

        response.Headers.ETag = ProductEntityTag.Format(product.Version);
        return Results.CreatedAtRoute("GetProduct", new { id = product.Id }, product);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        ProductCatalog catalog,
        HttpRequest httpRequest,
        HttpResponse response,
        CancellationToken cancellationToken)
    {
        Dictionary<string, string[]> errors = request.Validate();
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        if (!ProductEntityTag.TryReadRequiredVersion(httpRequest, out long expectedVersion, out IResult? error))
        {
            return error!;
        }

        ProductMutationResult result =
            await catalog.UpdateAsync(id, request, expectedVersion, cancellationToken);
        if (result.Status != ProductMutationStatus.Success)
        {
            return ProductProblem.FromMutation(id, request.Name!, result);
        }

        response.Headers.ETag = ProductEntityTag.Format(result.Product!.Version);
        return Results.Ok(result.Product);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        ProductCatalog catalog,
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        if (!ProductEntityTag.TryReadRequiredVersion(request, out long expectedVersion, out IResult? error))
        {
            return error!;
        }

        ProductMutationResult result = await catalog.DeleteAsync(id, expectedVersion, cancellationToken);
        return result.Status == ProductMutationStatus.Success
            ? Results.NoContent()
            : ProductProblem.FromMutation(id, name: string.Empty, result);
    }
}

internal static class ProductEntityTag
{
    public static string Format(long version) => $"\"{version}\"";

    public static bool TryReadRequiredVersion(
        HttpRequest request,
        out long version,
        out IResult? error)
    {
        version = default;
        error = null;

        // Require one strong entity tag. Supporting lists, weak tags, or '*' is possible, but
        // would obscure the core optimistic-concurrency example and its exact-version contract.
        Microsoft.Extensions.Primitives.StringValues values = request.Headers[HeaderNames.IfMatch];
        if (values.Count == 0)
        {
            error = Results.Problem(
                statusCode: StatusCodes.Status428PreconditionRequired,
                title: "An If-Match header is required.",
                detail: "Read the resource first and send its current ETag when modifying it.");
            return false;
        }

        string? candidate = values[0];
        if (values.Count != 1 || candidate is null || candidate.Contains(',') ||
            candidate.StartsWith("W/", StringComparison.OrdinalIgnoreCase))
        {
            error = InvalidEntityTag();
            return false;
        }

        string value = candidate;
        if (value.Length < 3 || value[0] != '"' || value[^1] != '"' ||
            !long.TryParse(value.AsSpan(1, value.Length - 2), out version) || version < 1)
        {
            error = InvalidEntityTag();
            return false;
        }

        return true;
    }

    private static IResult InvalidEntityTag() => Results.Problem(
        statusCode: StatusCodes.Status400BadRequest,
        title: "The If-Match header is invalid.",
        detail: "Send exactly one strong ETag in the quoted form returned by this API, for example \"1\".");
}

internal static class ProductProblem
{
    public static IResult FromMutation(Guid id, string name, ProductMutationResult result) =>
        result.Status switch
        {
            ProductMutationStatus.NotFound => NotFound(id),
            ProductMutationStatus.VersionMismatch => Results.Problem(
                statusCode: StatusCodes.Status412PreconditionFailed,
                title: "The product changed after it was read.",
                detail: "Fetch the latest representation and retry with its ETag."),
            ProductMutationStatus.DuplicateName => DuplicateName(name),
            _ => throw new InvalidOperationException($"Unexpected mutation status: {result.Status}.")
        };

    public static IResult NotFound(Guid id) => Results.Problem(
        statusCode: StatusCodes.Status404NotFound,
        title: "Product not found.",
        detail: $"No product with identifier '{id}' exists.");

    public static IResult DuplicateName(string name) => Results.Problem(
        statusCode: StatusCodes.Status409Conflict,
        title: "A product with the same name already exists.",
        detail: $"Product names are case-insensitively unique; '{name.Trim()}' is already in use.");
}
