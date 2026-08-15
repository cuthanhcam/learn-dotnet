---
title: "Advanced Graph Algorithms: DAGs, Shortest Paths, MST, SCC, and Union-Find"
description: "A correctness- and complexity-oriented guide to graph ordering, paths, connectivity, spanning trees, and components."
phase: 5
order: 12
topics: [dsa, graphs, dijkstra, topological-sort, mst, scc, union-find]
article-type: deep-dive
estimated-reading-minutes: 25
prerequisites: [bfs, dfs, priority-queues, complexity-analysis]
---

# Advanced Graph Algorithms

Graph problems become manageable when the representation, edge direction, weight constraints, and required output are stated before choosing an algorithm.

## Algorithm Selection

| Problem | Algorithm | Preconditions | Complexity |
|---|---|---|---|
| Unweighted shortest path | BFS | Equal edge cost | `O(V + E)` |
| Non-negative weighted paths | Dijkstra | No negative edge | `O((V + E) log V)` with heap |
| Negative edges | Bellman-Ford | Detects reachable negative cycle | `O(VE)` |
| All-pairs shortest paths | Floyd-Warshall | Dense/small graph | `O(V³)` time, `O(V²)` space |
| Dependency ordering | Kahn or DFS topo sort | Directed acyclic graph | `O(V + E)` |
| Minimum spanning tree | Kruskal or Prim | Weighted undirected graph | commonly `O(E log V)` |
| Strongly connected components | Kosaraju or Tarjan | Directed graph | `O(V + E)` |
| Dynamic connectivity merging | Union-Find | Merge/query workload | amortized `O(α(n))` |

## Modeling Rules

An adjacency list is usually best for sparse graphs. An adjacency matrix uses `O(V²)` space but gives constant-time edge lookup. Define whether parallel edges and self-loops are allowed. Ensure every referenced destination is represented, or define an API that creates missing vertices consistently.

## Topological Sorting

Kahn's algorithm maintains each vertex's in-degree. Zero-in-degree vertices are ready because all prerequisites have been emitted. Removing one conceptually deletes its outgoing edges. If work stops before all vertices are emitted, remaining vertices participate in or depend on a cycle.

Topological order is generally not unique. Tests should validate every edge's relative order rather than hard-code one ordering unless the algorithm specifies a deterministic tie-breaker.

## Dijkstra's Algorithm

Dijkstra finalizes shortest distances by repeatedly processing the smallest queued distance. Its proof depends on non-negative weights: extending a path cannot make it shorter after a vertex has the smallest known distance.

.NET's `PriorityQueue` does not expose decrease-key. Enqueue the improved distance again and skip a dequeued entry when its priority no longer equals the stored best distance. Negative edges must be rejected rather than producing silently incorrect results.

Distances use `long` and checked addition in the example. Unreachable vertices retain `long.MaxValue`; callers may instead prefer a nullable/result representation.

## Minimum Spanning Trees

Kruskal sorts all edges, then accepts an edge only if it joins different components. Union-Find detects that condition efficiently. Prim starts from one vertex and repeatedly chooses the cheapest crossing edge.

An MST minimizes total connection cost. It does not minimize distance from a source. Disconnected input produces a minimum spanning forest unless the API explicitly rejects it.

## Strongly Connected Components

In an SCC, every vertex can reach every other vertex. Kosaraju uses two DFS passes and a reversed graph. Tarjan uses discovery indexes, low-link values, and a stack in one traversal. SCC condensation turns a directed graph into a DAG and enables higher-level dependency analysis.

## Union-Find Invariant

Each component is a rooted parent tree. Roots parent themselves. `Find` returns the representative. Path compression rewrites visited parents to the root; union by size attaches the smaller tree below the larger. These heuristics produce nearly constant amortized operations.

## Testing Strategy

- Include isolated vertices and disconnected components.
- Test zero-weight edges and alternative routes.
- Reject negative weights in Dijkstra.
- Validate topological edge order and cycle rejection.
- Test redundant union and component count.
- Include self-loops and parallel edges when the contract permits them.
- Compare small random shortest-path cases with a slower reference algorithm.

## Implementation Map

- `TreesGraphs/WeightedGraphAlgorithms.cs`: Dijkstra and Kahn topological sorting.
- `TreesGraphs/DisjointSet.cs`: path compression and union by size.
- `AdvancedGraphAlgorithmsTests.cs`: correctness, invalid inputs, cycles, and connectivity.

## Exercises

1. Return predecessor data and reconstruct a Dijkstra shortest path.
2. Implement Bellman-Ford and identify vertices affected by a negative cycle.
3. Implement Kruskal with stable edge tie-breaking.
4. Implement Tarjan SCC and build the condensation DAG.
5. Implement multi-source BFS for nearest-service distance on a grid.

## Navigation

[← Advanced tree indexes](11-advanced-tree-indexes.md) · [Dynamic programming and greedy →](13-dynamic-programming-greedy.md)
