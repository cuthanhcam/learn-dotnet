namespace Learning.Patterns.Behavioral.Strategy.Refactored;

public sealed class PartnerDiscountPolicy : IDiscountPolicy
{
    public decimal CalculateDiscount(PricingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        // This is an inclusive subtotal threshold, not a cumulative/slab calculation. The threshold
        // test exists because changing >= to > silently changes the quote at exactly 500.
        return request.Subtotal * (request.Subtotal >= 500m ? 0.15m : 0.05m);
    }
}
