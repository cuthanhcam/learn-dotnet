---
title: "Common OOP Pitfalls"
description: "Frequent object-design mistakes, their consequences, and safer alternatives."
slug: oop-common-pitfalls
phase: 2
order: 10
difficulty: intermediate
article-type: pitfalls
estimated-reading-minutes: 16
topics: [oop, pitfalls, design]
prerequisites: [solid-principles-practical-guide]
status: maintained
last-reviewed: 2026-08-15
---

# Common OOP Pitfalls

## How to Use This Review

For each smell, identify the violated contract, the likely change that exposes the problem, the smallest repair, and the cost introduced by that repair. A pattern name without this reasoning is not a design improvement.

## 1. Forgetting to Use Access Modifiers
Default is private for class members, but public for top-level classes. Always specify!

## 2. Not Using Properties for Encapsulation
Exposing fields directly breaks encapsulation. Use properties to control access and validation.

## 3. Overusing Inheritance
Deep inheritance hierarchies are hard to maintain. Prefer composition for code reuse.

## 4. Not Implementing IDisposable
Implement `IDisposable` when the type owns a disposable resource or other state
that requires deterministic cleanup. A database itself is external; owning a
database connection or stream is the relevant lifetime responsibility. Do not
add an empty `IDisposable` implementation to every class that merely uses one.

## 5. Not Using Interfaces for Abstraction
Interfaces allow flexible, testable code. Don’t hardcode dependencies on concrete classes.

## 6. Not Overriding Virtual Methods Properly
Always use `override` for polymorphic behavior, not `new` (unless hiding is intentional).

## 7. Ignoring Object Lifecycle
Don’t rely on destructors for cleanup—use IDisposable and `using`.

## 8. Not Following SOLID Principles

SOLID principles are heuristics, not compiler rules. Applying all five mechanically can create fragmented interfaces, indirection, excessive dependency injection, and mock-heavy tests. Connect every abstraction to a known reason to change.

## 9. Anemic Domain Objects

An object with public setters for every field but no invariant-preserving behavior pushes business rules into unrelated services. Move state transitions next to the state they protect when that behavior belongs to the type.

Do not force behavior into data-transfer shapes whose purpose is serialization. Domain types and boundary DTOs serve different contracts.

## 10. Leaky Mutable Collections

Returning a mutable internal `List<T>` allows callers to bypass validation:

```csharp
public IReadOnlyList<OrderLine> Lines => _lines;
```

An interface alone does not make the object deeply immutable if callers retain another reference to the original list. Control ownership by copying input or using immutable collections when required.

## 11. Broken Equality in Hash Collections

If two objects are equal, their hash codes must match. Fields participating in hash/equality should not change while the object is a dictionary key or hash-set member. Prefer immutable value objects and test equality laws.

## 12. Inheritance for Reuse

A derived type must preserve the base contract, not merely share implementation. If ordinary base operations become unsupported or require stronger preconditions, use composition or split the capability contract.

## 13. Constructor Over-Injection

A constructor with many unrelated dependencies often signals too many responsibilities. Do not hide the problem behind a service locator or aggregate dependency bag; identify cohesive policies and workflows.

## Review Checklist

- Can invalid state be constructed or set publicly?
- Does every inheritance relationship satisfy substitution?
- Is the public surface narrower than the implementation surface?
- Are equality and hash contracts stable?
- Who owns each disposable dependency and event subscription?
- Does static mutable state leak between tests or requests?
- Is each interface owned by a real consumer need?
- Can behavior be explained without naming a design pattern?

## Navigation

[← SOLID principles](09-solid-principles.md) · [Phase 03 roadmap →](../../03-core-dotnet/docs/00-roadmap.md)

## References

- [C# object-oriented programming](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/oop)
- [Inheritance and polymorphism](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/inheritance)
- [Implement equality](https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-define-value-equality-for-a-type)
Learn and apply SOLID for maintainable OOP code.

---

**Pro Tips:**
- Start with the most restrictive access modifier.
- Use properties for all public data.
- Keep inheritance shallow.
- Always clean up resources.
- Program to interfaces, not implementations.
