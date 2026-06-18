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

## Common Problems

- Subsets
- Permutations
- Combination sum
- N-Queens
- Sudoku
- Word search

## Pruning

Pruning stops exploring branches that cannot lead to a valid answer. Sorting candidates often enables early stop rules such as `candidate > remaining`.

## Pitfalls

- Forgetting to undo the choice.
- Adding the mutable `path` list directly instead of copying it.
- Missing duplicate handling when inputs contain repeated values.
- Using recursion where input depth can be too large.

