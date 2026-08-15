---
title: "Synchronization and Shared State"
description: "Race conditions, atomicity, memory visibility, lock, Interlocked, SemaphoreSlim, and invariant protection."
phase: 6
order: 4
topics: [concurrency, synchronization, locks]
---

# Synchronization and Shared State

A race exists when correctness depends on uncontrolled operation ordering. `counter++` is a read-modify-write sequence, not one atomic operation. Use `Interlocked` for supported single-value transitions, `lock` for short synchronous critical sections protecting a compound invariant, and `SemaphoreSlim` for asynchronous admission control.

Never `await` inside `lock`. Keep critical sections small, avoid calling unknown code while holding a lock, and establish one global lock order when multiple locks are unavoidable.

Synchronization also provides memory-ordering guarantees. `volatile` affects visibility and ordering for a field but does not make compound operations atomic.

## Choosing a Primitive by Invariant

| Requirement | Typical starting point | Important limitation |
|---|---|---|
| Atomic numeric/reference transition | `Interlocked` | Only supported single-location operations |
| Short synchronous compound invariant | `lock` / `Monitor` | A waiting thread is blocked; never await inside |
| Limit in-process concurrent entrants | `SemaphoreSlim` | It is not ownership-aware or cross-process |
| Cross-process named permit | `Semaphore` | Kernel resource; synchronous waiting and higher overhead |
| Many readers, rare writers | `ReaderWriterLockSlim` | Complexity and upgrade/deadlock hazards |
| Async producer/consumer flow | `Channel<T>` | Queue coordination, not arbitrary critical sections |
| Shared value publication | immutable snapshot + atomic swap | Copy cost; updates need a clear writer policy |

Before synchronizing, try eliminating shared mutation, confining state to one owner, partitioning data, or passing messages.

## `SemaphoreSlim` Ownership Pattern

`WaitAsync` avoids occupying a thread while waiting for an in-process permit. Acquisition must be followed by `Release` only if acquisition succeeded, normally through `try/finally`:

```csharp
await semaphore.WaitAsync(token);
try
{
    await UseLimitedDependencyAsync(token);
}
finally
{
    semaphore.Release();
}
```

The `AsyncConcurrencyLimiter` example wraps this rule in an idempotent lease used with `await using`. The lease prevents accidental double release, which would otherwise inflate the semaphore count or throw when the configured maximum is exceeded.

Do not dispose a semaphore while other operations may still wait on or release it. The owner must coordinate shutdown and worker completion first.

## Signals and Coordination

`AsyncManualResetEvent` demonstrates a reusable async signal built with `TaskCompletionSource`. `Set` releases all current and future waiters until `Reset`. Cancellation applies to an individual wait, not the shared signal. `RunContinuationsAsynchronously` prevents the signaling caller from running every waiter inline.

For synchronous thread coordination, .NET also provides `ManualResetEventSlim`, `AutoResetEvent`, `CountdownEvent`, and `Barrier`. These have different one/all waiter and phase semantics; they are not interchangeable with mutual exclusion.
