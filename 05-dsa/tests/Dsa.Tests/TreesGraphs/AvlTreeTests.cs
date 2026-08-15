using Dsa.Examples.TreesGraphs;

namespace Dsa.Tests.TreesGraphs;

public sealed class AvlTreeTests
{
    [Theory]
    [InlineData(30, 20, 10)] // left-left
    [InlineData(10, 20, 30)] // right-right
    [InlineData(30, 10, 20)] // left-right
    [InlineData(10, 30, 20)] // right-left
    public void Add_AllRotationShapesPreserveOrder(int first, int second, int third)
    {
        var tree = new AvlTree<int>();

        tree.Add(first);
        tree.Add(second);
        tree.Add(third);

        Assert.Equal([10, 20, 30], tree.InOrder());
        Assert.Equal(2, tree.Height);
    }

    [Fact]
    public void SortedInsertionRemainsLogarithmicHeight()
    {
        var tree = new AvlTree<int>();

        foreach (int value in Enumerable.Range(1, 1_000))
        {
            Assert.True(tree.Add(value));
        }

        // The exact shape is an implementation detail. This conservative bound distinguishes
        // the AVL tree from an unbalanced 1,000-node chain without over-specifying rotations.
        Assert.InRange(tree.Height, 1, 2 * (int)Math.Ceiling(Math.Log2(tree.Count + 1)));
        Assert.Equal(Enumerable.Range(1, 1_000), tree.InOrder());
    }

    [Fact]
    public void RemoveLeafOneChildAndTwoChildNodesPreservesSearchAndOrder()
    {
        var tree = new AvlTree<int>();
        foreach (int value in new[] { 40, 20, 60, 10, 30, 50, 70, 25 })
        {
            tree.Add(value);
        }

        Assert.True(tree.Remove(10));
        Assert.True(tree.Remove(30));
        Assert.True(tree.Remove(40));

        Assert.Equal([20, 25, 50, 60, 70], tree.InOrder());
        Assert.False(tree.Contains(40));
        Assert.Equal(5, tree.Count);
    }

    [Fact]
    public void DuplicateAndMissingRemovalDoNotChangeCount()
    {
        var tree = new AvlTree<string>(StringComparer.OrdinalIgnoreCase);

        Assert.True(tree.Add("Alpha"));
        Assert.False(tree.Add("alpha"));
        Assert.False(tree.Remove("missing"));

        Assert.Equal(1, tree.Count);
        Assert.True(tree.Contains("ALPHA"));
    }
}
