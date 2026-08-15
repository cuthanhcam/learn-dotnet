---
title: "Generics in C#"
description: "Generic types and methods, constraints, variance, type safety, and reusable API design."
phase: 3
order: 2
topics: [csharp, generics, variance]
---

# 🔧 Generics: Reusable, Type-Safe Code

## Overview

Generics enable writing reusable code that works with any type while maintaining type safety. This section covers generic types, constraints, variance, and best practices.

## Table of Contents

1. [Generic Basics](#generic-basics)
2. [Generic Classes](#generic-classes)
3. [Generic Methods](#generic-methods)
4. [Generic Constraints](#generic-constraints)
5. [Covariance & Contravariance](#covariance--contravariance)
6. [Advanced Patterns](#advanced-patterns)
7. [Performance & Runtime](#performance--runtime)

## Generic Basics

### What Are Generics?

Generics allow type parameters to be specified when defining classes, methods, or interfaces:

```csharp
// Generic type parameter T
public class Container<T>
{
    public T Value { get; set; }
}

// Usage
var intContainer = new Container<int> { Value = 42 };
var stringContainer = new Container<string> { Value = "Hello" };
```

### Why Generics?

1. **Type Safety**: Compile-time checking, no boxing/unboxing
2. **Performance**: No boxing overhead for value types
3. **Reusability**: Single implementation for multiple types
4. **Readability**: Clear intent in code

## Generic Classes

### Simple Generic Class

```csharp
public class Box<T>
{
    private T? _value;

    public void Set(T value) => _value = value;
    public T? Get() => _value;
}

// Usage
var intBox = new Box<int>();
intBox.Set(100);
int value = intBox.Get(); // Type-safe, no casting
```

### Multiple Type Parameters

```csharp
public class Pair<T1, T2>
{
    public T1 First { get; set; }
    public T2 Second { get; set; }
}

var pair = new Pair<string, int>
{
    First = "Alice",
    Second = 30
};
```

### Generic Collections

```csharp
public class Stack<T>
{
    private readonly List<T> _items = new();

    public void Push(T item) => _items.Add(item);
    public T Pop() => _items.Remove(Value: _items.Count - 1);
    public T Peek() => _items[_items.Count - 1];
    public bool IsEmpty => _items.Count == 0;
}
```

## Generic Methods

### Simple Generic Method

```csharp
public static class Converter
{
    // Generic method that works with any type
    public static List<T> CreateList<T>(T item)
    {
        return new List<T> { item };
    }
}

var ints = Converter.CreateList(42);      // List<int>
var strings = Converter.CreateList("hi"); // List<string>
```

### Generic Method in Generic Class

```csharp
public class Processor<T>
{
    // T is class type parameter, U is method type parameter
    public U Process<U>(T input, Func<T, U> converter)
    {
        return converter(input);
    }
}

var processor = new Processor<int>();
var result = processor.Process(42, x => x.ToString());
```

### Type Inference

```csharp
// Type parameters can be inferred
public static T GetDefault<T>() => default(T)!;

var intDefault = GetDefault<int>(); // Explicit
var autoInt = GetDefault(); // ❌ Needs explicit type
```

## Generic Constraints

### Constraint Types

#### 1. Base Class Constraint

```csharp
// T must be or derive from Shape
public class ShapeProcessor<T> where T : Shape
{
    public void Process(T shape)
    {
        shape.Draw(); // Shape methods available
    }
}
```

#### 2. Interface Constraint

```csharp
// T must implement IComparable
public class Sorter<T> where T : IComparable<T>
{
    public void Sort(List<T> items)
    {
        items.Sort(); // Can compare
    }
}
```

#### 3. Constructor Constraint

```csharp
// T must have a parameterless constructor
public class Factory<T> where T : new()
{
    public T Create() => new T();
}
```

#### 4. Reference Type Constraint

```csharp
// T must be a reference type (class, interface, delegate)
public class Reference<T> where T : class
{
    // Can check for null, use null coalescing
    public T? GetOrDefault(T? current) => current ?? default!;
}
```

#### 5. Value Type Constraint

```csharp
// T must be a value type (struct)
public class ValueWrapper<T> where T : struct
{
    public T? Value { get; set; } // Can be nullable
}
```

#### 6. Unmanaged Constraint

```csharp
// T must be an unmanaged type (no references)
public class NativeBuffer<T> where T : unmanaged
{
    // Can use with P/Invoke and pointers
}
```

#### 7. Notnull Constraint (C# 11+)

```csharp
// T must not be nullable reference type
public class NotNullable<T> where T : notnull
{
    private Dictionary<T, int> _cache = new();
}
```

### Multiple Constraints

```csharp
public class Repository<T>
    where T : class           // Reference type
    where T : IEntity         // Implements IEntity
    where T : new()           // Has parameterless constructor
{
    public T Create() => new T();

    public void Save(T entity)
    {
        var id = entity.Id; // IEntity.Id available
    }
}
```

## Covariance & Contravariance

### Covariance (out)

Allows more derived types where base types expected:

```csharp
public interface IRepository<out T>
{
    T GetById(int id);
}

public class Animal { }
public class Dog : Animal { }

IRepository<Dog> dogRepo = GetDogRepository();
IRepository<Animal> animalRepo = dogRepo; // Covariance
var animal = animalRepo.GetById(1); // Returns Animal (actually Dog)
```

### Contravariance (in)

Allows base types where more derived types expected:

```csharp
public interface IComparator<in T>
{
    int Compare(T a, T b);
}

IComparator<Animal> animalComp = GetAnimalComparator();
IComparator<Dog> dogComp = animalComp; // Contravariance
int result = dogComp.Compare(dog1, dog2);
```

### Invariance (no modifier)

Type parameter used for both input and output:

```csharp
public interface IBuffer<T>
{
    void Write(T value);      // Input (contravariant position)
    T Read();                 // Output (covariant position)
}

// Cannot assign IBuffer<Dog> to IBuffer<Animal>
IBuffer<Animal> buffer = new Buffer<Dog>(); // ❌ Compiler error
```

## Advanced Patterns

### Generic Base Class

```csharp
public abstract class Repository<T> where T : class, IEntity
{
    protected abstract IEnumerable<T> GetAll();
    public virtual T? GetById(int id)
        => GetAll().FirstOrDefault(x => x.Id == id);
}

public class UserRepository : Repository<User>
{
    protected override IEnumerable<User> GetAll() => GetUsers();
}
```

### Recursive Generic Types

```csharp
// Tree node that can contain other TreeNode<T>
public class TreeNode<T> where T : IComparable<T>
{
    public T Value { get; set; }
    public List<TreeNode<T>> Children { get; } = new();
}
```

### Self-Referential Constraints (CRTP)

```csharp
// Curiously Recurring Template Pattern
public abstract class Entity<T> where T : Entity<T>
{
    public abstract T Clone();
}

public class User : Entity<User>
{
    public override User Clone() => (User)MemberwiseClone();
}
```

### Generic Delegates

```csharp
// Func and Action are generic delegates
public delegate TResult Transformer<in TInput, out TResult>(TInput input);

// Usage
Transformer<int, string> intToString = x => x.ToString();
string result = intToString(42);
```

## Performance & Runtime

### Generic Type Specialization

```csharp
// Runtime creates separate types for each value type
var intList = new List<int>();    // Specialized version
var strList = new List<string>();  // Different specialized version

// Reference types share implementation
var dogList = new List<Dog>();     // Shared implementation
var animalList = new List<Animal>(); // Same implementation
```

### Type Erasure vs Specialization

```csharp
// .NET: Generic types ARE specialized at runtime
// Different from Java which erases generic info

List<int> intList = new();
List<string> strList = new();

// These have different types at runtime
Console.WriteLine(intList.GetType() == strList.GetType()); // false

// Can use reflection to get type info
var elementType = intList.GetType().GetGenericArguments()[0];
```

### Performance Considerations

```csharp
// ✅ BETTER - No boxing for value types
var intList = new List<int> { 1, 2, 3 };
int first = intList[0]; // No boxing

// ❌ WORSE - Boxing/unboxing overhead
ArrayList list = new() { 1, 2, 3 };
int first = (int)list[0]!; // Boxing happened when added
```

## Best Practices

### 1. Prefer Generic Over Non-Generic

```csharp
// ✅ GOOD
public IEnumerable<T> Process<T>(IEnumerable<T> items)
    where T : IEntity
{
    return items.Where(x => x.IsActive);
}

// ❌ BAD
public IEnumerable Process(IEnumerable items)
{
    // No type safety, requires casting
}
```

### 2. Use Appropriate Constraints

```csharp
// ✅ GOOD - Clear requirements
public T GetOrDefault<T>(T? value) where T : class
{
    return value ?? Activator.CreateInstance<T>();
}

// ❌ BAD - Too restrictive
public T GetOrDefault<T>(T value) where T : class
{
    // Won't work for value types
}
```

### 3. Name Generic Parameters Meaningfully

```csharp
// ✅ GOOD
public class Repository<TEntity, TPrimaryKey>
    where TEntity : IEntity<TPrimaryKey>
{
}

// ❌ BAD
public class Repository<T, U>
{
    // Unclear what T and U represent
}
```

### 4. Leverage Type Inference

```csharp
// ✅ GOOD
var result = Convert<string, int>("42"); // Types inferred where possible
var list = new List<int> { 1, 2, 3 };

// ❌ UNNECESSARY
var result = Convert<string, int>("42");
```

## Common Pitfalls

### Pitfall 1: Mixing Generic and Non-Generic

```csharp
// ❌ CONFUSING
public object GetValue<T>(T input) => input;

// ✅ CLEAR
public T GetValue<T>(T input) => input;
```

### Pitfall 2: Forgetting Where Clause

```csharp
// ❌ WRONG - Won't compile
public class Factory<T>
{
    public T Create() => new T(); // T might not have parameterless ctor
}

// ✅ CORRECT
public class Factory<T> where T : new()
{
    public T Create() => new T();
}
```

### Pitfall 3: Type Parameter Capture in Closures

```csharp
// ❌ WRONG - Captures outer TItem
public Func<T, bool> CreateFilter<T>()
{
    T value = default!;
    return x => x.Equals(value); // Captures T
}

// ✅ CORRECT
public Func<T, bool> CreateFilter<T>(T filterValue)
{
    return x => x.Equals(filterValue);
}
```

## Key Takeaways

- Use generics for type-safe, reusable code
- Leverage constraints to express requirements
- Understand covariance, contravariance, and invariance
- Consider performance implications of generic specialization
- Use meaningful names for generic parameters
- Avoid mixing generic and non-generic APIs
- Apply appropriate constraints to guide usage
- Prefer generic over non-generic implementations
