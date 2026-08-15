---
title: "Core .NET Performance Guidelines"
description: "Measurement-first guidance for collections, LINQ, files, reflection, exceptions, and allocations."
slug: core-dotnet-performance-guidelines
phase: 3
order: 10
difficulty: intermediate
article-type: reference
estimated-reading-minutes: 14
topics: [dotnet, performance]
prerequisites: [dotnet-collections-deep-dive, linq-deferred-execution, csharp-attributes-reflection]
status: maintained
last-reviewed: 2026-08-15
---

# Performance Guidelines

## Purpose

This note connects the Core .NET topics in this module with practical performance habits. It is intentionally high level; detailed memory behavior continues in `04-memory-performance`.

## Collections

- Prefer `List<T>` for ordered, indexable data.
- Prefer `Dictionary<TKey, TValue>` for key-based lookup.
- Prefer `HashSet<T>` for membership checks and uniqueness.
- Pre-size collections when the final size is already known.
- Avoid repeated `Contains` checks on `List<T>` for large datasets; use a set.

```csharp
var knownIds = new HashSet<int>(ids);
bool exists = knownIds.Contains(candidateId);
```

## Generics

Generics usually avoid boxing for value types and preserve type safety.

Prefer:

```csharp
List<int> values = [];
```

Avoid older non-generic APIs for new code:

```csharp
ArrayList values = [];
```

## Exceptions

Exceptions are for exceptional control flow. Throwing and catching exceptions repeatedly in normal paths is expensive and makes intent harder to read.

Prefer `Try*` APIs when failure is expected:

```csharp
if (int.TryParse(input, out int value))
{
    Console.WriteLine(value);
}
```

## LINQ

LINQ is expressive, but it can allocate iterators, delegates, closures, groupings, and intermediate collections.

Guidelines:

- Materialize once when you need a stable snapshot.
- Avoid repeated enumeration of expensive queries.
- Watch LINQ inside nested loops.
- Use `ToLookup` or dictionaries for repeated keyed access.
- Keep LINQ when clarity matters more than micro-optimization.

## Delegates and Events

Delegates are objects. Lambdas that capture locals may allocate closure objects.

Use `static` lambdas when no capture is needed:

```csharp
var positive = numbers.Where(static number => number > 0);
```

Unsubscribe long-lived event handlers when the subscriber should be collectible.

## File I/O

- Prefer streaming large files instead of reading everything into memory.
- Use async I/O for scalable server-side waiting.
- Dispose streams deterministically with `using` or `await using`.
- Avoid building large strings when processing line-based files.

## Date and Time

Performance is rarely the main issue with date/time code; correctness is. Prefer `DateTimeOffset` for real-world timestamps and store UTC when possible.

## Measurement

Before optimizing:

1. Identify the hot path.
2. Measure the current behavior.
3. Change one thing.
4. Measure again.
5. Keep the change only if the result justifies the complexity.

Useful tools:

- `Stopwatch` for quick local checks
- BenchmarkDotNet for repeatable microbenchmarks
- profilers for application-level investigation
- runtime counters for production-like observation

