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
dotnet run -c Release --project benchmarks/StringBenchmark/StringBenchmark.csproj
dotnet run -c Release --project benchmarks/MemoryBenchmarks/MemoryBenchmarks.csproj
dotnet run -c Release --project benchmarks/CollectionsBenchmark/CollectionsBenchmark.csproj
```

---

## 📚 Documentation Structure

All detailed learning content lives in `docs/`.

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
