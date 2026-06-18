using Dsa.Exercises;

namespace Dsa.Tests.Exercises;

public sealed class RecursionBacktrackingExercisesTests
{
    [Fact]
    public void GenerateParenthesesProducesOnlyValidStrings()
    {
        Assert.Equal(["(())", "()()"], RecursionBacktrackingExercises.GenerateParentheses(2));
    }

    [Theory]
    [InlineData("ABCCED", true)]
    [InlineData("SEE", true)]
    [InlineData("ABCB", false)]
    public void WordExistsBacktracksVisitedCells(string word, bool expected)
    {
        char[][] board =
        [
            ['A', 'B', 'C', 'E'],
            ['S', 'F', 'C', 'S'],
            ['A', 'D', 'E', 'E']
        ];

        Assert.Equal(expected, RecursionBacktrackingExercises.WordExists(board, word));
    }
}
