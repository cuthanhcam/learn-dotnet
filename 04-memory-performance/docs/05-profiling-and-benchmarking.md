# Profiling and Benchmarking

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
