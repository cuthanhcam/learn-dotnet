using BenchmarkDotNet.Attributes;

namespace CoreDotNet.Benchmarks
{
    [MemoryDiagnoser]
    public class SortingPaginationBenchmarks : LinqBenchmarkBase
    {
        [Benchmark]
        public int OrderByPrice()
        {
            return Data.Products
                .OrderBy(product => product.Price)
                .Take(100)
                .Sum(product => product.Id);
        }

        [Benchmark]
        public int OrderByCategoryThenPrice()
        {
            return Data.Products
                .OrderBy(product => product.Category)
                .ThenByDescending(product => product.Price)
                .Take(100)
                .Sum(product => product.Id);
        }

        [Benchmark]
        public int SkipTakePage()
        {
            return Data.Products
                .OrderBy(product => product.Id)
                .Skip(250)
                .Take(50)
                .Sum(product => product.Id);
        }
    }
}
