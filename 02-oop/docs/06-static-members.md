---
title: "Static Members"
description: "Static state, methods, properties, constructors, initialization, and testability trade-offs."
phase: 2
order: 6
topics: [csharp, static-members]
---

# Static Members

Static members belong to the class itself, not to any object instance.

## Static Fields
```csharp
public class Counter
{
	public static int Count = 0;
}
Counter.Count++;
```

## Static Methods
```csharp
public static class MathUtils
{
	public static int Add(int a, int b) => a + b;
}
int sum = MathUtils.Add(2, 3);
```

## Static Properties
```csharp
public class AppConfig
{
	public static string Version { get; set; } = "1.0";
}
```

## Static Constructors
Used to initialize static data:
```csharp
public class Logger
{
	static Logger() { /* runs once */ }
}
```

## Practice Exercise

**Task:**
1. Create a static class with a static method and property.
2. Use a static constructor to initialize data.

## Interview Questions
- What is a static member? When would you use one?
- Can you have static constructors? What are they for?

## Pro Tips
- Use static for utility/helper methods.
- Avoid static state unless necessary (can cause issues in multithreaded code).
