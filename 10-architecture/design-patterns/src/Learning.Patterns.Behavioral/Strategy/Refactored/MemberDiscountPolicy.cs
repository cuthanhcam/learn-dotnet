namespace Learning.Patterns.Behavioral.Strategy.Refactored;

public sealed class MemberDiscountPolicy : IDiscountPolicy
{
    public decimal CalculateDiscount(PricingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        // Membership has a cap; a higher subtotal must not grow the discount without a bound.
        return Math.Min(request.Subtotal * 0.10m, 50m);
    }
}
