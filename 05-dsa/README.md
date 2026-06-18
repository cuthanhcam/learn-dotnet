# Data Structures & Algorithms (05-dsa)

> A practical C# module for algorithmic thinking, complexity analysis, and interview-style problem solving.

## Overview

This module focuses on learning how to reason about data shape, operation cost, trade-offs, and correctness. The goal is not to memorize solutions, but to recognize patterns and choose the right data structure or algorithm for the constraints in front of you.

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

## Module Rules

1. Every algorithm exposes deterministic methods that tests can verify.
2. `Run()` methods are for guided console demonstrations.
3. Complexity notes live near the code and in the matching docs.
4. Exercises favor small, focused problems over giant challenge dumps.
5. Correctness comes before micro-optimization.

## Study Order

1. Read `docs/00-roadmap.md`.
2. Read the topic doc before opening the matching code folder.
3. Run the console app and predict the output before reading the implementation.
4. Run tests after each topic.
5. Re-implement exercise methods without looking at examples.
6. Use benchmarks only after you understand the asymptotic difference.

## Key Mental Models

- Big-O describes growth as input size increases, not exact elapsed time.
- The best data structure depends on the operations you perform most often.
- Hashing buys average-case speed by spending memory and accepting collision handling.
- Trees and graphs are mostly about traversal order plus visited-state rules.
- Binary search is a boundary-management algorithm, not just "find middle".
- Backtracking is disciplined trial, validation, recursion, and undo.

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
| `common-pitfalls.md`               | Mistakes to catch during practice and review          |
