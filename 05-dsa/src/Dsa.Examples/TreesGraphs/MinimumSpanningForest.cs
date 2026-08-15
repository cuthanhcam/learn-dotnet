namespace Dsa.Examples.TreesGraphs;

public readonly record struct UndirectedWeightedEdge<T>(T First, T Second, int Weight);

public sealed record MinimumSpanningForestResult<T>(
    IReadOnlyList<UndirectedWeightedEdge<T>> Edges,
    long TotalWeight,
    int ComponentCount);

public static class MinimumSpanningForest
{
    /// <summary>
    /// Uses Kruskal's algorithm to select a minimum-cost acyclic edge set for every component.
    /// </summary>
    public static MinimumSpanningForestResult<T> Kruskal<T>(
        IEnumerable<T> vertices,
        IEnumerable<UndirectedWeightedEdge<T>> edges,
        IEqualityComparer<T>? comparer = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(vertices);
        ArgumentNullException.ThrowIfNull(edges);

        comparer ??= EqualityComparer<T>.Default;
        T[] vertexArray = vertices.ToArray();
        var indexByVertex = new Dictionary<T, int>(vertexArray.Length, comparer);
        for (int index = 0; index < vertexArray.Length; index++)
        {
            if (!indexByVertex.TryAdd(vertexArray[index], index))
            {
                throw new ArgumentException("Vertices must be unique.", nameof(vertices));
            }
        }

        // Retain input position as a stable tie-breaker. Equal-weight choices can produce
        // different valid forests, but deterministic output makes examples and diagnostics clearer.
        var orderedEdges = edges
            .Select(static (edge, inputOrder) => (Edge: edge, InputOrder: inputOrder))
            .OrderBy(static candidate => candidate.Edge.Weight)
            .ThenBy(static candidate => candidate.InputOrder)
            .ToArray();

        var sets = new DisjointSet(vertexArray.Length);
        var selected = new List<UndirectedWeightedEdge<T>>(Math.Max(0, vertexArray.Length - 1));
        long totalWeight = 0;

        foreach ((UndirectedWeightedEdge<T> edge, _) in orderedEdges)
        {
            if (!indexByVertex.TryGetValue(edge.First, out int firstIndex) ||
                !indexByVertex.TryGetValue(edge.Second, out int secondIndex))
            {
                throw new ArgumentException("Every edge endpoint must exist in the vertex set.", nameof(edges));
            }

            // Union returns false for self-loops and edges inside an existing component.
            // Rejecting those edges is exactly what preserves the forest invariant.
            if (!sets.Union(firstIndex, secondIndex))
            {
                continue;
            }

            selected.Add(edge);
            totalWeight = checked(totalWeight + edge.Weight);
        }

        return new MinimumSpanningForestResult<T>(selected, totalWeight, sets.ComponentCount);
    }
}
