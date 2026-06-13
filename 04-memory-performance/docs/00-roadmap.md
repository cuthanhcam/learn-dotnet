# Roadmap: Memory & Performance

## Purpose

This module teaches how to reason about memory and performance in modern .NET. It is not about making every method clever. It is about knowing where cost comes from, choosing the right tool, and validating the result.

The learning path moves from mental model to measurement:

1. Memory model
2. Garbage collection
3. Allocation patterns
4. Span, memory, and pooling
5. Profiling and benchmarking

## Phase 1: Memory Model

Focus:

- stack frames
- managed heap objects
- value semantics
- reference semantics
- object identity
- lifetime and reachability

Key idea:

Value type vs reference type is about copy behavior. Stack vs heap is about storage. These are related but not the same thing.

Expected outcomes:

- Explain why copying a `record struct` creates an independent value.
- Explain why two class variables can point to the same object.
- Avoid the phrase "value types always live on the stack."
- Identify when `in`, `ref`, and `out` are about avoiding copies or communicating mutation.

Matching code:

- `MemoryModelExample.ValueTypeCopyExample()`
- `MemoryModelExample.ReferenceAliasExample()`
- `MemoryModelExample.DistanceFromOrigin()`

## Phase 2: Garbage Collection

Focus:

- reachability
- GC roots
- Gen 0, Gen 1, Gen 2
- object promotion
- Large Object Heap
- deterministic cleanup with `IDisposable`

Key idea:

The GC reclaims unreachable managed memory. It does not replace deterministic cleanup for external resources.

Expected outcomes:

- Read `GC.CollectionCount()` as a pressure signal.
- Use `GC.GetAllocatedBytesForCurrentThread()` for allocation experiments.
- Explain why `using` is still necessary for disposable resources.
- Avoid using `GC.Collect()` as a routine fix.

Matching code:

- `GarbageCollectionExample.CaptureSnapshot()`
- `GarbageCollectionExample.AllocateShortLivedObjects()`
- `DisposableBuffer`

## Phase 3: Allocation Patterns

Focus:

- boxing and unboxing
- string immutability
- `StringBuilder`
- closures
- iterators
- LINQ allocation tradeoffs
- defensive copies

Key idea:

Allocation is not automatically bad. Unnoticed repeated allocation in an important path is the problem.

Expected outcomes:

- Spot boxing in common APIs.
- Recognize string churn in loops.
- Know that lambdas, iterators, and LINQ can allocate.
- Decide whether readability or allocation reduction matters more in a given path.

Matching code:

- `AllocationPatternsExample.SumBoxedNumbers()`
- `AllocationPatternsExample.SumGenericNumbers()`
- `AllocationPatternsExample.BuildWithStringBuilder()`
- `AllocationPatternsExample.CreateMultipliers()`

## Phase 4: Span, Memory, and Pooling

Focus:

- `Span<T>`
- `ReadOnlySpan<T>`
- `Memory<T>`
- `stackalloc`
- `ArrayPool<T>`
- span-based parsing and formatting

Key idea:

Spans are temporary views over memory. Pools are reusable ownership systems. Neither should be used casually without understanding lifetime.

Expected outcomes:

- Parse a slice without creating a substring.
- Use `stackalloc` only for small bounded buffers.
- Rent and return arrays safely.
- Understand why `Span<T>` cannot be stored in a normal class field.

Matching code:

- `SpanMemoryPoolingExample.ParseThreeNumbers()`
- `SpanMemoryPoolingExample.NormalizeProductCode()`
- `SpanMemoryPoolingExample.RentFillAndSum()`
- `SpanMemoryPoolingExample.FormatOrderId()`

## Phase 5: Profiling and Benchmarking

Focus:

- baseline measurement
- allocation deltas
- GC count deltas
- `Stopwatch`
- BenchmarkDotNet
- interpreting noise and tradeoffs

Key idea:

Optimization is a change-management activity. You need a baseline, a hypothesis, a measurement, and a decision.

Expected outcomes:

- Measure elapsed time and allocated bytes for a small code path.
- Run BenchmarkDotNet in Release mode.
- Read `Allocated`, `Mean`, `Gen0`, and `Ratio` columns.
- Avoid drawing conclusions from a single run.

Matching code:

- `ProfilingExample.Measure()`
- `benchmarks/MemoryPerformance.Benchmarks`

## Practice Sequence

1. Run the console demo.
2. Read one docs chapter at a time.
3. Open the matching example file and trace each method.
4. Run tests after changing an example.
5. Run benchmarks in Release mode.
6. Revisit modules 01-03 and identify at least five allocation patterns.

## Completion Criteria

You are ready to move on when you can:

- explain value/reference semantics without mixing them with stack/heap storage
- explain what makes an object reachable
- identify boxing, string churn, closure captures, and iterator allocation
- use span-based parsing for a simple text input
- use `ArrayPool<T>` without leaking ownership
- benchmark a before/after change and defend the result
