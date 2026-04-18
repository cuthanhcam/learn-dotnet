namespace CSharpBasics.Exercises.Medium;

public static class RemoveDuplicates
{
    // Collection exercise for distinct values.
    public static List<int> GetDistinctValues(IEnumerable<int> numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        return numbers.Distinct().ToList();
    }
}
