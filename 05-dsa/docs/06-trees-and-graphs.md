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

## BFS

Breadth-first search explores by distance from the start node. It is useful for:

- Shortest path in unweighted graphs
- Level-order traversal
- Minimum moves in state-space problems

## Visited State

Graphs can contain cycles, so traversal must track visited nodes. Mark nodes as visited before enqueueing or recursing into them to avoid repeated work.

