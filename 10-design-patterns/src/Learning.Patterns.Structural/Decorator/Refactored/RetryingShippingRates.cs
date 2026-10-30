using Learning.Patterns.Structural.Adapter.Contracts;
using Learning.Patterns.Structural.Decorator.Contracts;

namespace Learning.Patterns.Structural.Decorator.Refactored;

/// <summary>
/// Decorates a quote-only capability. Only the typed temporary-unavailability result is retried;
/// exceptions and cancellation propagate. Do not copy this policy onto booking/payment operations.
/// The caller owns the inner component and delay; this wrapper neither creates nor disposes them.
/// </summary>
public sealed class RetryingShippingRates : IShippingRates
{
    private readonly IShippingRates _inner;
    private readonly IQuoteRetryDelay _delay;
    private readonly int _maxAttempts;

    public RetryingShippingRates(IShippingRates inner, IQuoteRetryDelay delay, int maxAttempts = 3)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(delay);
        if (maxAttempts is < 1 or > 3)
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Total attempts must be 1..3.");
        _inner = inner;
        _delay = delay;
        _maxAttempts = maxAttempts;
    }

    public async ValueTask<ShippingQuote> QuoteAsync(ShippingRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        for (int attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ShippingQuote quote = await _inner.QuoteAsync(request, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            if (quote.Status != ShippingQuoteStatus.TemporarilyUnavailable || attempt == _maxAttempts)
                return quote;
            // The next iteration observes cancellation even if a custom delay ignores its token.
            await _delay.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
