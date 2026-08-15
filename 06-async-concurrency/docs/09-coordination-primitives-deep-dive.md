---
title: ".NET Coordination Primitives: Choosing by Invariant"
description: "A detailed comparison of Interlocked, Monitor, SemaphoreSlim, Semaphore, Mutex, reader-writer locks, signals, barriers, spinning, and async coordination."
phase: 6
order: 9
topics: [concurrency, synchronization, semaphore, locks, signals]
article-type: deep-dive
estimated-reading-minutes: 30
prerequisites: [threads, task-based-asynchronous-pattern, race-conditions]
---

# .NET Coordination Primitives

Synchronization should begin with the invariant, not a favorite primitive. The safest shared state is often state that was eliminated, made immutable, partitioned, confined to one owner, or communicated through messages.

## Learning Objectives

- Separate mutual exclusion, admission control, signaling, and phase coordination.
- Choose between atomic operations and critical sections.
- Use `SemaphoreSlim.WaitAsync` with correct acquisition/release ownership.
- Explain when kernel primitives are necessary.
- Identify reader-writer upgrade and lock-order hazards.
- Understand why spinning is specialized rather than broadly “faster.”
- Design deterministic tests for coordination behavior.

## Primitive Selection Matrix

| Primitive | Primary semantic | Async wait | Cross-process | Ownership/fairness notes |
|---|---|---:|---:|---|
| `Interlocked` | Atomic single-location transition | N/A | No | No compound invariant |
| `Volatile` | Visibility/order for field access | N/A | No | Does not make `++` atomic |
| `lock` / `Monitor` | Exclusive synchronous critical section | No | No | Monitor is reentrant; do not await inside |
| `SemaphoreSlim` | In-process permit count | Yes | No | Not ownership-aware; release must match acquisition |
| `Semaphore` | Kernel permit count | No TAP wait | Yes when named | Higher overhead; OS handle |
| `Mutex` | Kernel mutual exclusion | No TAP wait | Yes when named | Thread-affine ownership, abandoned mutex behavior |
| `ReaderWriterLockSlim` | Multiple readers or one writer | No | No | Upgradeable read complexity; recursion configurable |
| `ManualResetEventSlim` | Release all synchronous waiters until reset | No | No | Can inflate to kernel event when needed |
| `AutoResetEvent` | Release one synchronous waiter per signal | No | No | Signals can coalesce; not a work queue |
| `CountdownEvent` | Wait until count reaches zero | No | No | One phase of fan-in coordination |
| `Barrier` | Reusable multi-participant phases | No | No | Participant count/loss must be managed |
| `Channel<T>` | Async queue, capacity, completion | Yes | No | Best for producer/consumer ownership |

## Atomic Operations

`Interlocked` combines read, computation, and write for supported operations. Compare-and-exchange enables lock-free state machines: read a snapshot, compute a candidate, then replace only if the snapshot is unchanged. Retry on interference.

Atomic does not automatically mean lock-free progress for an entire algorithm, and lock-free does not mean wait-free. Complex invariants spanning multiple locations usually need a different representation or a critical section.

## Monitor-Based Mutual Exclusion

Use `lock` for a short synchronous invariant:

```csharp
lock (_gate)
{
    // Read and update all fields that form one invariant.
}
```

Do not expose the gate, lock on `this`, strings, or public type objects. Avoid unknown callbacks and blocking I/O while holding the lock. If multiple gates are acquired, define one global order.

The C# compiler prevents a direct `await` inside `lock`, reflecting the mismatch between thread-affine monitor ownership and asynchronous suspension.

## Semaphore and SemaphoreSlim

A semaphore controls a number of concurrent entrants. It is not a mutex unless configured with one permit, and even then it does not enforce owner-only release.

`SemaphoreSlim` is optimized for in-process coordination and exposes `WaitAsync`. Correct use requires:

```csharp
await semaphore.WaitAsync(token);
try
{
    await operation(token);
}
finally
{
    semaphore.Release();
}
```

Do not place `WaitAsync` inside the `try` and unconditionally release: cancellation before acquisition would release a permit never owned. The `AsyncConcurrencyLimiter` wraps successful acquisition in an idempotent lease.

A named `Semaphore` is relevant for cross-process coordination but uses OS resources and synchronous wait APIs. It is not interchangeable with `SemaphoreSlim` merely because both count permits.

## ReaderWriterLockSlim

Reader-writer locking can help when reads are frequent, protected work is substantial, and writes are rare. It can be slower than a simple lock for small sections. Upgradeable read mode permits at most one potential upgrader and avoids the classic two-readers-both-upgrade deadlock.

It has no asynchronous acquisition. Never hold it across `await`. For asynchronous read-mostly designs, consider immutable snapshots, copy-on-write, a single owner, or specialized async reader-writer coordination only after measurement.

## Signals and Phases

Manual-reset signals release all waiters and remain set. Auto-reset signals release at most one waiter and reset automatically. A signal indicates state; it does not carry an unbounded count of work items. Use a queue/channel for work.

`CountdownEvent` coordinates one completion count. `Barrier` coordinates repeated phases among known participants. Participant failure or cancellation needs an explicit policy to prevent remaining participants waiting forever.

The custom `AsyncManualResetEvent` uses `TaskCompletionSource` with asynchronous continuations. It is educational; production code should prefer established library abstractions when available and extensively stress-test custom primitives.

## Spinning

`SpinWait` and `SpinLock` avoid a kernel wait when contention is expected to be extremely brief. Spinning consumes CPU, behaves poorly on a single core, and can worsen oversubscription. Never spin while waiting for I/O or an operation that needs the same saturated workers.

## Deadlock, Livelock, Starvation, and Fairness

- Deadlock: participants form a wait cycle and none can progress.
- Livelock: participants keep reacting but make no useful progress.
- Starvation: one participant is repeatedly denied progress.
- Fairness: acquisition order guarantees; many high-performance primitives do not promise strict FIFO fairness.

Timeouts detect that progress took too long; they do not repair a broken invariant or prove deadlock freedom.

## Testing Coordination

- Count active entrants to verify a concurrency limit.
- Use `TaskCompletionSource` or channels to hold/release workers deterministically.
- Cancel before and during acquisition.
- Inject exceptions inside protected work and prove permit release.
- Dispose a lease twice and prove no over-release.
- Repeat stress scenarios to increase interleaving coverage, while retaining deterministic contract tests.

## Implementation Map

- `Synchronization/ThreadSafeCounter.cs`: atomic increment and volatile read.
- `Synchronization/BoundedExecutor.cs`: explicit semaphore/finally pattern.
- `Synchronization/AsyncConcurrencyLimiter.cs`: idempotent lease abstraction.
- `Synchronization/AsyncManualResetEvent.cs`: shared asynchronous signal.
- `AdvancedCoordinationTests.cs`: limits, double disposal, reset, and per-wait cancellation.

## Review Questions

1. Why is `SemaphoreSlim` admission control rather than general state protection?
2. Why must release occur only after successful acquisition?
3. When can reader-writer locking be slower than a simple monitor?
4. Why can an auto-reset event lose the meaning of multiple rapid signals?
5. What progress guarantee does your selected primitive actually provide?

## References

- [Semaphore and SemaphoreSlim](https://learn.microsoft.com/en-us/dotnet/standard/threading/semaphore-and-semaphoreslim)
- [Overview of synchronization primitives](https://learn.microsoft.com/en-us/dotnet/standard/threading/overview-of-synchronization-primitives)
- [Thread-safe collections](https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/)

## Navigation

[← Deadlocks and context](08-deadlocks-context.md) · [Production concurrency patterns →](10-production-concurrency-patterns.md)
