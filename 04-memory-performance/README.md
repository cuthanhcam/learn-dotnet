# Memory & Performance (04-memory-performance)

> A practical deep dive into .NET memory behavior, allocation costs, garbage collection, spans, pooling, and measurement.

## Overview

This module focuses on performance literacy rather than premature optimization. The goal is to understand what the runtime is doing, identify the allocation patterns that matter, and prove improvements with measurements.

You will learn:

- Stack frames, managed heap objects, value semantics, and reference semantics
- Garbage collection generations, object reachability, promotion, LOH, and disposal
- Hidden allocations from boxing, strings, closures, LINQ, iterators, and async patterns
- `Span<T>`, `ReadOnlySpan<T>`, `Memory<T>`, `stackalloc`, and `ArrayPool<T>`
- How to measure elapsed time, allocated bytes, and GC pressure
- How to use benchmarks without over-reading noisy microbenchmark results

## Setup

```bash
cd 04-memory-performance
dotnet --version
dotnet build
dotnet run --project src/MemoryPerformance.ConsoleApp
dotnet run --project exercises/MemoryPerformance.Exercises
dotnet test
dotnet run -c Release --project benchmarks/MemoryPerformance.Benchmarks
```

## Project Structure

```text
04-memory-performance/
|
|-- 04-memory-performance.slnx
|-- README.md
|
|-- src/
|   |-- MemoryPerformance.ConsoleApp/
|   |-- MemoryPerformance.Examples/
|       |-- MemoryModel/
|       |-- GarbageCollection/
|       |-- AllocationPatterns/
|       |-- SpanMemoryPooling/
|       |-- Profiling/
|
|-- exercises/
|   |-- MemoryPerformance.Exercises/
|
|-- tests/
|   |-- MemoryPerformance.Tests/
|
|-- benchmarks/
|   |-- MemoryPerformance.Benchmarks/
|
|-- docs/
    |-- 00-roadmap.md
    |-- 01-memory-model.md
    |-- 02-garbage-collection.md
    |-- 03-allocation-patterns.md
    |-- 04-span-memory-pooling.md
    |-- 05-profiling-and-benchmarking.md
    |-- common-pitfalls.md
```

## What's Inside

| Topic | Focus |
| --- | --- |
| Memory model | Stack frames, heap objects, values, references, copying |
| Garbage collection | Generations, reachability, finalization, disposal, LOH |
| Allocation patterns | Boxing, strings, closures, iterators, LINQ, defensive copies |
| Span and pooling | Zero-copy slicing, stack scratch buffers, rented arrays |
| Profiling | Allocation deltas, GC counts, `Stopwatch`, BenchmarkDotNet |
| Exercises | Small drills that reinforce allocation-aware coding |

## Module Rules

1. Examples expose deterministic methods that tests can verify.
2. `Run()` methods are for interactive learning and console output.
3. Exercises are intentionally small and self-contained.
4. Benchmarks compare realistic alternatives, not toy tricks alone.
5. Optimization decisions should be backed by measurement.

## Study Order

1. Read `docs/00-roadmap.md`.
2. Run the console app and predict the output before reading the code.
3. Read each doc chapter with the matching example file open.
4. Run the tests to see which behaviors are considered important.
5. Run benchmarks in Release mode and compare allocation columns.
6. Revisit earlier modules and look for allocation patterns you now understand.

## Key Mental Models

- Value type vs reference type is about copy semantics.
- Stack vs heap is about storage and lifetime, not a moral ranking.
- GC handles managed memory, not every external resource.
- `IDisposable` gives deterministic cleanup for resources such as files, sockets, handles, pooled objects, and timers.
- `Span<T>` is a temporary view. It does not own memory.
- `ArrayPool<T>` reduces repeated buffer allocation but introduces ownership rules.
- A faster-looking change is not a performance improvement until measured.

## Common Commands

```bash
# Build everything in this module
dotnet build

# Run demonstrations
dotnet run --project src/MemoryPerformance.ConsoleApp

# Run exercises
dotnet run --project exercises/MemoryPerformance.Exercises

# Run tests
dotnet test

# Run benchmarks
dotnet run -c Release --project benchmarks/MemoryPerformance.Benchmarks
```

## Documentation

| File | Purpose |
| --- | --- |
| `00-roadmap.md` | Learning path and completion criteria |
| `01-memory-model.md` | Stack, heap, value/reference semantics, lifetime |
| `02-garbage-collection.md` | GC generations, reachability, disposal, LOH |
| `03-allocation-patterns.md` | Everyday sources of avoidable allocations |
| `04-span-memory-pooling.md` | Spans, memory, stackalloc, and array pooling |
| `05-profiling-and-benchmarking.md` | Measurement techniques and benchmark discipline |
| `common-pitfalls.md` | Mistakes to recognize during code review |

## Performance Checklist

- Is this code on a hot path or just ordinary business logic?
- Did you measure the baseline before changing it?
- Are allocations visible in `Allocated` or GC counters?
- Does the optimized code still read clearly enough to maintain?
- Did pooling introduce ownership or stale-data risks?
- Are benchmark inputs representative of production data sizes?

