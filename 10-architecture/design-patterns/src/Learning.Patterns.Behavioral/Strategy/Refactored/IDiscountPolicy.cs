namespace Learning.Patterns.Behavioral.Strategy.Refactored;

/// <summary>
/// Strategy: evaluate a discount for validated input, without rounding, mutating input, or choosing
/// another strategy. Implementations must return a value between zero and the request subtotal.
/// </summary>
public interface IDiscountPolicy
{
    decimal CalculateDiscount(PricingRequest request);
}
