using Dsa.Exercises;

namespace Dsa.Tests.Exercises;

public sealed class StacksQueuesExercisesTests
{
    [Theory]
    [InlineData("abbaca", "ca")]
    [InlineData("azxxzy", "ay")]
    [InlineData("", "")]
    public void RemoveAdjacentDuplicatesUsesStack(string input, string expected)
    {
        Assert.Equal(expected, StacksQueuesExercises.RemoveAdjacentDuplicates(input));
    }

    [Fact]
    public void CountIslandsUsesQueueTraversal()
    {
        char[][] grid =
        [
            ['1', '1', '0', '0'],
            ['1', '0', '0', '1'],
            ['0', '0', '1', '1']
        ];

        Assert.Equal(2, StacksQueuesExercises.CountIslands(grid));
    }
}
