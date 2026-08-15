using Dsa.Examples.DynamicProgramming;

namespace Dsa.Tests.DynamicProgramming;

public sealed class DynamicProgrammingAlgorithmsTests
{
    [Theory]
    [InlineData(new int[0], 0)]
    [InlineData(new[] { 1, 2, 3 }, 3)]
    [InlineData(new[] { 3, 2, 1 }, 1)]
    [InlineData(new[] { 10, 9, 2, 5, 3, 7, 101, 18 }, 4)]
    public void LongestIncreasingSubsequenceLength_ReturnsStrictLength(int[] values, int expected)
    {
        Assert.Equal(expected, DynamicProgrammingAlgorithms.LongestIncreasingSubsequenceLength(values));
    }

    [Fact]
    public void MaximumKnapsackValue_UsesEachItemAtMostOnce()
    {
        int result = DynamicProgrammingAlgorithms.MaximumKnapsackValue(
            weights: [2, 3, 4],
            values: [4, 5, 7],
            capacity: 6);

        Assert.Equal(11, result);
    }

    [Theory]
    [InlineData("", "", 0)]
    [InlineData("kitten", "sitting", 3)]
    [InlineData("flaw", "lawn", 2)]
    public void LevenshteinDistance_ComputesMinimumEdits(string first, string second, int expected)
    {
        Assert.Equal(expected, DynamicProgrammingAlgorithms.LevenshteinDistance(first, second));
    }

    [Fact]
    public void IntervalScheduling_SelectsMaximumCompatibleSet()
    {
        Interval[] selected = GreedyAlgorithms.SelectMaximumNonOverlapping(
        [
            new Interval(1, 4),
            new Interval(3, 5),
            new Interval(0, 6),
            new Interval(5, 7),
            new Interval(8, 9),
            new Interval(5, 9)
        ]);

        Assert.Equal([new Interval(1, 4), new Interval(5, 7), new Interval(8, 9)], selected);
    }
}
