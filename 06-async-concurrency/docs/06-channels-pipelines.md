---
title: "Channels and Pipelines"
description: "Producer-consumer design, bounded capacity, backpressure, completion, failure propagation, and pipelines."
slug: dotnet-channels-pipelines
phase: 6
order: 6
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 32
topics: [channels, pipelines, backpressure]
prerequisites: [dotnet-parallelism-bounded-concurrency]
status: maintained
last-reviewed: 2026-08-15
---

# Channels and Pipelines

## Learning Objectives

- Model producers, buffering, workers, completion, and result collection as separate lifecycle stages.
- Choose bounded capacity and a full-mode policy intentionally.
- Propagate success, failure, and cancellation so neither side waits forever.
- Distinguish queue capacity from active worker concurrency.
- Preserve result ordering without requiring execution ordering.

`Channel<T>` coordinates asynchronous producers and consumers. A bounded channel makes capacity explicit: when full, a writer can wait, drop data according to policy, or reject input. Waiting provides backpressure so production cannot grow memory without limit.

Choose single-reader/single-writer options only when the topology guarantees them. The writer must complete, including on failure, so readers terminate. Consumers should drain with `ReadAllAsync` and observe propagated completion errors.

`System.IO.Pipelines` provides advanced buffer management for high-performance byte streams. Learn channels first; pipelines add parsing and buffer-lifetime contracts that should be justified by throughput needs.

## Pipeline Topology

A useful producer-consumer design names each responsibility:

```text
input enumeration -> bounded channel -> N workers -> indexed results / output channel
```

Capacity limits items waiting in the queue. Worker count limits active transformations. They solve
different resource problems and should be configured separately. A capacity of 100 with two workers
permits two active operations and roughly 100 queued items; it does not permit 100 concurrent calls.

Multiple stages can use separate channels when parsing, enrichment, and persistence have different
throughput or concurrency needs. Every additional buffer increases memory and complicates shutdown,
so introduce stages for a measurable or architectural reason.

## Bounded Full Modes

| Mode | Behavior when full | Appropriate only when |
|---|---|---|
| `Wait` | writer asynchronously waits | every accepted item must be processed |
| `DropNewest` | drops the newest buffered item | loss is explicit and observable |
| `DropOldest` | removes the oldest buffered item | fresh state supersedes stale state |
| `DropWrite` | rejects/drops the new write | producers tolerate loss by contract |

Dropping is a business decision, not a performance optimization to enable silently. Record dropped
counts and define what callers observe. For commands, payments, or audit events, durable external
messaging may be required; an in-memory channel disappears with the process.

## Completion Is Part of the Contract

The writer side owns completion. It should complete exactly once after production ends and attach
the production exception when it fails. Readers using `ReadAllAsync` then drain buffered items and
finish, or observe the completion failure.

```csharp
Exception? error = null;
try
{
    await ProduceItemsAsync(writer, token);
}
catch (Exception exception)
{
    error = exception;
    throw;
}
finally
{
    writer.TryComplete(error);
}
```

If consumers fail while a bounded producer is waiting, the producer also needs a signal to stop.
`ChannelWorkPool` links the caller token with pipeline-owned cancellation. A worker failure cancels
the producer and sibling workers, preventing a blocked `WriteAsync` from hanging the operation.

## Ordered Results With Concurrent Workers

Execution and completion can remain concurrent while returned results preserve input order. Assign
an index before enqueueing and let each worker write exactly one array slot. No lock is needed around
different slots when the array is allocated before workers start and observed only after all workers
finish.

If results must stream immediately in completion order, write them to a second output channel instead.
If event-time order is required, sequence numbers, watermarks, and reordering policies become part of
the domain contract.

## Cancellation and Partial Work

Cancellation stops waiting and asks participants to cooperate; it does not roll back completed side
effects. A pipeline that writes to external systems must define idempotency, checkpointing, retry, and
partial-success behavior. Passing one token everywhere is necessary but insufficient for transactional
semantics.

During shutdown:

1. stop accepting new external work;
2. complete input when graceful draining is desired, or cancel for immediate termination;
3. await producers and consumers;
4. record failures and incomplete items; and
5. dispose dependencies after workers have stopped.

## Channels Versus System.IO.Pipelines

`Channel<T>` transports typed messages between asynchronous participants. `System.IO.Pipelines`
manages high-performance byte buffers for streaming parsers and protocols. Pipelines require careful
handling of `ReadOnlySequence<byte>`, consumed/examined positions, incomplete messages, and buffer
lifetime. They are complementary: a socket parser may use `PipeReader` and publish parsed messages
through a channel.

## Observability

Capture queue depth or an approximation, enqueue wait duration, processing duration, active workers,
completion status, faults, cancellations, retries, and dropped items. Avoid high-cardinality labels
such as raw item IDs. Correlate pipeline metrics with downstream latency so backpressure is understood
as protection rather than misdiagnosed as the original bottleneck.

## Implementation Map

| Concern | Source | Tests |
|---|---|---|
| Single producer and consumer | `ChannelPipelineExample.cs` | `AsyncExamplesTests.cs` |
| Bounded multi-worker mapping | `ChannelWorkPool.cs` | `ChannelWorkPoolTests.cs` |
| Bounded fan-out without a queue | `BoundedExecutor.cs` | `AsyncExamplesTests.cs` |

## Review Questions

1. Why are channel capacity and worker count separate controls?
2. Who owns writer completion, and what happens if it is omitted?
3. How can a worker failure leave a producer blocked?
4. When is a dropping full mode safe?
5. Why does cancellation not undo external side effects?

## References

- [Channels in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/channels)
- [System.Threading.Channels API](https://learn.microsoft.com/en-us/dotnet/api/system.threading.channels)
- [System.IO.Pipelines](https://learn.microsoft.com/en-us/dotnet/standard/io/pipelines)

## Navigation

- Previous: [Parallelism and bounded concurrency](05-parallelism.md)
- Next: [Concurrent collections](07-concurrent-collections.md)
