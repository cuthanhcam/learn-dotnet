using Dsa.Examples.TreesGraphs;

namespace Dsa.Tests.TreesGraphs;

public sealed class TreesGraphsExampleTests
{
    [Fact]
    public void MaxDepthCountsLongestRootToLeafPath()
    {
        Assert.Equal(3, TreesGraphsExample.MaxDepth(TreesGraphsExample.SampleTree()));
        Assert.Equal(0, TreesGraphsExample.MaxDepth(null));
    }

    [Fact]
    public void InOrderTraversesLeftNodeRight()
    {
        Assert.Equal([1, 2, 3, 4, 5, 6, 7], TreesGraphsExample.InOrder(TreesGraphsExample.SampleTree()));
    }

    [Fact]
    public void LevelOrderTraversesByDepth()
    {
        Assert.Equal([4, 2, 6, 1, 3, 5, 7], TreesGraphsExample.LevelOrder(TreesGraphsExample.SampleTree()));
    }

    [Fact]
    public void DepthFirstGraphSkipsAlreadyVisitedNodes()
    {
        Dictionary<string, string[]> graph = new()
        {
            ["A"] = ["B", "C"],
            ["B"] = ["D"],
            ["C"] = ["D"],
            ["D"] = []
        };

        Assert.Equal(["A", "B", "D", "C"], TreesGraphsExample.DepthFirstGraph(graph, "A"));
    }

    [Fact]
    public void ShortestPathUnweightedUsesBfs()
    {
        Dictionary<string, string[]> graph = new()
        {
            ["A"] = ["B", "C"],
            ["B"] = ["D"],
            ["C"] = ["E"],
            ["D"] = ["F"],
            ["E"] = ["F"],
            ["F"] = []
        };

        Assert.Equal(["A", "B", "D", "F"], TreesGraphsExample.ShortestPathUnweighted(graph, "A", "F"));
    }
}
