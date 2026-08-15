using AsyncConcurrency.Examples.Parallelism;

namespace AsyncConcurrency.Tests;

public sealed class ParallelAggregationTests
{
    [Fact]
    public void SumOfSquares_ReturnsSameResultAsSequentialAggregation()
    {
        int[] values = Enumerable.Range(-1_000, 2_001).ToArray();
        long expected = values.Sum(static value => (long)value * value);

        long actual = ParallelAggregation.SumOfSquares(values, maxDegreeOfParallelism: 4);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SumOfSquares_EmptyInputReturnsIdentityValue()
    {
        Assert.Equal(0, ParallelAggregation.SumOfSquares([], maxDegreeOfParallelism: 2));
    }

    [Fact]
    public void SumOfSquares_RejectsInvalidParallelism()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ParallelAggregation.SumOfSquares([1], maxDegreeOfParallelism: 0));
    }
}
