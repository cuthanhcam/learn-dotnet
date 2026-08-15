---
title: "Span, Memory, and Pooling"
description: "Non-owning memory views, lifetime safety, stackalloc, Memory, and ArrayPool ownership."
slug: dotnet-span-memory-pooling
phase: 4
order: 4
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 32
topics: [dotnet, span, memory, arraypool]
prerequisites: [dotnet-memory-model-deep-dive, dotnet-allocation-patterns]
status: maintained
last-reviewed: 2026-08-15
---

# Span, Memory, and Pooling

## Learning Objectives

- Separate memory ownership from a temporary view over memory.
- Choose `Span<T>`, `Memory<T>`, stack allocation, or pooled storage by lifetime.
- Preserve the logical requested length when a pool returns a larger physical array.
- return pooled storage exactly once under success, failure, and repeated disposal;
- make data-clearing policy explicit for sensitive buffers.

## Why These APIs Exist

Many performance problems come from copying data just to look at part of it. `Span<T>` and `ReadOnlySpan<T>` let you work with a contiguous region of memory without owning or copying it.

They are useful for:

- parsing text
- slicing arrays
- formatting into existing buffers
- temporary scratch work
- avoiding substring and array-copy allocations

## Span Is a View

`Span<T>` is a view over existing memory. It does not own the memory.

```csharp
int[] numbers = [1, 2, 3, 4];
Span<int> middle = numbers.AsSpan(1, 2);
middle[0] = 99;
```

The original array now contains `99` at index 1.

`ReadOnlySpan<T>` is a readonly view. It prevents mutation through that view, but it does not make the underlying memory globally immutable.

## Ref Struct Restrictions

`Span<T>` is a `ref struct`, which means it is stack-only. This prevents spans from outliving the memory they point to.

You cannot:

- store `Span<T>` in a class field
- capture it in a lambda
- use it across `await`
- box it
- use it as a generic type argument in most ordinary generic APIs

These restrictions are features. They protect lifetime safety.

## Memory<T>

`Memory<T>` and `ReadOnlyMemory<T>` are heap-friendly wrappers that can be stored and passed around more flexibly than spans.

Use:

- `Span<T>` for immediate synchronous work
- `Memory<T>` when data must be stored or used with async APIs

You can get a span from memory when doing actual work:

```csharp
Memory<byte> memory = new byte[1024];
Span<byte> span = memory.Span;
```

## stackalloc

`stackalloc` creates a small temporary buffer in the current stack frame.

```csharp
Span<char> buffer = stackalloc char[32];
```

Good uses:

- small fixed upper-bound buffers
- formatting temporary values
- parsing short inputs

Avoid:

- large buffers
- user-controlled unbounded sizes
- storing the span beyond the method

The example `SpanMemoryPoolingExample.StackallocSum()` limits the size intentionally.

## ArrayPool<T>

`ArrayPool<T>` rents arrays so repeated workloads can reuse buffers.

```csharp
byte[] rented = ArrayPool<byte>.Shared.Rent(size);
try
{
    Span<byte> slice = rented.AsSpan(0, size);
    // use slice
}
finally
{
    ArrayPool<byte>.Shared.Return(rented, clearArray: true);
}
```

Important rules:

- The rented array may be larger than requested.
- Use only the slice you requested.
- Return the array exactly once.
- Do not use it after returning.
- Clear it before returning if it contains sensitive data.
- Do not assume rented arrays start empty.

Pooling improves repeated buffer-heavy workloads. It can make simple code more complex, so use it when measurement justifies it.

## Ownership Is a Protocol

A rented array has one logical owner at a time. The owner decides who may access it, which portion is
initialized, when access ends, and who returns it. `Span<T>` and `Memory<T>` are views; they do not by
themselves record or transfer ownership.

When a method returns a `Memory<T>` backed by a rented array, returning the array before the consumer
finishes creates use-after-return corruption: another pool user may overwrite the same storage. Either
keep all use inside the rent/`finally` scope or return an owner whose disposal ends the lifetime.

`IMemoryOwner<T>` expresses this pattern:

```csharp
using IMemoryOwner<byte> owner = new PooledBuffer<byte>(4096);
Memory<byte> logicalBuffer = owner.Memory;
await stream.ReadAsync(logicalBuffer, token);
```

The repository's `PooledBuffer<T>` is a reference type so passing the owner does not copy independent
return state. Its `Dispose` atomically exchanges the rented array with `null`, making repeated disposal
idempotent. Access after disposal throws instead of silently exposing storage that may belong to
someone else.

## Logical Length Versus Physical Capacity

`Rent(1000)` promises an array of at least 1,000 elements, not exactly 1,000. The larger array may be a
pool size class and may contain data from earlier use. Expose `array.AsMemory(0, requestedLength)` and
track the number of initialized elements separately from capacity.

This distinction appears in parsers and I/O loops:

- capacity: how much storage is available;
- requested length: how much the owner permits a caller to use;
- written length: how much currently contains meaningful data;
- consumed length: how much a downstream parser has processed.

Confusing these values can expose stale data, append unwanted zeros, or parse beyond a valid message.

## Clearing and Sensitive Data

Returning an array does not normally clear it. Set `clearArray: true` when references must be released
or when data such as credentials, cryptographic material, personal data, or request payloads must not
remain available to a future renter.

Clearing has a cost proportional to physical array length, which can exceed the logical length. For
cryptographic secrets, also consider `CryptographicOperations.ZeroMemory` for explicit zeroing and
recognize that immutable strings and earlier copies cannot be erased through the pooled buffer.

Security policy takes priority over a microbenchmark. Document whether callers may store sensitive
content and make the clearing choice at the owner boundary rather than relying on every use site.

## Pooling Decision Checklist

- Is allocation rate or GC pressure measured on an important path?
- Are buffers large or frequent enough for reuse to matter?
- Is one owner responsible for exactly one return?
- Can any view, callback, task, or queued operation outlive the owner?
- Are logical length and initialized length explicit?
- Must content be cleared, and is clearing the whole rented array acceptable?
- Would a simpler ordinary array be safer at an irrelevant cost?

## Formatting Without Intermediate Strings

Many APIs support span-based formatting:

```csharp
Span<char> buffer = stackalloc char[16];
value.TryFormat(buffer, out int written);
```

C# and .NET also support interpolated string handlers for spans in modern versions:

```csharp
buffer.TryWrite($"ORD-{id:000000}", out int written);
```

This is useful for IDs, logs, protocols, and serialization paths where repeated temporary strings add pressure.

## Common Mistakes

- Returning a rented array to the pool and then reading from it later
- Forgetting that the rented array may be larger than requested
- Using pooling for tiny infrequent allocations
- Creating spans but then calling `ToArray()` or `ToString()` too early
- Using `stackalloc` with unbounded user input
- Returning an owner while an asynchronous operation still uses its memory
- Storing the physical rented length as if every element were initialized
- Implementing a pooled owner as a freely copyable struct with independent disposal state

## Implementation and Test Map

| Concern | Source | Tests |
|---|---|---|
| Span parsing, stack scratch, direct rent/return | `SpanMemoryPoolingExample.cs` | `SpanMemoryPoolingExampleTests.cs` |
| Explicit pooled-memory ownership | `PooledBuffer.cs` | `PooledBufferTests.cs` |
| Allocation comparison | `SpanAndPoolingBenchmarks.cs` | BenchmarkDotNet output |

## Practice

1. Read `SpanMemoryPoolingExample.ParseThreeNumbers()`.
2. Compare span parsing with a `Split(',')` implementation.
3. Change `NormalizeProductCode()` to preserve dashes and update tests.
4. Use `ArrayPool<char>` for a longer normalization exercise.
5. Benchmark before and after.

## Further Reading

- [`Span<T>` and `Memory<T>` usage guidelines](https://learn.microsoft.com/en-us/dotnet/standard/memory-and-spans/memory-t-usage-guidelines)
- [`ArrayPool<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1)
- [`IMemoryOwner<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.imemoryowner-1)

## Continue Learning

- Previous: [Allocation patterns](03-allocation-patterns.md)
- Next: [Profiling and benchmarking](05-profiling-and-benchmarking.md)
