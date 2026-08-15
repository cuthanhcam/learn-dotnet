using Dsa.Examples.TreesGraphs;

namespace Dsa.Tests.TreesGraphs;

public sealed class BinarySearchTreeTests
{
    [Fact]
    public void Add_RejectsDuplicatesAndProducesSortedTraversal()
    {
        var tree = new BinarySearchTree<int>();

        foreach (int value in new[] { 5, 3, 7, 2, 4, 6, 8 })
        {
            Assert.True(tree.Add(value));
        }

        Assert.False(tree.Add(5));
        Assert.Equal(7, tree.Count);
        Assert.Equal([2, 3, 4, 5, 6, 7, 8], tree.InOrder());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(7)]
    [InlineData(5)]
    public void Remove_HandlesLeafOneChildAndTwoChildren(int value)
    {
        var tree = new BinarySearchTree<int>();
        foreach (int item in new[] { 5, 3, 7, 2, 4, 6, 8, 9 })
        {
            tree.Add(item);
        }

        Assert.True(tree.Remove(value));

        Assert.False(tree.Contains(value));
        Assert.Equal(7, tree.Count);
        Assert.Equal(tree.InOrder().Order(), tree.InOrder());
    }

    [Fact]
    public void CustomComparer_DefinesTreeOrdering()
    {
        var tree = new BinarySearchTree<string>(StringComparer.OrdinalIgnoreCase);

        Assert.True(tree.Add("beta"));
        Assert.True(tree.Add("Alpha"));
        Assert.False(tree.Add("BETA"));

        Assert.Equal(["Alpha", "beta"], tree.InOrder());
    }
}
