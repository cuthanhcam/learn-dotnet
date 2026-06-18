namespace Dsa.Examples.Complexity;

public static class ComplexityExample
{
    public static int ConstantFirstOrDefault(IReadOnlyList<int> values)
    {
        return values.Count == 0 ? 0 : values[0];
    }

    public static long LinearSum(ReadOnlySpan<int> values)
    {
        long total = 0;

        foreach (int value in values)
        {
            total += value;
        }

        return total;
    }

    public static int QuadraticPairCount(ReadOnlySpan<int> values, int target)
    {
        int count = 0;

        for (int i = 0; i < values.Length; i++)
        {
            for (int j = i + 1; j < values.Length; j++)
            {
                if (values[i] + values[j] == target)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public static int LogarithmicHalvingSteps(int inputSize)
    {
        if (inputSize < 1)
        {
            return 0;
        }

        int steps = 0;
        int remaining = inputSize;

        while (remaining > 1)
        {
            remaining /= 2;
            steps++;
        }

        return steps;
    }

    public static string[] DescribeGrowth()
    {
        return
        [
            "O(1): direct access",
            "O(log n): halve the search space",
            "O(n): scan once",
            "O(n log n): sort with divide and conquer",
            "O(n^2): compare pairs"
        ];
    }

    public static void Run()
    {
        int[] values = [2, 4, 6, 8, 10];

        Console.WriteLine("Big-O examples");
        Console.WriteLine($"O(1) first: {ConstantFirstOrDefault(values)}");
        Console.WriteLine($"O(n) sum: {LinearSum(values)}");
        Console.WriteLine($"O(n^2) pairs that sum to 12: {QuadraticPairCount(values, 12)}");
        Console.WriteLine($"O(log n) halving steps for 1024: {LogarithmicHalvingSteps(1024)}");
    }
}
