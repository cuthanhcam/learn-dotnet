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

Complement lookup:

```text
for each value:
    needed = target - value
    if needed has been seen:
        answer found
    remember value
```

This turns many pair-search problems from O(n^2) into average O(n).

## Equality In C#

Hash tables rely on:

- `GetHashCode()`
- `Equals()`
- Optional `IEqualityComparer<T>`

For strings, choose the comparer intentionally:

- `StringComparer.Ordinal`
- `StringComparer.OrdinalIgnoreCase`
- Culture-aware comparers when the domain truly needs culture rules

## Designing Keys

Good hash keys are stable and represent identity clearly.

Prefer:

- Immutable values
- Primitive IDs
- Normalized strings with an explicit comparer
- Records or structs with correct equality semantics

Avoid:

- Mutable objects whose fields can change after insertion
- Floating-point values unless the domain has clear precision rules
- Case-sensitive string keys for user-entered identifiers when the business rule is case-insensitive

## Backend Connections

Hash-table thinking shows up in:

- In-memory indexes
- Caches
- Deduplication
- Grouping query results
- Idempotency key lookup
- Request correlation maps
- Rate-limit counters
- Permission lookup sets
- Precomputed read models

## Pitfalls

- Mutating a key after inserting it into a dictionary.
- Using case-sensitive string keys when the domain is case-insensitive.
- Assuming dictionary iteration order is part of your business contract.
- Forgetting memory cost when storing large values or many keys.
- Calling `dictionary[key]` when the key may not exist; prefer `TryGetValue`.

## Practice Problems To Master

- Two sum with complement lookup.
- First non-repeating character.
- Group anagrams.
- Detect duplicates.
- Count frequencies.
- Find intersection of two arrays.

