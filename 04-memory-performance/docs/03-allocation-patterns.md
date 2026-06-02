# Allocation Patterns

## What This Chapter Covers

This chapter focuses on allocation sources you can miss during normal coding. Most of these patterns are small in isolation, but they add up in loops, parsers, API gateways, and other hot paths.

You will see:

- boxing and unboxing
- temporary strings and string churn
- hidden allocations from “simple” language features
- how to choose a practical fix

## Boxing and Unboxing

Boxing happens when a value type is converted to `object` or an interface. That conversion allocates a new object on the heap.

```csharp
int value = 12;
object boxed = value;
int unboxed = (int)boxed;
```

The allocation is small, but repeated boxing in a tight loop creates avoidable pressure. The `AllocationPatternsExample.BoxingExample()` helper exists to make this easy to demonstrate in tests and benchmarks.

## String Churn

Strings are immutable. Every operation that changes the text creates a new string.

Use the right tool for the workload:

- `StringBuilder` for repeated concatenation
- `Span<char>` for temporary transforms
- buffer reuse when the same pattern repeats often

The current module uses `ToUpperWithStackalloc()` as a simple example of a short-lived transform that avoids repeated heap allocations for small inputs.

## Hidden Allocations

Common hidden allocation sources include:

- lambda captures
- iterator blocks
- interface boxing
- temporary arrays for parsing or formatting
- repeated substring creation

That is why allocation review is partly about language features, not just explicit `new` expressions.

## A Useful Way To Think About It

Ask two questions:

- does this run once, or many times?
- is the allocation on a cold path, or in the middle of a repeated workload?

If the answer is “once” and “cold”, readability usually wins.

## Choosing the Right Fix

If the workload is not hot, the best optimization is often no optimization. Write the clear version first, measure it, and only then decide whether reducing allocations is worth the tradeoff.

Practical priorities:

1. remove obvious waste
2. verify the hotspot is real
3. keep the fix understandable
4. only then consider lower-level techniques
