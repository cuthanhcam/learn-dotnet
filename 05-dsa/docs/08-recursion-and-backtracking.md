# Recursion and Backtracking

Recursion solves a problem by reducing it to smaller versions of itself. Backtracking uses recursion to explore choices and undo them after each branch.

## Recursion

Every recursive function needs:

- A base case
- Progress toward the base case
- A way to combine smaller answers

Example:

```text
factorial(n) = n * factorial(n - 1)
factorial(0) = 1
```

## Call Stack

Each recursive call adds a stack frame. Deep recursion can overflow the stack. In C#, prefer iterative implementations for very deep or untrusted input.

## Recursion Decision

Recursion fits naturally when the data or problem is self-similar:

- Trees
- Divide-and-conquer sorting
- Nested structures
- Search spaces made of repeated choices

Prefer iteration when:

- Input depth can be huge.
- The recursive state is simple.
- You need tight control over memory.
- The algorithm runs in a production hot path with untrusted input.

## Memoization

Memoization caches repeated subproblems.

Naive Fibonacci:

```text
O(2^n)
```

Memoized Fibonacci:

```text
O(n)
```

## Backtracking Template

```text
if complete:
    record answer
    return

for choice in choices:
    if choice is invalid:
        continue

    apply choice
    backtrack(next state)
    undo choice
```

The key invariant is that after `undo choice`, the local state is exactly as it was before `apply choice`.

## Common Problems

- Subsets
- Permutations
- Combination sum
- N-Queens
- Sudoku
- Word search

## Pruning

Pruning stops exploring branches that cannot lead to a valid answer. Sorting candidates often enables early stop rules such as `candidate > remaining`.

## Duplicate Handling

Backtracking with duplicate input needs deliberate rules. A common pattern after sorting:

```text
for i from start to end:
    if i > start and values[i] == values[i - 1]:
        continue
```

This skips duplicate choices at the same decision depth while still allowing the same value to appear in deeper positions when valid.

## C# Notes

- Copy mutable paths with `path.ToArray()` or `path.ToList()` before storing results.
- Use `List<T>.Add` and `RemoveAt(path.Count - 1)` for apply/undo.
- Avoid recursion over untrusted linked lists or graphs with very deep paths.
- Memoization usually uses `Dictionary<TKey, TValue>`; choose a stable key.
- Local functions can keep recursive helpers close to the public method.

## Pitfalls

- Forgetting to undo the choice.
- Adding the mutable `path` list directly instead of copying it.
- Missing duplicate handling when inputs contain repeated values.
- Using recursion where input depth can be too large.
- Sharing one mutable result object across all answers.
- Memoizing with a key that does not include all relevant state.

## Practice Problems To Master

- Factorial and Fibonacci with memoization.
- Generate subsets.
- Generate permutations.
- Combination sum.
- Word search.
- N-Queens.

