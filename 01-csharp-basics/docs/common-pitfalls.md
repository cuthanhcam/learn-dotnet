# Common C# Pitfalls & Solutions

Eight critical mistakes beginners and intermediate developers make in C#, with clear fixes.

## 1. Using dynamic Unnecessarily

Problem: dynamic delays type checking until runtime, causing hidden bugs and poor performance.

Bad:
```csharp
dynamic value = GetUserInput();  // Type unknown at compile time
int result = value + 10;         // Might crash at runtime!

dynamic obj = GetObject();
obj.UndefinedMethod();           // Only fails at runtime
```

Good:
```csharp
// Use strong typing with type checks
object value = GetUserInput();
if (value is int intValue)
{
    int result = intValue + 10;
}

// Or use generics
T GetValue<T>(string key) => ...;
int value = GetValue<int>("age");
```

When dynamic IS acceptable:
- Calling COM objects
- Reflection scenarios
- Interop with dynamic languages (Python, etc.)

---

## 2. ❌ String Concatenation in Loops

### The Problem

Each `+` operation creates a **new string object**:
- Creates 1000 strings for 1000 iterations
- Memory bloat
- Performance death

### ❌ Bad Code (Performance Disaster)

```csharp
string result = "";
for (int i = 0; i < 10000; i++)
{
    result += i.ToString();  // Creates 10,000 strings!
}
```

### ✅ Good Code

```csharp
var sb = new StringBuilder();
for (int i = 0; i < 10000; i++)
{
    sb.Append(i);
}
string result = sb.ToString();  // Only ONE final string
```

### Performance Impact

| Approach | Time | Memory |
|----------|------|--------|
| String `+` (1000x) | ~500ms | ~5MB |
| StringBuilder | ~1ms | ~50KB |
| **Speedup** | **500x faster** | **100x less** |

---

## 3. ❌ Not Checking Nulls

### The Problem

**NullReferenceException** — the billion-dollar mistake

```csharp
string text = GetValue();  // What if null?
int length = text.Length;  // CRASH!
```

### ✅ Solution 1: Null Coalescing

```csharp
string text = GetValue();
int length = text?.Length ?? 0;  // Safe!
```

### ✅ Solution 2: Nullable Reference Types (C# 8+)

```csharp
// Enable in .csproj: <nullable>enable</nullable>

string? maybeNull = GetValue();        // Can be null (marked with ?)
string notNull = "Hello";              // Cannot be null (compile error if null assigned)

int length = maybeNull?.Length ?? 0;   // Safe, compiler enforces
```

### ✅ Solution 3: Pattern Matching

```csharp
if (text is not null)
{
    Console.WriteLine(text.Length);  // Compiler knows it's not null
}
```

---

## 4. ❌ Inefficient Loop Patterns

### The Problem

Calling `.Count` or `.Length` repeatedly is wasteful:

```csharp
for (int i = 0; i < list.Count; i++)      // Count called 1001 times!
{
    Console.WriteLine(list[i]);
}
```

### ✅ Better Patterns

```csharp
// Pattern 1: Cache the count
int count = list.Count;
for (int i = 0; i < count; i++)
{
    Console.WriteLine(list[i]);
}

// Pattern 2: Foreach (best for most cases)
foreach (var item in list)
{
    Console.WriteLine(item);
}

// Pattern 3: LINQ (readable)
list.ForEach(item => Console.WriteLine(item));
```

### Performance

| Pattern | Evaluations | Speed |
|---------|-------------|-------|
| `list.Count` in loop | 10,000+ | Slow |
| Cached count | 1 | Fast |
| `foreach` | N/A | Optimal |

---

## 5. ❌ Forgetting `readonly` vs `const`

### The Difference

| Feature | `const` | `readonly` |
|---------|---------|-----------|
| Runtime? | Compile-time only | Runtime |
| Performance | Best | Slightly slower |
| Can use in methods? | ❌ No | ✅ Yes |
| Can initialize from method? | ❌ No | ✅ Yes |
| Value changeable? | ❌ Never | ❌ Never (after init) |

### ❌ Incomplete Code

```csharp
// Compile-time constants only
const int MaxRetries = 3;
const string Name = "Alice";

// But what if you need runtime values?
const string ConfigPath = GetPath();  // ❌ ERROR!
```

### ✅ Correct Code

```csharp
// Compile-time constant
const int MaxRetries = 3;

// Runtime value that never changes
readonly string ConfigPath = GetPath();

class Settings
{
    public readonly int Timeout;  // Set in constructor
    
    public Settings(int timeout)
    {
        Timeout = timeout;  // ✅ Can set once
        Timeout = 500;      // ❌ ERROR: already assigned
    }
}
```

---

## 6. ❌ Mutating Collections During Iteration

### The Problem

Modifying a collection while iterating causes:
- `InvalidOperationException`
- Skipped items
- Undefined behavior

### ❌ Bad Code

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };

foreach (var num in numbers)
{
    if (num % 2 == 0)
        numbers.Remove(num);  // ❌ CRASH or wrong results!
}
```

### ✅ Solution 1: Collect, Then Modify

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };

var toRemove = numbers.Where(n => n % 2 == 0).ToList();
foreach (var num in toRemove)
{
    numbers.Remove(num);  // ✅ Safe
}
```

### ✅ Solution 2: Use LINQ

```csharp
var odd = numbers.Where(n => n % 2 != 0).ToList();
```

### ✅ Solution 3: Iterate Backwards

```csharp
for (int i = numbers.Count - 1; i >= 0; i--)
{
    if (numbers[i] % 2 == 0)
        numbers.RemoveAt(i);  // ✅ Safe when going backwards
}
```

---

## 7. ❌ Mixing `==` with Reference Types (Classes)

### The Problem

`==` checks **reference equality** by default (points to same memory), not **value equality**

```csharp
var person1 = new Person { Name = "Alice", Age = 30 };
var person2 = new Person { Name = "Alice", Age = 30 };

if (person1 == person2)  // ❌ False! Different objects
    Console.WriteLine("Same person");
else
    Console.WriteLine("Different people");
```

### ✅ Solution 1: Override `Equals()` and `GetHashCode()`

```csharp
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Person other) return false;
        return Name == other.Name && Age == other.Age;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Age);
    }
}

var person1 = new Person { Name = "Alice", Age = 30 };
var person2 = new Person { Name = "Alice", Age = 30 };

if (person1 == person2)  // ✅ True!
    Console.WriteLine("Same person");
```

### ✅ Solution 2: Use `record` (C# 9+)

```csharp
record Person(string Name, int Age);

var person1 = new Person("Alice", 30);
var person2 = new Person("Alice", 30);

if (person1 == person2)  // ✅ True! Auto-implemented
    Console.WriteLine("Same person");
```

---

## 8. ❌ Not Understanding Boxing/Unboxing Overhead

### The Problem

Boxing (value type → object) creates **heap allocations** and performance cost:

```csharp
int num = 42;
object boxed = num;      // ❌ Boxes: heap allocation + copy
int unboxed = (int)boxed; // ❌ Unboxes: copy back to stack
```

### ❌ Problematic Code

```csharp
// Boxing happens implicitly!
ArrayList list = new ArrayList();
list.Add(42);        // ❌ Boxes integer
list.Add("text");    
foreach (var item in list)
    Console.WriteLine(item);  // Mixed types, multiple boxing/unboxing
```

### ✅ Solution 1: Use Generics (Best)

```csharp
var list = new List<int> { 42, 100, 200 };
// ❌ No boxing!
```

### ✅ Solution 2: Use Specific Types

```csharp
var intList = new List<int>();
var stringList = new List<string>();
// ✅ No boxing, type-safe
```

### When Boxing Happens

```csharp
// Implicit boxing
object obj = 42;                    // ❌ Boxing
int? nullable = 42;                 // ❌ Boxing (to wrap)

// Explicit boxing
object boxed = (object)42;          // ❌ Boxing

// Collections without generics
ArrayList list = new ArrayList();
list.Add(42);                       // ❌ Boxing
```

---

## Quick Reference Checklist

### Before Committing Code, Check:

- [ ] No `dynamic` unless absolutely necessary?
- [ ] No string concatenation (`+`) in loops?
- [ ] Nulls handled properly (`?.`, `??`)?
- [ ] Efficient iteration patterns used?
- [ ] `readonly` vs `const` applied correctly?
- [ ] No collection mutations during iteration?
- [ ] `Equals()` / `GetHashCode()` overridden for classes?
- [ ] Generics used instead of `ArrayList` / non-generic collections?

---

## Reading Order

1. Start here (this file)
2. Then read: `02-variables-types.md`
3. Then read: `03-operators-control-flow.md`
4. Refer back anytime you're unsure

---

## Additional Resources

- [C# Pitfalls Guide](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [String Performance](https://learn.microsoft.com/en-us/dotnet/standard/base-types/stringbuilder)
- [Nullable Reference Types](https://learn.microsoft.com/en-us/dotnet/csharp/concepts/nullable-reference-types)
