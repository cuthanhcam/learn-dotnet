namespace Learning.Patterns.Behavioral.Observer.Observers;

/// <summary>Counts received updates at or above a threshold; it does not send external alerts.</summary>
public sealed class PriceThresholdObserver : IObserver<PriceUpdate>
{
    private readonly decimal _thresholdUsd;

    public PriceThresholdObserver(decimal thresholdUsd)
    {
        if (thresholdUsd is < 0 or > 1_000_000 || decimal.Round(thresholdUsd, 2) != thresholdUsd)
            throw new ArgumentOutOfRangeException(nameof(thresholdUsd));
        _thresholdUsd = thresholdUsd;
    }

    public int Matches { get; private set; }
    public bool IsStopped { get; private set; }
    public Exception? Error { get; private set; }

    public void OnNext(PriceUpdate value)
    {
        if (value.PriceUsd >= _thresholdUsd)
            Matches++;
    }

    public void OnCompleted() => IsStopped = true;
    public void OnError(Exception error)
    {
        Error = error;
        IsStopped = true;
    }
}
