namespace Dsa.Exercises;

public static class HashTableExercises
{
    public static int LongestConsecutiveSequence(ReadOnlySpan<int> values)
    {
        HashSet<int> set = [.. values.ToArray()];
        int best = 0;

        foreach (int value in set)
        {
            if (set.Contains(value - 1))
            {
                continue;
            }

            int current = value;
            int length = 1;

            while (set.Contains(current + 1))
            {
                current++;
                length++;
            }

            best = Math.Max(best, length);
        }

        return best;
    }

    public static bool ContainsNearbyDuplicate(ReadOnlySpan<int> values, int maxDistance)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxDistance);
        Dictionary<int, int> lastIndexByValue = [];

        for (int i = 0; i < values.Length; i++)
        {
            if (lastIndexByValue.TryGetValue(values[i], out int previousIndex) &&
                i - previousIndex <= maxDistance)
            {
                return true;
            }

            lastIndexByValue[values[i]] = i;
        }

        return false;
    }
}
