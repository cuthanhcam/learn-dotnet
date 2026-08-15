---
title: "Collections"
description: "Arrays, foundational generic collections, and practical selection criteria."
phase: 1
order: 5
topics: [csharp, collections]
---

# Collections

C# provides various collection types for organizing data.

## Arrays

Fixed-size, zero-indexed:

```csharp
// Declaration and initialization
int[] numbers = new int[5];        // Array of 5 integers (all 0)
int[] values = { 1, 2, 3, 4, 5 }; // Initialize with values
string[] names = new[] { "Alice", "Bob", "Charlie" };

// Access
int first = numbers[0];
numbers[0] = 10;

// Length
int length = numbers.Length;

// Iterate
foreach (int n in numbers)
    Console.WriteLine(n);

for (int i = 0; i < numbers.Length; i++)
    Console.WriteLine(numbers[i]);
```

## Jagged Arrays

Arrays of arrays:

```csharp
int[][] matrix = new int[3][];    // 3 rows
matrix[0] = new int[2];           // Row 0 has 2 columns
matrix[1] = new int[3];           // Row 1 has 3 columns
matrix[2] = new int[2];           // Row 2 has 2 columns

matrix[0][0] = 10;
matrix[1][2] = 20;

// Multidimensional arrays (fixed columns)
int[,] grid = new int[3, 3];      // 3x3 grid
grid[0, 0] = 1;
grid[1, 1] = 2;
```

## List<T>

Dynamic array (growable):

```csharp
var numbers = new List<int>();
numbers.Add(1);
numbers.Add(2);
numbers.Add(3);

// Initialize with collection initializer
var names = new List<string> { "Alice", "Bob", "Charlie" };

// Access
var first = names[0];
names[0] = "Alexander";

// Add/Remove
names.Add("Diana");
names.Remove("Bob");
names.RemoveAt(0);
names.Clear();

// Iteration
foreach (var name in names)
    Console.WriteLine(name);

//Properties & methods
int count = names.Count;
bool contains = names.Contains("Alice");
int index = names.IndexOf("Alice");
names.Insert(1, "Eve");  // Insert at position
names.Sort();
names.Reverse();
```

## Dictionary<TKey, TValue>

Key-value pairs:

```csharp
var ages = new Dictionary<string, int>();
ages["Alice"] = 30;
ages["Bob"] = 25;
ages.Add("Charlie", 28);

// Initialize
var scores = new Dictionary<string, int>
{
    { "Alice", 95 },
    { "Bob", 87 },
    { "Charlie", 92 }
};

// Access
int aliceAge = ages["Alice"];
int unknownAge = ages.GetValueOrDefault("Unknown", 0);  // Default if key missing

// Check existence
if (ages.ContainsKey("Alice"))
    Console.WriteLine(ages["Alice"]);

if (ages.TryGetValue("Alice", out int age))
    Console.WriteLine(age);

// Iterate
foreach (var kvp in ages)
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");

foreach (string key in ages.Keys)
    Console.WriteLine(key);

foreach (int value in ages.Values)
    Console.WriteLine(value);

// Methods
ages.Remove("Bob");
ages.Clear();
int count = ages.Count;
```

## HashSet<T>

Unique values only:

```csharp
var numbers = new HashSet<int> { 1, 2, 3, 2, 1 };
// Contains: 1, 2, 3 (duplicates removed)

numbers.Add(4);      // Success
numbers.Add(1);      // No effect (already exists)

bool contains = numbers.Contains(2);  // true

// Set operations
var a = new HashSet<int> { 1, 2, 3 };
var b = new HashSet<int> { 2, 3, 4 };

a.UnionWith(b);      // a: 1, 2, 3, 4
a.IntersectWith(b);  // Keep only common (2, 3)
a.ExceptWith(b);     // Remove common (1)
```

## IEnumerable<T>

Base interface for iterating collections:

```csharp
IEnumerable<int> GetNumbers()
{
    return new[] { 1, 2, 3, 4, 5 };
}

IEnumerable<int> GetNumbersLazy()
{
    yield return 1;
    yield return 2;
    yield return 3;
}

// Can iterate with foreach
foreach (int n in GetNumbers())
    Console.WriteLine(n);
```

## Choosing the Right Collection

| Collection | Use Case | Order | Performance |
|------------|----------|-------|-----------|
| Array[] | Fixed size, fast access | Preserved | O(1) access |
| List<T> | Growing collection | Preserved | O(1) access, O(n) insert/remove |
| Dictionary<K,V> | Key-value lookup | Not ordered | O(1) average |
| HashSet<T> | Unique values | Not ordered | O(1) average |
| Queue<T> | FIFO operations | FIFO | O(1) enqueue/dequeue |
| Stack<T> | LIFO operations | LIFO | O(1) push/pop |

## Iteration Methods

Standard for loop:
```csharp
for (int i = 0; i < list.Count; i++)
    Console.WriteLine(list[i]);
```

foreach loop (recommended):
```csharp
foreach (var item in list)
    Console.WriteLine(item);
```

while loop (for specific conditions):
```csharp
using var enumerator = list.GetEnumerator();
while (enumerator.MoveNext())
    Console.WriteLine(enumerator.Current);
```

## Common Mistakes

Modifying collection during iteration:
```csharp
var list = new List<int> { 1, 2, 3, 4, 5 };

// WRONG - modifies list during iteration
foreach (var item in list)
{
    if (item % 2 == 0)
        list.Remove(item);  // Exception!
}

// RIGHT - collect what to remove first
var toRemove = list.Where(x => x % 2 == 0).ToList();
foreach (var item in toRemove)
    list.Remove(item);
```

Array vs List performance:
```csharp
// For fixed size - use Array (faster, more memory efficient)
int[] arr = new int[1000];

// For dynamic size - use List<T>
var list = new List<int>();

// Don't use ArrayList (legacy) - no generic type checking
// ArrayList list = new ArrayList();  // Avoid!
```

## LINQ Extensions

With using System.Linq:

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

var evens = numbers.Where(n => n % 2 == 0);       // Filter
var doubled = numbers.Select(n => n * 2);         // Transform
var count = numbers.Count(n => n > 5);             // Count matching
var sum = numbers.Sum();                           // Sum all
var first = numbers.First(n => n > 5);            // Get first matching
var ordered = numbers.OrderByDescending(n => n);  // Sort
```

---

## Key Takeaways

- Arrays are fixed-size, efficient for sequential access
- List<T> is dynamic and most commonly used
- Dictionary<K,V> for key-value lookups
- HashSet<T> for unique values and set operations
- Choose collection type based on usage pattern
- Avoid modifying collections during iteration
- LINQ provides powerful filtering and transformation
