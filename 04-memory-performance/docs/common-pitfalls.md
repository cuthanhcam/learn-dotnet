---
title: "Common Memory and Performance Pitfalls"
description: "Misleading memory models, unsafe pooling, invalid benchmarks, and premature optimization."
slug: dotnet-memory-performance-pitfalls
phase: 4
order: 6
difficulty: advanced
article-type: pitfalls
estimated-reading-minutes: 14
topics: [dotnet, performance, pitfalls]
prerequisites: [dotnet-span-memory-pooling, dotnet-profiling-and-benchmarking]
status: maintained
last-reviewed: 2026-08-15
---

# Common Pitfalls

## 1. Treating Every Allocation As A Bug

Allocations are normal in .NET. Optimize allocations that are frequent, large, long-lived, or proven expensive.

Bad question:

> Can I remove every allocation?

Better question:

> Is this allocation meaningful for this workload?

## 2. Saying Value Types Always Live On The Stack

Value types are copied by value. They can live inside heap objects, arrays, closures, and boxed objects.

Use this rule instead:

- value type means value-copy semantics
- reference type means reference-copy semantics
- storage depends on containment and compiler/runtime decisions

## 3. Forgetting That Strings Are Immutable

Repeated string concatenation in loops creates intermediate strings.

Prefer:

- `string.Join` for joining known sequences
- `StringBuilder` for incremental construction
- span-based parsing to avoid substring allocation

## 4. Boxing Value Types Accidentally

Common boxing triggers:

- assigning value types to `object`
- non-generic collections
- some interface calls on structs
- string formatting paths
- params `object[]`

Prefer generic APIs and type-specific overloads.

## 5. Confusing GC With Dispose

GC reclaims managed memory. `Dispose` releases resources at a predictable time.

Use `using` for disposable resources:

```csharp
using var stream = File.OpenRead(path);
```

Do not wait for GC to close files, sockets, handles, or pooled ownership.

## 6. Calling GC.Collect As A Fix

`GC.Collect()` is rarely the right application-level solution. It can pause useful work and fight runtime heuristics.

Use it for experiments, diagnostics, or rare controlled lifecycle boundaries.

## 7. Returning Rented Buffers Too Late Or Too Early

With `ArrayPool<T>`:

- return arrays in `finally`
- use only the requested slice
- clear sensitive data
- never use the array after return
- never return the same array twice

Pooling is ownership management. Treat it with care.

## 8. Letting Benchmarks Lie

Benchmark mistakes:

- using Debug mode
- measuring one run
- using unrealistic data
- ignoring allocations
- benchmarking code that production does not execute
- forgetting JIT warmup

Use BenchmarkDotNet for serious comparisons.

## 9. Making Hot Code Clever But Fragile

Performance code still needs to be readable. A small speedup is not worth a maintenance trap unless the path is truly important.

Prefer the simplest measured improvement that solves the real problem.
