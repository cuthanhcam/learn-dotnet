namespace AsyncConcurrency.Exercises;

public static class AsyncRetry
{
    public static async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        int maxAttempts,
        TimeSpan delay,
        Func<Exception, bool> isTransient,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(isTransient);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts);
        ArgumentOutOfRangeException.ThrowIfLessThan(delay, TimeSpan.Zero);

        for (int attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return await operation(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (
                exception is not OperationCanceledException &&
                attempt < maxAttempts &&
                isTransient(exception))
            {
                // A production retry policy would normally add exponential
                // backoff and jitter. This fixed delay keeps the exercise small.
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
