# Span and Memory Pooling

## What This Chapter Covers

This chapter is about working with memory more directly while still staying in safe managed code.

You will see:

- how `Span<T>` gives you a temporary view over contiguous memory
- when `ReadOnlySpan<T>` is the better API surface
- why `stackalloc` is useful for small scratch buffers
- when `ArrayPool<T>` helps and when it is just extra complexity

## Span<T>

`Span<T>` is a stack-only view over a contiguous region of memory. It lets you slice data without copying it.

Use it when you need:

- temporary processing of an array segment
- parsing without extra allocations
- stack-based scratch space

The `MemoryPerformanceExample.SumWithSpan()` helper shows a simple read-only traversal with no extra copying.

## ReadOnlySpan<T>

Use `ReadOnlySpan<T>` when the caller should not modify the underlying data.

```csharp
int total = MemoryPerformanceExample.SumWithSpan([1, 2, 3, 4]);
```

This is especially useful for APIs that accept arrays, slices, string-like data, or temporary buffers.

## stackalloc

`stackalloc` creates a short-lived buffer on the stack.

This is ideal for small scratch buffers, but the lifetime is limited to the current method.

The memory-performance module uses `ToUpperWithStackalloc()` as an example of a transform where a short-lived stack buffer can be enough for modest input sizes.

## ArrayPool<T>

`ArrayPool<T>` helps reduce repeated array allocations by reusing buffers.

Use pooling for:

- repeated temporary arrays
- high-throughput parsing
- large buffers that are expensive to recreate

Do not pool when:

- the workload is tiny
- the code becomes hard to reason about
- the buffer is only used once

The `SpanAndPoolingExample.RentArrayPoolAndSum()` helper shows the basic rent/fill/read/return pattern.

## Safety Notes

Pooling makes ownership less obvious, so be strict about returning the buffer in a `finally` block. That is the pattern used in the example code.

Avoid keeping a span around after the source goes out of scope. A span is a view, not an owner.

## Rule of Thumb

Prefer spans for temporary views and pools for repeated buffers. Use neither if a simple array or string is already good enough.
