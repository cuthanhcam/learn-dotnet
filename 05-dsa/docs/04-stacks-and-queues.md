---
title: "Stacks and Queues"
description: "LIFO/FIFO invariants, balancing, monotonic stacks, two-stack queues, and breadth-first traversal."
slug: dsa-stacks-queues
phase: 5
order: 4
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 26
topics: [dsa, stacks, queues]
prerequisites: [dsa-linked-lists]
status: maintained
last-reviewed: 2026-08-15
---

# Stacks and Queues

Stacks and queues are restricted-access collections. Their value comes from making order explicit.

## Stack

A stack is LIFO: last in, first out.

Use a stack for:

- Parentheses matching
- Undo history
- DFS traversal
- Monotonic stack problems
- Simulating recursion
- Nested scopes such as parsing tags, JSON-like structures, or expression evaluation

Operations:

| Operation | Complexity |
| --- | --- |
| Push | O(1) |
| Pop | O(1) |
| Peek | O(1) |

## Queue

A queue is FIFO: first in, first out.

Use a queue for:

- BFS traversal
- Work scheduling
- Producer/consumer pipelines
- Level-order tree traversal
- Shortest path in unweighted state spaces

Operations:

| Operation | Complexity |
| --- | --- |
| Enqueue | O(1) |
| Dequeue | O(1) |
| Peek | O(1) |

## Queue From Two Stacks

One stack receives new values. The other stack serves dequeue operations. When the outgoing stack is empty, move all incoming values to it.

Each item moves at most twice, so operations are amortized O(1).

## Monotonic Stack

A monotonic stack keeps values or indexes in sorted order. It is common in "next greater element" problems.

Each index is pushed once and popped at most once, so the whole scan is O(n).

Store indexes instead of values when:

- You need distances.
- Values can repeat.
- You need to update an answer array.

Typical invariant:

```text
The stack contains indexes whose answer has not been found yet.
Values at those indexes are monotonic according to the problem rule.
```

## BFS

BFS uses a queue and a visited set. Mark a node as visited before enqueueing it so duplicate edges do not enqueue the same node repeatedly.

## Stack vs Queue Decision

| Need | Choose |
| ---- | ------ |
| Most recent item first | `Stack<T>` |
| Earliest item first | `Queue<T>` |
| Explore deep path before siblings | Stack or recursive DFS |
| Explore by distance or level | Queue-based BFS |
| Match nested open/close tokens | `Stack<T>` |
| Process background work in arrival order | `Queue<T>` or a concurrent/channel abstraction |

## .NET Notes

- Use `Stack<T>.TryPop` and `Queue<T>.TryDequeue` when empty collections are possible.
- `Queue<T>` and `Stack<T>` are not thread-safe for concurrent producers/consumers.
- For real concurrent pipelines, later phases should use `Channel<T>` or concurrent collections.
- Avoid using `List<T>.RemoveAt(0)` as a queue; shifting elements is O(n).

