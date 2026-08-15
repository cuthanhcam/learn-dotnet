namespace AsyncConcurrency.Examples.Synchronization;

public sealed class AsyncManualResetEvent
{
    private TaskCompletionSource _signal = CreateSignal();

    public AsyncManualResetEvent(bool initiallySet = false)
    {
        if (initiallySet)
        {
            _signal.TrySetResult();
        }
    }

    public bool IsSet => Volatile.Read(ref _signal).Task.IsCompleted;

    public Task WaitAsync(CancellationToken cancellationToken = default)
    {
        // WaitAsync cancels this caller's wait without canceling the shared
        // signal task observed by other waiters.
        return Volatile.Read(ref _signal).Task.WaitAsync(cancellationToken);
    }

    public void Set() => Volatile.Read(ref _signal).TrySetResult();

    public void Reset()
    {
        while (true)
        {
            TaskCompletionSource observed = _signal;
            if (!observed.Task.IsCompleted)
            {
                return;
            }

            TaskCompletionSource replacement = CreateSignal();
            if (ReferenceEquals(Interlocked.CompareExchange(ref _signal, replacement, observed), observed))
            {
                return;
            }
        }
    }

    private static TaskCompletionSource CreateSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);
}
