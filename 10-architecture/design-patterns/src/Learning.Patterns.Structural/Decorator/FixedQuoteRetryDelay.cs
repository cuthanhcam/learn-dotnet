using Learning.Patterns.Structural.Decorator.Contracts;

namespace Learning.Patterns.Structural.Decorator;

public sealed class FixedQuoteRetryDelay : IQuoteRetryDelay
{
    private readonly TimeSpan _delay;

    public FixedQuoteRetryDelay(TimeSpan delay)
    {
        if (delay <= TimeSpan.Zero || delay > TimeSpan.FromSeconds(30))
            throw new ArgumentOutOfRangeException(nameof(delay), "Retry delay must be positive and at most 30 seconds.");
        _delay = delay;
    }

    public async ValueTask WaitAsync(CancellationToken cancellationToken) =>
        await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);
}
