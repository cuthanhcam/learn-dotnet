using Learning.Patterns.Structural.Adapter.Contracts;
using Learning.Patterns.Structural.Decorator.Contracts;

namespace Learning.Patterns.Structural.Decorator.Refactored;

/// <summary>
/// Records completed results before returning them. Placement outside retry records the final
/// result; placement inside retry records each completed attempt. Neither dependency is owned here.
/// </summary>
public sealed class AuditedShippingRates : IShippingRates
{
    private readonly IShippingRates _inner;
    private readonly IQuoteAudit _audit;

    public AuditedShippingRates(IShippingRates inner, IQuoteAudit audit)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(audit);
        _inner = inner;
        _audit = audit;
    }

    public async ValueTask<ShippingQuote> QuoteAsync(ShippingRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        ShippingQuote quote = await _inner.QuoteAsync(request, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        await _audit.RecordAsync(request, quote, cancellationToken).ConfigureAwait(false);
        return quote;
    }
}
