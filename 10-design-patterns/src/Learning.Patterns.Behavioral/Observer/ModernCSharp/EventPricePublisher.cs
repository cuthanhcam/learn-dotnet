namespace Learning.Patterns.Behavioral.Observer.ModernCSharp;

/// <summary>
/// A smaller alternative for local notifications. Callers pair += with -=. Ordinary multicast
/// invocation stops if a handler throws; it has no completion/error protocol or subscription budget.
/// </summary>
public sealed class EventPricePublisher
{
    public event Action<PriceUpdate>? PriceChanged;

    public void Publish(PriceUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);
        PriceChanged?.Invoke(update);
    }
}
