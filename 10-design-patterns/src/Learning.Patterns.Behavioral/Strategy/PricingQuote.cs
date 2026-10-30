namespace Learning.Patterns.Behavioral.Strategy;

/// <summary>Only the shared quote boundary rounds and verifies algorithm output.</summary>
public sealed record PricingQuote
{
    private PricingQuote(decimal subtotal, decimal discount)
    {
        Subtotal = subtotal;
        Discount = discount;
    }

    public decimal Subtotal { get; }
    public decimal Discount { get; }
    public decimal Total => Subtotal - Discount;

    public static PricingQuote FromDiscount(PricingRequest request, decimal discount)
    {
        ArgumentNullException.ThrowIfNull(request);
        // A broken plugin is not silently clamped into a plausible result. Surface the violated
        // strategy contract before formatting/persisting a quote or charging a downstream customer.
        if (discount < 0 || discount > request.Subtotal)
            throw new InvalidOperationException("A pricing policy returned an out-of-range discount.");
        return new PricingQuote(request.Subtotal, decimal.Round(discount, 2, MidpointRounding.AwayFromZero));
    }
}
