namespace Learning.Patterns.Structural.Adapter.Contracts;

/// <summary>
/// Client-owned target contract: quote only, no booking/payment side effect. Cancellation propagates;
/// expected route/availability outcomes are values; corrupt provider data remains an exception.
/// </summary>
public interface IShippingRates
{
    ValueTask<ShippingQuote> QuoteAsync(ShippingRequest request, CancellationToken cancellationToken = default);
}
