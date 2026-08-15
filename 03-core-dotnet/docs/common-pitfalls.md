---
title: "Common Core .NET Pitfalls"
description: "Frequent Standard Library mistakes, observable consequences, and safer alternatives."
phase: 3
order: 11
topics: [dotnet, pitfalls, debugging]
---

# ⚠️ Common Pitfalls: What to Avoid

This document summarizes common mistakes across all Core .NET topics and how to avoid them.

## Collections Pitfalls

### 1. Modifying Collection During Iteration

```csharp
// ❌ WRONG
var list = new List<int> { 1, 2, 3 };
foreach (var item in list)
{
    if (item == 2)
        list.Remove(item); // InvalidOperationException
}

// ✅ CORRECT
list.RemoveAll(x => x == 2);
// OR
var toRemove = list.Where(x => x == 2).ToList();
foreach (var item in toRemove)
    list.Remove(item);
```

### 2. Wrong Collection Type for Scenario

```csharp
// ❌ BAD - Using List for frequent lookups
var list = new List<User>();
var user = list.FirstOrDefault(u => u.Id == searchId); // O(n)

// ✅ GOOD - Dictionary for ID lookup
var dict = new Dictionary<int, User>();
var user = dict.TryGetValue(searchId, out var u) ? u : null; // O(1)
```

### 3. Mutable Dictionary Keys

```csharp
// ❌ WRONG
var dict = new Dictionary<User, int>();
var user = new User { Name = "Alice" };
dict[user] = 1;
user.Name = "Bob"; // Dictionary now broken!

// ✅ CORRECT - Use immutable keys or override GetHashCode/Equals
```

## Generics Pitfalls

### 1. Over-Constraining Generic Types

```csharp
// ❌ BAD - Too restrictive
public T Create<T>() where T : class, new()
{
    return new T();
}

// ✅ GOOD - Only constrain what's necessary
public T Create<T>() where T : new()
{
    return new T();
}
```

### 2. Forgetting Where Clause

```csharp
// ❌ WRONG - Won't compile
public class Repository<T>
{
    public T Create() => new T(); // T might not have parameterless ctor
}

// ✅ CORRECT
public class Repository<T> where T : new()
{
    public T Create() => new T();
}
```

## Exception Handling Pitfalls

### 1. Catching Too Broadly

```csharp
// ❌ BAD
try { /* ... */ }
catch (Exception) { } // Silently swallows everything

// ✅ GOOD
try { /* ... */ }
catch (ValidationException ex)
{
    logger.LogWarning(ex, "Validation failed");
}
catch (DataAccessException ex)
{
    logger.LogError(ex, "Database error");
    throw;
}
```

### 2. Losing Stack Trace

```csharp
// ❌ WRONG - Stack trace starts here
catch (Exception ex)
{
    throw ex;
}

// ✅ CORRECT - Preserves original stack trace
catch (Exception ex) when (ShouldRethrow(ex))
{
    throw;
}
```

### 3. Using Exceptions for Control Flow

```csharp
// ❌ BAD
try
{
    var id = int.Parse(input);
}
catch (FormatException)
{
    id = -1;
}

// ✅ GOOD
int id = int.TryParse(input, out var parsed) ? parsed : -1;
```

## LINQ Pitfalls

### 1. Multiple Enumerations

```csharp
// ❌ BAD
IEnumerable<User> filtered = GetUsers().Where(u => u.Active);
var count = filtered.Count();
foreach (var user in filtered) { } // Enumerates again!

// ✅ GOOD
var filtered = GetUsers().Where(u => u.Active).ToList();
var count = filtered.Count;
foreach (var user in filtered) { }
```

### 2. Deferred Execution Surprise

```csharp
// ❌ WRONG - Variable captured, changes affect query
var minAge = 18;
var adults = users.Where(u => u.Age >= minAge);
minAge = 21;
foreach (var user in adults) { } // Uses minAge=21!

// ✅ CORRECT - Materialize immediately
var minAge = 18;
var adults = users.Where(u => u.Age >= minAge).ToList();
minAge = 21;
```

### 3. Inefficient Projection

```csharp
// ❌ BAD - Projects all data, then filters
var names = users
    .Select(u => new { u.Name, u.Age, u.Salary, u.Department })
    .Where(x => x.Age > 30)
    .Select(x => x.Name);

// ✅ GOOD - Filter first, then project
var names = users
    .Where(u => u.Age > 30)
    .Select(u => u.Name);
```

## Delegates & Events Pitfalls

### 1. Memory Leaks from Not Unsubscribing

```csharp
// ❌ WRONG
public class Form
{
    public Form()
    {
        button.Click += (s, e) => Process(); // Never unsubscribed
    }
    // Form kept in memory forever!
}

// ✅ CORRECT
public class Form : IDisposable
{
    public Form()
    {
        button.Click += Button_Click;
    }

    private void Button_Click(object? s, EventArgs e) => Process();

    public void Dispose()
    {
        button.Click -= Button_Click;
    }
}
```

### 2. Exception in Event Handler Stops Others

```csharp
// ❌ BAD - First exception stops remaining handlers
OnSomethingHappened?.Invoke(this, EventArgs.Empty);

// ✅ GOOD
var handlers = OnSomethingHappened;
if (handlers != null)
{
    foreach (EventHandler handler in handlers.GetInvocationList())
    {
        try
        {
            handler(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Handler failed");
        }
    }
}
```

## File I/O Pitfalls

### 1. Not Disposing Streams

```csharp
// ❌ WRONG
var stream = File.OpenRead("file.txt"); // Never disposed
var content = stream.ReadToEnd();

// ✅ CORRECT
using (var stream = File.OpenRead("file.txt"))
{
    var content = stream.ReadToEnd();
}

// OR
using var stream = File.OpenRead("file.txt");
```

### 2. Loading Huge Files in Memory

```csharp
// ❌ BAD
var allLines = File.ReadAllLines("huge.txt"); // OOM possible

// ✅ GOOD
foreach (var line in File.ReadLines("huge.txt"))
{
    ProcessLine(line);
}
```

### 3. Hard-Coded Paths

```csharp
// ❌ BAD - Not portable
var path = @"C:\Users\John\Documents\file.txt";

// ✅ GOOD
var path = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "file.txt"
);
```

## DateTime Pitfalls

### 1. Mixing UTC and Local

```csharp
// ❌ WRONG
var local = DateTime.Now;
var utc = DateTime.UtcNow;
if (local > utc) { } // Nonsensical comparison

// ✅ CORRECT
var local = DateTime.Now;
var utc = local.ToUniversalTime();
```

### 2. Ignoring DST

```csharp
// ❌ BAD - Doesn't consider DST
var date = new DateTime(2024, 3, 10, 12, 0, 0);
var plus2 = date.AddHours(2); // Not really 2 hours later!

// ✅ GOOD
var tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
var date = DateTime.Parse("2024-03-10 12:00:00");
var converted = TimeZoneInfo.ConvertTime(date.AddHours(2), tz);
```

### 3. Parse Without TryParse

```csharp
// ❌ RISKY
var date = DateTime.Parse(userInput); // Throws if invalid

// ✅ SAFE
if (DateTime.TryParse(userInput, out var date))
{
    UseDate(date);
}
```

## Attributes Pitfalls

### 1. Expensive Reflection on Hot Paths

```csharp
// ❌ BAD - Reflection on every call
public void Process(object item)
{
    var attrs = item.GetType().GetCustomAttributes(); // Expensive!
}

// ✅ GOOD - Cache results
private static readonly Dictionary<Type, object[]> _cache = new();

public object[] GetAttributes(Type type)
{
    if (!_cache.ContainsKey(type))
        _cache[type] = type.GetCustomAttributes();
    return _cache[type];
}
```

### 2. Attributes Not Inherited

```csharp
// ❌ WRONG
[Obsolete]
public class Base { }

public class Derived : Base { }
// Derived not marked as Obsolete

// ✅ CORRECT
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class ObsoleteAttribute : Attribute { }
```

## Nullable Reference Types Pitfalls

### 1. Ignoring Compiler Warnings

```csharp
// ❌ WRONG - Not initialized
#nullable enable
public class User
{
    public string Name { get; set; } // Warning!
}

// ✅ CORRECT
public class User
{
    public string Name { get; set; } = string.Empty;
}
```

### 2. Overusing Null-Forgiving Operator

```csharp
// ❌ BAD
string name = GetName()!.Trim()!.ToUpper()!;

// ✅ GOOD
var tempName = GetName();
if (tempName != null)
{
    string name = tempName.Trim().ToUpper();
}
```

### 3. Trusting External Libraries

```csharp
// ❌ BAD - Library might return null
var result = _externalApi.GetData()!;

// ✅ GOOD
var result = _externalApi.GetData();
if (result != null)
{
    UseData(result);
}
```

## General Best Practices

1. **Read Documentation** - Understand how classes behave
2. **Test Edge Cases** - Null values, empty collections, boundary conditions
3. **Use Static Analysis** - Enable compiler warnings
4. **Profile Performance** - Don't assume, measure
5. **Defensive Coding** - Assume inputs are invalid
6. **Consistent Naming** - Use `Async` suffix, nullable annotations
7. **Document Assumptions** - Make contracts clear
8. **Review Code** - Catch issues before production
9. **Keep It Simple** - Complex code has more bugs
10. **Learn from Mistakes** - This document is based on real issues

## Cross-Topic Pattern

### Problem: Null Reference Exception

**Root Causes Across Topics:**

- Collections: Accessing null element
- Generics: Unconstrained T could be null
- LINQ: Null in source sequence
- DateTime: Using DateTime.MinValue
- Attributes: Reflection returns null
- Events: Invoking null delegate
- File I/O: File doesn't exist

**Solution:**

```csharp
// ✅ Defensive null checks
if (value != null)
{
    UseValue(value);
}

// ✅ Null coalescing
var result = value ?? DefaultValue();

// ✅ Null conditional
var length = value?.ToString()?.Length;
```

## Key Takeaways

- Understand each feature's limitations
- Test edge cases thoroughly
- Use compiler warnings as guidance
- Profile before optimizing
- Document non-obvious behavior
- Handle errors explicitly
- Review code for common pitfalls
- Learn from others' mistakes
- Keep implementations simple
- Measure impact of changes
