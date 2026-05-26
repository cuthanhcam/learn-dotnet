using BenchmarkDotNet.Attributes;

namespace CoreDotNet.Benchmarks
{
    [MemoryDiagnoser]
    public class FilteringProjectionBenchmarks : LinqBenchmarkBase
    {
        [Benchmark]
        public int FilterExpensiveProducts()
        {
            return Data.Products.Count(product => product.Price > 100m);
        }

        [Benchmark]
        public int ProjectProductNames()
        {
            return Data.Products
                .Select(product => $"{product.Name} (${product.Price})")
                .Sum(name => name.Length);
        }

        [Benchmark]
        public int FlattenWordsWithSelectMany()
        {
            return Data.Sentences
                .SelectMany(sentence => sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Count();
        }
    }
}
