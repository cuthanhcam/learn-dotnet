---
title: "Memory Management and GC Fundamentals"
description: "An introduction to value/reference semantics, the managed heap, garbage collection, and disposal."
phase: 1
order: 8
topics: [dotnet, memory, garbage-collection]
---

# Memory Management & GC Fundamentals

Understanding how C# manages memory and the garbage collector.

## Stack vs Heap: The Basics

### Stack Allocation

Each thread uses a **stack** for call frames, including return information and
many method locals. The JIT may also keep values in registers. Value types are
not defined by living on the stack; they can be embedded in heap objects:

```csharp
int x = 42;              // A local value; exact placement is a JIT decision
double y = 3.14;
bool flag = true;

struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

Point p = new Point();   // Value semantics; storage is context-dependent
```

**Characteristics:**
- Call-frame allocation is typically very cheap
- A frame is reclaimed when its method returns
- Stack space is finite and excessive recursion can cause `StackOverflowException`
- Predictable performance
- No garbage collection

### Heap Allocation

The **managed heap** contains managed objects whose lifetimes are tracked by the
garbage collector. Value-type fields and array elements may be stored inline as
part of those objects:

```csharp
string text = "Hello";        // Heap: string reference stored here
Person person = new Person(); // Heap: class instance allocated here
List<int> list = new List<int>(); // Heap

// Variables store REFERENCES (addresses), not values
string? s1 = text;            // s1 points to same heap location as text
s1 = null;                     // text object still exists on heap
```

**Characteristics:**
- Small-object allocation is usually fast, but creates future GC work
- Requires garbage collection to free memory
- Larger available size (~GB range)
- Variables store references to heap objects
- Performance depends on allocation rate, object lifetime, and GC pressure

### Value vs Reference Semantics

```csharp
// VALUE SEMANTICS (independent value copy)
int a = 10;
int b = a;
b = 20;
// a = 10 (unaffected)

// REFERENCE SEMANTICS (both variables refer to the same object)
var list1 = new List<int> { 1, 2, 3 };
var list2 = list1;
list2.Add(4);
// list1 now also has { 1, 2, 3, 4 } (same object)

// Copy semantics for reference types
var list3 = new List<int>(list1);  // Creates new list, independent
list3.Add(5);
// list1 still { 1, 2, 3, 4 }
```

---

## String Interning: Optimization & Gotchas

String literals are automatically **interned** (cached in a special pool):

```csharp
string s1 = "hello";
string s2 = "hello";
object.ReferenceEquals(s1, s2);  // true - same heap location!

// But dynamically created strings are not interned
string s3 = new string(new[] { 'h', 'e', 'l', 'l', 'o' });
object.ReferenceEquals(s1, s3);  // false - different heap locations

// String concatenation creates new strings (not interned)
string s4 = "hel" + "lo";
object.ReferenceEquals(s1, s4);  // Often true due to compiler optimization
                                 // But not guaranteed
```

**Performance Implication:**
- Interned strings reduce memory for duplicate literals
- Reference equality (ReferenceEquals) can be misleading
- Use `Equals()` or `==` for value comparison instead

---

## Garbage Collection: How .NET Cleans Up

### Generational Collection

The .NET garbage collector uses **generations** to improve efficiency:

```csharp
GC.Collect();  // Force immediate collection (DON'T DO THIS NORMALLY)

// Check collection counts
int gen0 = GC.CollectionCount(0);  // Fast collections (young objects)
int gen1 = GC.CollectionCount(1);  // Medium collections
int gen2 = GC.CollectionCount(2);  // Full collections (expensive)

Console.WriteLine($"Gen 0: {gen0}, Gen 1: {gen1}, Gen 2: {gen2}");
```

**Generation Strategy:**
- **Gen 0:** Short-lived objects, collected frequently (fast)
- **Gen 1:** Medium-lived objects, collected less frequently
- **Gen 2:** Long-lived objects, collected rarely (slow)

**Typical Pattern:**
```csharp
// Allocate many temporary objects
for (int i = 0; i < 1_000_000; i++)
{
    var obj = new MyObject();  // Allocated to Gen 0
    Process(obj);              // Used briefly, then eligible for GC
}
// Most objects collected quickly in Gen 0 collection
// Few survive to Gen 1 or Gen 2

// Long-lived static objects
static List<MyObject> cache = new();
// These survive Gen 0 collections, promoted to Gen 1/2
// Gen 2 collection is expensive but infrequent
```

### Collection Events

Monitor when GC runs:

```csharp
long memBefore = GC.GetTotalMemory(false);
int gen0Before = GC.CollectionCount(0);

// Allocate many objects
var objects = new object[100_000];
for (int i = 0; i < objects.Length; i++)
{
    objects[i] = new byte[1024];  // 1KB each
}

long memAfter = GC.GetTotalMemory(false);
int gen0After = GC.CollectionCount(0);

Console.WriteLine($"Memory allocated: {(memAfter - memBefore) / 1024}KB");
Console.WriteLine($"Gen 0 collections: {gen0After - gen0Before}");

// Clean up
Array.Clear(objects);
GC.Collect();

long memAfterGC = GC.GetTotalMemory(true);
Console.WriteLine($"Memory freed: {(memAfter - memAfterGC) / 1024}KB");
```

---

## IDisposable Pattern: Cleaning Up Unmanaged Resources

Not all resources are managed by the GC. Some need explicit cleanup:

```csharp
// ❌ WRONG: No cleanup for unmanaged resources
public class FileReader
{
    private IntPtr fileHandle;
    
    public void Open(string path)
    {
        fileHandle = OpenFile(path);  // Native file handle
    }
    
    ~FileReader()  // Finalizer - unreliable cleanup timing
    {
        CloseFile(fileHandle);
    }
}

// ✅ CORRECT: IDisposable pattern
public class FileReader : IDisposable
{
    private IntPtr fileHandle;
    private bool disposed = false;
    
    public void Open(string path)
    {
        ThrowIfDisposed();
        fileHandle = OpenFile(path);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);  // Tell GC: finalizer not needed
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                // (e.g., other IDisposable objects)
            }
            
            // Clean up unmanaged resources
            if (fileHandle != IntPtr.Zero)
            {
                CloseFile(fileHandle);
                fileHandle = IntPtr.Zero;
            }
            
            disposed = true;
        }
    }
    
    ~FileReader()  // Finalizer - safety net
    {
        Dispose(false);
    }
    
    private void ThrowIfDisposed()
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(FileReader));
    }
}
```

### Using Statement: Automatic Disposal

The `using` statement ensures `Dispose()` is called:

```csharp
// ❌ BEFORE: Manual disposal
FileReader reader = null;
try
{
    reader = new FileReader();
    reader.Open("data.txt");
    // Use reader
}
finally
{
    reader?.Dispose();
}

// ✅ AFTER: Automatic disposal with using statement
using (var reader = new FileReader())
{
    reader.Open("data.txt");
    // Use reader
}
// Dispose() called automatically, even if exception occurs

// ✅ MODERN (C# 8+): Using declaration
using var reader = new FileReader();
reader.Open("data.txt");
// Use reader
// Dispose() called when scope ends
```

---

## Performance Implications

### Allocation Patterns

```csharp
// ❌ SLOW: Allocates new string each iteration
string result = "";
for (int i = 0; i < 1000; i++)
{
    result += i.ToString();  // Millions of small allocations
}
// GC pressure: HIGH, String moves around during concatenation

// ✅ FAST: Single allocation for final result
var sb = new StringBuilder();
for (int i = 0; i < 1000; i++)
{
    sb.Append(i.ToString());  // Buffer grows once
}
string result = sb.ToString();
// GC pressure: LOW

// ✅ FASTEST: Pre-sized allocation
var sb = new StringBuilder(20_000);  // Allocate sufficient capacity
for (int i = 0; i < 1000; i++)
{
    sb.Append(i.ToString());
}
string result = sb.ToString();
// GC pressure: MINIMAL
```

### Boxing & Unboxing

```csharp
// ❌ BOXING: Value type copied to heap
int x = 42;
object boxed = x;  // Allocates object on heap, copies value
int unboxed = (int)boxed;  // Copies value back to stack

// Multiple boxes create GC pressure
for (int i = 0; i < 1_000_000; i++)
{
    object o = i;  // Allocates 1M objects!
}

// ✅ GENERICS: Avoid boxing
List<int> list = new();
for (int i = 0; i < 1_000_000; i++)
{
    list.Add(i);  // No boxing, no allocation per item
}

// Generic delegates also avoid boxing
Action<int> action = x => Console.WriteLine(x);
for (int i = 0; i < 1_000_000; i++)
{
    action(i);  // No boxing
}
```

---

## Best Practices

### 1. Use Local Variables with Short Scope
```csharp
// ❌ AVOID: Long scope
public class Service
{
    private MyObject tempObject = new();
    
    public void Process()
    {
        tempObject.DoWork();  // Still held in memory
        DoOtherStuff();       // Memory not freed
    }
}

// ✅ PREFER: Scope-limited
public void Process()
{
    using (var tempObject = new MyObject())
    {
        tempObject.DoWork();
    }  // Freed immediately
    DoOtherStuff();
}
```

### 2. Use Object Pooling for High-Allocation Patterns
```csharp
// ✅ Object pool pattern
public class ObjectPool<T> where T : class, new()
{
    private Queue<T> available = new();
    private int maxSize;
    
    public ObjectPool(int size)
    {
        maxSize = size;
        for (int i = 0; i < size; i++)
            available.Enqueue(new T());
    }
    
    public T Rent()
    {
        return available.Count > 0 ? available.Dequeue() : new T();
    }
    
    public void Return(T obj)
    {
        if (available.Count < maxSize)
            available.Enqueue(obj);
    }
}

// Usage
var pool = new ObjectPool<StringBuilder>(10);
var sb = pool.Rent();
sb.Append("Hello");
pool.Return(sb);
```

### 3. Prefer Structs for Small Value Types
```csharp
// ✅ GOOD: Stack allocation, no GC pressure
struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

// ❌ AVOID: Unnecessary heap allocation
class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}
```

### 4. Avoid Large Object Heap Fragmentation
```csharp
// Objects > 85KB go to Large Object Heap (not compacted)
// ❌ AVOID: Large allocations in loops
for (int i = 0; i < 1000; i++)
{
    byte[] buffer = new byte[1_000_000];  // 1MB each
    ProcessBuffer(buffer);
}

// ✅ PREFER: Reuse large buffer
byte[] buffer = new byte[1_000_000];
for (int i = 0; i < 1000; i++)
{
    ProcessBuffer(buffer);
}
```

---

## Key Takeaways

- **Stack:** Fast allocation, automatic deallocation, limited size, value types
- **Heap:** Flexible allocation, requires GC, reference types
- **String interning:** Literal strings cached, but dynamic strings create new objects
- **Generations:** Gen 0 (fast) → Gen 1 → Gen 2 (expensive) collections
- **IDisposable:** Use for unmanaged resources and guaranteed cleanup
- **Using statement:** Automatic disposal of IDisposable objects
- **Object pooling:** Reduce allocations for high-frequency scenarios
- **Structs:** Better for small, short-lived value types
- **Avoid boxing:** Use generics instead of `object` when possible
- **Monitor GC:** Track collections and memory to identify pressure points

---

## Related Source & Benchmarks

- Source example: `src/CSharpBasics.Examples/Memory/MemoryConceptsExample.cs`
- Benchmark project: `benchmarks/MemoryBenchmarks/MemoryBenchmarks.csproj`

Run memory benchmark:

```bash
dotnet run -c Release --project benchmarks/MemoryBenchmarks/MemoryBenchmarks.csproj
```
