namespace Dsa.Examples.TreesGraphs;

public readonly record struct WeightedEdge<T>(T To, int Weight);

public static class WeightedGraphAlgorithms
{
    public static IReadOnlyDictionary<T, long> Dijkstra<T>(
        IReadOnlyDictionary<T, IReadOnlyList<WeightedEdge<T>>> graph,
        T start)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(graph);
        if (!graph.ContainsKey(start))
        {
            throw new ArgumentException("Start vertex must exist in the graph.", nameof(start));
        }

        foreach (IReadOnlyList<WeightedEdge<T>> edges in graph.Values)
        {
            if (edges.Any(static edge => edge.Weight < 0))
            {
                throw new ArgumentException("Dijkstra requires non-negative edge weights.", nameof(graph));
            }
        }

        var distances = graph.Keys.ToDictionary(vertex => vertex, _ => long.MaxValue);
        var queue = new PriorityQueue<T, long>();
        distances[start] = 0;
        queue.Enqueue(start, 0);

        while (queue.TryDequeue(out T? vertex, out long queuedDistance))
        {
            if (queuedDistance != distances[vertex])
            {
                continue; // Ignore stale entries superseded by a shorter route.
            }

            foreach (WeightedEdge<T> edge in graph[vertex])
            {
                if (!graph.ContainsKey(edge.To))
                {
                    throw new ArgumentException("Every edge destination must exist as a graph vertex.", nameof(graph));
                }

                long candidate = checked(queuedDistance + edge.Weight);
                if (candidate < distances[edge.To])
                {
                    distances[edge.To] = candidate;
                    queue.Enqueue(edge.To, candidate);
                }
            }
        }

        return distances;
    }

    public static T[] TopologicalSort<T>(IReadOnlyDictionary<T, IReadOnlyList<T>> graph)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(graph);

        var inDegree = graph.Keys.ToDictionary(vertex => vertex, _ => 0);
        foreach (IReadOnlyList<T> neighbors in graph.Values)
        {
            foreach (T neighbor in neighbors)
            {
                if (!inDegree.TryGetValue(neighbor, out int degree))
                {
                    throw new ArgumentException("Every edge destination must exist as a graph vertex.", nameof(graph));
                }

                inDegree[neighbor] = degree + 1;
            }
        }

        Queue<T> ready = new(inDegree.Where(pair => pair.Value == 0).Select(pair => pair.Key));
        var order = new List<T>(graph.Count);

        while (ready.TryDequeue(out T? vertex))
        {
            order.Add(vertex);
            foreach (T neighbor in graph[vertex])
            {
                if (--inDegree[neighbor] == 0)
                {
                    ready.Enqueue(neighbor);
                }
            }
        }

        if (order.Count != graph.Count)
        {
            throw new InvalidOperationException("A topological order does not exist because the graph contains a cycle.");
        }

        return order.ToArray();
    }
}
