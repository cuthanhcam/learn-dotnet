---
title: "Attributes and Metadata"
description: "Built-in and custom attributes, reflection, metadata contracts, and runtime inspection costs."
phase: 3
order: 8
topics: [csharp, attributes, reflection]
---

# 🏷️ Attributes: Metadata and Reflection

## Overview

Attributes enable attaching metadata to code elements. This section covers built-in attributes, custom attributes, and reflection-based usage.

## Table of Contents

1. [Attribute Basics](#attribute-basics)
2. [Built-in Attributes](#built-in-attributes)
3. [Custom Attributes](#custom-attributes)
4. [Reflection-Based Usage](#reflection-based-usage)
5. [Advanced Patterns](#advanced-patterns)
6. [Best Practices](#best-practices)
7. [Common Pitfalls](#common-pitfalls)

## Attribute Basics

### What Are Attributes?

Attributes are declarative tags that provide metadata:

```csharp
// Using built-in Obsolete attribute
[Obsolete("Use NewMethod instead")]
public void OldMethod()
{
}

// Using multiple attributes
[Serializable]
[Obsolete]
public class MyClass
{
}
```

### Attribute Targets

```csharp
[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Method)]
public class MyAttribute : Attribute
{
}

// Valid targets:
// Class, Struct, Enum, Interface, Method, Property, Field,
// Event, Parameter, ReturnValue, Delegate, GenericParameter, Assembly, Module
```

## Built-in Attributes

### Obsolete

```csharp
[Obsolete("Use NewMethod instead", error: false)]
public void OldMethod()
{
}

// error: true means compiler error, false means warning
```

### Serializable

```csharp
[Serializable]
public class Person
{
    public string Name { get; set; }

    [NonSerialized]
    private string _password;
}
```

### Conditional

```csharp
#define DEBUG

[Conditional("DEBUG")]
public void DebugLog(string message)
{
    Console.WriteLine($"DEBUG: {message}");
}

// Call is removed if DEBUG is not defined
```

### Flags for Enums

```csharp
[Flags]
public enum Permissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4
}

// Allows combining: Permissions.Read | Permissions.Write
```

### CLSCompliant

```csharp
[assembly: System.CLSCompliant(true)]

[CLSCompliant(true)]
public class MyClass
{
    // CLS compliant code
}
```

### Attribute for Methods

```csharp
[System.Diagnostics.DebuggerStepThrough]
public void QuickMethod()
{
    // Debugger won't step into this
}

[System.Runtime.CompilerServices.CallerMemberName]
public void LogMethod(string? memberName = null)
{
    Console.WriteLine($"Called from {memberName}");
}
```

## Custom Attributes

### Simple Custom Attribute

```csharp
// Define the attribute
[AttributeUsage(AttributeTargets.Class)]
public class DescriptionAttribute : Attribute
{
    public string Description { get; }

    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}

// Use the attribute
[Description("This class represents a user")]
public class User
{
    public string Name { get; set; }
}
```

### Attribute with Properties

```csharp
[AttributeUsage(AttributeTargets.Method)]
public class LoggingAttribute : Attribute
{
    public string Level { get; set; } = "Info";
    public bool Async { get; set; } = false;
    public int Timeout { get; set; } = 5000;
}

// Usage with properties
[Logging(Level = "Debug", Async = true)]
public void ProcessData()
{
}
```

### Attribute with Named Parameters

```csharp
[AttributeUsage(AttributeTargets.Property)]
public class ValidationAttribute : Attribute
{
    public int MinLength { get; set; }
    public int MaxLength { get; set; }
    public string Pattern { get; set; }
}

// Usage
public class Product
{
    [Validation(MinLength = 1, MaxLength = 100, Pattern = @"^[a-zA-Z0-9]*$")]
    public string Name { get; set; }
}
```

### Multiple Usage Attribute

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorAttribute : Attribute
{
    public string Name { get; }
    public string Date { get; }

    public AuthorAttribute(string name, string date)
    {
        Name = name;
        Date = date;
    }
}

// Multiple uses
[Author("Alice", "2024-01-01")]
[Author("Bob", "2024-02-01")]
public class Document
{
}
```

## Reflection-Based Usage

### Reading Attributes

```csharp
public static void PrintClassAttributes(Type type)
{
    var attributes = type.GetCustomAttributes(inherit: false);

    foreach (var attr in attributes)
    {
        Console.WriteLine($"Attribute: {attr.GetType().Name}");
    }
}

// Usage
PrintClassAttributes(typeof(User));
```

### Accessing Attribute Properties

```csharp
public static void PrintAuthors(Type type)
{
    var authorAttributes = type.GetCustomAttributes<AuthorAttribute>(inherit: false);

    foreach (var author in authorAttributes)
    {
        Console.WriteLine($"Author: {author.Name} ({author.Date})");
    }
}

// Usage
PrintAuthors(typeof(Document));
```

### Method Attributes

```csharp
public static void FindLoggingMethods(Type type)
{
    var methods = type.GetMethods();

    foreach (var method in methods)
    {
        if (method.GetCustomAttribute<LoggingAttribute>() != null)
        {
            Console.WriteLine($"Logging method: {method.Name}");
        }
    }
}
```

### Property Validation via Attributes

```csharp
public static void ValidateProperties<T>(T obj) where T : class
{
    var properties = typeof(T).GetProperties();

    foreach (var prop in properties)
    {
        var validationAttr = prop.GetCustomAttribute<ValidationAttribute>();
        if (validationAttr != null)
        {
            var value = prop.GetValue(obj) as string;
            // Perform validation...
        }
    }
}
```

## Advanced Patterns

### Attribute-Driven Serialization

```csharp
[AttributeUsage(AttributeTargets.Property)]
public class JsonPropertyAttribute : Attribute
{
    public string Name { get; }
    public bool Ignore { get; set; }

    public JsonPropertyAttribute(string name)
    {
        Name = name;
    }
}

public static Dictionary<string, object?> ToJsonDict<T>(T obj) where T : class
{
    var dict = new Dictionary<string, object?>();

    foreach (var prop in typeof(T).GetProperties())
    {
        var jsonAttr = prop.GetCustomAttribute<JsonPropertyAttribute>();

        if (jsonAttr?.Ignore == true)
            continue;

        var key = jsonAttr?.Name ?? prop.Name;
        dict[key] = prop.GetValue(obj);
    }

    return dict;
}
```

### Aspect-Oriented Programming

```csharp
[AttributeUsage(AttributeTargets.Method)]
public class TimingAttribute : Attribute
{
    public void MeasureExecution(Action action, string methodName)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        action();
        sw.Stop();
        Console.WriteLine($"{methodName} took {sw.ElapsedMilliseconds}ms");
    }
}

// Usage would typically be with AOP frameworks like PostSharp
[Timing]
public void SlowMethod()
{
    System.Threading.Thread.Sleep(1000);
}
```

### Attribute-Based Configuration

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationAttribute : Attribute
{
    public string Section { get; }
    public string? DefaultFile { get; set; }

    public ConfigurationAttribute(string section)
    {
        Section = section;
    }
}

[Configuration("Database", DefaultFile = "appsettings.json")]
public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }
}
```

## Best Practices

### 1. Use AttributeUsage Correctly

```csharp
// ✅ GOOD - Clear attribute usage
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class,
    AllowMultiple = false,
    Inherited = true)]
public class MyAttribute : Attribute
{
}

// ❌ BAD - No AttributeUsage
public class MyAttribute : Attribute
{
    // Can be used anywhere
}
```

### 2. Immutable Attribute Properties

```csharp
// ✅ GOOD - Immutable
public class ImmutableAttribute : Attribute
{
    public string Value { get; }

    public ImmutableAttribute(string value)
    {
        Value = value;
    }
}

// ❌ BAD - Mutable
public class MutableAttribute : Attribute
{
    public string Value { get; set; }
}
```

### 3. Document Attribute Purpose

```csharp
/// <summary>
/// Indicates that a method should be logged.
/// </summary>
/// <remarks>
/// Apply this attribute to methods that should have their
/// execution logged, including entry, exit, and any exceptions.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class LogAttribute : Attribute
{
}
```

### 4. Use Reflection Carefully

```csharp
// ✅ GOOD - Cache reflection results
private static readonly Dictionary<Type, object[]> _attributeCache = new();

public static T[]? GetAttributes<T>(Type type) where T : Attribute
{
    if (!_attributeCache.TryGetValue(type, out var attrs))
    {
        attrs = type.GetCustomAttributes(typeof(T));
        _attributeCache[type] = attrs;
    }

    return (T[])attrs;
}
```

## Common Pitfalls

### Pitfall 1: Not Inheriting Attributes

```csharp
// ❌ WRONG - Child class loses attributes
[Deprecated]
public class BaseClass { }

public class DerivedClass : BaseClass { }
// DerivedClass doesn't have Deprecated

// ✅ CORRECT - Set Inherited = true
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class DeprecatedAttribute : Attribute { }
```

### Pitfall 2: Expensive Reflection Calls

```csharp
// ❌ BAD - Reflection on every call
public void ProcessItem(object item)
{
    var attrs = item.GetType().GetCustomAttributes();
    // Expensive!
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

### Pitfall 3: Attribute Type Misuse

```csharp
// ❌ WRONG - Attribute on wrong target
[Serializable] // Intended for class
public void MyMethod() { }

// ✅ CORRECT
[AttributeUsage(AttributeTargets.Class)]
public class SerializableAttribute : Attribute { }
```

## Key Takeaways

- Create custom attributes by inheriting from Attribute
- Use AttributeUsage to restrict where attributes can be applied
- Make attribute properties immutable
- Use reflection to query attributes at runtime
- Cache reflection results for performance
- Document attribute purpose clearly
- Avoid expensive reflection on hot paths
- Set Inherited = true when appropriate
- Use attributes for cross-cutting concerns
- Leverage attributes for configuration and validation
