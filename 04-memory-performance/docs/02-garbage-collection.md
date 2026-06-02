# Garbage Collection

## What This Chapter Covers

This chapter explains how .NET reclaims managed memory and why collection activity matters when you are investigating latency or throughput issues.

You will see:

- why the GC exists
- how generations work
- what `Dispose` does and does not do
- which APIs are useful for observing collection behavior

## Why GC Exists

.NET uses garbage collection so application code does not have to manually free managed memory. That removes an entire class of memory-management bugs, but it does not eliminate responsibility for non-memory resources.

The GC handles managed objects. It does not magically close files, release sockets, or dispose native handles for you in a timely way.

## Generations

The GC groups objects by age.

- Gen 0: new, short-lived objects
- Gen 1: objects that survived one collection
- Gen 2: long-lived objects

Short-lived allocations are common and often cheap. The expensive case is sustained pressure that keeps promoting objects into older generations.

The `MemoryPerformance.Examples.GarbageCollectionExample` class creates temporary customer objects to show that allocation pressure changes GC behavior even when the objects are immediately discarded.

## Finalization and Dispose

Some resources are not managed memory at all. File handles, sockets, and native buffers should be released deterministically with `IDisposable`.

```csharp
using var buffer = MemoryPerformanceExample.CreateDisposableBuffer("session", 512);
```

Use finalizers only as a safety net, not as the main cleanup strategy. They exist for rare fallback cases, not for normal control flow.

The example `DisposableBuffer` type is intentionally small so you can see the cleanup pattern clearly.

## Measuring GC Activity

Useful signals include:

- `GC.CollectionCount(0)`, `GC.CollectionCount(1)`, `GC.CollectionCount(2)`
- `GC.GetTotalMemory(false)` for a rough heap snapshot
- `GC.GetAllocatedBytesForCurrentThread()` for allocation tracking on one thread

These values are most useful when compared before and after a specific workload.

## Interpreting Results

A rise in `GC.CollectionCount(0)` usually means the code is creating more short-lived garbage. That is not always bad, but it is worth investigating if it happens in a hot path.

`GC.GetTotalMemory(false)` is a broad snapshot. It can move around for reasons that are not directly tied to one tiny code block, so treat it as a trend indicator rather than a precise truth source.

## Practical Rule

If a change reduces allocations but makes the code much harder to understand, keep the simpler version unless profiling shows the memory cost actually matters.

The right tradeoff is usually:

- readable code first
- measured bottleneck second
- targeted optimization third
