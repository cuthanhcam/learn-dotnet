using BenchmarkDotNet.Attributes;
using Dsa.Examples.SortingSearching;

namespace Dsa.Benchmarks;

public class SearchingBenchmarks
{
    private readonly int[] _values = Enumerable.Range(1, 100_000).ToArray();

    [Benchmark(Baseline = true)]
    public int LinearSearch()
    {
        for (int i = 0; i < _values.Length; i++)
        {
            if (_values[i] == 87_654)
            {
                return i;
            }
        }

        return -1;
    }

    [Benchmark]
    public int BinarySearch()
    {
        return SortingSearchingExample.BinarySearch(_values, 87_654);
    }
}
