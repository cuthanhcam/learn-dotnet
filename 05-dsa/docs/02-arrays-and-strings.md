# Arrays and Strings

Arrays and strings are the first place where DSA becomes concrete. They are contiguous, indexable sequences, so many problems are about moving indexes carefully.

## Arrays

An array stores values in contiguous slots.

Common operations:

| Operation | Complexity |
| --- | --- |
| Read by index | O(1) |
| Write by index | O(1) |
| Scan | O(n) |
| Insert in middle | O(n) |
| Remove from middle | O(n) |

Use arrays when size is known, indexed access matters, or compact memory layout is useful. Use `List<T>` when the collection needs to grow.

## `Array` vs `List<T>`

| Choose | When |
| ------ | ---- |
| `T[]` | Size is fixed, hot indexed access matters, or APIs require arrays |
| `List<T>` | You append dynamically and still need O(1) indexing |
| `IReadOnlyList<T>` | Callers should read by index but not mutate |
| `IEnumerable<T>` | You only need streaming iteration |

Do not choose `IEnumerable<T>` when the algorithm needs repeated indexing. Re-enumerating a lazy sequence can accidentally multiply work.

## Strings

In C#, `string` is immutable. Any operation that appears to modify a string creates a new string.

Important consequences:

- Repeated concatenation in a loop can become O(n^2).
- Use `StringBuilder` for many appends.
- Use `ReadOnlySpan<char>` when slicing without allocation is important.
- Character comparison should be explicit about casing and culture.

## Two Pointers

Two pointers use two indexes that move through the sequence.

Best fit:

- Sorted arrays
- Palindrome checks
- Removing duplicates
- Pair-sum problems

Example invariant for pair sum in sorted data:

- If `values[left] + values[right]` is too small, increase `left`.
- If it is too large, decrease `right`.
- This works because the array is sorted.

## Sliding Window

Sliding window keeps a contiguous range `[start, end]` and updates it as the scan advances.

Best fit:

- Longest or shortest substring/subarray
- Constraints such as "no repeated characters"
- Sum, count, or frequency inside a moving range

The hard part is deciding when the left boundary must move.

Typical invariant:

```text
The current window contains exactly the elements from start through end.
The window satisfies the constraint after every shrink step.
```

If the constraint can be violated multiple times, shrink with `while`, not `if`.

## Prefix Sums

Prefix sums trade O(n) preprocessing for O(1) range queries.

For array `values`, build:

```text
prefix[0] = 0
prefix[i + 1] = prefix[i] + values[i]
```

Then:

```text
sum(startInclusive, endExclusive) = prefix[endExclusive] - prefix[startInclusive]
```

This pattern is common in analytics, reporting, and interval problems.

## Pattern Selection

| Problem signal | Pattern to consider |
| -------------- | ------------------- |
| Sorted input and pair-like decision | Two pointers |
| Longest or shortest contiguous range | Sliding window |
| Many range-sum queries | Prefix sums |
| Need previous/next greater item | Monotonic stack |
| Need membership or counts | Hash set or dictionary |
| Need all combinations | Backtracking |

## C# Notes

- Prefer `ReadOnlySpan<T>` for read-only algorithm input when the method does not need to store it.
- Return indexes when the caller needs positions; return values when positions are irrelevant.
- Document whether the algorithm mutates input.
- Be careful with Unicode: `char` is a UTF-16 code unit, not always a full user-perceived character.
- Prefer `StringComparer.Ordinal` or `OrdinalIgnoreCase` for technical identifiers.
- Use `StringBuilder` when appending repeatedly in a loop.

## Practice Problems To Master

- Reverse a string or array in place.
- Check whether a string is a palindrome.
- Remove duplicates from a sorted array.
- Find two numbers in a sorted array that sum to a target.
- Find the longest substring without repeating characters.
- Count subarray sums with prefix sums.

