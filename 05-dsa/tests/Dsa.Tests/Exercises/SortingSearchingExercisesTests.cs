using Dsa.Exercises;

namespace Dsa.Tests.Exercises;

public sealed class SortingSearchingExercisesTests
{
    [Theory]
    [InlineData(new[] { 1, 3, 5, 6 }, 5, 2)]
    [InlineData(new[] { 1, 3, 5, 6 }, 2, 1)]
    [InlineData(new[] { 1, 3, 5, 6 }, 7, 4)]
    public void SearchInsertPositionReturnsLowerBound(int[] values, int target, int expected)
    {
        Assert.Equal(expected, SortingSearchingExercises.SearchInsertPosition(values, target));
    }

    [Fact]
    public void SortSquaresUsesTwoPointersFromBothEnds()
    {
        Assert.Equal([0, 1, 9, 16, 100], SortingSearchingExercises.SortSquares([-4, -1, 0, 3, 10]));
    }
}
