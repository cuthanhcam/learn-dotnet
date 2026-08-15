namespace CoreDotNet.Exercises;

public static class QueryExercises
{
    public static int[] GetDistinctEvenSquares(IEnumerable<int> numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        // Materialization is deliberate: callers receive a stable snapshot and
        // the source is enumerated exactly once by this method.
        return numbers
            .Where(number => number % 2 == 0)
            .Select(number => checked(number * number))
            .Distinct()
            .Order()
            .ToArray();
    }
}
