using Dsa.Exercises;

namespace Dsa.Tests.Exercises;

public sealed class ArraysStringsExercisesTests
{
    [Fact]
    public void MoveZeroesToEndPreservesNonZeroOrder()
    {
        int[] values = [0, 1, 0, 3, 12];

        int[] result = ArraysStringsExercises.MoveZeroesToEnd(values);

        Assert.Same(values, result);
        Assert.Equal([1, 3, 12, 0, 0], result);
    }

    [Theory]
    [InlineData("listen", "silent", true)]
    [InlineData("rat", "car", false)]
    [InlineData("aabb", "abbb", false)]
    public void AreAnagramsUsesCharacterCounts(string left, string right, bool expected)
    {
        Assert.Equal(expected, ArraysStringsExercises.AreAnagrams(left, right));
    }

    [Fact]
    public void MaxSubarraySumOfSizeKUsesSlidingWindow()
    {
        Assert.Equal(9, ArraysStringsExercises.MaxSubarraySumOfSizeK([2, 1, 5, 1, 3, 2], 3));
    }
}
