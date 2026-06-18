using Dsa.Examples.Complexity;

namespace Dsa.Tests.Complexity;

public sealed class ComplexityExampleTests
{
    [Fact]
    public void ConstantFirstOrDefaultReturnsZeroForEmptyInput()
    {
        Assert.Equal(0, ComplexityExample.ConstantFirstOrDefault([]));
    }

    [Fact]
    public void LinearSumAddsAllValues()
    {
        Assert.Equal(15, ComplexityExample.LinearSum([1, 2, 3, 4, 5]));
    }

    [Fact]
    public void QuadraticPairCountCountsMatchingPairs()
    {
        Assert.Equal(2, ComplexityExample.QuadraticPairCount([1, 2, 3, 4, 5], 6));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(1024, 10)]
    public void LogarithmicHalvingStepsCountsDivisions(int inputSize, int expected)
    {
        Assert.Equal(expected, ComplexityExample.LogarithmicHalvingSteps(inputSize));
    }
}
