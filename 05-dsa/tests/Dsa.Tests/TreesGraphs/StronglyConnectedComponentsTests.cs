using Dsa.Examples.TreesGraphs;

namespace Dsa.Tests.TreesGraphs;

public sealed class StronglyConnectedComponentsTests
{
    [Fact]
    public void Tarjan_GroupsCyclesAndLeavesSingletonComponents()
    {
        Dictionary<int, IReadOnlyList<int>> graph = new()
        {
            [1] = [2],
            [2] = [3],
            [3] = [1, 4],
            [4] = [5],
            [5] = [4],
            [6] = [6]
        };

        IReadOnlyList<IReadOnlyList<int>> components = StronglyConnectedComponents.Tarjan(graph);
        int[][] normalized = components
            .Select(component => component.Order().ToArray())
            .OrderBy(component => component[0])
            .ToArray();

        Assert.Equal(3, normalized.Length);
        Assert.Equal([1, 2, 3], normalized[0]);
        Assert.Equal([4, 5], normalized[1]);
        Assert.Equal([6], normalized[2]);
    }

    [Fact]
    public void Tarjan_HandlesEmptyGraph()
    {
        Dictionary<int, IReadOnlyList<int>> graph = [];

        Assert.Empty(StronglyConnectedComponents.Tarjan(graph));
    }

    [Fact]
    public void Tarjan_RejectsDestinationMissingFromVertexSet()
    {
        Dictionary<string, IReadOnlyList<string>> graph = new()
        {
            ["known"] = ["missing"]
        };

        Assert.Throws<ArgumentException>(() => StronglyConnectedComponents.Tarjan(graph));
    }
}
