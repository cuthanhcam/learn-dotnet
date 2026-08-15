---
title: "Phase 02 — Object-Oriented Programming in C#"
description: "A detailed guide to classes, invariants, inheritance, polymorphism, accessibility, lifetime, equality, records, composition, patterns, and SOLID design."
phase: 2
status: complete
target-framework: net8.0
prerequisites: [phase-01-csharp-fundamentals]
previous-phase: ../01-csharp-basics/README.md
next-phase: ../03-core-dotnet/README.md
---

# Object-Oriented Programming in C# (02-oop)

> Reference documentation for C# OOP concepts and best practices

## Overview

Comprehensive guide covering:
- Classes, objects, and encapsulation
- Inheritance and polymorphism
- Interfaces and abstraction
- Constructors and destructors
- Access modifiers
- Static members and properties
- Object initializers
- OOP design patterns (intro)
- Object equality, hashing, and diagnostic representation
- Records, value objects, and practical immutability
- SOLID principles and their trade-offs

## Learning outcomes

After completing this phase, you should be able to:

- model a type so invalid state is difficult or impossible to represent;
- distinguish fields, properties, methods, constructors, and object initializers;
- choose composition or inheritance based on substitutability rather than reuse alone;
- explain compile-time abstraction and runtime virtual dispatch;
- choose between an interface and an abstract class for a concrete variation point;
- use access modifiers to minimize the public surface and protect invariants;
- distinguish construction, finalization, garbage collection, and deterministic disposal;
- design static members without hiding mutable global state;
- implement consistent equality, hashing, and `ToString` behavior;
- choose records or classes based on value semantics versus identity;
- apply SOLID as change-oriented heuristics without creating speculative abstractions.

## Setup

```bash
# Prerequisites
dotnet --version  # 8.0 or later

# Commands
dotnet build
dotnet run --project src/OopBasics.ConsoleApp
dotnet test
```

## 🏗️ Project Structure

```
02-oop/
│
├── 02-oop.slnx                      # Main solution file
├── global.json                       # .NET version lock
│
├── src/                              # Main source code
│   ├── OopBasics.ConsoleApp/         # Entry point for demos
│   ├── OopBasics.Examples/           # Organized code examples
│   └── OopBasics.Playground/         # Quick testing environment
│
├── exercises/                        # Practice problems (structured by difficulty)
│   └── OopBasics.Exercises/
│
├── tests/                            # Unit tests
│   └── OopBasics.Tests/
│
├── benchmarks/                       # Performance benchmarking (optional)
│
├── docs/                             # Module documentation & notes
│
└── README.md                         # This file
```

---

## 🎬 What's Inside

| Topic | Focus |
|-------|-------|
| **Classes & Objects** | Encapsulation, fields, properties |
| **Inheritance** | Base/derived classes, virtual/override |
| **Polymorphism** | Interfaces, abstract classes, dynamic dispatch |
| **Access Modifiers** | public, private, protected, internal |
| **Constructors/Destructors** | Object lifecycle |
| **Static Members** | Static fields, methods, properties |
| **Design Patterns** | OOP best practices (intro) |

**👉 Deep dives** are in [`docs/`](docs/) — this README is your **entry point**, not a textbook.

---

## 🏛️ Design Principles

This module follows:

- **Small, focused examples** - One concept per file
- **Runnable code** - All examples compile and execute
- **Clear naming** - Self-documenting code
- **Avoid magic** - Explicit > implicit
- **Testing mindset** - Examples are testable, exercises have tests

---

## 📋 Module Rules

1. Each example has a `Run()` method or `Main()`
2. Playground = experimentation zone (no business logic)
3. Exercises = self-contained problems
4. Tests = verify learning outcomes
5. Benchmarks = performance investigation (optional labs)

---

## 💡 Skills Covered

```
✅ OOP Syntax & Semantics
✅ Encapsulation & Abstraction
✅ Inheritance & Polymorphism
✅ Interface Design
✅ Object Lifecycle
✅ Static Members
✅ OOP Design Patterns (intro)
✅ Debugging & Problem Solving
✅ Testing Fundamentals
```

---

## 🚀 Getting Started

### Prerequisites

- **.NET 8 LTS** (or later)
- **C# IDE** (VS 2022+, VS Code, Rider)
- **Git** (basic familiarity)

### Setup

```bash
cd 02-oop
dotnet --version           # Should be 8.0 or later
dotnet build
dotnet run --project src/OopBasics.ConsoleApp
dotnet run --project src/OopBasics.Playground
dotnet test
dotnet run -c Release --project benchmarks/OopBasics.Benchmarks.csproj
```

---

## 📚 Documentation Structure

All detailed learning content lives in `docs/` and should be read in order.

| Order | Lesson | Primary focus |
|---:|---|---|
| 0 | [Roadmap](docs/00-roadmap.md) | Study sequence and checkpoints |
| 1 | [Classes and objects](docs/01-classes-objects.md) | State, behavior, properties, and invariants |
| 2 | [Inheritance](docs/02-inheritance.md) | Base contracts, overriding, sealing, and substitution |
| 3 | [Polymorphism](docs/03-polymorphism.md) | Interfaces, abstract classes, and dynamic dispatch |
| 4 | [Access modifiers](docs/04-access-modifiers.md) | Public surface and encapsulation boundaries |
| 5 | [Construction and disposal](docs/05-constructors-destructors.md) | Valid construction and resource ownership |
| 6 | [Static members](docs/06-static-members.md) | Type-level behavior and shared-state risks |
| 7 | [Patterns and composition](docs/07-oop-patterns.md) | Composition, strategy, factory, and observer |
| 8 | [Object contracts and records](docs/08-object-contracts-records.md) | Equality, hashing, records, and immutability |
| 9 | [SOLID principles](docs/09-solid-principles.md) | Change boundaries and dependency direction |
| 10 | [Common pitfalls](docs/common-pitfalls.md) | Failure modes and safer alternatives |

## Core design decisions

### Encapsulation means protecting invariants

Encapsulation is more than changing a field from `public` to `private`. A type is well encapsulated when every public operation preserves its rules. Validate constructor arguments, expose behavior instead of unrestricted setters, and return read-only views or copies when callers must not mutate internal collections.

### Prefer composition until an `is-a` contract is proven

Inheritance couples a derived type to the base type's public and protected contract. Use it when every derived instance can safely substitute for the base type. Use composition when one object merely uses another capability, when behavior must change at runtime, or when inheritance would expose unrelated operations.

### Interface versus abstract class

Choose an interface for a capability contract that can be implemented across unrelated type hierarchies or when multiple capabilities must be combined. Choose an abstract class when derived types share a genuine identity, protected extension points, and reusable implementation. Neither option is automatically more testable; the quality and ownership of the contract matter.

### Entities versus value objects

An entity is tracked by identity through changes. A value object is defined by its components and is usually immutable. Equality must follow that domain meaning. Records are convenient for value-oriented data, but they do not automatically guarantee deep immutability or correct domain validation.

### Own only the lifetime you create

A type should generally dispose resources it owns. It should not unexpectedly dispose a dependency supplied and owned by its caller. Express lifetime responsibility in the API and use `using`/`await using` at the ownership boundary.

## Practice workflow

For each lesson:

1. Predict the example's behavior before running the console app.
2. Identify the invariant or contract represented by the public API.
3. Find one design that compiles but violates substitutability or encapsulation.
4. Read the matching tests and add a boundary case.
5. Reimplement the exercise without reading its solution.
6. Explain both the benefit and cost of the chosen abstraction.

## Exercise progression

- `ClassesExercises` practices state, behavior, and construction.
- `EncapsulationExercises` protects state transitions and validates boundaries.
- `InheritanceExercises` explores base contracts and overrides.
- `PolymorphismExercises` replaces concrete branching with capability contracts.

The exercise project is runnable, but its main value is in the public types and their tests. Add exercises for equality and SOLID refactoring after completing lessons 08 and 09.

## Testing and benchmark guidance

Tests should verify public behavior, invariant preservation, equality laws, and substitution—not private implementation details. Useful cases include invalid construction, repeated state transitions, subtype use through a base reference, equivalent value objects, and hash-based collection behavior.

The benchmark project is an optional lab. Object creation and virtual/interface dispatch are normally insignificant beside I/O or allocations in a real service. Run Release builds, avoid conclusions from a single timing, and use the measurement discipline taught in Phase 04.

## Completion checklist

- [ ] Run all examples and explain which method calls are statically versus dynamically dispatched.
- [ ] Design one type whose constructor guarantees its invariant.
- [ ] Refactor one inheritance-for-reuse design into composition.
- [ ] Explain when an interface and abstract class are each appropriate.
- [ ] Implement a value object with consistent equality and hashing.
- [ ] Explain why a record is not necessarily deeply immutable.
- [ ] Apply each SOLID principle to a concrete change scenario and name its cost.
- [ ] Pass `dotnet test 02-oop.slnx`.
- [ ] Review every item in `docs/common-pitfalls.md`.

## Next phase

Continue with [Phase 03 — Core .NET and the Standard Library](../03-core-dotnet/README.md), where these design skills are applied to generics, collections, LINQ, events, files, time, attributes, and nullable APIs.

---

## 🔧 Code Style

This project follows **consistent C# standards** via `.editorconfig`:

```bash
dotnet format
```

Before committing:
```bash
dotnet format
dotnet build
dotnet test
git add .
git commit -m "feat: [description]"
```

---

## 🎯 Tips for Success

1. **Code along** - Don't just read, type every example
2. **Experiment** - Break things intentionally
3. **Test frequently** - `dotnet test` after changes
4. **Read docs** - Don't skip documentation files
5. **Refactor** - Revisit old code with new knowledge
6. **Teach others** - Explain concepts to solidify them
