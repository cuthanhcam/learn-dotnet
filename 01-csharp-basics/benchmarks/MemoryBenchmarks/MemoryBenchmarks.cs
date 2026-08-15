using System;
using System.Diagnostics;
using CSharpBasics.Examples.Memory;

namespace CSharpBasics.Benchmarks;

/// <summary>
/// Performance benchmarks for memory allocation and GC behavior.
/// Demonstrates the practical impact of different allocation patterns.
/// 
/// Run with: dotnet run -c Release --project benchmarks/MemoryBenchmarks/
/// </summary>
public static class MemoryBenchmarks
{
    public static void Run()
    {
        Console.WriteLine("Memory Allocation & GC Behavior Benchmarks");
        Console.WriteLine(new string('=', 60));

        BenchmarkValueTypeAllocation();
        BenchmarkReferenceTypeAllocation();
        BenchmarkStringAllocation();
        BenchmarkStackVsHeapPerformance();
        BenchmarkGCCollection();

        Console.WriteLine();
        Console.WriteLine("Benchmarks completed. Results shown above.");
    }

    /// <summary>
    /// Measures arithmetic on value-type locals. This is not an allocation
    /// benchmark because the JIT may keep these values in registers or remove
    /// work whose result cannot be observed.
    /// </summary>
    private static void BenchmarkValueTypeAllocation()
    {
        Console.WriteLine("\n--- VALUE-TYPE LOCAL OPERATIONS ---");

        const int iterations = 10_000_000;
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            int value = i;
            double d = i * 1.5;
            bool flag = i % 2 == 0;
        }

        sw.Stop();
        double msPerOp = sw.Elapsed.TotalMilliseconds / iterations;

        Console.WriteLine($"Iterations: {iterations:N0}");
        Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"Per operation: {msPerOp * 1_000:F3}µs");
    }

    /// <summary>
    /// Benchmarks allocation speed of reference types on heap.
    /// The observed ratio is environment-specific and must not be generalized.
    /// </summary>
    private static void BenchmarkReferenceTypeAllocation()
    {
        Console.WriteLine("\n--- REFERENCE TYPE ALLOCATION (Heap) ---");

        const int iterations = 100_000;
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            var person = new MemoryConceptsExample.Person 
            { 
                Name = $"Person{i}", 
                Age = i % 100 
            };
        }

        sw.Stop();
        double msPerOp = sw.Elapsed.TotalMilliseconds / iterations;

        Console.WriteLine($"Iterations: {iterations:N0}");
        Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"Per operation: {msPerOp * 1_000:F3}µs");
    }

    /// <summary>
    /// Benchmarks string allocation and interning behavior.
    /// Shows cost of string concatenation vs StringBuilder approach.
    /// </summary>
    private static void BenchmarkStringAllocation()
    {
        Console.WriteLine("\n--- STRING ALLOCATION PATTERNS ---");

        // Test 1: Literal strings (interned)
        Console.WriteLine("Literal string allocation (interned):");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < 1_000_000; i++)
        {
            string s = "literal_string";
            _ = s.Length;
        }
        sw1.Stop();
        Console.WriteLine($"  1M iterations: {sw1.ElapsedMilliseconds}ms");

        // Test 2: Dynamic strings (not interned)
        Console.WriteLine("Dynamic string allocation (not interned):");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < 1_000_000; i++)
        {
            string s = new string(new[] { 'h', 'i' });
            _ = s.Length;
        }
        sw2.Stop();
        Console.WriteLine($"  1M iterations: {sw2.ElapsedMilliseconds}ms");

        // Test 3: String concatenation vs StringBuilder
        Console.WriteLine("String concatenation (O(n²) allocations):");
        var sw3 = Stopwatch.StartNew();
        string result = "";
        for (int i = 0; i < 1000; i++)
        {
            result += i.ToString();
        }
        sw3.Stop();
        Console.WriteLine($"  1000 concatenations: {sw3.ElapsedMilliseconds}ms");

        // Test 4: StringBuilder
        Console.WriteLine("StringBuilder (O(n) allocations):");
        var sw4 = Stopwatch.StartNew();
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < 1000; i++)
        {
            sb.Append(i.ToString());
        }
        string result2 = sb.ToString();
        sw4.Stop();
        Console.WriteLine($"  1000 appends: {sw4.ElapsedMilliseconds}ms");

        if (sw4.ElapsedMilliseconds == 0)
        {
            Console.WriteLine("Concatenation vs StringBuilder ratio: too small to measure at ms precision");
        }
        else
        {
            double ratio = (double)sw3.ElapsedMilliseconds / sw4.ElapsedMilliseconds;
            Console.WriteLine($"Concatenation is {ratio:F1}x slower than StringBuilder");
        }
    }

    /// <summary>
    /// Contrasts operations on a struct local with creation of class instances.
    /// The loops perform different work, so this is an exploratory demo rather
    /// than proof of a universal "stack versus heap" performance ratio.
    /// </summary>
    private static void BenchmarkStackVsHeapPerformance()
    {
        Console.WriteLine("\n--- STACK VS HEAP PERFORMANCE ---");

        const int iterations = 100_000_000;

        // Stack test: Value type
        Console.WriteLine("Struct local operations:");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var point = new MemoryConceptsExample.Point { X = i, Y = i * 2 };
            int x = point.X;
        }
        sw1.Stop();
        Console.WriteLine($"  {iterations:N0} iterations: {sw1.ElapsedMilliseconds}ms");

        // Heap test: Reference type
        const int heapIterations = 1_000_000;
        Console.WriteLine("Heap allocation (reference type - class):");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < heapIterations; i++)
        {
            var person = new MemoryConceptsExample.Person { Age = i };
            int age = person.Age;
        }
        sw2.Stop();
        Console.WriteLine($"  {heapIterations:N0} iterations: {sw2.ElapsedMilliseconds}ms");

        double stackTimePerOp = sw1.Elapsed.TotalNanoseconds / iterations;
        double heapTimePerOp = sw2.Elapsed.TotalNanoseconds / heapIterations;

        Console.WriteLine($"\nStack: {stackTimePerOp:F1}ns per operation");
        Console.WriteLine($"Heap:  {heapTimePerOp:F1}ns per operation");
        Console.WriteLine($"Heap is {heapTimePerOp / stackTimePerOp:F1}x slower");
    }

    /// <summary>
    /// Demonstrates GC collection impact on performance.
    /// Shows Gen 0, Gen 1, Gen 2 collection counts and memory freed.
    /// </summary>
    private static void BenchmarkGCCollection()
    {
        Console.WriteLine("\n--- GARBAGE COLLECTION IMPACT ---");

        // Force clean state
        GC.Collect();
        GC.WaitForPendingFinalizers();

        int gen0Before = GC.CollectionCount(0);
        int gen1Before = GC.CollectionCount(1);
        int gen2Before = GC.CollectionCount(2);

        Console.WriteLine("Before allocations:");
        Console.WriteLine($"  Gen 0 collections: {gen0Before}");
        Console.WriteLine($"  Gen 1 collections: {gen1Before}");
        Console.WriteLine($"  Gen 2 collections: {gen2Before}");

        long memBefore = GC.GetTotalMemory(false);

        // Allocate many objects
        var objects = new object[10_000];
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i] = new MemoryConceptsExample.Person 
            { 
                Name = $"Person{i}",
                Age = i % 100
            };
        }

        long memAfter = GC.GetTotalMemory(false);

        Console.WriteLine("\nAfter allocating 10,000 objects:");
        Console.WriteLine($"  Memory allocated: {(memAfter - memBefore) / 1024:N0}KB");

        // Clear and collect
        Array.Clear(objects);
        GC.Collect();
        GC.WaitForPendingFinalizers();

        int gen0After = GC.CollectionCount(0);
        int gen1After = GC.CollectionCount(1);
        int gen2After = GC.CollectionCount(2);

        long memAfterCollection = GC.GetTotalMemory(false);

        Console.WriteLine("\nAfter GC.Collect():");
        Console.WriteLine($"  Gen 0 collections: {gen0After - gen0Before} new collections");
        Console.WriteLine($"  Gen 1 collections: {gen1After - gen1Before} new collections");
        Console.WriteLine($"  Gen 2 collections: {gen2After - gen2Before} new collections");
        Console.WriteLine($"  Memory freed: {(memAfter - memAfterCollection) / 1024:N0}KB");
    }
}
