using Learning.Patterns.Structural.Adapter.Contracts;
using Learning.Patterns.Structural.Decorator.Contracts;

namespace Learning.Patterns.Structural.Decorator.Baseline;

// A correct fixed workflow is a reasonable starting point. It always audits the final result.
// The refactor becomes useful when callers need independent behaviors and different wrapper order.
public sealed class RetryAndAuditQuoteWorkflow
{
    private readonly IShippingRates _rates;
    private readonly IQuoteRetryDelay _delay;
    private readonly IQuoteAudit _audit;
    private readonly int _maxAttempts;

    public RetryAndAuditQuoteWorkflow(IShippingRates rates, IQuoteRetryDelay delay, IQuoteAudit audit, int maxAttempts = 3)
    {
        ArgumentNullException.ThrowIfNull(rates);
        ArgumentNullException.ThrowIfNull(delay);
        ArgumentNullException.ThrowIfNull(audit);
        if (maxAttempts is < 1 or > 3)
            throw new ArgumentOutOfRangeException(nameof(maxAttempts));
        _rates = rates;
        _delay = delay;
        _audit = audit;
        _maxAttempts = maxAttempts;
    }

    public async ValueTask<ShippingQuote> QuoteAsync(ShippingRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        for (int attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ShippingQuote quote = await _rates.QuoteAsync(request, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            if (quote.Status != ShippingQuoteStatus.TemporarilyUnavailable || attempt == _maxAttempts)
            {
                await _audit.RecordAsync(request, quote, cancellationToken).ConfigureAwait(false);
                return quote;
            }
            await _delay.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
