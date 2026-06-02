# Profiling and Benchmarking

## What This Chapter Covers

This chapter is about measurement discipline. It separates “this looks faster” from “this is actually faster for the workload I care about.”

The module uses a simple console benchmark runner so you can compare patterns without needing a full benchmark framework first.

## Measure the Right Thing

There is a difference between observing allocations and proving a real improvement.

Use the following tools for the right question:

- `GC.GetAllocatedBytesForCurrentThread()` to compare two code paths on one thread
- `GC.CollectionCount()` to see whether your workload is causing extra collections
- `Stopwatch` for quick local experiments
- benchmark projects for repeatable comparisons

The `PerformanceMeasurementExample.MeasureAllocations()` helper demonstrates the current-thread allocation snapshot pattern.

## Benchmark Runner in This Module

The benchmark console app compares several common tradeoffs:

- value type loops versus reference type loops
- boxing versus direct value handling
- span-based work versus string slicing
- pooled buffers versus new array allocation
- allocation pressure from repeated object creation

That makes the benchmark output useful for teaching, not just for timing.

## Benchmark Rules

- warm up the code before drawing conclusions
- compare equivalent workloads
- avoid I/O in the measured section
- keep the benchmark small enough to understand and large enough to matter

## What Not To Conclude

Do not assume a single benchmark run proves a universal result.

Also do not assume the fastest version is always the best version. A slightly slower implementation that is much easier to maintain can be the correct choice if the measured difference is tiny.

## Good Questions

- Does this change reduce allocations per operation?
- Does it also improve throughput?
- Did it make the code harder to maintain?
- Is the difference large enough to justify the tradeoff?

## Practical Outcome

By the end of this chapter, you should be comfortable reading benchmark output and using it to make a bounded, evidence-based decision instead of a guess.
