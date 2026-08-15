---
title: "Concurrent Collections"
description: "ConcurrentDictionary, ConcurrentQueue, ConcurrentStack, partitioning, and compound-operation semantics."
slug: dotnet-concurrent-collections
phase: 6
order: 7
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 28
topics: [concurrency, concurrent-collections]
prerequisites: [dotnet-channels-pipelines]
status: maintained
last-reviewed: 2026-08-15
---

# Concurrent Collections

## Learning Objectives

- Choose a collection from the required access and coordination semantics.
- Distinguish thread-safe individual methods from an atomic business workflow.
- Explain why dictionary factories may execute more than once.
- Design request coalescing with separate shared-operation and caller-wait cancellation.
- Recognize when a bounded channel is more suitable than a concurrent queue.

Concurrent collections support thread-safe individual operations. Use `ConcurrentDictionary.GetOrAdd` or `AddOrUpdate` rather than composing `ContainsKey` with an update. Even atomic APIs may invoke a value factory more than once; factories must tolerate duplicate computation and should not rely on exactly-once side effects.

`ConcurrentQueue<T>` and `ConcurrentStack<T>` provide non-blocking-style FIFO/LIFO operations. A channel is usually a better fit when consumers must wait asynchronously for new items and require completion or capacity semantics.

Thread safety does not define a business transaction. Protect multi-object or multi-step invariants with a higher-level design, synchronization boundary, actor/queue ownership, or transactional store.

## Collection Selection Matrix

| Need | Starting point | Missing semantics to consider |
|---|---|---|
| Concurrent lookup and atomic per-key update | `ConcurrentDictionary<TKey,TValue>` | eviction, size bounds, multi-key transactions |
| Immediate FIFO enqueue/dequeue | `ConcurrentQueue<T>` | async waiting, capacity, completion |
| Immediate LIFO push/pop | `ConcurrentStack<T>` | async waiting, capacity, completion |
| Unordered producer/consumer work | `ConcurrentBag<T>` | global ordering and predictable cross-thread behavior |
| Blocking synchronous producer/consumer | `BlockingCollection<T>` | occupies waiting threads; not async-first |
| Bounded asynchronous producer/consumer | `Channel<T>` | durability across process failure |
| Immutable read-mostly publication | immutable collection + atomic replacement | copy/update cost |

Choose based on the entire protocol. “Thread-safe” alone does not say whether consumers wait,
whether producers receive backpressure, whether the sequence completes, or whether data survives a
process restart.

## ConcurrentDictionary Delegate Semantics

Convenience methods make the dictionary mutation atomic, but user delegates are generally invoked
outside internal locks. Under contention, `GetOrAdd` can invoke a value factory several times even
though only one value is stored. `AddOrUpdate` factories can also repeat.

Therefore factories should be side-effect-free, cheap enough to duplicate, or return a wrapper that
coordinates one expensive operation. Do not send email, charge a card, increment a durable counter,
or assume exactly-once execution directly inside such a delegate.

For conditional replacement, `TryUpdate(key, newValue, comparisonValue)` is the dictionary analogue
of compare-and-swap. A retry loop may still be needed because another writer can win between reading
and updating.

## Async Memoization and Request Coalescing

Request coalescing lets concurrent callers for one key observe one shared in-flight operation. The
repository combines:

- `ConcurrentDictionary<TKey, Lazy<Task<TValue>>>` for atomic entry selection;
- `LazyThreadSafetyMode.ExecutionAndPublication` so the selected entry invokes one factory;
- a cache-owned lifetime token for the shared operation;
- `Task.WaitAsync(callerToken)` so one caller cancels only its own wait; and
- conditional removal of faulted or canceled entries so a later request can retry.

These two lifetimes must not be conflated:

```text
shared operation lifetime ─────────────── completion
caller A wait          ───── cancellation
caller B wait          ───────────────── result
```

If caller A's token were passed directly into the shared factory, A could cancel work still needed
by B. Conversely, canceling only the wait means work may continue with no active waiter. The cache
owner must decide when orphaned work should stop, usually through service shutdown, eviction, or an
explicit shared lifetime policy.

Memoization is not a complete cache. A production cache also needs expiry, size limits, memory-cost
accounting, invalidation, observability, and possibly distributed coherence.

## Enumeration and Snapshots

Concurrent enumeration does not throw merely because another thread updates the collection, but it
usually represents a moving view rather than an atomic snapshot of one instant. If a business rule
requires a consistent snapshot across several entries, use higher-level synchronization, immutable
publication, versioning, or a transactional store.

Calling `ToArray()` creates a stable array for later local use, but the copy itself reflects whatever
the collection's enumeration contract allowed during concurrent updates.

## Compound Operations

This sequence is still a race even on a concurrent dictionary:

```csharp
if (!inventory.ContainsKey(sku))
{
    inventory[sku] = initialQuantity;
}
```

Use `TryAdd` when “insert if absent” is the whole invariant. If an operation spans several keys—for
example transferring quantity between warehouses—no single dictionary method makes that transaction
atomic. Redesign ownership or protect the complete transition.

## Performance and Contention

Concurrent collections reduce or partition synchronization; they do not remove its cost. Hot keys,
large factories, frequent global enumeration, and false assumptions about access distribution can
still limit scalability. Benchmark with representative key skew and read/write ratios. A plain
`Dictionary` protected by one lock can be simpler and sometimes faster for small, low-contention state.

## Implementation Map

| Concern | Source | Tests |
|---|---|---|
| Shared async result per key | `Collections/AsyncMemoizer.cs` | `AsyncMemoizerTests.cs` |
| Async queue with backpressure | `Channels/ChannelWorkPool.cs` | `ChannelWorkPoolTests.cs` |

## Review Questions

1. Why may a `GetOrAdd` factory execute more than once?
2. Which cancellation token should control a shared memoized operation?
3. Why is `ContainsKey` followed by assignment a race?
4. When is `Channel<T>` a better choice than `ConcurrentQueue<T>`?
5. Does concurrent enumeration represent a transactional snapshot?

## References

- [Thread-safe collections](https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/)
- [`ConcurrentDictionary<TKey,TValue>`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2)
- [When to use a thread-safe collection](https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/when-to-use-a-thread-safe-collection)

## Navigation

- Previous: [Channels and pipelines](06-channels-pipelines.md)
- Next: [Deadlocks and synchronization context](08-deadlocks-context.md)
