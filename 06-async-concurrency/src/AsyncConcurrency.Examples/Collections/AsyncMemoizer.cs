using System.Collections.Concurrent;

namespace AsyncConcurrency.Examples.Collections;

public sealed class AsyncMemoizer<TKey, TValue>(IEqualityComparer<TKey>? comparer = null)
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _entries = new(comparer);

    public int Count => _entries.Count;

    public async Task<TValue> GetOrAddAsync(
        TKey key,
        Func<TKey, CancellationToken, Task<TValue>> factory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        Lazy<Task<TValue>> candidate = new(
            () => factory(key, cancellationToken),
            LazyThreadSafetyMode.ExecutionAndPublication);

        Lazy<Task<TValue>> selected = _entries.GetOrAdd(key, candidate);
        try
        {
            // A caller token participates in factory creation only when this
            // caller wins insertion. Shared cancellation semantics must be a
            // deliberate application decision; one caller should not silently
            // cancel work already shared by unrelated callers.
            return await selected.Value.ConfigureAwait(false);
        }
        catch
        {
            // Do not poison the cache permanently with a faulted or canceled
            // task. KeyValuePair removal ensures a newer entry is not removed.
            _entries.TryRemove(new KeyValuePair<TKey, Lazy<Task<TValue>>>(key, selected));
            throw;
        }
    }

    public bool TryRemove(TKey key) => _entries.TryRemove(key, out _);
}
