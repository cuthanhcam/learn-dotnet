namespace AsyncConcurrency.Examples.Synchronization;

public static class BoundedExecutor
{
    public static async Task<TResult[]> SelectAsync<TSource, TResult>(
        IEnumerable<TSource> source,
        int maxConcurrency,
        Func<TSource, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxConcurrency);

        TSource[] items = source.ToArray();
        var results = new TResult[items.Length];
        using var gate = new SemaphoreSlim(maxConcurrency, maxConcurrency);

        Task[] workers = items.Select(ProcessOneAsync).ToArray();
        await Task.WhenAll(workers).ConfigureAwait(false);
        return results;

        async Task ProcessOneAsync(TSource item, int index)
        {
            await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                results[index] = await selector(item, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                // Release must run for success, failure, and cancellation so
                // another waiting operation cannot be starved permanently.
                gate.Release();
            }
        }
    }

}
