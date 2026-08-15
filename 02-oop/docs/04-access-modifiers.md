---
title: "Access Modifiers in C#"
description: "Accessibility rules and their role in protecting implementation details and invariants."
slug: csharp-accessibility-and-api-surface
phase: 2
order: 4
difficulty: intermediate
article-type: reference
estimated-reading-minutes: 14
topics: [csharp, accessibility, encapsulation]
prerequisites: [csharp-classes-objects-encapsulation]
status: maintained
last-reviewed: 2026-08-15
---

# Access Modifiers in C#

Access modifiers control the visibility and accessibility of classes, methods, and members.

## Types of Access Modifiers

- **public**: Accessible from anywhere
- **private**: Accessible only within the same class
- **protected**: Accessible in the class and derived classes
- **internal**: Accessible within the same assembly
- **protected internal**: Accessible in derived classes or same assembly
- **private protected**: Accessible in derived classes within the same assembly

## Examples
```csharp
public class Example
{
	public int PublicValue;           // Anywhere
	private int PrivateValue;         // Only in Example
	protected int ProtectedValue;     // Example + derived
	internal int InternalValue;       // Same assembly
	protected internal int ProtInt;   // Derived or same assembly
	private protected int PrivProt;   // Derived + same assembly
}
```

## Accessibility in Inheritance
- `private` members are NOT accessible in derived classes.
- `protected` members ARE accessible in derived classes.

## Best Practices
- Use the most restrictive modifier possible (start with private).
- Expose only what is necessary for other classes.
- Use properties to control access to fields.

## Practice Exercise

**Task:**
1. Create a class with all access modifiers.
2. Try to access each member from a derived class and from outside.

## Interview Questions
- What is the difference between protected and private?
- When would you use internal?
- How do access modifiers affect inheritance?
