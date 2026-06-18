using Dsa.Examples.SortingSearching;

namespace Dsa.Tests.SortingSearching;

public sealed class SortingSearchingExampleTests
{
    [Fact]
    public void MergeSortReturnsSortedCopy()
    {
        int[] input = [5, 2, 9, 1, 5, 6];

        int[] sorted = SortingSearchingExample.MergeSort(input);

        Assert.Equal([1, 2, 5, 5, 6, 9], sorted);
        Assert.Equal([5, 2, 9, 1, 5, 6], input);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 5, 5, 6, 9 }, 6, 4)]
    [InlineData(new[] { 1, 2, 5, 5, 6, 9 }, 100, -1)]
    public void BinarySearchFindsTargetOrMinusOne(int[] values, int target, int expected)
    {
        Assert.Equal(expected, SortingSearchingExample.BinarySearch(values, target));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 5, 5, 6, 9 }, 5, 2)]
    [InlineData(new[] { 1, 2, 5, 5, 6, 9 }, 4, 2)]
    [InlineData(new[] { 1, 2, 5, 5, 6, 9 }, 10, 6)]
    public void LowerBoundFindsFirstGreaterOrEqualIndex(int[] values, int target, int expected)
    {
        Assert.Equal(expected, SortingSearchingExample.LowerBound(values, target));
    }

    [Fact]
    public void FirstBadVersionFindsLeftmostBadVersion()
    {
        Assert.Equal(4, SortingSearchingExample.FirstBadVersion(10, version => version >= 4));
    }

    [Fact]
    public void QuickSelectFindsKthSmallestAndMayMutateInput()
    {
        Assert.Equal(5, SortingSearchingExample.QuickSelectKthSmallest([5, 2, 9, 1, 5, 6], 3));
    }
}
