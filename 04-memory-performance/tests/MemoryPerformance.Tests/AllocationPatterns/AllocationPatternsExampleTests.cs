using MemoryPerformance.Examples.AllocationPatterns;

namespace MemoryPerformance.Tests.AllocationPatterns;

public class AllocationPatternsExampleTests
{
    [Fact]
    public void Boxed_And_Generic_Sums_Return_Same_Result()
    {
        int[] numbers = [1, 2, 3, 4];

        Assert.Equal(
            AllocationPatternsExample.SumGenericNumbers(numbers),
            AllocationPatternsExample.SumBoxedNumbers(numbers));
    }

    [Fact]
    public void StringBuilder_And_Concatenation_Return_Same_Text()
    {
        Assert.Equal(
            AllocationPatternsExample.BuildWithConcatenation(5),
            AllocationPatternsExample.BuildWithStringBuilder(5));
    }

    [Fact]
    public void CreateMultipliers_Captures_Separate_Factors()
    {
        IReadOnlyList<Func<int, int>> multipliers = AllocationPatternsExample.CreateMultipliers(3);

        Assert.Equal([10, 20, 30], multipliers.Select(function => function(10)));
    }
}
