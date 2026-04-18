using System.Diagnostics;
using System.Text;

namespace CSharpBasics.Benchmarks;

public static class StringBenchmark
{
    public static void Run()
    {
        Console.WriteLine("String Benchmark Lab");
        Console.WriteLine(new string('=', 60));

        const int iterations = 50_000;

        Benchmark("Concatenation (+)", iterations, () =>
        {
            string result = string.Empty;
            for (int i = 0; i < 100; i++)
            {
                result += i;
            }
            _ = result.Length;
        });

        Benchmark("string.Concat", iterations, () =>
        {
            var items = new string[100];
            for (int i = 0; i < 100; i++)
            {
                items[i] = i.ToString();
            }
            string result = string.Concat(items);
            _ = result.Length;
        });

        Benchmark("StringBuilder", iterations, () =>
        {
            var sb = new StringBuilder(256);
            for (int i = 0; i < 100; i++)
            {
                sb.Append(i);
            }
            string result = sb.ToString();
            _ = result.Length;
        });

        Benchmark("Interpolation", iterations, () =>
        {
            int a = 42;
            int b = 99;
            string result = $"A={a}, B={b}, Sum={a + b}";
            _ = result.Length;
        });

        Benchmark("Split + Join", iterations, () =>
        {
            const string csv = "csharp,dotnet,performance,benchmark,string";
            string[] parts = csv.Split(',');
            string result = string.Join("|", parts);
            _ = result.Length;
        });

        Console.WriteLine();
        Console.WriteLine("Done. Run with Release mode for stable numbers:");
        Console.WriteLine("dotnet run -c Release --project benchmarks/StringBenchmark/StringBenchmark.csproj");
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

        Console.WriteLine($"{name,-20} | {nsPerOp,10:F1} ns/op | {bytesPerOp,6} B/op");
    }
}
