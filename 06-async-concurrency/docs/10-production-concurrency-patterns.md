---
title: "Production Concurrency Patterns in .NET"
description: "Bounded fan-out, async memoization, producer-consumer pipelines, retries, idempotency, graceful shutdown, and deterministic testing."
phase: 6
order: 10
topics: [concurrency, channels, caching, retries, graceful-shutdown]
article-type: deep-dive
estimated-reading-minutes: 26
prerequisites: [tasks, cancellation, channels, concurrent-collections]
---

# Production Concurrency Patterns in .NET

Correct primitives are necessary but insufficient. Production workflows also need capacity limits, failure ownership, idempotency, observability, and shutdown behavior.

## Bounded Fan-Out

Launching one task per input eagerly can overload memory and dependencies. Bound in-flight operations with `Parallel.ForEachAsync`, a semaphore lease, a worker pool, or a bounded channel. Preserve input ordering only when the contract requires it; completion-order streaming can reduce latency and memory.

Concurrency limits and rate limits solve different problems. Concurrency bounds simultaneous work. A rate limit bounds starts or requests over time. One cannot generally substitute for the other.

## Async Memoization and Request Coalescing

Concurrent callers requesting the same expensive key can share one in-flight task. A plain `ConcurrentDictionary.GetOrAdd(key, factory)` may execute the factory more than once because value factories run outside internal locks.

`AsyncMemoizer<TKey,TValue>` stores `Lazy<Task<TValue>>` with `ExecutionAndPublication`, so the selected lazy instance starts one factory. Faulted or canceled tasks are removed with key-and-value matching to avoid permanently poisoning the cache or deleting a newer replacement.

Cancellation is subtle: if many callers share work, one caller's token should not necessarily cancel it for everyone. Separate shared-operation lifetime from individual caller wait cancellation according to product semantics.

## Producer-Consumer Pipelines

A bounded channel creates backpressure. Always define:

- capacity and full mode;
- number of writers/readers;
- item ordering requirements;
- who completes the writer;
- how producer failure reaches consumers;
- whether shutdown drains or abandons queued work;
- how per-item failures are represented.

Multi-stage pipelines need a completion chain. Each stage completes its output only after its input is exhausted and its workers finish. Failure should cancel siblings and complete downstream writers so no reader waits forever.

## Retry, Backoff, and Idempotency

Retry only transient failures and operations safe to repeat. Use exponential backoff with jitter to prevent synchronized retry storms. Respect server hints such as `Retry-After`. Bound total attempts and elapsed budget, propagate cancellation, and record attempt outcomes.

Idempotency may come from the operation itself, a deduplication key, conditional update, transaction, or outbox/inbox design. “HTTP call failed” does not prove the server performed no side effect.

## Graceful Shutdown

Stop accepting new work, signal cancellation, complete producers, drain or abandon according to policy, await workers, observe failures, then dispose resources. Set a shutdown deadline at the application boundary. Do not dispose channels, semaphores, clients, or scopes while workers still use them.

## Concurrent Collections

Use atomic APIs such as `GetOrAdd`, `AddOrUpdate`, and `TryUpdate`, but understand their delegate semantics. Factories can execute multiple times. Enumeration is thread-safe but represents a moving view rather than a transactionally consistent snapshot.

Choose `ConcurrentQueue`/`ConcurrentStack` for immediate non-blocking-style operations, `ConcurrentBag` for unordered thread-local-friendly workloads, and `Channel<T>` when asynchronous waiting, capacity, or completion matters.

## Observability

Measure queue length, active workers, wait duration, processing duration, throughput, error classification, retry count, cancellation, drops, and shutdown duration. Avoid high-cardinality labels such as raw IDs. Correlate work across stages without relying on thread identity because async continuations move between threads.

## Testing Strategy

- Replace wall-clock sleeps with explicit gates and completion sources.
- Assert peak active count for bounded work.
- Inject factory, producer, consumer, and shutdown failures.
- Test cancellation before start, while queued, and during execution.
- Verify every task is awaited or deliberately supervised.
- Run repeated stress tests separately from deterministic unit tests.
- Test that faulted cache entries can recover and successful requests coalesce.

## Implementation Map

- `Collections/AsyncMemoizer.cs`: concurrent request coalescing and failure eviction.
- `Channels/ChannelPipelineExample.cs`: bounded single-stage flow.
- `Synchronization/BoundedExecutor.cs`: ordered bounded fan-out.
- `Exercises/AsyncRetry.cs`: selective retry baseline.
- `AsyncMemoizerTests.cs`: single execution and recovery after failure.

## Review Questions

1. What resource is each concurrency limit protecting?
2. Can one caller cancel work shared with other callers?
3. Who completes every channel, including failure paths?
4. Is the retried operation provably safe to repeat?
5. Does shutdown drain, reject, or abandon queued work?
6. Which metrics reveal saturation before users see timeouts?

## References

- [Asynchronous programming scenarios](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios)
- [Thread-safe collections](https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/)
- [Channels library](https://learn.microsoft.com/en-us/dotnet/core/extensions/channels)

## Navigation

[← Coordination primitives](09-coordination-primitives-deep-dive.md) · [Common pitfalls →](common-pitfalls.md)
