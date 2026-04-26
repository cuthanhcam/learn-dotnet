# Polymorphism in C#

## What is Polymorphism?
Polymorphism means "many forms". It allows objects to be treated as instances of their base type, enabling flexible and reusable code.

**Analogy:**
- Remote control (base type) can control TV, AC, or Fan (derived types)

## Interfaces
Interfaces define contracts (what must be done), not how.
```csharp
public interface IShape
{
	double Area();
}
public class Rectangle : IShape
{
	public double Width, Height;
	public double Area() => Width * Height;
}
```

## Abstract Classes
Abstract classes can have both abstract (no body) and concrete methods.
```csharp
public abstract class Animal
{
	public abstract void Speak();
	public void Eat() => Console.WriteLine("Eating");
}
public class Dog : Animal
{
	public override void Speak() => Console.WriteLine("Woof!");
}
```

## Virtual/Override
Use `virtual` in base, `override` in derived for runtime polymorphism.
```csharp
public class Base
{
	public virtual void Show() => Console.WriteLine("Base");
}
public class Derived : Base
{
	public override void Show() => Console.WriteLine("Derived");
}
```

## Dynamic Dispatch
The correct method is chosen at runtime based on the actual object type.
```csharp
Base obj = new Derived();
obj.Show(); // Outputs "Derived"
```

## Practice Exercise

**Task:**
1. Define an interface `IVehicle` with a method `Drive()`.
2. Implement `Car` and `Bike` classes that implement `IVehicle`.
3. Write a method that takes an `IVehicle` and calls `Drive()`.

## Interview Questions
- What is polymorphism? Give an example.
- Difference between interface and abstract class?
- How does C# achieve runtime polymorphism?

## Pro Tips
- Use interfaces for contracts, abstract classes for shared code.
- Always use `override` for polymorphic behavior.
