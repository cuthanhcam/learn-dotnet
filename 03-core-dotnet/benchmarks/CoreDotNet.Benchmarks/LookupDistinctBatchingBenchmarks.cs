using BenchmarkDotNet.Attributes;

namespace CoreDotNet.Benchmarks
{
    [MemoryDiagnoser]
    public class LookupDistinctBatchingBenchmarks : LinqBenchmarkBase
    {
        [Benchmark]
        public int DistinctCategories()
        {
            return Data.Products
                .Select(product => product.Category)
                .Distinct()
                .Count();
        }

        [Benchmark]
        public int ToLookupCategories()
        {
            return Data.Products
                .ToLookup(product => product.Category, product => product.Name)
                .Count;
        }

        [Benchmark]
        public int ChunkProducts()
        {
            return Data.Products
                .OrderBy(product => product.Id)
                .Chunk(25)
                .Count();
        }
    }
}
