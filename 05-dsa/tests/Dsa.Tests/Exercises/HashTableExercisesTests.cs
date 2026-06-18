using Dsa.Exercises;

namespace Dsa.Tests.Exercises;

public sealed class HashTableExercisesTests
{
    [Theory]
    [InlineData(new[] { 100, 4, 200, 1, 3, 2 }, 4)]
    [InlineData(new[] { 0, 3, 7, 2, 5, 8, 4, 6, 0, 1 }, 9)]
    [InlineData(new int[] { }, 0)]
    public void LongestConsecutiveSequenceStartsOnlyAtSequenceHeads(int[] values, int expected)
    {
        Assert.Equal(expected, HashTableExercises.LongestConsecutiveSequence(values));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 1 }, 3, true)]
    [InlineData(new[] { 1, 0, 1, 1 }, 1, true)]
    [InlineData(new[] { 1, 2, 3, 1, 2, 3 }, 2, false)]
    public void ContainsNearbyDuplicateTracksLastSeenIndex(int[] values, int maxDistance, bool expected)
    {
        Assert.Equal(expected, HashTableExercises.ContainsNearbyDuplicate(values, maxDistance));
    }
}
