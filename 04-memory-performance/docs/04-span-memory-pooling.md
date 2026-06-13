# Span, Memory, and Pooling

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

## Practice

1. Read `SpanMemoryPoolingExample.ParseThreeNumbers()`.
2. Compare span parsing with a `Split(',')` implementation.
3. Change `NormalizeProductCode()` to preserve dashes and update tests.
4. Use `ArrayPool<char>` for a longer normalization exercise.
5. Benchmark before and after.
