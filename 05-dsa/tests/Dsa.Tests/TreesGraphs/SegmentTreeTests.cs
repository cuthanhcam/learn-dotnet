using Dsa.Examples.TreesGraphs;

namespace Dsa.Tests.TreesGraphs;

public sealed class SegmentTreeTests
{
    [Fact]
    public void Query_ReturnsHalfOpenRangeSum()
    {
        var tree = new SegmentTree([2, 1, 3, 5, 4]);

        Assert.Equal(0, tree.Query(2, 2));
        Assert.Equal(9, tree.Query(1, 4));
        Assert.Equal(15, tree.Query(0, 5));
    }

    [Fact]
    public void Update_RefreshesEveryAffectedRange()
    {
        var tree = new SegmentTree([2, 1, 3, 5, 4]);

        tree.Update(2, 10);

        Assert.Equal(16, tree.Query(1, 4));
        Assert.Equal(22, tree.Query(0, 5));
    }

    [Fact]
    public void EmptyTree_AllowsOnlyEmptyQuery()
    {
        var tree = new SegmentTree([]);

        Assert.Equal(0, tree.Query(0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => tree.Update(0, 1));
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 6)]
    [InlineData(4, 3)]
    public void Query_RejectsInvalidBoundaries(int start, int end)
    {
        var tree = new SegmentTree([1, 2, 3, 4, 5]);

        Assert.ThrowsAny<ArgumentException>(() => tree.Query(start, end));
    }
}
