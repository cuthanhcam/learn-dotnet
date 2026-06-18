# Common DSA Pitfalls

- Confusing Big-O with exact runtime.
- Forgetting to handle empty input and single-item input.
- Using nested loops when a hash table would express the lookup directly.
- Mutating caller-owned arrays or lists without documenting it.
- Missing duplicate values in two-pointer or sorting problems.
- Writing binary search with `while (left < right)` without proving the boundary invariant.
- Recursing without a clear base case.
- Forgetting to mark graph nodes as visited before enqueuing or recursing.
- Reusing backtracking state without undoing the choice.
- Optimizing before the algorithm is correct and tested.

