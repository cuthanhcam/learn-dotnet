---
title: "Trees and Graphs"
description: "Representations, recursive and iterative DFS, BFS, visited state, cycles, and disconnected components."
slug: dsa-trees-graphs
phase: 5
order: 6
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 34
topics: [dsa, trees, graphs, traversal]
prerequisites: [dsa-hash-tables]
status: maintained
last-reviewed: 2026-08-15
---

# Trees and Graphs

Trees and graphs represent relationships. A tree is hierarchical. A graph is more general and can contain cycles.

## Trees

A binary tree node has up to two children.

Common traversals:

- Preorder: node, left, right
- Inorder: left, node, right
- Postorder: left, right, node
- Level order: breadth-first by depth

For a binary search tree, inorder traversal returns values in sorted order.

## Tree Traversal Decision

| Question | Traversal |
| -------- | --------- |
| Need parent before children | Preorder |
| Need sorted values from a BST | Inorder |
| Need children before parent | Postorder |
| Need depth-by-depth order | Level-order BFS |
| Need path existence | DFS |
| Need minimum edges in an unweighted tree/graph | BFS |

## Tree Complexity

Most traversals are O(n) time because each node is visited once. Recursive DFS uses O(h) call-stack space where `h` is tree height.

Balanced tree:

```text
h = O(log n)
```

Skewed tree:

```text
h = O(n)
```

## Graph Representations

Adjacency list:

```text
A -> B, C
B -> D
C -> D
```

Good when graphs are sparse. Traversal cost is O(V + E), where `V` is vertices and `E` is edges.

Adjacency matrix:

```text
matrix[from][to] = edge exists
```

Good when graphs are dense or edge lookup must be O(1), but space is O(V^2).

## DFS

Depth-first search follows one path deeply before backing up. It is useful for:

- Connected components
- Cycle detection
- Topological ordering
- Backtracking-style search

Recursive DFS is concise, but iterative DFS with `Stack<T>` is safer for very deep inputs.

## BFS

Breadth-first search explores by distance from the start node. It is useful for:

- Shortest path in unweighted graphs
- Level-order traversal
- Minimum moves in state-space problems

BFS invariant:

```text
When a node is dequeued, it has been reached using the fewest number of edges from the start node.
```

This invariant holds for unweighted graphs. Weighted shortest paths need different algorithms.

## Visited State

Graphs can contain cycles, so traversal must track visited nodes. Mark nodes as visited before enqueueing or recursing into them to avoid repeated work.

## Graph Modeling In C#

Common representations:

```csharp
var graph = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
```

Use a dictionary when node IDs are sparse or meaningful. Use arrays/lists when nodes are dense integer indexes.

For directed graphs, store only outgoing edges. For undirected graphs, add both directions.

## Backend Connections

- Role and permission inheritance can be modeled as a graph.
- Category trees and organization charts are trees.
- Dependency resolution is graph traversal plus cycle detection.
- Workflow engines often move through graph-like state transitions.
- BFS can find the fewest transformations or hops in an unweighted state space.

## Practice Problems To Master

- Maximum depth of a binary tree.
- Validate a binary search tree.
- Level-order traversal.
- Count connected components.
- Detect a graph cycle.
- Find shortest path length in an unweighted graph.

## Binary Search Trees

A BST maintains an ordering invariant: every key in a node's left subtree compares smaller, and every key in its right subtree compares larger, under one stable comparer. In-order traversal therefore returns sorted keys.

Search and update cost is `O(h)`, where `h` is tree height. A balanced tree has `h = O(log n)`; sorted insertion into an unbalanced BST can produce `h = O(n)`. The custom `BinarySearchTree<T>` teaches search, insertion, duplicate policy, and the three deletion cases:

1. leaf: replace the node with `null`;
2. one child: replace the node with that child;
3. two children: copy the in-order successor and remove it from the right subtree.

Use `SortedSet<T>` or `SortedDictionary<TKey,TValue>` in ordinary .NET application code unless a custom tree is itself the requirement.

## Self-Balancing Search Trees

AVL trees maintain a balance factor in `{-1, 0, 1}` and restore it with single or double rotations. Red-black trees use coloring rules to provide a looser balance with fewer rotations on many update workloads. Both guarantee logarithmic search and updates; their implementation details belong in dedicated lessons rather than being hidden inside a basic BST.

Database indexes commonly use B-trees or B+ trees because high branching factors reduce storage-page reads. They solve a different locality problem than pointer-heavy in-memory binary trees.

## Specialized Trees

- A binary heap provides `O(1)` access to the minimum/maximum and `O(log n)` insertion/removal; .NET exposes `PriorityQueue<TElement,TPriority>`.
- A trie indexes keys by prefix and trades memory for prefix-query speed.
- A Fenwick tree supports prefix aggregation and point updates in `O(log n)` with compact storage.
- A segment tree supports configurable range queries and updates in `O(log n)` with more memory and implementation complexity.

## Tree Problem Checklist

State whether the tree is binary, ordered, balanced, complete, or arbitrary. Decide whether recursion depth is safe. Identify whether parent pointers, duplicate keys, mutation, or order statistics are part of the contract. Then write the traversal invariant before implementation.

## Directed Acyclic Graphs and Topological Order

A topological order places every prerequisite before each dependent vertex. Kahn's algorithm counts incoming edges, repeatedly removes zero-in-degree vertices, and decrements their outgoing neighbors. If fewer than `V` vertices are emitted, a directed cycle prevents an ordering.

The algorithm is `O(V + E)` time and `O(V)` auxiliary space beyond the graph. Common applications include build plans, course prerequisites, migrations, and dependency scheduling. A topological order is generally not unique.

## Weighted Shortest Paths

- BFS solves shortest paths in an unweighted graph or when every edge has equal cost.
- Dijkstra uses a min-priority queue and requires non-negative weights. With an adjacency list and binary heap it runs in `O((V + E) log V)`.
- Bellman-Ford supports negative edges and can detect a reachable negative cycle, at `O(VE)` cost.
- Floyd-Warshall computes all-pairs distances in `O(V³)` time and `O(V²)` space.

The provided Dijkstra implementation allows duplicate priority-queue entries and skips stale ones. This avoids requiring a decrease-key operation while retaining the expected complexity.

## Minimum Spanning Trees and Union-Find

A minimum spanning tree connects every vertex of a connected undirected weighted graph with minimum total edge weight. Kruskal sorts edges and accepts an edge only when Union-Find says its endpoints are currently disconnected. Prim grows one tree through a priority queue.

Union-Find (disjoint-set union) supports connectivity queries. Path compression plus union by size/rank gives nearly constant amortized operations, conventionally written `O(α(n))`, where the inverse Ackermann function grows extremely slowly.

Do not confuse an MST with shortest paths: an MST minimizes total tree weight, while a shortest-path tree minimizes distance from one source.
