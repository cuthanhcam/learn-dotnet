---
title: "Sorting and Searching"
description: "Sorting trade-offs, stability, comparison bounds, binary-search intervals, and boundary variants."
phase: 5
order: 7
topics: [dsa, sorting, searching, binary-search]
---

# Sorting and Searching

Sorting changes the shape of a problem. Once data is ordered, binary search, two pointers, range queries, and duplicate handling become much easier.

## Sorting

Common sorting options in C#:

- `Array.Sort`
- `List<T>.Sort`
- LINQ `OrderBy`, which returns a new ordered sequence

Comparison sorting has a lower bound of O(n log n). Specialized sorts can be faster when the input domain is constrained.

## Stability

A stable sort preserves the relative order of equal keys. Stability matters when multiple sort passes or secondary ordering rules are important.

Example: sort users by `LastName` while preserving original order for users with the same last name. If the original order carries meaning, stability is part of correctness.

## Merge Sort

Merge sort divides the array in half, sorts both halves, then merges them.

Complexity:

- O(n log n) time
- O(n) extra space
- Stable when merge prefers the left item on equal keys

## Quickselect

Quickselect finds the kth smallest or largest element without fully sorting the input.

Average complexity:

- O(n) time
- O(1) extra space

Worst case:

- O(n^2), depending on pivot choices

## Binary Search

Binary search repeatedly halves a sorted search space.

Classic exact search:

```text
left = 0
right = n - 1
while left <= right
```

Lower bound search:

```text
left = 0
right = n
while left < right
```

Lower bound returns the first index whose value is greater than or equal to the target. This is often more useful than exact search when duplicates exist.

Upper bound returns the first index whose value is greater than the target. Together, lower and upper bounds can find the range occupied by duplicate values.

## Boundary Discipline

For every binary search, define:

- What does `left` mean?
- What does `right` mean?
- Is `right` inclusive or exclusive?
- What condition proves the answer is not in the discarded half?

Most binary search bugs are invariant bugs.

## Binary Search On Answer

Binary search is not only for arrays. It can search a numeric answer when:

- The answer space is ordered.
- You can test whether a candidate answer is feasible.
- Feasibility is monotonic.

Example signals:

- Minimum capacity to ship packages within D days.
- Smallest speed to finish work within H hours.
- Lowest threshold that satisfies a requirement.

## Sorting Trade-Offs

Sorting can be worth O(n log n) when it enables:

- Two-pointer scans
- Duplicate grouping
- Range queries
- Binary search
- Greedy decisions

Do not sort when original order is required unless you store original indexes or sort a copy.

## .NET Notes

- `Array.Sort` and `List<T>.Sort` mutate input.
- LINQ `OrderBy` returns a sorted sequence and does not mutate the source.
- Comparers should be explicit for strings and domain-specific ordering.
- For large hot paths, avoid repeated sorting when a maintained sorted structure or index fits better.

## Practice Problems To Master

- Binary search exact target.
- Lower bound and upper bound.
- Search insert position.
- Search in a rotated sorted array.
- Merge intervals.
- Find kth largest with quickselect.

