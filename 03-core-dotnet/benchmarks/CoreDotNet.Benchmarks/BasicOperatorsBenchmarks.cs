using BenchmarkDotNet.Attributes;

namespace CoreDotNet.Benchmarks
{
    [MemoryDiagnoser]
    public class BasicOperatorsBenchmarks : LinqBenchmarkBase
    {
        [Benchmark(Baseline = true)]
        public int Where_Select_Sum()
        {
            return Data.Numbers
                .Where(number => number % 2 == 0)
                .Select(number => number * 2)
                .Sum();
        }

        [Benchmark]
        public int ForLoop_Where_Select_Sum()
        {
            int sum = 0;

            for (int index = 0; index < Data.Numbers.Length; index++)
            {
                int number = Data.Numbers[index];

                if (number % 2 == 0)
                {
                    sum += number * 2;
                }
            }

            return sum;
        }

        [Benchmark]
        public bool Any_GreaterThanThreshold()
        {
            return Data.Numbers.Any(number => number > 9_000);
        }

        [Benchmark]
        public bool All_Positive()
        {
            return Data.Numbers.All(number => number > 0);
        }

        [Benchmark]
        public int First_GreaterThanThreshold()
        {
            return Data.Numbers.First(number => number > 9_000);
        }

        [Benchmark]
        public int Last_Element()
        {
            return Data.Numbers.Last();
        }
    }
}
