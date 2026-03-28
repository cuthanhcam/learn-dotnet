# Null Safety & Nullability

Understanding and handling null values safely.

## The Null Problem

NullReferenceException is the "billion-dollar mistake":

```csharp
string text = GetValue();  // Returns null
int length = text.Length;  // CRASH - NullReferenceException
```

## Nullable Reference Types (C# 8+)

Enable nullability checking in project:

In .csproj:
```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

Or in file:
```csharp
#nullable enable
```

Usage:
```csharp
string notNull = "text";        // Cannot be null
string? maybeNull = null;       // Can be null

// Compiler errors
string x = null;                // ERROR: null assigned to non-nullable
string? y = null;               // OK
y = GetValue();                 // GetValue() must return string?
```

## Safe Member Access (?.)

Safe null checking:

```csharp
string? text = GetValue();

// Without ?. - could crash if null
if (text != null)
{
    int length = text.Length;
}

// With ?. - returns null if text is null
int? length = text?.Length;
```

More examples:
```csharp
person?.GetName();              // Returns null if person is null
list?[0];                       // Returns null if list is null
obj?.Method?.SubProperty;        // Returns null at first null
```

## Null Coalescing Operator (??)

Use default value if null:

```csharp
string? name = GetName();
string display = name ?? "Unknown";  // "Unknown" if name is null

// Chaining
string value = option1 ?? option2 ?? option3 ?? "default";
```

Null coalescing assignment (??=):
```csharp
string? name = null;
name ??= "Default";  // Assigns if null
// name is now "Default"

name ??= "Other";    // No effect, name already set
// name still "Default"
```

## Pattern Matching for Null

Modern null checks:

```csharp
string? text = GetValue();

// is null
if (text is null)
    Console.WriteLine("null value");

// is not null
if (text is not null)
    Console.WriteLine(text.Length);  // Safe - compiler knows not null

// Traditional null check still works
if (text != null)
    Console.WriteLine(text.Length);
```

With switch:
```csharp
return obj switch
{
    null => "empty",
    string s => s,
    int i => i.ToString(),
    _ => "unknown"
};
```

## Nullable Value Types

Value types (int, bool, double) can't be null without ?:

```csharp
int x = null;        // ERROR
int? x = null;       // OK

int? value = 42;
int result = value.Value;           // Gets value, throws if null
int result2 = value.GetValueOrDefault();  // Returns 0 if null
int result3 = value.GetValueOrDefault(10);  // Returns 10 if null
```

Checking for value:
```csharp
int? age = GetAge();

if (age.HasValue)
{
    Console.WriteLine($"Age: {age.Value}");
}

if (age != null)
{
    Console.WriteLine($"Age: {age}");  // Implicit unwrap
}
```

## Null Conditional Operators

Combining various operators:

```csharp
Person? person = GetPerson();

// Get name safely
string? name = person?.Name;       // Returns null if person is null

// Call method safely
person?.UpdateProfile();            // Does nothing if person is null

// Invoke delegate safely
Action? action = GetAction();
action?.Invoke();                   // Only calls if not null

// Access collection safely
int? count = list?[0];             // Null if list null
```

## DefaultIfNull Extension

Simple default value helper:

```csharp
using System.Diagnostics.CodeAnalysis;

string? text = GetValue();
text = text.DefaultIfNull("Unknown");  // C# 11+

// Manual equivalent
text = text ?? "Unknown";
```

## Guarding Against Null

Early returns prevent null issues:

```csharp
public void ProcessUser(User? user)
{
    if (user is null)
        return;  // Exit early
    
    // Safe to use user
    Console.WriteLine(user.Name);
}
```

Null coalescing in method returns:
```csharp
public string GetUserName(User? user)
{
    return user?.Name ?? "Anonymous";
}
```

## ArgumentNullException

Validate method arguments:

```csharp
public class UserService
{
    private List<User> users;
    
    public void AddUser(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));
        
        users.Add(user);
    }
    
    // Or using ArgumentNullException.ThrowIfNull (C# 11+)
    public void AddUserModern(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        users.Add(user);
    }
}
```

## Dammit Operator (!)

Force null-forgiving:

```csharp
string? text = GetValue();
int length = text!.Length;  // Tells compiler: trust me, not null

// Use sparingly - you're telling compiler to ignore warnings
```

Only use when:
- You know better than the compiler
- Working with legacy non-nullable code
- Performance-critical path where you've verified safety

## Nullable Annotations

Providing nullability info:

```csharp
#nullable enable

// This method requires non-null argument
public void Process(string input)
{
    // input cannot be null
}

// This method accepts null
public void Process(string? input)
{
    // input can be null
}

// Return value can be null
public string? GetName()
{
    return null;  // OK
}

// Return value cannot be null
public string GetName()
{
    return null;  // ERROR (if enabled)
}
```

## Stack vs Heap (Brief Overview)

Value types allocated on stack:
```csharp
int x = 10;        // Stack: fixed location, immediate access
struct Point { int x; int y; }
Point p = new Point();  // Stack

// Can't be null
int? nullable = null;  // Wrapped in Nullable<int>
```

Reference types allocated on heap:
```csharp
string text = "Hello";  // Heap: variable location, accessed via reference
Person person = new Person();  // Heap
List<int> list = new List<int>();  // Heap

// Can be null
string? maybeNull = null;
```

---

## Key Takeaways

- Enable nullable reference types for compile-time checks
- Use ?. for safe member access
- Use ?? for default values
- Use is null / is not null for null checks
- Validate parameters with ArgumentNullException
- Pattern matching provides powerful null handling
- Avoid ! operator unless you know better than compiler
- Nullable value types wrapped in Nullable<T>
