namespace CSharpBasics.Exercises.Medium;

public static class FibonacciSequence
{
    public static List<int> Generate(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
        }

        var sequence = new List<int>(count);
        if (count == 0)
        {
            return sequence;
        }

        sequence.Add(0);
        if (count == 1)
        {
            return sequence;
        }

        sequence.Add(1);
        for (int i = 2; i < count; i++)
        {
            sequence.Add(sequence[i - 1] + sequence[i - 2]);
        }

        return sequence;
    }
}
