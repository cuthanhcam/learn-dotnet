namespace Learning.Patterns.Behavioral.Strategy.Refactored;

/// <summary>
/// Context: delegates the varying algorithm but retains shared output validation/rounding. A readonly
/// strategy prevents one request changing another request's policy through a mutable shared context.
/// This does not magically make an arbitrary stateful strategy implementation thread-safe.
/// </summary>
public sealed class PricingService
{
    private readonly IDiscountPolicy _policy;

    public PricingService(IDiscountPolicy policy) => _policy = policy ?? throw new ArgumentNullException(nameof(policy));

    public PricingQuote Quote(PricingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PricingQuote.FromDiscount(request, _policy.CalculateDiscount(request));
    }
}
