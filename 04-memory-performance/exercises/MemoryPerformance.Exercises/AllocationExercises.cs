namespace MemoryPerformance.Exercises;

public static class AllocationExercises
{
    public static IReadOnlyList<string> UniqueWords(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return text
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(static word => word.ToUpperInvariant())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static word => word, StringComparer.Ordinal)
            .ToList();
    }

    public static string JoinNumbersEfficiently(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        return string.Join(',', Enumerable.Range(0, count));
    }
}
