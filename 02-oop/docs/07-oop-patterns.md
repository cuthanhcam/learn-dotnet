---
title: "OOP Patterns and Composition"
description: "Composition, strategy, factory, and observer as introductory object-design patterns."
slug: oop-composition-and-patterns
phase: 2
order: 7
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 22
topics: [oop, composition, design-patterns]
prerequisites: [csharp-polymorphism-and-abstraction]
status: maintained
last-reviewed: 2026-08-15
---

# OOP Patterns (Intro)

## Learning Objectives

After this article, you should be able to distinguish a pattern from a reusable library, choose composition over inheritance when behavior varies independently, explain Strategy and Factory responsibilities, and identify the ownership risks in Observer-style subscriptions.

OOP patterns are reusable solutions to common design problems.

## Encapsulation
Hide internal state, expose only what’s needed (via properties/methods).

## Inheritance
Share code and behavior via base/derived classes.

## Polymorphism
Write code that works with base types, but runs derived behavior.

## Composition

Composition builds behavior by holding references to collaborating objects. It expresses a **has-a/uses-a** relationship and keeps each collaborator replaceable behind a focused contract.

The key benefit is not “fewer base classes.” It is isolating independent reasons to change. Shipping calculation varies independently from shipment data, so `ShippingService` composes an `IShippingPolicy` rather than deriving `DomesticShipment`, `ExpressShipment`, and every combination.

Composition has costs: more objects, dependency wiring, and indirection. Keep a concrete dependency when variation is not useful.
Favor combining objects over deep inheritance.
```csharp
public class Engine { }
public class Car { public Engine Engine { get; set; } }
```

## Simple Design Patterns

### Strategy

Strategy represents interchangeable algorithms behind one capability contract. The context delegates work without knowing the concrete policy.

```csharp
public interface IShippingPolicy
{
    decimal Calculate(Shipment shipment);
}
```

Use Strategy when behavior varies by configuration, tenant, domain policy, or runtime selection. A delegate can be simpler when the strategy is only one operation and does not need a named domain abstraction.

### Factory

A factory owns a construction decision. It is valuable when creation requires validation, selects among implementations, or hides a complex construction graph. A factory that merely writes `new Product()` adds ceremony without policy.

Static factory methods can provide meaningful names and return a subtype or cached value. A separate factory service is useful when creation itself needs dependencies.

### Observer and .NET Events

Observer lets a publisher notify subscribers without knowing their concrete types. .NET events restrict invocation to the publisher, but subscriptions are strong references. A long-lived publisher can retain a short-lived subscriber; unsubscription and ownership are part of the design.

Events are in-process notifications, not durable messaging. They do not provide persistence, retries, ordering across processes, or delivery guarantees.

## Pattern Selection Questions

1. Which behavior or construction decision actually varies?
2. Which consumer owns the smallest useful contract?
3. Would a delegate or direct method be clearer?
4. Who owns collaborator and subscription lifetimes?
5. Does the abstraction reduce the cost of a known change?
6. Can callers observe and test behavior without knowing implementation details?

## Implementation Map

- `Patterns/CompositionPatternsExample.cs` demonstrates Strategy through shipping policies.
- `ShippingService` owns policy selection but not policy lifetime.
- `Shipment` validates its invariant at construction.
- `CompositionPatternsExampleTests` proves substitution with a consumer-defined policy.

## Testing Patterns

Test the public contract, not that one collaborator method was called unless the interaction is itself the contract. For Strategy, run the same input through multiple policies and verify results. For Factory, verify selection and invalid inputs. For Observer, verify payload, subscription, unsubscription, and duplicate-subscription behavior.

## Common Misuse

- Adding an interface to every class without a variation boundary.
- Choosing inheritance solely to reuse code.
- Creating a global service locator disguised as a factory.
- Publishing mutable internal state through an event payload.
- Treating in-process events as reliable integration messages.
- Naming ordinary conditional logic a “pattern” without clarifying the problem solved.
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

## Review Questions

1. When is a delegate preferable to a Strategy interface?
2. Which responsibility belongs in a factory and which stays in the created type?
3. How can an event subscription cause a memory leak?
4. What new complexity does composition introduce?
5. Why should patterns follow a change pressure rather than precede it?

## Navigation

[← Static members](06-static-members.md) · [Object contracts and records →](08-object-contracts-records.md)

## References

- [C# interfaces](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces)
- [C# events](https://learn.microsoft.com/en-us/dotnet/csharp/events-overview)
- [.NET dependency injection guidelines](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines)
- What is the difference between inheritance and composition?
- How would you implement a singleton in C#?
- What is a factory pattern?
