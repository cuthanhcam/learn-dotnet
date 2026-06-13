using BenchmarkDotNet.Attributes;
using MemoryPerformance.Examples.SpanMemoryPooling;

[MemoryDiagnoser]
public class SpanAndPoolingBenchmarks
{
    private const string ProductCode = "  ab-123-cd-456  ";

    [Benchmark]
    public int[] ParseThreeNumbers()
    {
        return SpanMemoryPoolingExample.ParseThreeNumbers("10,20,30");
    }

    [Benchmark]
    public string NormalizeProductCode()
    {
        return SpanMemoryPoolingExample.NormalizeProductCode(ProductCode);
    }

    [Benchmark]
    [Arguments(512)]
    public int RentFillAndSum(int length)
    {
        return SpanMemoryPoolingExample.RentFillAndSum(length);
    }
}
