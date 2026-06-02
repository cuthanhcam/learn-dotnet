# Roadmap: Memory & Performance

## Overview

This module starts with the memory model and ends with measurement. The goal is not to turn every allocation into a bug. The goal is to understand where allocations come from, which ones matter, and how to verify a memory optimization with evidence instead of instinct.

The learning path is intentionally practical:

- first, recognize where data lives and how it is copied
- next, understand how the GC reclaims managed memory
- then, identify hidden allocations in everyday code
- after that, use spans and pooling to reduce copying in hot paths
- finally, measure the result with targeted benchmarks

## Phase 1: Memory Model

**Focus**: stack frames, heap objects, value semantics, reference semantics, and lifetime.

What to learn:

- local values and call frames are short-lived and predictable
- objects, arrays, and captured state live on the managed heap
- copying a value type creates a new independent value
- copying a reference type copies the reference, not the object

Expected outcomes:

- explain why `ValueTypeCopyExample()` produces two independent values
- explain why `ReferenceAliasExample()` mutates both variables
- distinguish between storage location and type category

## Phase 2: Garbage Collection

**Focus**: object reachability, generations, disposal, and allocation pressure.

What to learn:

- Gen 0, Gen 1, and Gen 2 describe object age, not object kind
- short-lived allocations are usually inexpensive but still measurable
- `IDisposable` is for non-memory resources that need deterministic cleanup
- forcing a GC is usually a diagnostic action, not a normal fix

Expected outcomes:

- read `GC.CollectionCount()` as a trend signal
- use `using` or `using var` when a type owns a disposable resource
- explain why object promotion is a useful mental model for long-lived allocations

## Phase 3: Allocation Patterns

**Focus**: boxing, strings, closures, and other common allocation triggers.

What to learn:

- boxing turns a value type into a heap object
- repeated string concatenation creates temporary strings
- lambdas and iterators can allocate even when the syntax looks simple
- the best optimization is often not the cleverest one

Expected outcomes:

- spot boxing in code review before it becomes a runtime issue
- explain why `StringBuilder` or buffering can be better than repeated concatenation
- know when the simpler readable version is still the right choice

## Phase 4: Span and Pooling

**Focus**: temporary slices, short-lived buffers, and reusable arrays.

What to learn:

- `Span<T>` and `ReadOnlySpan<T>` let you work on slices without copying
- `stackalloc` is useful for small scratch buffers with a tight lifetime
- `ArrayPool<T>` helps when repeated allocations dominate a workload
- pooling is a tradeoff, not an automatic win

Expected outcomes:

- choose span when you need a temporary view over existing memory
- choose pooling when the workload repeatedly creates similar buffers
- avoid leaking pooled buffers or using them after return

## Phase 5: Profiling and Benchmarking

**Focus**: verify changes with measurements.

What to learn:

- `GC.GetAllocatedBytesForCurrentThread()` is useful for allocation deltas
- `GC.CollectionCount()` shows whether a path is creating collection pressure
- `Stopwatch` is fine for simple local comparisons
- benchmark projects are better when you need repeatable comparison

Expected outcomes:

- compare two code paths using the same workload
- avoid drawing conclusions from a single run
- know when a microbenchmark result is meaningful and when it is noise

## Practice Sequence

1. Run the memory model examples and predict the output before reading it.
2. Inspect the GC example and note which values change after allocation pressure.
3. Compare boxing and non-boxing code paths in the allocation examples.
4. Rewrite a small parsing or normalization routine with `Span<T>`.
5. Compare pooled and non-pooled buffer reuse in the benchmark runner.
6. Use the tests to confirm the behavior stayed correct after refactoring.

## Completion Criteria

You are ready to move on when you can:

- explain stack vs heap without mixing up storage and type
- identify the main allocation triggers in a code review
- choose spans or pooling only where they pay off
- defend a performance change with measured evidence
