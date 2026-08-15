namespace Dsa.Examples.TreesGraphs;

public static class StronglyConnectedComponents
{
    /// <summary>
    /// Finds strongly connected components with Tarjan's single-pass depth-first search.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<T>> Tarjan<T>(
        IReadOnlyDictionary<T, IReadOnlyList<T>> graph)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(graph);
        ValidateDestinations(graph);

        var discoveryIndex = new Dictionary<T, int>(graph.Count);
        var lowLink = new Dictionary<T, int>(graph.Count);
        var active = new HashSet<T>();
        var stack = new Stack<T>();
        var components = new List<IReadOnlyList<T>>();
        int nextIndex = 0;

        foreach (T vertex in graph.Keys)
        {
            if (!discoveryIndex.ContainsKey(vertex))
            {
                Visit(vertex);
            }
        }

        return components;

        void Visit(T vertex)
        {
            discoveryIndex[vertex] = nextIndex;
            lowLink[vertex] = nextIndex;
            nextIndex++;
            stack.Push(vertex);
            active.Add(vertex);

            foreach (T neighbor in graph[vertex])
            {
                if (!discoveryIndex.ContainsKey(neighbor))
                {
                    Visit(neighbor);
                    lowLink[vertex] = Math.Min(lowLink[vertex], lowLink[neighbor]);
                }
                else if (active.Contains(neighbor))
                {
                    // Only an edge to a vertex still on Tarjan's stack closes a path inside
                    // the current DFS region. An edge to a completed component is ignored.
                    lowLink[vertex] = Math.Min(lowLink[vertex], discoveryIndex[neighbor]);
                }
            }

            if (lowLink[vertex] != discoveryIndex[vertex])
            {
                return;
            }

            var component = new List<T>();
            T member;
            do
            {
                member = stack.Pop();
                active.Remove(member);
                component.Add(member);
            }
            while (!EqualityComparer<T>.Default.Equals(member, vertex));

            components.Add(component);
        }
    }

    private static void ValidateDestinations<T>(IReadOnlyDictionary<T, IReadOnlyList<T>> graph)
        where T : notnull
    {
        foreach ((T _, IReadOnlyList<T> neighbors) in graph)
        {
            ArgumentNullException.ThrowIfNull(neighbors);
            foreach (T neighbor in neighbors)
            {
                if (!graph.ContainsKey(neighbor))
                {
                    throw new ArgumentException(
                        "Every edge destination must exist as a graph vertex.",
                        nameof(graph));
                }
            }
        }
    }
}
