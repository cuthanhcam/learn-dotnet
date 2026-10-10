namespace Learning.Patterns.Behavioral.Strategy.Refactored;

public sealed class StandardDiscountPolicy : IDiscountPolicy
{
    public decimal CalculateDiscount(PricingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return 0m;
    }
}
