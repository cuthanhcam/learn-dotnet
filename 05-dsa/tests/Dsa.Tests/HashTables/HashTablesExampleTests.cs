using Dsa.Examples.HashTables;

namespace Dsa.Tests.HashTables;

public sealed class HashTablesExampleTests
{
    [Fact]
    public void CountWordsUsesCaseInsensitiveComparer()
    {
        Dictionary<string, int> counts = HashTablesExample.CountWords(["API", "api", "cache"]);

        Assert.Equal(2, counts["api"]);
        Assert.Equal(1, counts["cache"]);
    }

    [Fact]
    public void TwoSumUnsortedUsesComplementLookup()
    {
        Assert.Equal([0, 1], HashTablesExample.TwoSumUnsorted([2, 7, 11, 15], 9));
    }

    [Theory]
    [InlineData("swiss", "w")]
    [InlineData("aabb", "")]
    public void FirstUniqueCharacterUsesFrequencyMap(string input, string expected)
    {
        Assert.Equal(expected, HashTablesExample.FirstUniqueCharacter(input));
    }

    [Fact]
    public void GroupAnagramsUsesCanonicalSortedKey()
    {
        string[][] groups = HashTablesExample.GroupAnagrams(["eat", "tea", "tan", "ate", "nat", "bat"]);

        Assert.Equal(
            [
                ["ate", "eat", "tea"],
                ["bat"],
                ["nat", "tan"]
            ],
            groups);
    }
}
