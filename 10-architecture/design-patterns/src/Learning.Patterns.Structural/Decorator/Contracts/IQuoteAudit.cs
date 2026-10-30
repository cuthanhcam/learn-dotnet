using Learning.Patterns.Structural.Adapter.Contracts;

namespace Learning.Patterns.Structural.Decorator.Contracts;

/// <summary>
/// Required completion recording: QuoteAsync returns only after this operation completes. Failures
/// propagate. This is not best-effort telemetry, durable delivery, or an exactly-once audit protocol.
/// </summary>
public interface IQuoteAudit
{
    ValueTask RecordAsync(ShippingRequest request, ShippingQuote quote, CancellationToken cancellationToken);
}
