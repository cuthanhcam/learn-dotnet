namespace AsyncConcurrency.Examples.Cancellation;

public static class CancellationExample
{
    public static async Task<int> CountUntilCancelledAsync(
        int limit,
        TimeSpan interval,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(limit);
        ArgumentOutOfRangeException.ThrowIfLessThan(interval, TimeSpan.Zero);

        int completed = 0;
        while (completed < limit)
        {
            // Passing the token makes the wait cancellable. The resulting
            // OperationCanceledException communicates cooperative cancellation.
            await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
            completed++;
        }

        return completed;
    }

    public static async Task<T> WithTimeoutAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeout, TimeSpan.Zero);

        using var timeoutSource = new CancellationTokenSource(timeout);
        using CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            timeoutSource.Token);

        try
        {
            return await operation(linkedSource.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"The operation exceeded the timeout of {timeout}.");
        }
    }
}
