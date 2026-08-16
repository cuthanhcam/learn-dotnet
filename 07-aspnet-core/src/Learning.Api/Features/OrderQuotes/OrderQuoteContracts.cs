using System.ComponentModel.DataAnnotations;

namespace Learning.Api.Features.OrderQuotes;

public sealed class CreateOrderQuoteRequest
{
    [Required, StringLength(40, MinimumLength = 3)]
    [RegularExpression("^[A-Za-z0-9-]+$", ErrorMessage = "SKU may contain letters, digits, and hyphens only.")]
    public string? Sku { get; init; }

    [Range(1, 1_000)]
    public int Quantity { get; init; }

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal UnitPrice { get; init; }
}

public sealed record OrderQuote(
    string TenantId,
    string Sku,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal,
    decimal Discount,
    decimal Total);
