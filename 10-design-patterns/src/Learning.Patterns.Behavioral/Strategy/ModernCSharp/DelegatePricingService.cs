namespace Learning.Patterns.Behavioral.Strategy.ModernCSharp;

/// <summary>
/// A one-operation strategy need not always have a class. Delegates are useful for small pure policies;
/// named interfaces remain valuable for domain vocabulary, dependencies, discovery, and richer contracts.
/// A closure can capture mutable state, so delegate syntax alone provides no isolation guarantee.
/// </summary>
public sealed class DelegatePricingService
{
    private readonly Func<PricingRequest, decimal> _discount;

    public DelegatePricingService(Func<PricingRequest, decimal> discount) =>
        _discount = discount ?? throw new ArgumentNullException(nameof(discount));

    public PricingQuote Quote(PricingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PricingQuote.FromDiscount(request, _discount(request));
    }
}
