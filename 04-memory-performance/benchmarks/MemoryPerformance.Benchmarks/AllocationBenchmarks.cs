using BenchmarkDotNet.Attributes;
using MemoryPerformance.Examples.AllocationPatterns;

[MemoryDiagnoser]
public class AllocationBenchmarks
{
    private readonly int[] _numbers = Enumerable.Range(1, 1_000).ToArray();

    [Benchmark(Baseline = true)]
    public int SumGeneric()
    {
        return AllocationPatternsExample.SumGenericNumbers(_numbers);
    }

    [Benchmark]
    public int SumWithBoxing()
    {
        return AllocationPatternsExample.SumBoxedNumbers(_numbers);
    }

    [Benchmark]
    [Arguments(100)]
    public string BuildWithStringBuilder(int count)
    {
        return AllocationPatternsExample.BuildWithStringBuilder(count);
    }

    [Benchmark]
    [Arguments(100)]
    public string BuildWithConcatenation(int count)
    {
        return AllocationPatternsExample.BuildWithConcatenation(count);
    }
}
