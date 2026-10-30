using Learning.Patterns.Behavioral.Observer.Observers;

namespace Learning.Patterns.Behavioral.Observer.Baseline;

// Correct for an initial requirement with two permanent, known consumers. Runtime subscriptions,
// independent lifetimes, and replaceable observers are later requirements, not baseline bugs.
public sealed class FixedPricePublisher(PriceDisplay display, PriceThresholdObserver threshold)
{
    private readonly PriceDisplay _display = display ?? throw new ArgumentNullException(nameof(display));
    private readonly PriceThresholdObserver _threshold = threshold ?? throw new ArgumentNullException(nameof(threshold));

    public void Publish(PriceUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);
        _display.OnNext(update);
        _threshold.OnNext(update);
    }
}
