using Dsa.Examples.TreesGraphs;

namespace Dsa.Tests.TreesGraphs;

public sealed class AdvancedTreeIndexTests
{
    [Fact]
    public void Trie_DistinguishesWholeWordsFromPrefixes()
    {
        var trie = new PrefixTrie();
        trie.Add("car");
        trie.Add("cart");
        trie.Add("cat");

        Assert.True(trie.Contains("car"));
        Assert.False(trie.Contains("ca"));
        Assert.True(trie.ContainsPrefix("ca"));
        Assert.Equal(["car", "cart", "cat"], trie.FindByPrefix("ca"));
    }

    [Fact]
    public void Trie_RemovePreservesWordsThatShareThePrefix()
    {
        var trie = new PrefixTrie();
        trie.Add("car");
        trie.Add("cart");

        Assert.True(trie.Remove("car"));

        Assert.False(trie.Contains("car"));
        Assert.True(trie.Contains("cart"));
        Assert.Equal(1, trie.Count);
    }

    [Fact]
    public void FenwickTree_SupportsPrefixRangeAndPointUpdate()
    {
        var tree = new FenwickTree([2, 1, 3, 5, 4]);

        Assert.Equal(6, tree.PrefixSum(2));
        Assert.Equal(12, tree.RangeSum(2, 4));

        tree[2] = 10;

        Assert.Equal(13, tree.PrefixSum(2));
        Assert.Equal(19, tree.RangeSum(2, 4));
    }

    [Fact]
    public void FenwickTree_RejectsInvalidRange()
    {
        var tree = new FenwickTree([1L, 2L, 3L]);

        Assert.Throws<ArgumentException>(() => tree.RangeSum(2, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => tree.PrefixSum(3));
    }
}
