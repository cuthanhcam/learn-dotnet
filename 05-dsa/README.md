---
title: "Phase 05 — Data Structures and Algorithms"
description: "A practical C# curriculum for complexity analysis, core data structures, traversal, searching, sorting, recursion, backtracking, and deliberate problem-solving practice."
phase: 5
status: complete
target-framework: net8.0
prerequisites: [phase-04-memory-performance]
previous-phase: ../04-memory-performance/README.md
next-phase: ../06-async-concurrency/README.md
---

# Data Structures & Algorithms (05-dsa)

> A practical C# module for algorithmic thinking, complexity analysis, interview-style problem solving, and better backend design decisions.

## Overview

This module focuses on learning how to reason about data shape, operation cost, trade-offs, and correctness. The goal is not to memorize solutions, but to recognize patterns and choose the right data structure or algorithm for the constraints in front of you.

DSA is a bridge between "I know C# syntax" and "I can design reliable software." In .NET backend work, the same thinking appears in API pagination, indexing, caching, deduplication, scheduling, graph-like permissions, dependency traversal, query optimization, and performance reviews.

You will learn:

- Big-O notation and how to compare time and space complexity
- Arrays, strings, two pointers, sliding windows, and prefix sums
- Linked lists and pointer manipulation
- Stacks, queues, monotonic stacks, and breadth-first traversal
- Hash tables, sets, frequency maps, and collision-aware thinking
- Trees, graphs, DFS, BFS, and graph representations
- Sorting, searching, binary search boundaries, and selection patterns
- Recursion, backtracking, pruning, and state restoration

## Setup

```bash
cd 05-dsa
dotnet --version
dotnet build
dotnet run --project src/Dsa.ConsoleApp
dotnet run --project exercises/Dsa.Exercises
dotnet test
dotnet run -c Release --project benchmarks/Dsa.Benchmarks
```

## Project Structure

```text
05-dsa/
|
|-- 05-dsa.slnx
|-- README.md
|
|-- src/
|   |-- Dsa.ConsoleApp/
|   |-- Dsa.Examples/
|       |-- Complexity/
|       |-- ArraysStrings/
|       |-- LinkedLists/
|       |-- StacksQueues/
|       |-- HashTables/
|       |-- TreesGraphs/
|       |-- SortingSearching/
|       |-- RecursionBacktracking/
|
|-- exercises/
|   |-- Dsa.Exercises/
|
|-- tests/
|   |-- Dsa.Tests/
|
|-- benchmarks/
|   |-- Dsa.Benchmarks/
|
|-- docs/
    |-- 00-roadmap.md
    |-- 01-big-o-notation.md
    |-- 02-arrays-and-strings.md
    |-- 03-linked-lists.md
    |-- 04-stacks-and-queues.md
    |-- 05-hash-tables.md
    |-- 06-trees-and-graphs.md
    |-- 07-sorting-and-searching.md
    |-- 08-recursion-and-backtracking.md
    |-- common-pitfalls.md
```

## What's Inside

| Topic                      | Focus                                                                   |
| -------------------------- | ----------------------------------------------------------------------- |
| Big-O                      | Growth rate, input size, nested loops, amortized cost, space complexity |
| Arrays and strings         | Indexing, two pointers, sliding windows, prefix sums, immutable strings |
| Linked lists               | Node references, reversal, merging, slow/fast pointers                  |
| Stacks and queues          | LIFO/FIFO, parentheses, monotonic stacks, BFS queues                    |
| Hash tables                | Frequency maps, membership, grouping, collision trade-offs              |
| Trees and graphs           | Traversal, recursion, BFS/DFS, adjacency lists, visited sets            |
| Sorting and searching      | Comparison sorting, binary search, stable sorting, boundary bugs        |
| Recursion and backtracking | Base cases, call stack, choices, pruning, restore state                 |

## Why This Phase Matters

- It trains you to read constraints before writing code.
- It turns performance from guesswork into explainable trade-offs.
- It makes standard .NET collections feel predictable instead of magical.
- It improves debugging because you can reason about invariants and state transitions.
- It prepares you for interviews without separating interview practice from real engineering.

## Module Rules

1. Every algorithm exposes deterministic methods that tests can verify.
2. `Run()` methods are for guided console demonstrations.
3. Complexity notes live near the code and in the matching docs.
4. Exercises favor small, focused problems over giant challenge dumps.
5. Correctness comes before micro-optimization.
6. Every solution should be explainable with a short invariant or proof idea.
7. Every exercise should include edge cases before performance tuning.

## Study Order

1. Read `docs/00-roadmap.md`.
2. Read the topic doc before opening the matching code folder.
3. Run the console app and predict the output before reading the implementation.
4. Run tests after each topic.
5. Re-implement exercise methods without looking at examples.
6. Use benchmarks only after you understand the asymptotic difference.
7. Capture mistakes in `docs/common-pitfalls.md` or your own notes.

## Practice Loop

Use this loop for every problem, even small ones:

1. Restate the problem in your own words.
2. Write down inputs, outputs, constraints, and edge cases.
3. Start with the simplest correct solution.
4. Identify the bottleneck.
5. Choose a better data structure or pattern.
6. Prove the key invariant.
7. Implement in C#.
8. Test empty, single-item, duplicate, sorted, reverse-sorted, and large cases when relevant.
9. State time and space complexity.

## .NET Collection Map

| Need                           | Common .NET type                  | Typical cost model                         |
| ------------------------------ | --------------------------------- | ------------------------------------------ |
| Indexed contiguous data        | `T[]`, `List<T>`                  | O(1) index read/write, O(n) middle insert  |
| Membership lookup              | `HashSet<T>`                      | Average O(1) add/contains/remove           |
| Key-value lookup               | `Dictionary<TKey, TValue>`        | Average O(1) lookup/update                 |
| FIFO processing                | `Queue<T>`                        | O(1) enqueue/dequeue                       |
| LIFO processing                | `Stack<T>`                        | O(1) push/pop                              |
| Sorted unique data             | `SortedSet<T>`                    | O(log n) add/contains/remove               |
| Sorted key-value lookup        | `SortedDictionary<TKey, TValue>`  | O(log n) lookup/update                     |
| Immutable read-mostly data     | `ImmutableArray<T>`, immutable collections | More allocation, safer sharing    |
| Allocation-sensitive slicing   | `Span<T>`, `ReadOnlySpan<T>`      | Avoids copying, lifetime must stay scoped  |

## Key Mental Models

- Big-O describes growth as input size increases, not exact elapsed time.
- The best data structure depends on the operations you perform most often.
- Hashing buys average-case speed by spending memory and accepting collision handling.
- Trees and graphs are mostly about traversal order plus visited-state rules.
- Binary search is a boundary-management algorithm, not just "find middle".
- Backtracking is disciplined trial, validation, recursion, and undo.
- Invariants are promises that stay true while the algorithm runs.
- Better algorithms usually come from changing the representation, not typing faster code.

## Documentation

| File                               | Purpose                                               |
| ---------------------------------- | ----------------------------------------------------- |
| `00-roadmap.md`                    | Learning path and completion criteria                 |
| `01-big-o-notation.md`             | Complexity vocabulary and common growth classes       |
| `02-arrays-and-strings.md`         | Contiguous data, string costs, two pointers, windows  |
| `03-linked-lists.md`               | Nodes, references, reversal, merging, cycle detection |
| `04-stacks-and-queues.md`          | LIFO/FIFO abstractions and traversal use cases        |
| `05-hash-tables.md`                | Dictionaries, sets, frequency maps, grouping          |
| `06-trees-and-graphs.md`           | Hierarchical and network data traversal               |
| `07-sorting-and-searching.md`      | Sorting trade-offs and binary search variants         |
| `08-recursion-and-backtracking.md` | Recursive decomposition and search-space exploration  |
| `09-practice-system.md`            | Daily/weekly practice cadence and review habits       |
| `10-dotnet-dsa-reference.md`       | .NET collection and API reference for DSA choices     |
| `11-advanced-tree-indexes.md`      | AVL, heaps, tries, Fenwick/segment trees, and B-trees |
| `12-advanced-graph-algorithms.md`  | DAGs, shortest paths, MST, SCC, and Union-Find        |
| `13-dynamic-programming-greedy.md` | DP state design, memory compression, and greedy proof |
| `common-pitfalls.md`               | Mistakes to catch during practice and review          |

Advanced implementations include `AvlTree<T>`, `BinaryMinHeap<T>`, `PrefixTrie`, `FenwickTree`,
`SegmentTree`, `DisjointSet`, Dijkstra shortest paths, Kahn topological sorting, Kruskal minimum
spanning forests, and Tarjan strongly connected components. Each type is kept separate so its
invariant, complexity, and boundary tests remain easy to study.

## Mastery Check

You are ready to move beyond this phase when you can:

- Solve easy problems without pattern hints.
- Solve medium problems by reducing them to a known pattern.
- Explain why a chosen data structure fits the dominant operation.
- Write tests before polishing the implementation.
- Compare a brute-force solution with an optimized solution using Big-O.
- Connect algorithm choices back to .NET backend scenarios.

## Verification Matrix

| Area | Correctness evidence | Complexity explanation |
|---|---|---|
| Arrays and strings | Boundary, duplicate, Unicode-aware caveat, and empty-input tests | Indexing, scanning, window, and prefix-sum costs |
| Linked lists | Empty, one-node, reversal, merge, and cycle cases | Traversal versus known-node mutation |
| Stacks and queues | Empty behavior, ordering, balancing, and two-stack queue tests | Amortized movement and LIFO/FIFO invariants |
| Hash tables | Missing keys, duplicates, grouping, and complement lookup | Average and worst-case lookup plus memory trade-off |
| Trees and graphs | Empty tree, traversal order, disconnected graph, and cycle tests | `O(V + E)` traversal and auxiliary state |
| Sorting and searching | Sorted/reverse/duplicate inputs and boundary searches | Comparison lower bound and binary-search interval invariant |
| Recursion and backtracking | Base case, restoration, duplicate choices, and pruning | Call-stack depth and search-space growth |

## Completion Criteria

- [ ] State the input size and dominant operation before giving Big-O.
- [ ] Distinguish worst-case, average-case, and amortized complexity where relevant.
- [ ] Explain the invariant for each implemented algorithm.
- [ ] Solve at least one problem with two pointers, sliding window, prefix sums, hashing, BFS, DFS, binary search, and backtracking.
- [ ] Compare a brute-force solution with an improved representation or data structure.
- [ ] Test empty, singleton, duplicate, boundary, and adversarial inputs.
- [ ] Run benchmarks only after predicting the asymptotic outcome.
- [ ] Pass `dotnet test 05-dsa.slnx`.

## Next Phase

Continue with [Phase 06 — Async and Concurrency](../06-async-concurrency/README.md), where algorithmic reasoning is applied to scheduling, cancellation, synchronization, bounded work, and concurrent state.
