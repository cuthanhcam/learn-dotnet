using Dsa.Examples.TreesGraphs;

namespace Dsa.Tests.TreesGraphs;

public sealed class AdvancedGraphAlgorithmsTests
{
    [Fact]
    public void DisjointSet_TracksComponentsAndRejectsRedundantUnion()
    {
        var sets = new DisjointSet(5);

        Assert.True(sets.Union(0, 1));
        Assert.True(sets.Union(1, 2));
        Assert.False(sets.Union(0, 2));

        Assert.True(sets.Connected(0, 2));
        Assert.False(sets.Connected(0, 4));
        Assert.Equal(3, sets.ComponentCount);
    }

    [Fact]
    public void Dijkstra_FindsShortestDistancesAndLeavesUnreachableInfinity()
    {
        Dictionary<string, IReadOnlyList<WeightedEdge<string>>> graph = new()
        {
            ["A"] = [new("B", 4), new("C", 1)],
            ["B"] = [new("D", 1)],
            ["C"] = [new("B", 2), new("D", 5)],
            ["D"] = [],
            ["E"] = []
        };

        IReadOnlyDictionary<string, long> distances = WeightedGraphAlgorithms.Dijkstra(graph, "A");

        Assert.Equal(0, distances["A"]);
        Assert.Equal(3, distances["B"]);
        Assert.Equal(1, distances["C"]);
        Assert.Equal(4, distances["D"]);
        Assert.Equal(long.MaxValue, distances["E"]);
    }

    [Fact]
    public void Dijkstra_RejectsNegativeWeights()
    {
        Dictionary<string, IReadOnlyList<WeightedEdge<string>>> graph = new()
        {
            ["A"] = [new("B", -1)],
            ["B"] = []
        };

        Assert.Throws<ArgumentException>(() => WeightedGraphAlgorithms.Dijkstra(graph, "A"));
    }

    [Fact]
    public void TopologicalSort_OrdersEveryDependencyBeforeItsConsumer()
    {
        Dictionary<string, IReadOnlyList<string>> graph = new()
        {
            ["parse"] = ["compile"],
            ["compile"] = ["test", "package"],
            ["test"] = ["package"],
            ["package"] = []
        };

        string[] order = WeightedGraphAlgorithms.TopologicalSort(graph);

        Assert.True(Array.IndexOf(order, "parse") < Array.IndexOf(order, "compile"));
        Assert.True(Array.IndexOf(order, "compile") < Array.IndexOf(order, "test"));
        Assert.True(Array.IndexOf(order, "test") < Array.IndexOf(order, "package"));
    }

    [Fact]
    public void TopologicalSort_RejectsCycle()
    {
        Dictionary<int, IReadOnlyList<int>> graph = new()
        {
            [1] = [2],
            [2] = [1]
        };

        Assert.Throws<InvalidOperationException>(() => WeightedGraphAlgorithms.TopologicalSort(graph));
    }
}
