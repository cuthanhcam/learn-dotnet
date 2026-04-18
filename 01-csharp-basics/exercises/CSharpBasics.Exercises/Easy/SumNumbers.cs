namespace CSharpBasics.Exercises.Easy;

public static class SumNumbers
{
    public static int CalculateSum(params int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        int sum = 0;
        for (int i = 0; i < numbers.Length; i++)
        {
            sum += numbers[i];
        }

        return sum;
    }
}
