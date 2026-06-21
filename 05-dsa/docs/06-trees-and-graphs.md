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

