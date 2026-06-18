namespace Dsa.Exercises;

public static class ArraysStringsExercises
{
    public static int[] MoveZeroesToEnd(int[] values)
    {
        int write = 0;

        for (int read = 0; read < values.Length; read++)
        {
            if (values[read] != 0)
            {
                values[write] = values[read];
                write++;
            }
        }

        while (write < values.Length)
        {
            values[write] = 0;
            write++;
        }

        return values;
    }

    public static bool AreAnagrams(string left, string right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }

        Dictionary<char, int> counts = [];

        foreach (char character in left)
        {
            counts[character] = counts.GetValueOrDefault(character) + 1;
        }

        foreach (char character in right)
        {
            if (!counts.TryGetValue(character, out int count))
            {
                return false;
            }

            if (count == 1)
            {
                counts.Remove(character);
            }
            else
            {
                counts[character] = count - 1;
            }
        }

        return counts.Count == 0;
    }

    public static int MaxSubarraySumOfSizeK(ReadOnlySpan<int> values, int windowSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowSize);

        if (windowSize > values.Length)
        {
            throw new ArgumentException("Window size cannot exceed input length.");
        }

        int current = 0;

        for (int i = 0; i < windowSize; i++)
        {
            current += values[i];
        }

        int best = current;

        for (int i = windowSize; i < values.Length; i++)
        {
            current += values[i] - values[i - windowSize];
            best = Math.Max(best, current);
        }

        return best;
    }
}
