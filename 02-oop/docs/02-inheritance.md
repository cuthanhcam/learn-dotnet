# Inheritance in C#

## What is Inheritance?
Inheritance allows you to create a new class (derived/child) that reuses, extends, or modifies the behavior of another class (base/parent).

**Analogy:**
- Base class = Parent
- Derived class = Child (inherits traits, can add/override behaviors)

## Base and Derived Classes
```csharp
public class Animal
{
	public string Name { get; set; }
	public void Eat() => Console.WriteLine($"{Name} is eating.");
}

public class Dog : Animal
{
	public void Bark() => Console.WriteLine($"{Name} barks!");
}
```

## Constructors in Inheritance
Derived classes can call base class constructors:
```csharp
public class Person
{
	public string Name { get; }
	public Person(string name) => Name = name;
}
public class Student : Person
{
	public int Id { get; }
	public Student(string name, int id) : base(name) => Id = id;
}
```

## Sealed Classes
Use `sealed` to prevent further inheritance:
```csharp
public sealed class FinalClass { }
// class SubClass : FinalClass {} // Error!
```

## Hiding vs Overriding
- **Override:** Use `virtual` in base, `override` in derived to change behavior.
- **Hide:** Use `new` keyword to hide base member (not recommended unless necessary).

```csharp
public class Base
{
	public virtual void Speak() => Console.WriteLine("Base speaks");
}
public class Derived : Base
{
	public override void Speak() => Console.WriteLine("Derived speaks");
}
```

## Practice Exercise

**Task:**
1. Create a base class `Shape` with a method `Area()` (returns 0 by default).
2. Create a derived class `Rectangle` with width/height and override `Area()`.
3. Create a derived class `Circle` with radius and override `Area()`.

## Interview Questions
- What is inheritance and why is it useful?
- How do you call a base class constructor?
- What is the difference between override and new?
- What is a sealed class?

## Pro Tips
- Favor composition over inheritance for code reuse.
- Use `override` for polymorphic behavior, avoid `new` unless hiding is required.
