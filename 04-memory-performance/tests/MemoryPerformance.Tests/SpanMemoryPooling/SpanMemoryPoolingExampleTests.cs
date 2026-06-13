using MemoryPerformance.Examples.SpanMemoryPooling;

namespace MemoryPerformance.Tests.SpanMemoryPooling;

public class SpanMemoryPoolingExampleTests
{
    [Fact]
    public void ParseThreeNumbers_Returns_Expected_Values()
    {
        int[] values = SpanMemoryPoolingExample.ParseThreeNumbers("10,20,30");

        Assert.Equal([10, 20, 30], values);
    }

    [Fact]
    public void NormalizeProductCode_Trims_Removes_Dashes_And_Uppercases()
    {
        string normalized = SpanMemoryPoolingExample.NormalizeProductCode("  ab-123  ");

        Assert.Equal("AB123", normalized);
    }

    [Fact]
    public void RentFillAndSum_Uses_Requested_Length()
    {
        int sum = SpanMemoryPoolingExample.RentFillAndSum(5);

        Assert.Equal(15, sum);
    }

    [Fact]
    public void FormatOrderId_Uses_Fixed_Width_Number()
    {
        Assert.Equal("ORD-000042", SpanMemoryPoolingExample.FormatOrderId(42));
    }
}
