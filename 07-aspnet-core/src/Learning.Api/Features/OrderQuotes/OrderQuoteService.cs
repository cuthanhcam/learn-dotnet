namespace Learning.Api.Features.OrderQuotes;

public sealed class OrderQuoteService(TenantContext tenantContext)
{
    private const decimal BulkDiscountRate = 0.10m;

    public OrderQuote Create(CreateOrderQuoteRequest request)
    {
        // Decimal is intentional for monetary arithmetic. The sample rounds at the explicit
        // business boundary rather than relying on JSON formatting or a database provider.
        decimal subtotal = request.Quantity * request.UnitPrice;
        decimal discount = request.Quantity >= 10 ? subtotal * BulkDiscountRate : 0m;
        decimal total = decimal.Round(subtotal - discount, 2, MidpointRounding.AwayFromZero);

        return new OrderQuote(
            tenantContext.GetRequiredTenant(),
            request.Sku!.ToUpperInvariant(),
            request.Quantity,
            request.UnitPrice,
            subtotal,
            discount,
            total);
    }
}
