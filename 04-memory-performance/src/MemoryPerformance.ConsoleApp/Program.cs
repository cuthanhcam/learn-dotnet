using MemoryPerformance.Examples.AllocationPatterns;
using MemoryPerformance.Examples.GarbageCollection;
using MemoryPerformance.Examples.MemoryModel;
using MemoryPerformance.Examples.Profiling;
using MemoryPerformance.Examples.SpanMemoryPooling;

namespace MemoryPerformance.ConsoleApp;

public static class Program
{
    public static void Main()
    {
        PrintHeader("Memory & Performance Demo Runner");

        RunSection("Memory Model", MemoryModelExample.Run);
        RunSection("Garbage Collection", GarbageCollectionExample.Run);
        RunSection("Allocation Patterns", AllocationPatternsExample.Run);
        RunSection("Span, Memory, and Pooling", SpanMemoryPoolingExample.Run);
        RunSection("Profiling and Benchmarking", ProfilingExample.Run);

        PrintFooter();
    }

    private static void RunSection(string title, Action action)
    {
        Console.WriteLine();
        Console.WriteLine(new string('-', 70));
        Console.WriteLine(title.ToUpperInvariant().PadLeft((70 + title.Length) / 2));
        Console.WriteLine(new string('-', 70));
        Console.WriteLine();

        action();
    }

    private static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine(title.ToUpperInvariant().PadLeft((70 + title.Length) / 2));
        Console.WriteLine(new string('=', 70));
    }

    private static void PrintFooter()
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("END OF DEMO".PadLeft(40));
        Console.WriteLine(new string('=', 70));
        Console.WriteLine();
    }
}
