---
title: "Profiling and Benchmarking"
description: "Baselines, allocation metrics, profilers, BenchmarkDotNet, and experimental discipline."
slug: dotnet-profiling-and-benchmarking
phase: 4
order: 5
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 38
topics: [dotnet, profiling, benchmarking]
prerequisites: [dotnet-allocation-patterns]
status: maintained
last-reviewed: 2026-08-15
---

# Profiling and Benchmarking

## Learning Objectives

After completing this article, you should be able to:

- translate a vague performance complaint into a measurable scenario and budget;
- select metrics that distinguish CPU, allocation, retention, GC, and latency problems;
- choose between application telemetry, runtime counters, traces, GC dumps, process dumps, and microbenchmarks;
- run a repeatable investigation without changing several variables at once; and
- preserve diagnostic artifacts safely because they can contain production data.

## The Rule

Performance work without measurement is guessing. Sometimes the guess is right, but you do not know that until you measure.

A good performance workflow:

1. Define the scenario.
2. Measure the baseline.
3. Make one targeted change.
4. Measure again using the same workload.
5. Keep the change only if the improvement is worth the complexity.

## What To Measure

Common metrics:

- elapsed time
- allocated bytes
- GC collection counts
- CPU usage
- memory size over time
- throughput
- latency percentiles

For this module, the most useful beginner metrics are elapsed time and allocated bytes.

## Simple Local Measurement

`Stopwatch` is useful for rough local comparisons:

```csharp
var stopwatch = Stopwatch.StartNew();
DoWork();
stopwatch.Stop();
Console.WriteLine(stopwatch.Elapsed);
```

Problems:

- JIT warmup affects first runs
- background processes add noise
- small operations are hard to measure
- Debug builds distort results
- CPU frequency scaling can affect timing

Use it for quick exploration, not final conclusions.

## Allocation Deltas

`GC.GetAllocatedBytesForCurrentThread()` is excellent for learning:

```csharp
long before = GC.GetAllocatedBytesForCurrentThread();
DoWork();
long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
```

This tells you how many bytes were allocated by the current thread during the measured section.

Limitations:

- thread-specific
- not a full process memory profile
- does not show retained memory
- can include measurement overhead if used carelessly

The example `ProfilingExample.Measure()` combines elapsed time, allocation deltas, and GC counts.

Do not use the current-thread counter around an `await`: an asynchronous continuation may
resume on another thread. The repository's `MeasureAsync()` uses the runtime-wide
`GC.GetTotalAllocatedBytes()` counter for a coarse demonstration and documents the tradeoff
that unrelated process activity can contribute to the result.

## BenchmarkDotNet

BenchmarkDotNet is the standard .NET microbenchmarking library. It handles warmup, repeated iterations, statistics, environment info, and memory diagnostics.

Run this module's benchmarks:

```bash
dotnet run -c Release --project benchmarks/MemoryPerformance.Benchmarks
```

Use Release mode. Debug mode is not representative.

## Reading Benchmark Results

Typical columns:

| Column | Meaning |
| --- | --- |
| Mean | Average execution time |
| Error | Uncertainty range |
| StdDev | Variation between measurements |
| Ratio | Comparison to baseline |
| Gen0/Gen1/Gen2 | Collections per operation scale |
| Allocated | Bytes allocated per operation |

Do not overreact to tiny differences. A 2 percent change in a microbenchmark may be noise or may not matter in the real app.

## Microbenchmark Traps

Watch for:

- dead-code elimination
- unrealistic input sizes
- measuring setup instead of the operation
- comparing Debug code
- using one data shape only
- ignoring readability and maintenance cost
- optimizing code that is not on a hot path

## Profiling Real Applications

Microbenchmarks answer "which implementation is faster for this isolated workload?"

Profilers answer "where is my application spending time and memory?"

Useful tools:

- Visual Studio Performance Profiler
- JetBrains dotTrace and dotMemory
- `dotnet-counters`
- `dotnet-trace`
- `dotnet-gcdump`
- PerfView

For backend systems, combine runtime metrics with request traces and production-like load tests.

## Choose Evidence Before Choosing a Tool

Different artifacts answer different questions. Capturing the largest artifact first often
adds overhead and analysis time without improving the diagnosis.

| Question | Start with | Escalate to |
|---|---|---|
| Is CPU, allocation rate, exception rate, or GC changing? | application metrics and `dotnet-counters` | `dotnet-trace` |
| Which code consumes CPU? | `dotnet-trace` or an IDE CPU profiler | a process dump for stuck-state analysis |
| Is the managed heap growing because objects remain reachable? | repeated runtime metrics | `dotnet-gcdump`, then a process dump if necessary |
| Is one isolated implementation faster? | BenchmarkDotNet | an end-to-end load test |
| Did request tail latency regress? | request metrics and distributed traces | runtime trace correlated with the workload |
| Is the process deadlocked or hung? | stack snapshot | process dump |

Metrics show trends. Traces show activity over an interval. A dump shows detailed process
state at one instant. A GC dump focuses on the managed object graph. A benchmark compares
controlled implementations; it does not prove production impact.

## A Repeatable Investigation Workflow

### 1. Define the user-visible symptom

Record the operation, workload, environment, expected service-level objective, observed
percentiles, and the time window. “The API is slow” is not yet an actionable statement;
“checkout p99 increased from 250 ms to 900 ms under 300 requests/second” is.

### 2. Reproduce or identify the window

Use representative input sizes and concurrency. Preserve the deployed runtime version,
GC configuration, container limits, architecture, and external dependencies. If local
reproduction is impossible, correlate production telemetry around a narrowly defined window.

### 3. Observe low-overhead signals

Install diagnostic tools at a version compatible with the target runtime:

```bash
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-trace
dotnet tool install --global dotnet-gcdump
dotnet tool install --global dotnet-dump
```

List .NET processes, then monitor the selected process:

```bash
dotnet-counters ps
dotnet-counters monitor --process-id <PID> --counters System.Runtime
```

Watch trends under a known workload. A single counter value rarely establishes causality.
Useful runtime signals include CPU usage, allocation rate, GC heap size, collection counts,
time spent in GC, exception count, thread-pool queue length, and thread count.

### 4. Capture focused evidence

For a CPU or timing investigation, collect a bounded trace:

```bash
dotnet-trace collect --process-id <PID> --duration 00:00:30
```

For managed-heap retention, capture snapshots at comparable workload points:

```bash
dotnet-gcdump collect --process-id <PID> --output before.gcdump
dotnet-gcdump collect --process-id <PID> --output after.gcdump
```

`dotnet-gcdump` triggers a collection to reconstruct the managed object graph. That makes it
useful for type counts, sizes, and roots, but unsuitable as a zero-impact operation or as a
complete view of native memory. Use a process dump when thread state, native memory, or richer
object inspection is required.

### 5. Form and test one hypothesis

Examples of testable hypotheses:

- repeated JSON buffering drives allocation rate and Gen 0 frequency;
- an unbounded cache retains request-specific graphs;
- synchronous blocking causes thread-pool queue growth and tail latency;
- one parsing implementation dominates CPU for large payloads.

Change one relevant variable, repeat the same measurement, and compare both performance and
correctness. Record unsuccessful experiments as well; they prevent the team from repeating
the same investigation.

## Memory Investigation Patterns

### High allocation rate is not automatically a leak

Allocation rate measures how quickly new managed objects are created. Heap size measures the
currently observed managed heap. Retained size describes memory kept alive through references.
A service can allocate heavily while maintaining a stable live set, or allocate slowly while
an unbounded cache steadily retains objects.

Collect comparable evidence after warmup:

1. apply the same workload for a fixed interval;
2. observe allocation rate, heap size, and Gen 2 collections;
3. capture more than one heap snapshot at equivalent points;
4. compare types whose count or retained size grows; and
5. inspect their root paths before changing code.

### Process memory is larger than the managed heap

The process also contains native allocations, loaded images, JIT-compiled code, thread stacks,
memory-mapped files, runtime bookkeeping, and reserved address space. Do not label the
difference between process working set and GC heap size a managed leak without further evidence.

## Production Safety

- Reproduce in a non-production environment when possible.
- Bound collection duration and estimate storage before capture.
- Expect tracing, heap walking, and dumps to add different levels of overhead.
- Treat dumps and traces as sensitive artifacts: memory may contain credentials, personal data,
  request bodies, connection strings, and cryptographic material.
- Encrypt artifacts, restrict access, define retention, and delete them according to policy.
- Match tool architecture and permissions to the target process; container and diagnostic-port
  configuration can also affect attachment.
- Record UTC timestamps, process ID, deployment version, workload, command, and tool version with
  every artifact so another engineer can reproduce the analysis.

## Benchmark Review Checklist

Before accepting a microbenchmark result, verify that:

- the benchmark runs an optimized build outside the debugger;
- setup and input construction are separated from the operation being compared;
- the result is consumed, so the work remains observable;
- inputs represent normal, boundary, and adversarial production shapes;
- both time and allocation columns are examined;
- asynchronous benchmarks return and await `Task` or `ValueTask` correctly;
- the alleged improvement survives repeated runs and an end-to-end measurement; and
- the new complexity is justified by the relevant performance budget.

## Decision Checklist

Before keeping an optimization, ask:

- Did it improve the measured scenario?
- Is the scenario important enough?
- Is the code still maintainable?
- Did it add ownership risks?
- Did it hurt other scenarios?
- Is there a simpler architectural fix?

## Practice

1. Run `ProfilingExample.Run()`.
2. Add a measured action that uses `string.Split`.
3. Add another action that parses with spans.
4. Compare allocated bytes.
5. Move the comparison into BenchmarkDotNet.

## Further Reading

- [Diagnostics in .NET](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/)
- [.NET diagnostic tools](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/tools-overview)
- [`dotnet-counters`](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters)
- [`dotnet-trace`](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-trace)
- [`dotnet-gcdump`](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-gcdump)
- [Garbage collection fundamentals](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals)

## Continue Learning

- Previous: [Span, memory, and pooling](04-span-memory-pooling.md)
- Review: [Common memory and performance pitfalls](common-pitfalls.md)
- Practice: [Phase 04 exercises](../exercises/MemoryPerformance.Exercises/)
