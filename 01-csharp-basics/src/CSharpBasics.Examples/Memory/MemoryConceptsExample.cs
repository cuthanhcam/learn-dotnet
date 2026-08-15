using System;
using System.Diagnostics;

namespace CSharpBasics.Examples.Memory;

/// <summary>
/// Demonstrates memory concepts in C#/.NET.
/// 
/// Covers:
/// - Stack vs Heap allocation
/// - Value types vs Reference types
/// - Garbage collection basics
/// - String interning and pooling
/// - Memory leaks and disposal patterns
/// - Struct vs Class tradeoffs
/// 
/// Why memory concepts matter:
/// - Understand performance implications
/// - Avoid unintentional allocations in hot paths
/// - Use appropriate patterns for disposal
/// - Prevent memory leaks in long-running apps
/// </summary>
public static class MemoryConceptsExample
{
    public static void Run()
    {
        Console.WriteLine($"{new string('=', 5)} Memory Concepts Examples {new string('=', 5)}");
        
        PrintSection("STACK VS HEAP");
        DemoStackVsHeap();

        PrintSection("VALUE TYPES VS REFERENCE TYPES");
        DemoValueVsReference();

        PrintSection("STRING INTERNING");
        DemoStringInterning();

        PrintSection("GARBAGE COLLECTION");
        DemoGarbageCollection();

        PrintSection("USING PATTERN FOR DISPOSAL");
        DemoDisposal();

        Console.WriteLine();
    }

    // PUBLIC TEACHING METHODS

    /// <summary>
    /// Returns a value-type local to demonstrate value semantics.
    /// The JIT decides whether the local uses a register or stack slot.
    /// </summary>
    public static int StackAllocationExample()
    {
        int value = 42;
        return value;
    }

    /// <summary>
    /// Allocates a reference type on heap.
    /// Requires GC cleanup after use.
    /// </summary>
    public static Person HeapAllocationExample()
    {
        var person = new Person { Name = "Alice", Age = 30 };  // Allocated on heap
        return person;  // Returned; remains on heap until GC collects
    }

    /// <summary>
    /// Counts memory allocations during operation.
    /// Useful for detecting unexpected allocations in loops.
    /// </summary>
    public static long MeasureAllocations(Action operation)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long before = GC.GetTotalMemory(false);

        operation();

        long after = GC.GetTotalMemory(false);
        return Math.Max(0, after - before);  // Bytes allocated
    }

    // PRIVATE DEMO METHODS

    private static void PrintSection(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"{new string('-', 3)} {title} {new string('-', 3)}");
    }

    /// <summary>
    /// Demonstrates stack vs heap memory layout.
    /// 
    /// STACK:
    /// - Fixed size allocation
    /// - LIFO (Last-In-First-Out)
    /// - Thread-local
    /// - Automatic cleanup on scope exit
    /// - Limited size (~1MB per thread)
    /// 
    /// HEAP:
    /// - Dynamic allocation
    /// - Shared across threads
    /// - Garbage collected
    /// - Much larger size (GB+)
    /// - Access cost depends on locality and workload; benchmark the real path
    /// </summary>
    private static void DemoStackVsHeap()
    {
        // These are value-type locals. The JIT can place them in registers or
        // stack-frame slots, so source code alone does not prove their location.
        int x = 10;
        double y = 3.14;
        bool flag = true;

        Console.WriteLine("Value-type locals (independent values):");
        Console.WriteLine($"  x (int) = {x}");
        Console.WriteLine($"  y (double) = {y}");
        Console.WriteLine($"  flag (bool) = {flag}");

        // Heap allocation: reference type
        var person = new Person { Name = "Bob", Age = 25 };  // Heap allocation
        var address = new Person { Name = "Charlie", Age = 30 };

        Console.WriteLine();
        Console.WriteLine("Heap allocations (GC cleanup later):");
        Console.WriteLine($"  person.Name = {person.Name}");
        Console.WriteLine($"  address.Name = {address.Name}");
        Console.WriteLine("  (Both remain on heap until GC collects)");
    }

    /// <summary>
    /// Demonstrates value vs reference type semantics.
    /// 
    /// VALUE TYPES (struct, int, double, etc.):
    /// - Copied by value
    /// - Own independent copy after assignment
    /// - Default(T) produces zero values
    /// - Allocated on stack (usually)
    /// 
    /// REFERENCE TYPES (class, string, object, etc.):
    /// - Copied by reference (pointer)
    /// - Assignment creates alias to same object
    /// - null represents no value
    /// - Allocated on heap
    /// </summary>
    private static void DemoValueVsReference()
    {
        // VALUE TYPE: int
        int a = 10;
        int b = a;      // Copy by value
        b = 20;

        Console.WriteLine("Value type (int) - independent copies:");
        Console.WriteLine($"  a = {a}, b = {b}  (b's change doesn't affect a)");

        // REFERENCE TYPE: Person (class)
        var person1 = new Person { Name = "Alice", Age = 25 };
        var person2 = person1;  // Copy by reference (same object)
        person2.Name = "Modified";

        Console.WriteLine();
        Console.WriteLine("Reference type (Person) - same object:");
        Console.WriteLine($"  person1.Name = {person1.Name}");
        Console.WriteLine($"  person2.Name = {person2.Name}  (both point to same object)");

        // VALUE STRUCT: Point
        var point1 = new Point { X = 1, Y = 2 };
        var point2 = point1;
        point2.X = 99;

        Console.WriteLine();
        Console.WriteLine("Value type (struct Point) - independent copies:");
        Console.WriteLine($"  point1.X = {point1.X}, point2.X = {point2.X}");
    }

    /// <summary>
    /// Demonstrates string interning.
    /// 
    /// STRING INTERNING:
    /// - Compiler pools identical string literals
    /// - Saves heap memory for duplicate strings
    /// - Strings created dynamically are NOT interned by default
    /// - Use string.Intern() to manually intern
    /// 
    /// Performance implication:
    /// - Literal strings reuse same reference
    /// - Dynamically created strings allocate separate memory
    /// </summary>
    private static void DemoStringInterning()
    {
        // Literal strings: automatically interned by compiler
        string s1 = "hello";
        string s2 = "hello";
        Console.WriteLine($"Literal strings same reference: {ReferenceEquals(s1, s2)}");  // True

        // Dynamic strings: NOT interned by default
        string s3 = new string(new[] { 'h', 'e', 'l', 'l', 'o' });
        string s4 = new string(new[] { 'h', 'e', 'l', 'l', 'o' });
        Console.WriteLine($"Dynamic strings same reference: {ReferenceEquals(s3, s4)}");  // False

        // Manual interning
        string interned1 = string.Intern(s3);
        string interned2 = string.Intern(s4);
        Console.WriteLine($"After intern() same reference: {ReferenceEquals(interned1, interned2)}");  // True

        // Duplicate detection for memory savings
        string[] strings = ["log", "log", "log", "log"];
        Console.WriteLine($"Array with 4 'log' literals uses ~1 object due to interning");
    }

    /// <summary>
    /// Demonstrates garbage collection basics.
    /// 
    /// GARBAGE COLLECTION:
    /// - Automatic memory reclamation
    /// - Generational (Gen 0, 1, 2)
    /// - Stops execution during collection
    /// - Gen 0 collected frequently, Gen 2 rarely
    /// - Can manually trigger with GC.Collect() (rarely needed)
    /// </summary>
    private static void DemoGarbageCollection()
    {
        // Create many heap objects
        Console.WriteLine("Creating 1000 temporary objects...");
        for (int i = 0; i < 1000; i++)
        {
            var _ = new Person { Name = $"Person{i}", Age = i % 100 };
            // 'p' goes out of scope; eventually collected by GC
        }

        // Measure GC collections
        long gen0Before = GC.GetTotalMemory(false);
        GC.Collect();  // Force collection (rarely needed in production)
        long gen0After = GC.GetTotalMemory(false);

        Console.WriteLine($"Memory before collection: {gen0Before} bytes");
        Console.WriteLine($"Memory after collection: {gen0After} bytes");
        Console.WriteLine($"Freed: {gen0Before - gen0After} bytes");

        // Gen 0, 1, 2 info
        Console.WriteLine();
        Console.WriteLine("Generational stats:");
        Console.WriteLine($"  Gen 0 collections: {GC.CollectionCount(0)}");
        Console.WriteLine($"  Gen 1 collections: {GC.CollectionCount(1)}");
        Console.WriteLine($"  Gen 2 collections: {GC.CollectionCount(2)}");
    }

    /// <summary>
    /// Demonstrates IDisposable pattern and using statement.
    /// 
    /// DISPOSAL PATTERN:
    /// - Resources (files, connections, handles) need explicit cleanup
    /// - IDisposable interface marks types that need cleanup
    /// - 'using' statement ensures Dispose() called even on exception
    /// - 'using var' is C# 8.0+ implicit disposal syntax
    /// </summary>
    private static void DemoDisposal()
    {
        Console.WriteLine("Using explicit 'using' block:");
        using (var resource = new ManagedResource("TestResource"))
        {
            Console.WriteLine($"  Using resource: {resource.Name}");
        }  // Dispose() called here automatically
        Console.WriteLine("  Resource disposed");

        Console.WriteLine();
        Console.WriteLine("Using 'using var' declaration (C# 8.0+):");
        using var resource2 = new ManagedResource("TestResource2");
        Console.WriteLine($"  Using resource: {resource2.Name}");
        // Dispose() called here when scope exits
        Console.WriteLine("  Resource disposed (at end of scope)");
    }

    // SUPPORTING TYPES FOR DEMOS

    /// <summary>
    /// Reference type: allocated on heap.
    /// </summary>
    public class Person
    {
        public string? Name { get; set; }
        public int Age { get; set; }

        public override string ToString() => $"{Name} ({Age})";
    }

    /// <summary>
    /// Value type: allocated on stack.
    /// </summary>
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString() => $"({X}, {Y})";
    }

    /// <summary>
    /// Example of managed resource using IDisposable pattern.
    /// </summary>
    public class ManagedResource : IDisposable
    {
        public string Name { get; }
        private bool _disposed = false;

        public ManagedResource(string name)
        {
            Name = name;
            Console.WriteLine($"    Acquired resource: {Name}");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Console.WriteLine($"    Releasing resource: {Name}");
            _disposed = true;
            GC.SuppressFinalize(this);  // Prevent finalizer from running
        }

        ~ManagedResource()
        {
            if (!_disposed)
            {
                Console.WriteLine($"    WARNING: {Name} not explicitly disposed!");
            }
        }
    }
}
