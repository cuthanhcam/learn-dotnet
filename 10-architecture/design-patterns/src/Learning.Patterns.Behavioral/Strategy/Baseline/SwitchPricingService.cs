namespace Learning.Patterns.Behavioral.Strategy.Baseline;

/// <summary>
/// Correct and readable for a small stable rule set. The pressure for refactoring is repeated policy
/// changes/extensions, not the mere presence of a switch. Preserve this version for comparison.
/// </summary>
public sealed class SwitchPricingService
{
    public PricingQuote Quote(PricingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        decimal discount = request.Customer switch
        {
            CustomerCategory.Standard => 0m,
            CustomerCategory.Member => Math.Min(request.Subtotal * 0.10m, 50m),
            CustomerCategory.Partner => request.Subtotal * (request.Subtotal >= 500m ? 0.15m : 0.05m),
            _ => throw new ArgumentOutOfRangeException(nameof(request))
        };
        return PricingQuote.FromDiscount(request, discount);
    }
}
