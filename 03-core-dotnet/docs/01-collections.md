---
title: "Collections in .NET"
description: "Collection interfaces, concrete data structures, equality, complexity, and selection trade-offs."
phase: 3
order: 1
topics: [dotnet, collections]
---

# 📦 Collections: Lists, Dictionaries, and Sets

## Overview

Collections are fundamental to .NET programming. This section covers generic collections, their performance characteristics, and when to use each type.

## Table of Contents

1. [Collection Interfaces](#collection-interfaces)
2. [List<T> - Ordered Collections](#listt---ordered-collections)
3. [Dictionary<TKey, TValue> - Key-Value Pairs](#dictionarytkey-tvalue---key-value-pairs)
4. [HashSet<T> - Unique Elements](#hashsett---unique-elements)
5. [Choosing Collections](#choosing-collections)
6. [Performance Characteristics](#performance-characteristics)
7. [Custom Collections](#custom-collections)

## Collection Interfaces

### IEnumerable<T>

Base interface for all collections - provides iteration capabilities.

```csharp
IEnumerable<int> numbers = new List<int> { 1, 2, 3 };
foreach (var num in numbers) { /* ... */ }
```

### ICollection<T>

Extends `IEnumerable<T>` - adds count, add, remove capabilities.

```csharp
ICollection<int> numbers = new List<int>();
numbers.Add(1);
numbers.Remove(1);
int count = numbers.Count;
```

### IList<T>

Extends `ICollection<T>` - adds indexed access.

```csharp
IList<int> numbers = new List<int> { 1, 2, 3 };
int first = numbers[0];
numbers.Insert(1, 5);
```

### IDictionary<TKey, TValue>

Key-value collection interface.

```csharp
IDictionary<string, int> ages = new Dictionary<string, int>();
ages.Add("Alice", 30);
if (ages.TryGetValue("Alice", out var age)) { /* ... */ }
```

## List<T> - Ordered Collections

### Characteristics

- ✅ Ordered (maintains insertion order)
- ✅ Allows duplicates
- ✅ Zero-based indexing
- ✅ Dynamic sizing
- ✅ O(1) access by index
- ❌ O(n) insert/remove in middle

### Common Operations

```csharp
var list = new List<int> { 1, 2, 3 };

// Access
int first = list[0];

// Add
list.Add(4);
list.AddRange(new[] { 5, 6 });

// Insert
list.Insert(0, 0);

// Remove
list.Remove(4);
list.RemoveAt(0);

// Find
int index = list.IndexOf(2);
bool contains = list.Contains(3);

// Capacity management
list.Capacity = 100; // Pre-allocate space
list.TrimExcess(); // Reduce to actual size
```

### Performance Tips

- Pre-allocate capacity if you know the size
- Use `AddRange` instead of repeated `Add`
- `RemoveAt` is faster than `Remove` if you know the index

## Dictionary<TKey, TValue> - Key-Value Pairs

### Characteristics

- ✅ Unordered (hash-based lookup)
- ✅ Fast O(1) average lookup
- ✅ Keys must be unique
- ✅ Values can be duplicates
- ❌ No indexed access (unless using extension methods)

### Common Operations

```csharp
var ages = new Dictionary<string, int>
{
    { "Alice", 30 },
    { "Bob", 25 }
};

// Add
ages.Add("Charlie", 35);
ages["Diana"] = 28; // Updates if exists

// Access
int aliceAge = ages["Alice"]; // Throws KeyNotFoundException
if (ages.TryGetValue("Alice", out var age)) { /* ... */ }

// Remove
ages.Remove("Bob");
ages.Clear();

// Iterate
foreach (var kvp in ages)
{
    string name = kvp.Key;
    int age = kvp.Value;
}

// Keys and Values collections
var names = ages.Keys;
var allAges = ages.Values;
```

### Custom Key Types

For custom objects as keys, implement `GetHashCode()` and `Equals()`:

```csharp
public class PersonKey
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public override int GetHashCode()
        => HashCode.Combine(FirstName, LastName);

    public override bool Equals(object? obj)
        => obj is PersonKey pk
            && pk.FirstName == FirstName
            && pk.LastName == LastName;
}

var people = new Dictionary<PersonKey, int>();
```

## HashSet<T> - Unique Elements

### Characteristics

- ✅ Unordered
- ✅ Only unique elements (duplicates ignored)
- ✅ O(1) average add/remove/lookup
- ✅ Set operations (Union, Intersect, Difference)

### Common Operations

```csharp
var numbers = new HashSet<int> { 1, 2, 3 };

// Add
numbers.Add(4);
bool added = numbers.Add(2); // Returns false (duplicate)

// Remove
numbers.Remove(1);
numbers.Clear();

// Contains
bool has3 = numbers.Contains(3);

// Set operations
var set1 = new HashSet<int> { 1, 2, 3 };
var set2 = new HashSet<int> { 2, 3, 4 };

set1.UnionWith(set2);        // {1, 2, 3, 4}
set1.IntersectWith(set2);    // {2, 3}
set1.ExceptWith(set2);       // Remove common elements
set1.SymmetricExceptWith(set2); // Keep only different elements
```

## Choosing Collections

| Collection                 | Use Case                               | Performance                            |
| -------------------------- | -------------------------------------- | -------------------------------------- |
| `List<T>`                  | Ordered data, frequent access by index | O(1) access, O(n) insert/remove middle |
| `Dictionary<TKey, TValue>` | Fast key-based lookup                  | O(1) lookup                            |
| `HashSet<T>`               | Unique elements, set operations        | O(1) add/remove                        |
| `LinkedList<T>`            | Frequent insert/remove at ends         | O(1) at ends                           |
| `Queue<T>`                 | FIFO operations                        | O(1) enqueue/dequeue                   |
| `Stack<T>`                 | LIFO operations                        | O(1) push/pop                          |
| `SortedList<T>`            | Sorted key-value pairs                 | O(n) insert, O(1) access               |
| `SortedSet<T>`             | Sorted unique elements                 | O(log n) operations                    |

## Performance Characteristics

### Time Complexity

```
Operation        List    Dictionary  HashSet
Add              O(1)*   O(1)        O(1)
Remove           O(n)    O(1)        O(1)
Lookup by index  O(1)    N/A         N/A
Lookup by value  O(n)    O(1)**      O(1)
Contains         O(n)    O(1)        O(1)
Insert/Remove middle O(n) O(1)***   N/A

* O(n) if capacity exceeded
** Uses key lookup
*** Linked list operations only
```

### Memory Considerations

```csharp
// List over-allocates for growth
var list = new List<int>();
list.Capacity; // Growth factor: 1.5x-2x

// Dictionary has higher per-item overhead
// Each entry stores hash, key, value

// HashSet similar to Dictionary

// Use TrimExcess() to reduce memory
list.TrimExcess();
```

## Common Patterns

### Filtered Collection

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };
var evens = new HashSet<int>(numbers.Where(n => n % 2 == 0));
```

### Group By Key

```csharp
var people = new List<(string Name, int Age)>
{
    ("Alice", 30), ("Bob", 25), ("Charlie", 30)
};

var grouped = new Dictionary<int, List<string>>();
foreach (var person in people)
{
    if (!grouped.ContainsKey(person.Age))
        grouped[person.Age] = new();
    grouped[person.Age].Add(person.Name);
}
```

### Reverse Lookup

```csharp
var nameToId = new Dictionary<string, int> { { "Alice", 1 } };
var idToName = new Dictionary<int, string>();
foreach (var kvp in nameToId)
    idToName[kvp.Value] = kvp.Key;
```

## Custom Collections

Implement custom collections by extending `Collection<T>`:

```csharp
public class UniqueList<T> : Collection<T> where T : IComparable<T>
{
    protected override void InsertItem(int index, T item)
    {
        if (Items.Contains(item))
            throw new InvalidOperationException("Duplicate");
        base.InsertItem(index, item);
    }
}
```

## Best Practices

1. ✅ Use most specific interface (IList<T> vs IEnumerable<T>)
2. ✅ Pre-allocate List capacity if known
3. ✅ Use HashSet for uniqueness checks
4. ✅ Implement proper GetHashCode/Equals for custom keys
5. ✅ Consider memory when choosing collection types
6. ✅ Use appropriate LINQ operators for collection operations
7. ❌ Don't modify collections while iterating
8. ❌ Don't use Dictionary with mutable key types

## Common Pitfalls

### Pitfall 1: Modifying During Iteration

```csharp
// ❌ WRONG
var list = new List<int> { 1, 2, 3 };
foreach (var item in list)
    list.Remove(item); // InvalidOperationException

// ✅ CORRECT
list.RemoveAll(x => x > 1);
// OR
var toRemove = list.Where(x => x > 1).ToList();
foreach (var item in toRemove)
    list.Remove(item);
```

### Pitfall 2: Dictionary Key Not Immutable

```csharp
// ❌ WRONG - mutable key
public class Person { public string Name { get; set; } }
var dict = new Dictionary<Person, int>();
var key = new Person { Name = "Alice" };
dict.Add(key, 30);
key.Name = "Bob"; // Dictionary now broken

// ✅ CORRECT - immutable or override GetHashCode/Equals
```

### Pitfall 3: Inefficient List Operations

```csharp
// ❌ SLOW - O(n²)
var list = new List<int>();
foreach (var item in source)
    list.Add(item); // Each Add may reallocate

// ✅ FAST - pre-allocate
var list = new List<int>(source.Count);
list.AddRange(source);
```

## Key Takeaways

- Use `List<T>` for ordered collections with indexed access
- Use `Dictionary<TKey, TValue>` for fast key-based lookups
- Use `HashSet<T>` for unique elements and set operations
- Choose collection based on access patterns and performance needs
- Consider memory and performance implications
- Implement GetHashCode/Equals for custom key types
- Avoid modifying collections during enumeration
- Pre-allocate capacity when size is known
