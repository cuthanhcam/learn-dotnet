namespace AsyncConcurrency.Examples.AsyncBasics;

public static class AsyncBasicsExample
{
    public static async Task<string> SimulateIoAsync(
        string value,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentOutOfRangeException.ThrowIfLessThan(delay, TimeSpan.Zero);

        // Task.Delay represents non-blocking waiting. No worker thread is held
        // while the timer is pending, which models network and file I/O waits.
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        return value.ToUpperInvariant();
    }

    public static async Task<string[]> RunConcurrentlyAsync(
        IEnumerable<string> values,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(values);

        Task<string>[] pending = values
            .Select(value => SimulateIoAsync(value, TimeSpan.FromMilliseconds(5), cancellationToken))
            .ToArray();

        // Start all independent operations before awaiting their combined result.
        // WhenAll preserves task-array order even if completion order differs.
        return await Task.WhenAll(pending).ConfigureAwait(false);
    }
}
