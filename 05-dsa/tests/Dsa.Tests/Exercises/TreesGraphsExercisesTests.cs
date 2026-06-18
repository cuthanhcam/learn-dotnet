using Dsa.Exercises;

namespace Dsa.Tests.Exercises;

public sealed class TreesGraphsExercisesTests
{
    [Fact]
    public void IsValidBinarySearchTreePropagatesBounds()
    {
        TreesGraphsExercises.Node root = new(5)
        {
            Left = new TreesGraphsExercises.Node(3),
            Right = new TreesGraphsExercises.Node(7)
        };

        Assert.True(TreesGraphsExercises.IsValidBinarySearchTree(root));
    }

    [Fact]
    public void IsValidBinarySearchTreeRejectsDeepBoundViolation()
    {
        TreesGraphsExercises.Node root = new(5)
        {
            Left = new TreesGraphsExercises.Node(1),
            Right = new TreesGraphsExercises.Node(8)
            {
                Left = new TreesGraphsExercises.Node(4)
            }
        };

        Assert.False(TreesGraphsExercises.IsValidBinarySearchTree(root));
    }

    [Fact]
    public void CountConnectedComponentsUsesVisitedSet()
    {
        Dictionary<int, int[]> graph = new()
        {
            [1] = [2],
            [2] = [1],
            [3] = [4],
            [4] = [3],
            [5] = []
        };

        Assert.Equal(3, TreesGraphsExercises.CountConnectedComponents(graph));
    }
}
