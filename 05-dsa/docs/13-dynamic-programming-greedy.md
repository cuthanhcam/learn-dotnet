---
title: "Dynamic Programming and Greedy Algorithms"
description: "How to recognize overlapping subproblems, design DP state and transitions, optimize memory, and prove greedy choices."
slug: dsa-dynamic-programming-greedy
phase: 5
order: 13
topics: [dsa, dynamic-programming, greedy, knapsack, subsequences]
article-type: deep-dive
estimated-reading-minutes: 28
prerequisites: [dsa-recursion-backtracking, dsa-big-o, dsa-sorting-searching]
difficulty: advanced
status: maintained
last-reviewed: 2026-08-15
---

# Dynamic Programming and Greedy Algorithms

Dynamic programming and greedy algorithms both avoid exhaustive search, but their correctness arguments differ. DP records solutions to repeated subproblems. Greedy commits to a locally optimal choice that must be proven compatible with a global optimum.

## Learning Objectives

- Recognize optimal substructure and overlapping subproblems.
- Define state in one precise sentence.
- Derive transitions, base cases, evaluation order, and final extraction.
- Convert memoization to tabulation when useful.
- Compress DP memory without reusing a state too early.
- Distinguish polynomial, pseudo-polynomial, and output-dependent complexity.
- Prove a greedy strategy with exchange or staying-ahead reasoning.

## A Repeatable DP Design Process

1. Start with a correct recursive relation.
2. Identify the minimal variables that uniquely describe a subproblem.
3. Define `dp[state]` in plain language.
4. Write choices and the transition equation.
5. Define base cases and impossible-state sentinels.
6. Determine a dependency-safe evaluation order.
7. Identify where the answer lives.
8. Analyze state count multiplied by transition cost.
9. Compress memory only after proving which prior states remain necessary.

## Memoization and Tabulation

Top-down memoization follows only reachable states and maps naturally from recursion, but consumes call-stack space and has lookup overhead. Bottom-up tabulation makes evaluation order explicit, avoids recursive depth, and can enable compact arrays. Neither is universally faster or clearer.

## 0/1 Knapsack

State: the best value achievable with processed items and capacity `c`.

For each item `(weight, value)`, the transition chooses skip or take. A one-dimensional array is valid when capacity is iterated downward. Upward iteration would see the current item's freshly updated state and convert the problem accidentally into unbounded knapsack.

Complexity is `O(nC)` time and `O(C)` space, where `C` is numeric capacity. This is pseudo-polynomial: input encoding needs only `log C` bits.

## Longest Increasing Subsequence

The classic DP checks every earlier item in `O(n²)`. The patience-style algorithm maintains the smallest possible tail for each subsequence length. Binary search finds the first tail greater than or equal to the current value, giving `O(n log n)` time.

The tail array is optimization state and is not necessarily an actual subsequence. Reconstructing a sequence requires predecessor and position tracking.

Strictly increasing and non-decreasing variants use different lower/upper-bound comparisons; state this contract explicitly.

## Edit Distance

For prefixes ending at `(i, j)`, choose insertion, deletion, or substitution. If characters match, substitution cost is zero. The full table uses `O(mn)` space, but each row depends only on the previous row and current row, so distance-only output needs `O(min(m,n))` space.

The included implementation uses spans and stack storage only for small rows. Unicode and domain-specific edit costs need explicit policy beyond UTF-16 code-unit comparison.

## Other DP Families

- Grid paths: position state, obstacles, movement rules.
- Coin change: minimum/count variants and loop order.
- Subset sum/partition: boolean reachable sums.
- Longest common subsequence: two-prefix state.
- Interval DP: solve by interval length and split point.
- Tree DP: return selected state summaries from children.
- Bitmask DP: subset state, usually exponential but useful for small `n`.

## Greedy Proof Patterns

A greedy implementation is incomplete without a reason the local choice is safe.

- Exchange argument: transform an optimal solution to use the greedy first choice without making it worse.
- Staying ahead: show every prefix of the greedy solution is at least as good as another solution's prefix.
- Cut property: the cheapest safe edge across a cut can belong to an MST.
- Matroid structure: independence properties make greedy selection valid in a broader class of problems.

## Interval Scheduling

To select the maximum number of non-overlapping intervals, choose the compatible interval with the earliest finishing time. An exchange argument replaces the first interval of an optimal solution with the greedy interval; finishing no later leaves at least as much room for all later selections.

Sorting costs `O(n log n)` and the scan is `O(n)`. Define whether touching endpoints overlap. The example treats `[a,b)`-style adjacency as compatible when `next.Start >= previous.End`.

Greedy fails for weighted interval scheduling because maximizing count and maximizing total value are different objectives; weighted scheduling requires DP after predecessor search.

## Implementation Map

- `DynamicProgramming/DynamicProgrammingAlgorithms.cs`: LIS, 0/1 knapsack, edit distance.
- `DynamicProgramming/GreedyAlgorithms.cs`: earliest-finish interval scheduling.
- `DynamicProgrammingAlgorithmsTests.cs`: empty, monotonic, capacity, edit, and compatibility cases.

## Common Pitfalls

- Defining a state that omits information needed by future choices.
- Using a mutable memoization key.
- Filling a table in an order that reads unavailable states.
- Iterating one-dimensional knapsack capacity in the wrong direction.
- Reporting `O(nC)` as polynomial without noting pseudo-polynomial capacity.
- Copying a greedy rule from a similar problem without a proof.
- Returning the LIS tail optimization array as the actual subsequence.
- Ignoring overflow or impossible-state sentinel arithmetic.

## Exercises

1. Reconstruct one LIS, not only its length.
2. Return selected item indexes for 0/1 knapsack.
3. Implement longest common subsequence with reconstruction.
4. Solve weighted interval scheduling with binary-searched predecessors.
5. Compare memoized and tabulated coin-change implementations.
6. Implement an interval DP for matrix-chain multiplication.

## Review Questions

1. What exactly does each DP cell mean?
2. Why does descending capacity enforce 0/1 usage?
3. Why is knapsack pseudo-polynomial?
4. What proof makes earliest-finish interval scheduling correct?
5. Which information is lost during memory compression?

## Navigation

[← Advanced graph algorithms](12-advanced-graph-algorithms.md) · [Common pitfalls →](common-pitfalls.md)
