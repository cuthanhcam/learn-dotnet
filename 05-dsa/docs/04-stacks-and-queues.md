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

## BFS

BFS uses a queue and a visited set. Mark a node as visited before enqueueing it so duplicate edges do not enqueue the same node repeatedly.

