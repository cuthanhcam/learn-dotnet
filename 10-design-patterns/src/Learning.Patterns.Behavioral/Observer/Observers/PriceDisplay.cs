namespace Learning.Patterns.Behavioral.Observer.Observers;

public sealed class PriceDisplay : IObserver<PriceUpdate>
{
    public PriceUpdate? Latest { get; private set; }
    public bool IsStopped { get; private set; }
    public Exception? Error { get; private set; }

    public void OnNext(PriceUpdate value) => Latest = value;
    public void OnCompleted() => IsStopped = true;
    public void OnError(Exception error)
    {
        Error = error;
        IsStopped = true;
    }
}
