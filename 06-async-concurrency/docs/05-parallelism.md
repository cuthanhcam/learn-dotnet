---
title: "Parallelism and Bounded Concurrency"
description: "CPU parallelism, async fan-out, Parallel APIs, ordering, degree limits, and workload suitability."
slug: dotnet-parallelism-bounded-concurrency
phase: 6
order: 5
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 28
topics: [parallelism, concurrency, semaphore]
prerequisites: [dotnet-synchronization-shared-state]
status: maintained
last-reviewed: 2026-08-15
---

# Parallelism and Bounded Concurrency

## Learning Objectives

- Separate CPU parallelism from asynchronous I/O concurrency.
- Decide whether a workload is large, independent, and expensive enough to parallelize.
- Bound work according to processor capacity or downstream dependency capacity.
- Aggregate through partition-local state instead of contending on every element.
- Preserve ordering only when the caller's contract requires it.

Parallelism is useful for sufficiently large independent CPU work. Coordination, partitioning, cache contention, and context switching can make small workloads slower. Measure representative input.

`Parallel.ForEach` targets synchronous CPU work. `Parallel.ForEachAsync` supports asynchronous delegates with bounded parallelism. For custom async fan-out, `SemaphoreSlim` can cap in-flight operations while an indexed result array preserves input order.

Unbounded `Select(async ...)` plus `WhenAll` eagerly creates all operations. That may overwhelm sockets, databases, rate limits, or memory even though the code is concise.

## CPU-Bound Versus I/O-Bound Work

CPU-bound work spends time executing instructions. Parallelism may reduce wall-clock time by using
multiple cores, but adds partitioning, scheduling, merging, cache, and synchronization costs.
Examples include image transforms, compression, numeric simulation, and large in-memory analysis.

I/O-bound work spends most of its lifetime waiting for an external system. Use naturally async APIs
and bound the number of in-flight calls according to the dependency's tested capacity. Wrapping an
already-async database or HTTP call in `Task.Run` consumes another thread without making the I/O
complete sooner.

## API Selection

| Workload | Starting API | Main control |
|---|---|---|
| Indexed synchronous CPU loop | `Parallel.For` | `MaxDegreeOfParallelism` |
| Synchronous CPU sequence | `Parallel.ForEach` | partitioning and local state |
| Async operation per item | `Parallel.ForEachAsync` | degree and cancellation |
| Query-style CPU transformation | PLINQ | ordering and merge behavior |
| Async results returned in input order | indexed tasks plus a bound | result mapping |
| Stream with queue backpressure | bounded `Channel<T>` | capacity plus worker count |

Do not expose thread count as an arbitrary tuning knob without documenting its meaning. CPU work
often starts near logical processor count; remote calls are bounded by measured latency, quotas,
connection pools, and service-level objectives rather than CPU count.

## Partition-Local Aggregation

Updating one shared accumulator for every element can serialize an otherwise parallel loop. The
`ParallelAggregation.SumOfSquares` example uses the overload of `Parallel.ForEach` with:

1. `localInit` to create one accumulator for a partition;
2. a body that updates only that local value; and
3. `localFinally` to merge once per partition with `Interlocked.Add`.

The merge operation must still be correct under concurrency. Checked arithmetic and conversion
order also matter: cast an `int` to `long` before multiplying if the result is intended to be `long`.

## Ordering and Determinism

Parallel execution order is normally nondeterministic. Preserve order only at an explicit boundary:

- write results into an array position associated with each input;
- call `AsOrdered()` in PLINQ when its cost is justified; or
- attach sequence numbers before entering a pipeline and reorder at collection time.

Do not use completion timing as a business ordering contract. Tests should assert the specified
result order, not which worker happened to run first.

## Failure and Cancellation

Parallel APIs may have several operations in flight when one fails. Cancellation is cooperative;
already-running delegates must observe the token or finish normally. Await or otherwise observe the
whole operation so exceptions do not become detached. Decide whether partial results are discarded,
returned with errors, or committed transactionally—there is no universal default.

## Performance Review

- Measure Release builds with representative sizes.
- Compare against a clear sequential baseline.
- Include small inputs, where parallel overhead often dominates.
- Check allocation, CPU utilization, throughput, and tail latency—not elapsed mean alone.
- Watch oversubscription when nested parallel APIs, request concurrency, or several service replicas
  already compete for the same processors.
- Keep a sequential implementation when the measured benefit does not justify added complexity.

## Implementation Map

| Concern | Source | Tests |
|---|---|---|
| Bounded async fan-out with ordered results | `BoundedExecutor.cs` | `AsyncExamplesTests.cs` |
| Partition-local CPU aggregation | `ParallelAggregation.cs` | `ParallelAggregationTests.cs` |
| Bounded queued worker pool | `ChannelWorkPool.cs` | `ChannelWorkPoolTests.cs` |

## Review Questions

1. Why is `Task.Run` generally unnecessary around naturally asynchronous I/O?
2. What costs can make a parallel loop slower than a sequential loop?
3. Why does partition-local aggregation reduce contention?
4. What determines a reasonable concurrency limit for a remote API?
5. How should partial work be handled after one parallel operation fails?

## References

- [Data parallelism with Task Parallel Library](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/data-parallelism-task-parallel-library)
- [`Parallel.ForEachAsync`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.foreachasync)
- [PLINQ introduction](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/introduction-to-plinq)

## Navigation

- Previous: [Synchronization and shared state](04-synchronization-shared-state.md)
- Next: [Channels and pipelines](06-channels-pipelines.md)
