---
title: "Common DSA Pitfalls"
description: "Complexity mistakes, broken invariants, boundary errors, recursion hazards, and weak testing patterns."
slug: dsa-common-pitfalls
phase: 5
order: 99
difficulty: reference
article-type: pitfalls
estimated-reading-minutes: 18
topics: [dsa, pitfalls, debugging]
prerequisites: [dsa-dynamic-programming-greedy]
status: maintained
last-reviewed: 2026-08-15
---

# Common DSA Pitfalls

## Complexity

- Confusing Big-O with exact runtime.
- Ignoring hidden loops inside LINQ methods.
- Forgetting the O(n log n) cost of sorting.
- Calling dictionary operations O(1) without saying average case.
- Counting time but not extra memory.
- Collapsing two input sizes into one `n` too early.

## Input and Edge Cases

- Forgetting empty input and single-item input.
- Missing duplicate values in two-pointer or sorting problems.
- Not testing target at the first or last position.
- Assuming input is sorted when the problem does not guarantee it.
- Mutating caller-owned arrays or lists without documenting it.
- Losing original indexes after sorting.

## Data Structure Choice

- Using nested loops when a hash table would express the lookup directly.
- Using `List<T>.RemoveAt(0)` as a queue.
- Using a dictionary key that can mutate after insertion.
- Using case-sensitive string keys when the domain is case-insensitive.
- Assuming dictionary iteration order is part of the business contract.
- Choosing recursion for input depth that can be huge.

## Implementation

- Writing binary search without proving the boundary invariant.
- Moving a sliding-window boundary only once when it must move until valid.
- Losing the rest of a linked list before storing `next`.
- Comparing linked-list node values when reference equality is required.
- Recursing without a clear base case.
- Forgetting to mark graph nodes as visited before enqueueing or recursing.
- Reusing backtracking state without undoing the choice.
- Adding a mutable `path` directly to results instead of copying it.
- Memoizing with a key that does not represent the full state.

## Learning Habits

- Optimizing before the algorithm is correct and tested.
- Reading many solutions without re-implementing from memory.
- Memorizing problem names instead of recognizing operations.
- Skipping the brute-force explanation.
- Skipping tests because the sample input passes.
- Treating DSA as separate from real .NET backend engineering.

