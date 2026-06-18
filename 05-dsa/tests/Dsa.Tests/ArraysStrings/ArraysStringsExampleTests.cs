using Dsa.Examples.ArraysStrings;

namespace Dsa.Tests.ArraysStrings;

public sealed class ArraysStringsExampleTests
{
    [Fact]
    public void PrefixSumsSupportConstantTimeRangeQueries()
    {
        int[] prefix = ArraysStringsExample.BuildPrefixSums([3, -1, 4, 10, 2]);

        Assert.Equal([0, 3, 2, 6, 16, 18], prefix);
        Assert.Equal(13, ArraysStringsExample.RangeSum(prefix, 1, 4));
    }

    [Theory]
    [InlineData("Never odd or even", true)]
    [InlineData("A man, a plan, a canal: Panama", true)]
    [InlineData("algorithm", false)]
    public void IsPalindromeIgnoringNonLettersUsesTwoPointers(string input, bool expected)
    {
        Assert.Equal(expected, ArraysStringsExample.IsPalindromeIgnoringNonLetters(input));
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("bbbb", 1)]
    [InlineData("pwwkew", 3)]
    [InlineData("abcabcbb", 3)]
    public void LongestSubstringWithoutRepeatingCharactersUsesSlidingWindow(string input, int expected)
    {
        Assert.Equal(expected, ArraysStringsExample.LongestSubstringWithoutRepeatingCharacters(input));
    }

    [Fact]
    public void TwoSumSortedReturnsIndexesWhenPairExists()
    {
        Assert.Equal([1, 4], ArraysStringsExample.TwoSumSorted([1, 3, 4, 6, 8], 11));
    }

    [Fact]
    public void TwoSumSortedReturnsEmptyArrayWhenPairDoesNotExist()
    {
        Assert.Empty(ArraysStringsExample.TwoSumSorted([1, 3, 4, 6, 8], 100));
    }

    [Fact]
    public void ReverseWordsTrimsAndCollapsesSpaces()
    {
        Assert.Equal("fun are algorithms dotnet", ArraysStringsExample.ReverseWords("  dotnet algorithms are fun  "));
    }

    [Theory]
    [InlineData("aaabbbbcc", "a3b4c2")]
    [InlineData("abc", "abc")]
    [InlineData("", "")]
    public void CompressRunsOnlyKeepsShorterResult(string input, string expected)
    {
        Assert.Equal(expected, ArraysStringsExample.CompressRuns(input));
    }
}
