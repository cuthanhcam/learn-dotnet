using System.Collections.Concurrent;

namespace AsyncConcurrency.Examples.Collections;

public sealed class AsyncMemoizer<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _entries;
    private readonly CancellationToken _sharedLifetimeToken;

    public AsyncMemoizer(
        IEqualityComparer<TKey>? comparer = null,
        CancellationToken sharedLifetimeToken = default)
    {
        _entries = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>(comparer);
        _sharedLifetimeToken = sharedLifetimeToken;
    }

    public int Count => _entries.Count;

    public async Task<TValue> GetOrAddAsync(
        TKey key,
        Func<TKey, CancellationToken, Task<TValue>> factory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        Lazy<Task<TValue>> candidate = new(
            // The operation belongs to the cache, not to whichever caller wins insertion.
            // Its lifetime is therefore controlled separately from an individual caller wait.
            () => factory(key, _sharedLifetimeToken),
            LazyThreadSafetyMode.ExecutionAndPublication);

        Lazy<Task<TValue>> selected = _entries.GetOrAdd(key, candidate);
        Task<TValue> sharedTask = selected.Value;

        // Cleanup belongs to shared-task completion rather than to one waiter's outcome.
        // ExecuteSynchronously runs the tiny removal inline when safe and avoids another queued task.
        _ = sharedTask.ContinueWith(
            static (_, state) =>
            {
                var cleanup = (CleanupState)state!;
                cleanup.Entries.TryRemove(
                    new KeyValuePair<TKey, Lazy<Task<TValue>>>(cleanup.Key, cleanup.Entry));
            },
            new CleanupState(_entries, key, selected),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion,
            TaskScheduler.Default);

        // WaitAsync cancels only this caller's wait. Other callers continue observing the shared task.
        return await sharedTask.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public bool TryRemove(TKey key) => _entries.TryRemove(key, out _);

    private sealed record CleanupState(
        ConcurrentDictionary<TKey, Lazy<Task<TValue>>> Entries,
        TKey Key,
        Lazy<Task<TValue>> Entry);
}
