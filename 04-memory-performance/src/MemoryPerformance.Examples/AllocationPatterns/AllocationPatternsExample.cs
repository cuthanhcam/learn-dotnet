using System.Text;

namespace MemoryPerformance.Examples.AllocationPatterns;

/// <summary>
/// Demonstrates common allocation sources: boxing, strings, closures, LINQ, and iterators.
/// </summary>
public static class AllocationPatternsExample
{
    public static void Run()
    {
        int[] numbers = [1, 2, 3, 4, 5];

        Console.WriteLine($"Boxed sum: {SumBoxedNumbers(numbers)}");
        Console.WriteLine($"Generic sum: {SumGenericNumbers(numbers)}");
        Console.WriteLine($"StringBuilder output: {BuildWithStringBuilder(5)}");
        Console.WriteLine($"Closure output: {string.Join(", ", CreateMultipliers(3).Select(f => f(10)))}");
        Console.WriteLine($"Iterator output: {string.Join(", ", EvenNumbers(numbers))}");
    }

    public static int SumBoxedNumbers(IEnumerable<int> numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        var boxed = new List<object>();
        foreach (int number in numbers)
        {
            boxed.Add(number);
        }

        int sum = 0;
        foreach (object value in boxed)
        {
            sum += (int)value;
        }

        return sum;
    }

    public static int SumGenericNumbers(IEnumerable<int> numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        int sum = 0;
        foreach (int number in numbers)
        {
            sum += number;
        }

        return sum;
    }

    public static string BuildWithConcatenation(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        string result = string.Empty;
        for (int i = 0; i < count; i++)
        {
            result += i;
            if (i < count - 1)
            {
                result += ",";
            }
        }

        return result;
    }

    public static string BuildWithStringBuilder(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var builder = new StringBuilder(capacity: count * 2);
        for (int i = 0; i < count; i++)
        {
            if (i > 0)
            {
                builder.Append(',');
            }

            builder.Append(i);
        }

        return builder.ToString();
    }

    public static IReadOnlyList<Func<int, int>> CreateMultipliers(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var functions = new List<Func<int, int>>(count);
        for (int i = 1; i <= count; i++)
        {
            int factor = i;
            functions.Add(value => value * factor);
        }

        return functions;
    }

    public static IEnumerable<int> EvenNumbers(IEnumerable<int> numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        foreach (int number in numbers)
        {
            if (number % 2 == 0)
            {
                yield return number;
            }
        }
    }
}
