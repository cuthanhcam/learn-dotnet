---
title: "Variables and Types"
description: "The C# type system, type inference, dynamic binding, conversions, constants, and nullable value types."
slug: csharp-variables-and-types
phase: 1
order: 2
difficulty: beginner
article-type: tutorial
estimated-reading-minutes: 24
topics: [csharp, types, variables]
prerequisites: [dotnet-ecosystem-fundamentals]
status: maintained
last-reviewed: 2026-08-15
---

# Variables & Types

Understanding C#'s type system is fundamental.

## Primitive Types

C# has built-in value types. Value types have value-copy semantics; their storage
depends on context. A local may be held in a stack frame or register, while a
value-type field or array element can live inline inside a managed heap object:

```csharp
// Integer types
byte b = 255;              // 0-255
short s = 32767;           // -32,768 to 32,767
int i = 2147483647;        // -2,147,483,648 to 2,147,483,647
long l = 9223372036854775807L;  // Very large numbers

// Floating point
float f = 3.14f;           // Single precision
double d = 3.14159;        // Double precision (default)
decimal dec = 19.99m;      // Exact for financial calculations

// Logical
bool flag = true;

// Character
char c = 'A';
```

## Reference Types

Variables of reference type hold references. Class instances and arrays normally
live on the managed heap, while the variable containing the reference may be a
local, field, array element, or register:

```csharp
string text = "Hello";     // String
object obj = new object(); // Object (base type)
int[] arr = { 1, 2, 3 };   // Array
```

## var Keyword (Type Inference)

var lets the compiler infer the type:

```csharp
var age = 25;              // Inferred as int
var name = "Alice";        // Inferred as string
var prices = new List<decimal>();  // Inferred as List<decimal>

// var is resolved at compile-time, providing intellisense
var person = GetPerson();  // Type depends on what GetPerson() returns
```

When to use var:
- Type is obvious from the assignment
- Used with LINQ queries
- To reduce verbosity with complex generic types

When NOT to use var:
- When the inferred type isn't clear
- In public APIs (be explicit for clarity)

## dynamic Keyword

dynamic bypasses compile-time type checking:

```csharp
dynamic value = "text";
value = value.ToUpper();   // Works

value = 42;
Type runtimeType = value.GetType(); // Resolves against Int32 at runtime

value.MethodThatDoesNotExist(); // Compiles, then fails during runtime binding
```

`dynamic` does not make unknown local identifiers valid. `something_undefined` still fails to
compile because name lookup is separate from dynamic member binding.

Avoid dynamic in most cases. It:
- Disables intellisense
- Causes runtime errors
- Has performance overhead
- Makes debugging harder

Use dynamic only for:
- COM interop
- Reflection scenarios
- Calling dynamic language objects

## Type Conversion

Implicit conversion (safe, no data loss):
```csharp
int i = 100;
long l = i;           // Implicit: int fits in long

// Numeric widening
int smaller = 10;
double larger = smaller;  // int to double is safe
```

Explicit conversion (requires cast):
```csharp
double d = 3.14;
int i = (int)d;       // Explicit: loses decimal part, becomes 3

// Narrowing conversion - data loss possible
long big = 99999999999;
int small = checked((int)big);  // Throws OverflowException when out of Int32 range
```

An explicit cast states that narrowing is intentional; it does not by itself guarantee that the
value fits. In an unchecked context, high-order bits can be discarded. Use a checked cast when
overflow should be a visible failure, or compare against the destination boundaries for a `Try...`
contract.

```csharp
public static bool TryToInt32(long value, out int result)
{
    if (value is < int.MinValue or > int.MaxValue)
    {
        result = default;
        return false;
    }

    result = (int)value;
    return true;
}
```

### Parsing text is not casting

Parsing interprets a textual representation. The accepted decimal and group separators depend on
the selected culture and `NumberStyles`. Never assume the development machine's current culture is
the wire-format contract.

```csharp
CultureInfo german = CultureInfo.GetCultureInfo("de-DE");
bool valid = decimal.TryParse(
    "1.234,56",
    NumberStyles.Number,
    german,
    out decimal amount);
```

Use invariant culture for machine-controlled formats that define it. Use the user's intended culture
for human-entered values. Currency symbols, exponent notation, whitespace, and signs should be
accepted or rejected through an explicit style rather than by accident.

`TryParse` is appropriate when invalid input is expected. `Parse` is appropriate when invalid text
means the caller violated a contract and an exception is the intended outcome.

## Nullable Value Types

Value types can't be null by default:

```csharp
int x = null;  // ERROR

// But with ? they can be
int? x = null;         // Valid
int? y = 42;          // Valid

// Check if has value
if (x.HasValue)
    Console.WriteLine(x.Value);

// Using GetValueOrDefault()
int val = x.GetValueOrDefault(0);  // Returns 0 if null
```

## Constants

const for compile-time constants:

```csharp
const int MaxAttempts = 3;
const string CompanyName = "ACME";

// Must be evaluated at compile time
const DateTime now = DateTime.Now;  // ERROR - not compile-time!
```

readonly for compile-time or runtime:

```csharp
class Config
{
    public readonly int Timeout;
    public readonly string DbConnectionString;
    
    public Config(string connStr, int timeout)
    {
        // Can be set once from runtime values
        DbConnectionString = connStr;
        Timeout = timeout;
    }
}
```

## Static Variables

Static fields belong to the type, not instances:

```csharp
class Counter
{
    public static int Count = 0;  // Shared across all instances
    
    public Counter()
    {
        Count++;
    }
}

var c1 = new Counter();  // Count = 1
var c2 = new Counter();  // Count = 2
Console.WriteLine(Counter.Count);  // 2
```

## Default Values

Every type has a default value:

```csharp
int i = default;        // 0
bool b = default;       // false
string s = default;     // null
List<int> l = default;  // null
```

## Type Information at Runtime

Use typeof() and GetType():

```csharp
Type t = typeof(string);      // Get type at compile time
string name = t.Name;         // "String"

var obj = "text";
Type t2 = obj.GetType();      // Get type of instance at runtime
string name2 = t2.Name;       // "String"
```

## Naming Conventions

Follow C# conventions:

```csharp
// PascalCase for public types, properties, methods
public class PersonInfo { }
public string FirstName { get; set; }
public void GetUserData() { }

// camelCase for private fields, parameters
private string lastName;
public void SetName(string firstName) { }

// CONSTANT_CASE for constants
const int MAX_ATTEMPTS = 3;

// _camelCase for private fields (optional style)
private string _internalState;
```

---

## Key Takeaways

- Value types (`int`, `bool`, `double`, custom structs) use value-copy semantics
- Reference types (`string`, `object`, arrays, classes) use reference-copy semantics
- Storage location follows context and runtime optimization, not a simple type-category rule
- var provides type inference; compiler still knows the type
- dynamic bypasses compile-time checking; avoid in most cases
- Explicit casts required for narrowing conversions
- Use checked narrowing or a range-based `Try...` API when overflow must not be silent
- Parse text with an explicit culture and number-style contract at boundaries
- Nullable value types use ? syntax
- const is compile-time; readonly can be runtime
- Static fields are shared across all instances of a type

## Implementation and Test Map

| Concern | Source | Tests |
|---|---|---|
| Primitive values, inference, nullable values | `Variables/VariablesExample.cs` | `VariablesTests.cs` |
| Static typing versus runtime dynamic binding | `Variables/DynamicVsTypedExample.cs` | `VariablesTests.cs` |
| Culture-aware parsing and checked narrowing | `Variables/NumericConversionExample.cs` | `VariablesTests.cs` |

## Further Reading

- [Built-in numeric conversions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/numeric-conversions)
- [Checked and unchecked statements](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/checked-and-unchecked)
- [Parsing numeric strings](https://learn.microsoft.com/en-us/dotnet/standard/base-types/parsing-numeric)

## Continue Learning

- Previous: [.NET ecosystem](01-dotnet-ecosystem.md)
- Next: [Operators and control flow](03-operators-control-flow.md)
