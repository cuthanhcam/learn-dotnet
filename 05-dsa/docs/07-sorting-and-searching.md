# Sorting and Searching

Sorting changes the shape of a problem. Once data is ordered, binary search, two pointers, range queries, and duplicate handling become much easier.

## Sorting

Common sorting options in C#:

- `Array.Sort`
- `List<T>.Sort`
- LINQ `OrderBy`, which returns a new ordered sequence

Comparison sorting has a lower bound of O(n log n). Specialized sorts can be faster when the input domain is constrained.

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

## Boundary Discipline

For every binary search, define:

- What does `left` mean?
- What does `right` mean?
- Is `right` inclusive or exclusive?
- What condition proves the answer is not in the discarded half?

Most binary search bugs are invariant bugs.

