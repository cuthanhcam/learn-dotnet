---
title: "Common OOP Pitfalls"
description: "Frequent object-design mistakes, their consequences, and safer alternatives."
phase: 2
order: 10
topics: [oop, pitfalls, design]
---

# Common OOP Pitfalls

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
Learn and apply SOLID for maintainable OOP code.

---

**Pro Tips:**
- Start with the most restrictive access modifier.
- Use properties for all public data.
- Keep inheritance shallow.
- Always clean up resources.
- Program to interfaces, not implementations.
