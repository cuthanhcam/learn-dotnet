---
title: "Advanced Graph Algorithms: DAGs, Shortest Paths, MST, SCC, and Union-Find"
description: "A correctness- and complexity-oriented guide to graph ordering, paths, connectivity, spanning trees, and components."
slug: dsa-advanced-graph-algorithms
phase: 5
order: 12
topics: [dsa, graphs, dijkstra, topological-sort, mst, scc, union-find]
article-type: deep-dive
estimated-reading-minutes: 25
prerequisites: [dsa-trees-graphs, dsa-advanced-tree-indexes, dsa-big-o]
difficulty: advanced
status: maintained
last-reviewed: 2026-08-15
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

The repository's Kruskal implementation accepts an explicit vertex set because isolated vertices
cannot be inferred from an edge list. It validates unique vertices and known endpoints, then sorts
edges by `(weight, input order)`. Input order is not required for correctness, but makes equal-weight
choices deterministic for teaching, tests, and diagnostics.

Negative edge weights are valid for a minimum spanning tree: adding a negative-cost connection can
reduce the total. Self-loops are ignored because both endpoints already belong to the same component.
Parallel edges are allowed; sorted processing naturally considers the cheapest useful edge first.

For `V` vertices and `E` edges, sorting dominates at `O(E log E)`. Union-Find operations are almost
constant amortized time. A connected tree selects exactly `V - 1` edges. A forest with `C` final
components selects `V - C` edges—an effective invariant for tests.

## Strongly Connected Components

In an SCC, every vertex can reach every other vertex. Kosaraju uses two DFS passes and a reversed graph. Tarjan uses discovery indexes, low-link values, and a stack in one traversal. SCC condensation turns a directed graph into a DAG and enables higher-level dependency analysis.

Tarjan assigns each vertex a discovery index. Its low-link value is the smallest discovery index
reachable through the active DFS region. When a vertex has `lowLink == discoveryIndex`, it is the
root of one component; pop until that vertex is removed.

The active-stack test is essential. An edge to a vertex in an already completed component must not
lower the current low-link value. Self-loops remain valid and produce or participate in an SCC.
Every destination must exist as a graph key so isolated and destination-only vertices are modeled
consistently.

Recursive DFS is concise, but a path with extreme depth can exhaust the call stack. Production code
for adversarial graphs may require an explicit-frame iterative implementation.

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
- `TreesGraphs/MinimumSpanningForest.cs`: stable Kruskal processing over connected or disconnected input.
- `TreesGraphs/StronglyConnectedComponents.cs`: Tarjan discovery, low-link, and active-stack mechanics.
- `AdvancedGraphAlgorithmsTests.cs`: shortest paths, topological cycles, and connectivity.
- `MinimumSpanningForestTests.cs`: cycles, disconnected input, negative edges, self-loops, and validation.
- `StronglyConnectedComponentsTests.cs`: multiple cycles, self-loops, empty input, and invalid destinations.

## Exercises

1. Return predecessor data and reconstruct a Dijkstra shortest path.
2. Implement Bellman-Ford and identify vertices affected by a negative cycle.
3. Extend Kruskal to return a clear error when a caller requires one connected tree.
4. Use the Tarjan result to build and topologically order the condensation DAG.
5. Implement multi-source BFS for nearest-service distance on a grid.

## Navigation

[← Advanced tree indexes](11-advanced-tree-indexes.md) · [Dynamic programming and greedy →](13-dynamic-programming-greedy.md)
