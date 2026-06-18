using Dsa.Examples.RecursionBacktracking;

namespace Dsa.Tests.RecursionBacktracking;

public sealed class RecursionBacktrackingExampleTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(5, 120)]
    public void FactorialUsesBaseCase(int input, int expected)
    {
        Assert.Equal(expected, RecursionBacktrackingExample.Factorial(input));
    }

    [Fact]
    public void FibonacciMemoizedComputesRepeatedSubproblemsOncePerValue()
    {
        Assert.Equal(55, RecursionBacktrackingExample.FibonacciMemoized(10));
    }

    [Fact]
    public void SubsetsExploresIncludeAndExcludeChoices()
    {
        int[][] subsets = RecursionBacktrackingExample.Subsets([1, 2]);

        Assert.Equal([[], [2], [1], [1, 2]], subsets);
    }

    [Fact]
    public void PermutationsExploresAllPositions()
    {
        int[][] permutations = RecursionBacktrackingExample.Permutations([1, 2, 3]);

        Assert.Equal(6, permutations.Length);
        Assert.Contains([1, 2, 3], permutations);
        Assert.Contains([3, 2, 1], permutations);
    }

    [Fact]
    public void CombinationSumAllowsReusingCandidate()
    {
        int[][] combinations = RecursionBacktrackingExample.CombinationSum([2, 3, 6, 7], 7);

        Assert.Equal([[2, 2, 3], [7]], combinations);
    }
}
