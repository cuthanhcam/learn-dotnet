namespace CoreDotNet.Exercises;

public sealed class ThresholdReachedEventArgs(int value) : EventArgs
{
    public int Value { get; } = value;
}

public sealed class ThresholdCounter
{
    private readonly int _threshold;
    private bool _raised;

    public ThresholdCounter(int threshold)
    {
        if (threshold <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold must be positive.");
        }

        _threshold = threshold;
    }

    public int Value { get; private set; }

    public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;

    public void Increment()
    {
        Value++;

        if (!_raised && Value >= _threshold)
        {
            _raised = true;
            ThresholdReached?.Invoke(this, new ThresholdReachedEventArgs(Value));
        }
    }
}
