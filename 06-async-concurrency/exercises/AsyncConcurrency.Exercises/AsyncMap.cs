namespace AsyncConcurrency.Exercises;

public static class AsyncMap
{
    public static async Task<TResult[]> MapAsync<TSource, TResult>(
        IReadOnlyList<TSource> source,
        int maxConcurrency,
        Func<TSource, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxConcurrency);

        var results = new TResult[source.Count];
        using var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);

        Task[] operations = Enumerable.Range(0, source.Count)
            .Select(ProcessIndexAsync)
            .ToArray();

        await Task.WhenAll(operations).ConfigureAwait(false);
        return results;

        async Task ProcessIndexAsync(int index)
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                results[index] = await selector(source[index], cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
