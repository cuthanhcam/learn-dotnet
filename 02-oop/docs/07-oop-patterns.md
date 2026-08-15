---
title: "OOP Patterns and Composition"
description: "Composition, strategy, factory, and observer as introductory object-design patterns."
phase: 2
order: 7
topics: [oop, composition, design-patterns]
---

# OOP Patterns (Intro)

OOP patterns are reusable solutions to common design problems.

## Encapsulation
Hide internal state, expose only what’s needed (via properties/methods).

## Inheritance
Share code and behavior via base/derived classes.

## Polymorphism
Write code that works with base types, but runs derived behavior.

## Composition
Favor combining objects over deep inheritance.
```csharp
public class Engine { }
public class Car { public Engine Engine { get; set; } }
```

## Simple Design Patterns
- **Singleton:** Only one instance exists.
	```csharp
	public class Singleton
	{
		private static Singleton _instance;
		public static Singleton Instance => _instance ??= new Singleton();
		private Singleton() { }
	}
	```
- **Factory:** Creates objects without exposing instantiation logic.
	```csharp
	public class AnimalFactory
	{
		public static Animal Create(string type) =>
			type == "dog" ? new Dog() : new Cat();
	}
	```

## Practice Exercise

**Task:**
1. Implement a singleton Logger class.
2. Create a factory for shapes (circle, rectangle).

## Interview Questions
- What is the difference between inheritance and composition?
- How would you implement a singleton in C#?
- What is a factory pattern?
