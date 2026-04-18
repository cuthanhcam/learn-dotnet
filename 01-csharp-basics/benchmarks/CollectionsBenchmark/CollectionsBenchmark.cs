using System.Diagnostics;

namespace CSharpBasics.Benchmarks;

public static class CollectionsBenchmark
{
    public static void Run()
    {
        Console.WriteLine("Collections Benchmark Lab");
        Console.WriteLine(new string('=', 60));

        const int size = 100_000;
        const int iterations = 200;

        int[] array = Enumerable.Range(0, size).ToArray();
        List<int> list = array.ToList();
        HashSet<int> set = array.ToHashSet();
        Dictionary<int, int> dictionary = array.ToDictionary(x => x, x => x * 2);

        Benchmark("Array sum", iterations, () =>
        {
            long sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }
            _ = sum;
        });

        Benchmark("List sum", iterations, () =>
        {
            long sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }
            _ = sum;
        });

        Benchmark("HashSet.Contains", iterations * 10, () =>
        {
            int found = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (set.Contains(i))
                {
                    found++;
                }
            }
            _ = found;
        });

        Benchmark("Dictionary lookup", iterations * 10, () =>
        {
            long sum = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (dictionary.TryGetValue(i, out int value))
                {
                    sum += value;
                }
            }
            _ = sum;
        });

        Benchmark("List add/remove tail", iterations * 2, () =>
        {
            var temp = new List<int>(1024);
            for (int i = 0; i < 10_000; i++)
            {
                temp.Add(i);
            }

            for (int i = 0; i < 10_000; i++)
            {
                temp.RemoveAt(temp.Count - 1);
            }
        });

        Console.WriteLine();
        Console.WriteLine("Done. Run with Release mode for stable numbers:");
        Console.WriteLine("dotnet run -c Release --project benchmarks/CollectionsBenchmark/CollectionsBenchmark.csproj");
    }

    private static void Benchmark(string name, int iterations, Action action)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        action();

        long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            action();
        }

        sw.Stop();
        long allocatedAfter = GC.GetAllocatedBytesForCurrentThread();

        double nsPerOp = sw.Elapsed.TotalNanoseconds / iterations;
        long bytesPerOp = (allocatedAfter - allocatedBefore) / iterations;

        Console.WriteLine($"{name,-22} | {nsPerOp,10:F1} ns/op | {bytesPerOp,8} B/op");
    }
}
