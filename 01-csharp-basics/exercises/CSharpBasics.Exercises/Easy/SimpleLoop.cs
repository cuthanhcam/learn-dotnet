namespace CSharpBasics.Exercises.Easy;

public static class SimpleLoop
{
    public static List<int> GenerateRange(int start, int end)
    {
        if (end < start)
        {
            throw new ArgumentException("End must be greater than or equal to start.", nameof(end));
        }

        var result = new List<int>();
        for (int i = start; i <= end; i++)
        {
            result.Add(i);
        }

        return result;
    }
}
