---
title: "Operators and Control Flow"
description: "Operators, branching, loops, switch expressions, and pattern matching in C#."
phase: 1
order: 3
topics: [csharp, operators, control-flow]
---

# Operators & Control Flow

## Arithmetic Operators

```csharp
int a = 10, b = 3;

int sum = a + b;        // 13
int diff = a - b;       // 7
int product = a * b;    // 30
int quotient = a / b;   // 3 (integer division)
int remainder = a % b;  // 1

// Increment/decrement
a++;  // 11
a--;  // 10

// Compound assignment
a += 5;  // a = a + 5
a -= 5;  // a = a - 5
a *= 2;  // a = a * 2
a /= 2;  // a = a / 2
a %= 3;  // a = a % 3
```

## Comparison Operators

```csharp
int x = 10, y = 20;

bool equal = x == y;        // false
bool notEqual = x != y;     // true
bool less = x < y;          // true
bool lessOrEqual = x <= y;  // true
bool greater = x > y;       // false
bool greaterOrEqual = x >= y;  // false
```

## Logical Operators

```csharp
bool a = true, b = false;

bool and = a && b;  // false (both must be true)
bool or = a || b;   // true (at least one is true)
bool not = !a;      // false (negation)

// Short-circuit evaluation
if (x > 0 && y / x > 2)  // If x <= 0, second part isn't evaluated
    Console.WriteLine("OK");
```

## String Operators

Concatenation with +:
```csharp
string first = "Hello";
string second = "World";
string result = first + " " + second;  // "Hello World"
```

String interpolation (modern, preferred):
```csharp
string name = "Alice";
int age = 30;

string message = $"Name: {name}, Age: {age}";  // Interpolation
string formatted = $"Name: {name,10}";         // With padding
string calculation = $"Sum: {10 + 20}";        // Expressions allowed
```

## Null Coalescing Operators

```csharp
// ?? - Use right side if left is null
string? text = null;
string result = text ?? "default";  // "default"

// ??= - Assign right side only if left is null
text ??= "fallback";  // text now "fallback"

// ?. - Safe member access (returns null if object is null)
string? name = person?.GetName();
int? length = text?.Length;  // null if text is null
```

## Conditional (Ternary) Operator

```csharp
int age = 20;
string status = age >= 18 ? "Adult" : "Minor";

// Nested
string category = age switch
{
    >= 65 => "Senior",
    >= 18 => "Adult",
    >= 13 => "Teenager",
    _ => "Child"
};
```

## if / else if / else

```csharp
int score = 85;

if (score >= 90)
    Console.WriteLine("A");
else if (score >= 80)
    Console.WriteLine("B");
else if (score >= 70)
    Console.WriteLine("C");
else
    Console.WriteLine("F");
```

## switch Statement (Traditional)

```csharp
string day = "Monday";

switch (day)
{
    case "Monday":
    case "Tuesday":
    case "Wednesday":
    case "Thursday":
    case "Friday":
        Console.WriteLine("Weekday");
        break;
    case "Saturday":
    case "Sunday":
        Console.WriteLine("Weekend");
        break;
    default:
        Console.WriteLine("Unknown");
        break;
}
```

## switch Expression (Modern, C# 8+)

```csharp
string day = "Monday";

string dayType = day switch
{
    "Monday" or "Tuesday" or "Wednesday" or "Thursday" or "Friday" => "Weekday",
    "Saturday" or "Sunday" => "Weekend",
    _ => "Unknown"
};

// With conditions
string category = age switch
{
    < 0 => "Invalid",
    >= 0 and < 13 => "Child",
    >= 13 and < 18 => "Teen",
    >= 18 => "Adult"
};
```

## for Loop

```csharp
// Count from 0 to 9
for (int i = 0; i < 10; i++)
    Console.WriteLine(i);

// Reverse
for (int i = 10; i > 0; i--)
    Console.WriteLine(i);

// Decrement by 2
for (int i = 10; i > 0; i -= 2)
    Console.WriteLine(i);

// Multiple variables
for (int i = 0, j = 10; i < 10; i++, j--)
    Console.WriteLine($"{i}, {j}");
```

## while Loop

```csharp
int i = 0;
while (i < 10)
{
    Console.WriteLine(i);
    i++;
}

// Be careful of infinite loops
while (true)
{
    if (someCondition)
        break;
}
```

## do-while Loop

Executes body at least once:

```csharp
int input;

do
{
    Console.Write("Enter a number (1-10): ");
    input = int.Parse(Console.ReadLine());
} while (input < 1 || input > 10);
```

## foreach Loop

Iterates over collections:

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };

foreach (int num in numbers)
    Console.WriteLine(num);

// With IEnumerable
List<string> names = new() { "Alice", "Bob", "Charlie" };
foreach (var name in names)
    Console.WriteLine(name);

// With dictionaries
Dictionary<string, int> ages = new()
{
    { "Alice", 30 },
    { "Bob", 25 }
};

foreach (var kvp in ages)
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");

foreach (string name in ages.Keys)  // Just keys
    Console.WriteLine(name);
```

## break and continue

```csharp
// break - exit loop immediately
for (int i = 0; i < 10; i++)
{
    if (i == 5)
        break;  // Exits when i == 5
    Console.WriteLine(i);
}

// continue - skip to next iteration
for (int i = 0; i < 10; i++)
{
    if (i % 2 == 0)
        continue;  // Skip even numbers
    Console.WriteLine(i);  // Prints: 1, 3, 5, 7, 9
}
```

## Operator Precedence

Higher precedence evaluated first:

1. Postfix (i++, i--)
2. Unary (!x, +x, -x, ++x, --x)
3. Multiplicative (*, /, %)
4. Additive (+, -)
5. Relational (<, >, <=, >=)
6. Equality (==, !=)
7. Logical AND (&&)
8. Logical OR (||)
9. Conditional (?:)
10. Assignment (=, +=, -=, etc.)

Example:
```c#
int result = 2 + 3 * 4;  // 14, not 20 (multiplication first)
int result2 = (2 + 3) * 4;  // 20 (parentheses override)
```

## Pattern Matching

Modern C# provides rich pattern matching:

```csharp
// Type patterns
if (obj is string text)
    Console.WriteLine($"Text length: {text.Length}");

// Property patterns
if (person is { Age: >= 18, Name: not null })
    Console.WriteLine("Valid adult");

// List patterns (C# 9+)
if (numbers is [1, 2, 3])
    Console.WriteLine("Exact match");

if (numbers is [.. , 5])  // Ends with 5
    Console.WriteLine("Found");

// Relational patterns
string category = age switch
{
    < 0 => "Invalid",
    < 13 => "Child",
    < 18 => "Teen",
    _ => "Adult"
};
```

---

## Key Takeaways

- Operators follow standard precedence (use parentheses for clarity)
- Logical operators use short-circuit evaluation
- Use string interpolation for readability
- Null coalescing (??) and safe operators (?.) prevent null errors
- switch expressions (C# 8+) are preferred over switch statements
- foreach is preferred over for for collections
- Pattern matching provides powerful condition checking
