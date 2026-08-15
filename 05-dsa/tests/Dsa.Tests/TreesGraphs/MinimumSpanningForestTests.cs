using Dsa.Examples.TreesGraphs;

namespace Dsa.Tests.TreesGraphs;

public sealed class MinimumSpanningForestTests
{
    [Fact]
    public void Kruskal_SelectsMinimumTreeAndRejectsCycleEdges()
    {
        UndirectedWeightedEdge<string>[] edges =
        [
            new("A", "B", 4),
            new("A", "C", 1),
            new("C", "B", 2),
            new("B", "D", 1),
            new("C", "D", 5)
        ];

        MinimumSpanningForestResult<string> result =
            MinimumSpanningForest.Kruskal(["A", "B", "C", "D"], edges);

        Assert.Equal(4, result.TotalWeight);
        Assert.Equal(3, result.Edges.Count);
        Assert.Equal(1, result.ComponentCount);
    }

    [Fact]
    public void Kruskal_ReturnsForestForDisconnectedVertices()
    {
        MinimumSpanningForestResult<int> result = MinimumSpanningForest.Kruskal(
            [1, 2, 3, 4, 5],
            [new(1, 2, -2), new(2, 3, 1), new(4, 5, 7)]);

        Assert.Equal(6, result.TotalWeight);
        Assert.Equal(3, result.Edges.Count);
        Assert.Equal(2, result.ComponentCount);
    }

    [Fact]
    public void Kruskal_IgnoresSelfLoopsAndKeepsStableEqualWeightOrder()
    {
        UndirectedWeightedEdge<int> first = new(1, 2, 1);
        UndirectedWeightedEdge<int> second = new(2, 3, 1);

        MinimumSpanningForestResult<int> result = MinimumSpanningForest.Kruskal(
            [1, 2, 3],
            [new(1, 1, -100), first, second, new(1, 3, 1)]);

        Assert.Equal([first, second], result.Edges);
    }

    [Fact]
    public void Kruskal_RejectsUnknownEndpointAndDuplicateVertex()
    {
        Assert.Throws<ArgumentException>(() =>
            MinimumSpanningForest.Kruskal([1, 1], Array.Empty<UndirectedWeightedEdge<int>>()));
        Assert.Throws<ArgumentException>(() =>
            MinimumSpanningForest.Kruskal([1], [new(1, 2, 1)]));
    }
}
