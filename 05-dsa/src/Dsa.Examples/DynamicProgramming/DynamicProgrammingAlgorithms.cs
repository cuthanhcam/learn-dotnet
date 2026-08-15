namespace Dsa.Examples.DynamicProgramming;

public static class DynamicProgrammingAlgorithms
{
    public static int LongestIncreasingSubsequenceLength(IEnumerable<int> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var minimumTailByLength = new List<int>();
        foreach (int value in values)
        {
            int position = LowerBound(minimumTailByLength, value);
            if (position == minimumTailByLength.Count)
            {
                minimumTailByLength.Add(value);
            }
            else
            {
                minimumTailByLength[position] = value;
            }
        }

        // The tail values are optimization state, not necessarily one actual
        // subsequence. Their count is the optimal subsequence length.
        return minimumTailByLength.Count;
    }

    public static int MaximumKnapsackValue(
        IReadOnlyList<int> weights,
        IReadOnlyList<int> values,
        int capacity)
    {
        ArgumentNullException.ThrowIfNull(weights);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);
        if (weights.Count != values.Count)
        {
            throw new ArgumentException("Weights and values must have the same length.");
        }

        var best = new int[capacity + 1];
        for (int item = 0; item < weights.Count; item++)
        {
            int weight = weights[item];
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(weight, nameof(weights));

            // Descending capacity is the 0/1 invariant: each item reads states
            // from the previous logical row and therefore cannot be reused.
            for (int currentCapacity = capacity; currentCapacity >= weight; currentCapacity--)
            {
                best[currentCapacity] = Math.Max(
                    best[currentCapacity],
                    checked(best[currentCapacity - weight] + values[item]));
            }
        }

        return best[capacity];
    }

    public static int LevenshteinDistance(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        if (first.Length > second.Length)
        {
            return LevenshteinDistance(second, first);
        }

        Span<int> previous = second.Length + 1 <= 256
            ? stackalloc int[second.Length + 1]
            : new int[second.Length + 1];
        Span<int> current = second.Length + 1 <= 256
            ? stackalloc int[second.Length + 1]
            : new int[second.Length + 1];

        for (int column = 0; column <= second.Length; column++)
        {
            previous[column] = column;
        }

        for (int row = 1; row <= first.Length; row++)
        {
            current[0] = row;
            for (int column = 1; column <= second.Length; column++)
            {
                int substitutionCost = first[row - 1] == second[column - 1] ? 0 : 1;
                current[column] = Math.Min(
                    Math.Min(current[column - 1] + 1, previous[column] + 1),
                    previous[column - 1] + substitutionCost);
            }

            Span<int> swap = previous;
            previous = current;
            current = swap;
        }

        return previous[second.Length];
    }

    private static int LowerBound(IReadOnlyList<int> values, int target)
    {
        int low = 0;
        int high = values.Count;
        while (low < high)
        {
            int middle = low + ((high - low) / 2);
            if (values[middle] < target)
            {
                low = middle + 1;
            }
            else
            {
                high = middle;
            }
        }

        return low;
    }
}
