---
title: "Synchronization and Shared State"
description: "Race conditions, atomicity, memory visibility, lock, Interlocked, SemaphoreSlim, and invariant protection."
slug: dotnet-synchronization-shared-state
phase: 6
order: 4
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 32
topics: [concurrency, synchronization, locks]
prerequisites: [dotnet-threading-threadpool]
status: maintained
last-reviewed: 2026-08-15
---

# Synchronization and Shared State

## Learning Objectives

After completing this article, you should be able to:

- identify the invariant that must change atomically instead of merely identifying a field;
- distinguish atomicity, visibility, ordering, mutual exclusion, signaling, and admission control;
- select `Interlocked`, `lock`, an async mutex, `SemaphoreSlim`, or message passing intentionally;
- define ownership and disposal rules for synchronization primitives; and
- write deterministic tests that force relevant interleavings without relying on arbitrary sleeps.

A race exists when correctness depends on uncontrolled operation ordering. `counter++` is a read-modify-write sequence, not one atomic operation. Use `Interlocked` for supported single-value transitions, `lock` for short synchronous critical sections protecting a compound invariant, and `SemaphoreSlim` for asynchronous admission control.

Never `await` inside `lock`. Keep critical sections small, avoid calling unknown code while holding a lock, and establish one global lock order when multiple locks are unavoidable.

Synchronization also provides memory-ordering guarantees. `volatile` affects visibility and ordering for a field but does not make compound operations atomic.

## Start With the Invariant

Suppose a withdrawal requires `balance >= amount` and then subtracts the amount. Protecting only
the subtraction does not make the check-and-update pair atomic. Two callers can both observe the
same old balance and both proceed. The protected unit is the business invariant, not whichever
line happens to write a field.

Ask four questions before choosing a primitive:

1. Which values must be observed and changed as one transition?
2. Can the state be immutable, partitioned, confined to one owner, or sent through a channel?
3. Must the critical section await asynchronous work?
4. Who owns the primitive, and how does shutdown guarantee that no waiter or lease remains?

The strongest design often removes shared mutation. Synchronization is required only after
ownership and data flow have been made as simple as practical.

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

## Async Mutual Exclusion

The C# `lock` statement is the right default for a short synchronous critical section. It cannot
contain `await`, because suspension would retain exclusive ownership for an unbounded time and
the continuation may resume elsewhere.

When an invariant genuinely spans asynchronous work, an async mutex can be built from a
single-permit `SemaphoreSlim`. The repository's `AsyncLock` returns an idempotent lease:

```csharp
await using AsyncLock.Releaser lease = await mutex.EnterAsync(token);
await UpdateProtectedStateAsync(token);
```

This is mutual exclusion, not throttling: exactly one operation enters. `AsyncLock` is deliberately
non-reentrant. A method that already owns it must not call another path that attempts to acquire
the same instance, or it will wait for itself indefinitely.

An async lock should not become permission to place slow network calls inside every critical
section. Prefer computing outside the lock and committing a small state transition inside it.
Calling callbacks, events, or unknown application code while holding exclusive access can create
reentrancy, long hold times, or lock-order cycles.

## Atomic Operations and Compare-Exchange Loops

`Interlocked` is ideal when the complete invariant is one supported atomic transition:

- incrementing a counter;
- exchanging a reference;
- adding a numeric delta; or
- replacing a value only if it still equals an observed snapshot.

A compare-exchange loop follows this shape:

```csharp
int observed;
do
{
    observed = Volatile.Read(ref maximum);
    if (candidate <= observed)
    {
        return;
    }
}
while (Interlocked.CompareExchange(ref maximum, candidate, observed) != observed);
```

The body may run more than once under contention, so it must not perform irreversible side effects.
For a multi-field invariant, a normal lock is usually clearer than composing several atomics.

## Memory Visibility Is Not Atomicity

Compilers, CPUs, caches, and the runtime may reorder or delay observations while preserving the
single-threaded rules of the language. Synchronization operations establish ordering relationships
that allow one participant to publish state and another to observe it safely.

`Volatile.Read` and `Volatile.Write` are useful for simple publication flags and snapshots. They do
not turn `value++`, check-then-act, or updates across multiple fields into atomic transitions.
Similarly, a thread-safe collection protects its operations but not an arbitrary workflow made of
several calls.

## Signals and Coordination

`AsyncManualResetEvent` demonstrates a reusable async signal built with `TaskCompletionSource`. `Set` releases all current and future waiters until `Reset`. Cancellation applies to an individual wait, not the shared signal. `RunContinuationsAsynchronously` prevents the signaling caller from running every waiter inline.

For synchronous thread coordination, .NET also provides `ManualResetEventSlim`, `AutoResetEvent`, `CountdownEvent`, and `Barrier`. These have different one/all waiter and phase semantics; they are not interchangeable with mutual exclusion.

## Lifetime and Disposal Rules

- The component that creates a primitive normally owns its disposal.
- Stop accepting work before disposal.
- Cancel pending work, complete producers, and await workers.
- Dispose only after no operation can acquire, wait on, release, or signal the primitive.
- Never dispose a dependency-owned semaphore merely because one method finished using it.

Disposal racing with `WaitAsync` or `Release` is a lifecycle defect. A boolean disposed flag can make
errors clearer, but it does not replace coordinated shutdown.

## Testing Synchronization

Prefer explicit signals such as `TaskCompletionSource` or `AsyncManualResetEvent`:

1. let workers announce that they entered;
2. hold them at a known barrier;
3. assert the maximum active count or protected state;
4. release the barrier; and
5. await every worker and verify final invariants.

Also test canceled acquisition, failure inside the critical section, repeated lease disposal, and
shutdown. A stress loop can increase confidence, but it does not prove that a race is absent.

## Implementation Map

| Concern | Source | Executable specification |
|---|---|---|
| Atomic counter | `ThreadSafeCounter.cs` | `AsyncExamplesTests.cs` |
| Bounded admission with a lease | `AsyncConcurrencyLimiter.cs` | `AdvancedCoordinationTests.cs` |
| Async mutual exclusion | `AsyncLock.cs` | `AsyncLockTests.cs` |
| Reusable broadcast signal | `AsyncManualResetEvent.cs` | `AdvancedCoordinationTests.cs` |

## Review Questions

1. Why does making the balance field `volatile` not fix a withdrawal race?
2. When is `lock` preferable to an async mutex?
3. At what exact point does semaphore ownership begin?
4. Why must a lease guard against double release?
5. How can unknown code inside a critical section create deadlock or reentrancy?

## References

- [`lock` statement](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock)
- [Interlocked operations](https://learn.microsoft.com/en-us/dotnet/api/system.threading.interlocked)
- [Semaphore and SemaphoreSlim](https://learn.microsoft.com/en-us/dotnet/standard/threading/semaphore-and-semaphoreslim)
- [Managed threading best practices](https://learn.microsoft.com/en-us/dotnet/standard/threading/managed-threading-best-practices)

## Navigation

- Previous: [Threads and the thread pool](03-threading-threadpool.md)
- Next: [Parallelism and bounded concurrency](05-parallelism.md)
- Related deep dive: [Coordination primitives](09-coordination-primitives-deep-dive.md)
