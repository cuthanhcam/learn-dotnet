# Variables & Types

Understanding C#'s type system is fundamental.

## Primitive Types

C# has value types that are allocated on the stack:

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

Reference types are allocated on the heap and accessed by reference:

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
value = value.GetType();   // String.GetType() called, works

value = something_undefined;  // Only fails at runtime!
```

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
int small = (int)big;  // May overflow
```

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

- Value types (int, bool, double) live on the stack
- Reference types (string, object, arrays) live on the heap
- var provides type inference; compiler still knows the type
- dynamic bypasses compile-time checking; avoid in most cases
- Explicit casts required for narrowing conversions
- Nullable value types use ? syntax
- const is compile-time; readonly can be runtime
- Static fields are shared across all instances of a type
