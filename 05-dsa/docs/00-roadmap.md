---
title: "DSA Learning Roadmap"
description: "The ordered study path, checkpoints, and mastery criteria for Phase 05."
phase: 5
order: 0
topics: [dsa, roadmap]
---

# DSA Roadmap

## Goal

Build enough algorithmic fluency to solve common data-structure problems in C#, explain the complexity, and choose appropriate trade-offs for backend systems.

This phase is intentionally important. DSA is where programming starts to feel less like syntax recall and more like controlled thinking: given a constraint, you choose a representation; given a representation, you know the cost of each operation; given the cost, you can defend the design.

## Learning Path

### Phase 1: Complexity and Measurement

1. **Big-O notation**
   - Growth classes
   - Time vs space
   - Worst, average, and amortized cost
   - Reading benchmark results without confusing them with Big-O

**Expected competencies:**

- Explain why O(n) beats O(n^2) as input grows.
- Name the dominant operation in a method.
- Describe the space cost of auxiliary collections.
- Use BenchmarkDotNet only after the algorithmic cost is understood.

---

### Phase 2: Linear Data and Index Thinking

2. **Arrays and strings**
   - Indexing
   - Two pointers
   - Sliding windows
   - Prefix sums
   - String immutability and allocation cost in C#

3. **Linked lists**
   - Node references
   - Reversal
   - Merge patterns
   - Slow/fast pointers
   - Cycle detection

**Expected competencies:**

- Move indexes without off-by-one errors.
- Choose between `T[]`, `List<T>`, `string`, `StringBuilder`, and `ReadOnlySpan<char>`.
- Reverse and merge linked lists by relinking nodes.
- Explain when linked lists are educational versus practical in C#.

---

### Phase 3: Order-Controlled Collections

4. **Stacks and queues**
   - LIFO and FIFO order
   - Parentheses and undo problems
   - BFS queues
   - Monotonic stacks
   - Queue from two stacks

5. **Hash tables**
   - `Dictionary<TKey, TValue>`
   - `HashSet<T>`
   - Frequency maps
   - Grouping
   - Equality and hash code rules

**Expected competencies:**

- Recognize when order of processing is the main problem.
- Use stacks for nested or reversible state.
- Use queues for level-by-level or first-arrived processing.
- Replace nested lookup loops with hash-based membership or counting.
- Choose string comparers intentionally.

---

### Phase 4: Relationships and Search Spaces

6. **Trees and graphs**
   - DFS and BFS
   - Binary tree traversal
   - Binary search trees
   - Adjacency lists and matrices
   - Visited state
   - Connected components and shortest paths in unweighted graphs

7. **Sorting and searching**
   - Comparison sorting
   - Stability
   - Binary search variants
   - Lower and upper bounds
   - Quickselect

8. **Recursion and backtracking**
   - Base cases
   - Call stack
   - Memoization
   - Choice/apply/recurse/undo
   - Pruning

**Expected competencies:**

- Traverse trees and graphs without revisiting nodes incorrectly.
- Choose DFS or BFS based on the question being asked.
- Use sorting to unlock simpler downstream logic.
- Write binary search with a clear boundary invariant.
- Build recursive and backtracking solutions without leaking mutable state.

## Completion Criteria

- You can explain the time and space complexity of each example.
- You can identify which data structure fits a problem's operations.
- You can write tests for empty, single-item, duplicate, and boundary cases.
- You can solve each exercise without copying from the examples.
- You can describe why an algorithm is correct, not only what it returns.
- You can connect each topic to at least one .NET/backend scenario.
- You can rewrite a solution after 24 hours without looking at the previous code.

## Practice Rhythm

For every topic:

1. Read the notes.
2. Trace the example by hand.
3. Run the tests.
4. Re-implement the exercise.
5. Add at least one edge-case test.
6. Write the complexity beside the method.

## Weekly Persistence Plan

Use this plan when studying the phase over multiple weeks.

| Day | Focus | Output |
| --- | ----- | ------ |
| 1 | Read one topic doc and trace one example | Handwritten or markdown trace |
| 2 | Implement two easy exercises | Passing tests |
| 3 | Re-implement from memory | Cleaner solution or named invariant |
| 4 | Add edge cases and compare approaches | Extra tests |
| 5 | Solve one medium problem from the topic | Complexity note |
| 6 | Review mistakes | Update notes or pitfalls |
| 7 | Rest or light recap | One-page summary |

Repeat the loop per topic. The goal is compounding recall, not rushing.

## Problem-Solving Template

Before coding, write:

```text
Problem:
Input:
Output:
Constraints:
Edge cases:
Brute force:
Better idea:
Data structure:
Invariant:
Time:
Space:
```

After coding, ask:

- Did I mutate the input?
- Did I handle empty and single-item cases?
- Did I handle duplicates?
- Did I prove the loop or recursion stops?
- Is the complexity based on the actual dominant operation?

## Backend Connections

| DSA topic | Backend connection |
| --------- | ------------------ |
| Big-O | API latency analysis, query planning, hot path reviews |
| Arrays/strings | request parsing, CSV processing, payload transformations |
| Linked lists | understanding references, queues, caches, low-level structures |
| Stacks/queues | background jobs, undo workflows, BFS dependency processing |
| Hash tables | caches, idempotency keys, deduplication, lookup tables |
| Trees/graphs | authorization hierarchies, dependency graphs, category trees |
| Sorting/searching | pagination, ranking, indexes, binary search over answer space |
| Recursion/backtracking | tree walking, rule engines, combinatorial validation |

## Recommended Phase Exit Exercise

Build a small in-memory "route planner" console feature:

- Store locations as graph nodes.
- Store roads as unweighted edges.
- Use BFS to find the shortest number of stops.
- Use a dictionary for location lookup.
- Add tests for no route, same start/end, and disconnected components.
- Explain time and space complexity in comments or docs.
