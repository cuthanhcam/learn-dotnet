---
title: "Methods"
description: "Method declarations, parameters, ref/out/in, overloads, local functions, and recursion."
phase: 1
order: 4
topics: [csharp, methods, parameters]
---

# Methods

Methods are reusable blocks of code.

## Basic Method Declaration

```csharp
// Return type, name, parameters, body
public int Add(int a, int b)
{
    return a + b;
}

// Void - doesn't return a value
public void PrintMessage(string message)
{
    Console.WriteLine(message);
}

// Expression-bodied member (C# 6+)
public int Multiply(int a, int b) => a * b;

// With default return value
public bool IsPositive(int number) => number > 0;
```

## Parameters

Regular parameters (pass by value):

```csharp
void UpdateValue(int value)
{
    value = 100;  // Only affects the parameter, not the caller's variable
}

int x = 5;
UpdateValue(x);
Console.WriteLine(x);  // Still 5
```

## ref Parameter (Pass by Reference)

ref allows the method to modify the original variable:

```csharp
void Increment(ref int value)
{
    value++;  // Modifies the original
}

int x = 5;
Increment(ref x);
Console.WriteLine(x);  // 6
```

Use ref when:
- You need to modify the caller's variable
- Passing by reference for performance (avoids copying large structs)

## out Parameter

out requires the method to assign a value:

```csharp
// TryParse pattern
bool TryGetUser(int id, out User? user)
{
    if (UserDatabase.Contains(id))
    {
        user = UserDatabase.GetUser(id);
        return true;
    }
    
    user = null;
    return false;
}

if (TryGetUser(123, out var foundUser))
    Console.WriteLine(foundUser.Name);
```

Use out when:
- You need to return multiple values
- Following the TryXxx pattern

## in Parameter (C# 7.2+)

in passes by reference but read-only (for optimization):

```csharp
void ProcessStruct(in LargeStruct data)
{
    // Can read data, but cannot modify
    Console.WriteLine(data.Name);
    
    // This would error:
    // data.Name = "New";  // ERROR
}

struct LargeStruct
{
    public string Name { get; set; }
}

var s = new LargeStruct { Name = "Data" };
ProcessStruct(in s);
```

Use in when:
- Passing large structs (avoid copying)
- Caller should not worry about modification

## Optional Parameters

Parameters with default values:

```csharp
public void PrintMessage(string message, int times = 1, char separator = '-')
{
    for (int i = 0; i < times; i++)
        Console.WriteLine(separator + message + separator);
}

PrintMessage("Hello");                          // times=1, separator='-'
PrintMessage("Hello", 3);                       // times=3, separator='-'
PrintMessage("Hello", 2, '*');                  // All specified
PrintMessage("Hello", separator: '*', times: 2);  // Named arguments (any order)
```

Rules for optional parameters:
- Must come after required parameters
- Must have compile-time constant values (or new in C# 12+)

## Named Arguments

Call methods with parameter names:

```csharp
public void BookFlight(string destination, int seats, bool round_trip = false)
{
    // ...
}

// Traditional positional
BookFlight("Tokyo", 2, true);

// Named arguments (any order)
BookFlight(seats: 2, destination: "Tokyo", round_trip: true);
BookFlight(destination: "Tokyo", round_trip: true, seats: 2);

// Mix positional and named
BookFlight("Tokyo", round_trip: true, seats: 2);
```

## params Keyword

Variable number of arguments:

```csharp
public void PrintNumbers(params int[] numbers)
{
    foreach (int num in numbers)
        Console.WriteLine(num);
}

PrintNumbers(1);              // One argument
PrintNumbers(1, 2, 3);        // Multiple
PrintNumbers(1, 2, 3, 4, 5);  // Many

// params accepts collection too
int[] arr = { 1, 2, 3 };
PrintNumbers(arr);
```

## Method Overloading

Multiple methods with same name, different parameters:

```csharp
public void Print(int value)
    => Console.WriteLine($"Integer: {value}");

public void Print(double value)
    => Console.WriteLine($"Double: {value}");

public void Print(string value)
    => Console.WriteLine($"String: {value}");

public void Print(string value, int count)
    => Console.WriteLine($"String x{count}: {value}");

// Compiler chooses based on argument types
Print(42);          // Calls Print(int)
Print(3.14);        // Calls Print(double)
Print("Hello");     // Calls Print(string)
Print("Hi", 3);     // Calls Print(string, int)
```

## Return Values

Methods must return the declared type:

```csharp
public string GetUserName(int id)
{
    // Must return a string
    return database.GetUser(id).Name;
}

public int Calculate(int a, int b)
{
    return a + b;  // Must return int
}

// Multiple return points
public bool IsValidAge(int age)
{
    if (age < 0)
        return false;
    
    if (age > 150)
        return false;
    
    return true;
}
```

## Local Functions

Methods defined inside other methods:

```csharp
public void ProcessData(int[] data)
{
    int Sum(int[] arr)
    {
        int sum = 0;
        foreach (var item in arr)
            sum += item;
        return sum;
    }
    
    int total = Sum(data);
    Console.WriteLine($"Total: {total}");
}
```

Useful for:
- Helper methods used in one place only
- Recursive operations
- Keeping code organized

## Recursion

Methods calling themselves:

```csharp
public int Factorial(int n)
{
    if (n <= 1)
        return 1;
    
    return n * Factorial(n - 1);
}

// Call
int result = Factorial(5);  // 120
```

Be careful:
- Infinite recursion causes StackOverflowException
- Each call consumes stack memory
- Iterative approaches often better for performance

## Static Methods

Methods belonging to the type, not instances:

```csharp
public class MathHelper
{
    public static int Add(int a, int b)
        => a + b;
    
    public static int Subtract(int a, int b)
        => a - b;
}

// Call without creating instance
int sum = MathHelper.Add(5, 3);
int diff = MathHelper.Subtract(10, 3);
```

## Naming Conventions

```csharp
// PascalCase for public methods
public void GetUserData() { }
public string CalculateTotal() { }

// Verb-first for clarity
public void DoSomething() { }
public bool TryParseInput() { }
public IEnumerable GetUsers() { }
```

---

## Key Takeaways

- Methods should have single responsibility
- Use meaningful parameter names
- ref modifies original; out requires assignment; in is read-only
- Optional parameters reduce overload count
- params allows variable-length arguments
- Method overloading works on parameter count/types
- Local functions useful for one-off helpers
- Be cautious with recursion depth
