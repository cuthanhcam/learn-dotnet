using BenchmarkDotNet.Attributes;
using Dsa.Examples.ArraysStrings;

namespace Dsa.Benchmarks;

public class ArraysStringsBenchmarks
{
    private readonly int[] _values = Enumerable.Range(1, 10_000).ToArray();
    private int[] _prefix = [];

    [GlobalSetup]
    public void Setup()
    {
        _prefix = ArraysStringsExample.BuildPrefixSums(_values);
    }

    [Benchmark(Baseline = true)]
    public int SumRangeByLoop()
    {
        int sum = 0;

        for (int i = 2500; i < 7500; i++)
        {
            sum += _values[i];
        }

        return sum;
    }

    [Benchmark]
    public int SumRangeByPrefixSums()
    {
        return ArraysStringsExample.RangeSum(_prefix, 2500, 7500);
    }
}
