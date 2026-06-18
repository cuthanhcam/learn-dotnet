# Hash Tables

A hash table maps a key to a bucket by using a hash code. In C#, the common hash-table-backed collections are `Dictionary<TKey, TValue>` and `HashSet<T>`.

## Complexity

| Operation | Average | Worst case |
| --- | --- | --- |
| Insert | O(1) | O(n) |
| Lookup | O(1) | O(n) |
| Delete | O(1) | O(n) |

The worst case appears when many keys collide or equality/hash implementations are poor. The average case is what makes hash tables so useful.

## Common Patterns

Frequency map:

```text
count each value
use counts to answer duplicate, anagram, or majority questions
```

Membership set:

```text
add seen values
ask whether a value has appeared before
```

Index map:

```text
value -> index
use complement lookup for two-sum problems
```

Grouping:

```text
computed key -> list of original values
```

## Equality In C#

Hash tables rely on:

- `GetHashCode()`
- `Equals()`
- Optional `IEqualityComparer<T>`

For strings, choose the comparer intentionally:

- `StringComparer.Ordinal`
- `StringComparer.OrdinalIgnoreCase`
- Culture-aware comparers when the domain truly needs culture rules

## Backend Connections

Hash-table thinking shows up in:

- In-memory indexes
- Caches
- Deduplication
- Grouping query results
- Idempotency key lookup
- Request correlation maps

## Pitfalls

- Mutating a key after inserting it into a dictionary.
- Using case-sensitive string keys when the domain is case-insensitive.
- Assuming dictionary iteration order is part of your business contract.
- Forgetting memory cost when storing large values or many keys.

