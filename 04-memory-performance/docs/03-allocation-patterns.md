# Allocation Patterns

## Why Allocation Awareness Matters

Most allocations are fine. Clean business code should not be twisted into unreadable shapes just to avoid one tiny object. Performance work matters most when allocation happens frequently, on hot paths, under high concurrency, or with large buffers.

Good allocation awareness helps you notice:

- repeated temporary objects
- hidden boxing
- string churn
- closure captures
- iterator and LINQ overhead
- unnecessary materialization
- large arrays created repeatedly

## Boxing

Boxing wraps a value type in an object on the heap.

```csharp
int number = 42;
object boxed = number;
```

Unboxing extracts the value:

```csharp
int copy = (int)boxed;
```

Boxing often appears through non-generic APIs:

```csharp
var values = new ArrayList();
values.Add(42); // boxing
```

Prefer generic APIs:

```csharp
var values = new List<int>();
values.Add(42); // no boxing
```

`AllocationPatternsExample.SumBoxedNumbers()` intentionally boxes values so you can compare it with `SumGenericNumbers()`.

## Strings

`string` is immutable. Every operation that appears to modify a string actually creates a new string.

Problem pattern:

```csharp
string result = "";
for (int i = 0; i < count; i++)
{
    result += i;
}
```

Better options:

- `string.Join` when joining known values
- `StringBuilder` for incremental construction
- `string.Create` for advanced fixed-format creation
- spans for parsing without substring allocation

Use `StringBuilder` when repeated concatenation creates many intermediate strings and the code path matters.

## Substrings and Parsing

Modern .NET improved some string operations, but `Substring` still creates a new string.

Allocation-heavy parsing:

```csharp
string first = text.Substring(0, commaIndex);
int value = int.Parse(first);
```

Span-based parsing:

```csharp
ReadOnlySpan<char> first = text.AsSpan(0, commaIndex);
int value = int.Parse(first);
```

This avoids creating a temporary string for the token.

## Closures

A lambda that captures local state may require a compiler-generated object to hold captured variables.

```csharp
int factor = 10;
Func<int, int> multiply = value => value * factor;
```

Closures are not bad. They are expressive and often worth it. But in hot loops, repeated closure allocation can matter.

Tips:

- Use `static` lambdas when no capture is needed.
- Avoid capturing large objects accidentally.
- Be careful when storing captured delegates in long-lived objects.

## Iterators

Methods using `yield return` create a state machine object.

```csharp
IEnumerable<int> EvenNumbers(IEnumerable<int> numbers)
{
    foreach (int number in numbers)
    {
        if (number % 2 == 0)
        {
            yield return number;
        }
    }
}
```

This is excellent for streaming and deferred execution. It is not allocation-free.

## LINQ

LINQ can allocate iterators, delegates, closures, groupings, lookup tables, and materialized collections. It can also make code dramatically clearer.

Use LINQ freely in normal code. Inspect it more carefully when:

- it runs per request at high volume
- it runs inside nested loops
- it repeatedly enumerates the same query
- it creates large intermediate lists
- it closes over expensive state

## Defensive Copies

APIs sometimes copy to protect ownership:

```csharp
public IReadOnlyList<int> Values => _values.ToList();
```

This is safe but allocates every access. Alternatives include:

- return `IReadOnlyList<T>` backed by immutable data
- return `ReadOnlyMemory<T>`
- expose enumeration only
- document ownership clearly

Choose based on safety first, then measure.

## Practice

1. Compare boxed and generic summing in `AllocationPatternsExample`.
2. Benchmark string concatenation against `StringBuilder`.
3. Rewrite a substring parser using `ReadOnlySpan<char>`.
4. Find one LINQ chain in earlier modules and identify what it allocates.
5. Decide whether the allocation matters based on frequency and data size.
