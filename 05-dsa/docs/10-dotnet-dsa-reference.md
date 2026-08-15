---
title: ".NET DSA Reference"
description: "A selection guide for .NET collections, APIs, equality, sorting, and allocation-aware algorithms."
slug: dsa-dotnet-reference
phase: 5
order: 10
difficulty: reference
article-type: reference
estimated-reading-minutes: 28
topics: [dsa, dotnet, collections, reference]
prerequisites: [dsa-practice-system]
status: maintained
last-reviewed: 2026-08-15
---

# .NET DSA Reference

This page connects algorithmic ideas to the C# and .NET types you will use most often.

## Collection Selection

| Use case | Prefer | Why |
| -------- | ------ | --- |
| Fixed-size indexed data | `T[]` | Compact and direct O(1) indexing |
| Growing indexed data | `List<T>` | Dynamic array with amortized O(1) append |
| Membership checks | `HashSet<T>` | Average O(1) `Contains` |
| Key-value lookup | `Dictionary<TKey, TValue>` | Average O(1) lookup by key |
| FIFO order | `Queue<T>` | Clear queue semantics |
| LIFO order | `Stack<T>` | Clear stack semantics |
| Sorted unique values | `SortedSet<T>` | Maintains order with O(log n) updates |
| Sorted keys | `SortedDictionary<TKey, TValue>` | Ordered keys with O(log n) operations |
| Read-only sharing | `IReadOnlyList<T>`, immutable collections | Communicates mutation boundaries |
| Allocation-sensitive slices | `Span<T>`, `ReadOnlySpan<T>` | Slice without allocating |

## Complexity Cheat Sheet

| Type | Index | Add | Remove | Contains |
| ---- | ----- | --- | ------ | -------- |
| `T[]` | O(1) | fixed size | fixed size | O(n) |
| `List<T>` | O(1) | amortized O(1) at end | O(n) by value/index shift | O(n) |
| `LinkedList<T>` | O(n) | O(1) with node | O(1) with node, O(n) by value | O(n) |
| `Stack<T>` | top only | O(1) push | O(1) pop | O(n) |
| `Queue<T>` | front only | O(1) enqueue | O(1) dequeue | O(n) |
| `HashSet<T>` | none | average O(1) | average O(1) | average O(1) |
| `Dictionary<TKey, TValue>` | by key | average O(1) | average O(1) | average O(1) key lookup |
| `SortedSet<T>` | none | O(log n) | O(log n) | O(log n) |

## Equality Rules

Hash-based collections depend on equality.

For a key type:

- Equal values must return the same hash code.
- Hash codes should not change while the value is used as a key.
- `Equals` should be reflexive, symmetric, and transitive.
- Prefer immutable key types.
- For strings, choose `StringComparer.Ordinal` or `StringComparer.OrdinalIgnoreCase` unless culture-specific behavior is required.

Example:

```csharp
var usersByEmail = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);
```

## Sorting APIs

| API | Mutates input | Notes |
| --- | ------------- | ----- |
| `Array.Sort(array)` | Yes | Sorts in place |
| `list.Sort()` | Yes | Sorts in place |
| `source.OrderBy(x => x.Key)` | No | Deferred LINQ query until enumerated |
| `source.OrderBy(...).ThenBy(...)` | No | Useful for multi-key ordering |

When practicing DSA, be explicit about whether sorting is allowed. Sorting often changes complexity to O(n log n), but it can simplify duplicate handling, two-pointer scans, and binary search.

## LINQ During DSA Practice

LINQ is excellent production C# when used thoughtfully, but it can hide algorithmic details during practice.

Prefer manual loops when learning:

- Two pointers
- Sliding windows
- Binary search
- DFS/BFS
- Backtracking
- In-place mutation

Use LINQ when it clarifies intent after you already understand the cost:

- Simple filtering
- Projection
- Grouping for readable non-hot paths
- Test assertions and setup

## Span Notes

`Span<T>` and `ReadOnlySpan<T>` are useful when you need slices without allocations.

Use them when:

- Input is contiguous.
- The method does not need to store the span beyond the call.
- You want to avoid substring or array-slice allocations.

Avoid them when:

- You need to store the data in fields.
- You are still learning the algorithm and spans distract from the core idea.
- The lifetime rules make the code harder to explain.

## Backend Examples

| Backend scenario | DSA idea |
| ---------------- | -------- |
| Deduplicate incoming IDs | `HashSet<T>` membership |
| Count requests per user | `Dictionary<TKey, int>` frequency map |
| Process jobs in arrival order | `Queue<T>` |
| Roll back nested operations | `Stack<T>` |
| Resolve category hierarchy | Tree traversal |
| Resolve service dependencies | Graph traversal and cycle detection |
| Find first record above threshold in sorted data | Lower-bound binary search |
| Avoid repeated expensive subcalls | Memoization/cache |

## Naming Conventions For Practice Code

Good names make invariants easier to see:

- `left`, `right` for two boundaries.
- `start`, `end` for windows.
- `slow`, `fast` for linked-list runners.
- `visited` for graph traversal state.
- `path` for current backtracking choices.
- `result` for finalized answers.
- `remaining` for constrained search.

Avoid vague names like `temp`, `data`, and `flag` when the variable carries algorithmic meaning.
