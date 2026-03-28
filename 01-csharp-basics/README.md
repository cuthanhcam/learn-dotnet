# C# Fundamentals (01-csharp-basics)

> Reference documentation for C# core concepts and best practices

## Overview

Comprehensive guide covering:
- .NET ecosystem (CLR, JIT, assemblies)
- Variables, types, type system
- Control flow and operators
- Methods and parameter modifiers
- Collections (arrays, lists, dictionaries, sets)
- String handling and StringBuilder
- Null handling and memory concepts
- Common pitfalls and debugging

## Setup

```bash
# Prerequisites
dotnet --version  # 8.0 or later

# Commands
dotnet build
dotnet run --project src/CSharpBasics.ConsoleApp
dotnet test
```

## 🏗️ Project Structure

```
01-csharp-basics/
│
├── 01-csharp-basics.sln                # Main solution file
├── global.json                         # .NET version lock
├── .gitignore                          # Git ignore rules
│
├── src/                                # Main source code
│   │
│   ├── CSharpBasics.ConsoleApp/        # Entry point for demos
│   │   ├── Program.cs                  # Main demonstration app
│   │   └── CSharpBasics.ConsoleApp.csproj
│   │
│   ├── CSharpBasics.Examples/          # Organized code examples
│   │   ├── Variables/
│   │   │   ├── VariablesExample.cs         # var, int, string, etc.
│   │   │   └── DynamicVsTypedExample.cs    # dynamic vs var
│   │   │
│   │   ├── ControlFlow/
│   │   │   ├── IfElseExample.cs            # if, else if, else
│   │   │   ├── SwitchExample.cs            # switch expressions & statements
│   │   │   └── LoopsExample.cs             # for, while, do-while, foreach
│   │   │
│   │   ├── Methods/
│   │   │   ├── MethodBasicsExample.cs      # method declaration, return types
│   │   │   ├── ParamModifiersExample.cs    # ref, out, in parameters
│   │   │   ├── OverloadingExample.cs       # method overloading
│   │   │   └── OptionalParametersExample.cs # default values, named args
│   │   │
│   │   ├── Collections/
│   │   │   ├── ArraysExample.cs            # arrays, jagged arrays
│   │   │   ├── ListExample.cs              # List<T>, Add, Remove, etc.
│   │   │   ├── DictionaryExample.cs        # Dictionary<K,V>
│   │   │   ├── HashSetExample.cs           # HashSet<T>
│   │   │   └── EnumerableExample.cs        # IEnumerable, foreach
│   │   │
│   │   ├── Strings/
│   │   │   ├── StringBasicsExample.cs      # string literals, interpolation
│   │   │   ├── StringBuilderExample.cs     # StringBuilder for performance
│   │   │   ├── StringMethodsExample.cs     # Split, Join, Contains, etc.
│   │   │   └── StringPerformanceExample.cs # string vs StringBuilder comparison
│   │   │
│   │   └── CSharpBasics.Examples.csproj
│   │
│   └── CSharpBasics.Playground/        # Quick testing environment
│       ├── Program.cs                  # Playground program
│       └── CSharpBasics.Playground.csproj
│
├── exercises/                          # Practice problems (structured by difficulty)
│   │
│   ├── Easy/
│   │   ├── SumNumbers/
│   │   │   ├── SumNumbers.cs
│   │   │   └── README.md
│   │   │
│   │   ├── MaxOfThree/
│   │   │   ├── MaxOfThree.cs
│   │   │   └── README.md
│   │   │
│   │   ├── SimpleLoop/
│   │   │   ├── SimpleLoop.cs
│   │   │   └── README.md
│   │   │
│   │   └── VariableTypes/
│   │       ├── VariableTypes.cs
│   │       └── README.md
│   │
│   ├── Medium/
│   │   ├── ReverseString/
│   │   │   ├── ReverseString.cs
│   │   │   └── README.md
│   │   │
│   │   ├── CountWords/
│   │   │   ├── CountWords.cs
│   │   │   └── README.md
│   │   │
│   │   ├── FibonacciSequence/
│   │   │   ├── FibonacciSequence.cs
│   │   │   └── README.md
│   │   │
│   │   └── PrimeNumbers/
│   │       ├── PrimeNumbers.cs
│   │       └── README.md
│   │
│   └── Hard/
│       ├── BasicCalculator/
│       │   ├── BasicCalculator.cs
│       │   └── README.md
│       │
│       └── NestedCollections/
│           ├── NestedCollections.cs
│           └── README.md
│
├── tests/                              # Unit tests
│   └── CSharpBasics.Tests/
│       ├── VariablesTests.cs
│       ├── MethodsTests.cs
│       ├── StringTests.cs
│       ├── CollectionsTests.cs
│       └── CSharpBasics.Tests.csproj
│
├── benchmarks/                         # Performance benchmarking (optional)
│   └── StringBenchmark/
│       ├── StringBenchmark.cs          # BenchmarkDotNet examples
│       └── StringBenchmark.csproj
│
├── docs/                               # Module documentation & notes
│   ├── 00-roadmap.md                   # Learning path for this module
│   ├── 01-dotnet-ecosystem.md          # CLR, JIT, assemblies explained
│   ├── 02-variables-and-types.md       # In-depth type system
│   ├── 03-operators-control-flow.md    # All operators and control structures
│   ├── 04-methods.md                   # Method patterns and best practices
│   ├── 05-collections-deep-dive.md     # Arrays, Lists, Dictionaries, etc.
│   ├── 06-strings.md                   # String handling and performance
│   ├── 07-nullability.md               # Null handling, nullable ref types
│   ├── cheatsheet.md                   # Quick reference
│   ├── common-pitfalls.md              # Common mistakes and how to avoid them
│   └── troubleshooting.md              # Debugging common issues
│
└── README.md                           # This file
```

---

## 🎬 What's Inside

| Topic | Focus |
|-------|-------|
| **.NET Ecosystem** | CLR, JIT, Assemblies, Namespaces |
| **Variables & Types** | var, dynamic, type inference, constants |
| **Operators & Control Flow** | if, switch, for, while, foreach |
| **Methods** | Declarations, ref/out/in, overloading, parameters |
| **Collections** | Arrays, List<T>, Dictionary, HashSet |
| **Strings** | Interpolation, StringBuilder, methods |
| **Null Handling** | Nullability, ?., ??, pattern matching |

**👉 Deep dives** are in [`docs/`](docs/) — this README is your **entry point**, not a textbook.

---

## 🏛️ Design Principles

This module follows:

- **Small, focused examples** - One concept per file
- **Runnable code** - All examples compile and execute
- **Clear naming** - Self-documenting code
- **Avoid magic** - Explicit > implicit (except for `var`)
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
✅ C# Syntax & Semantics
✅ Type System Understanding
✅ Control Flow Mastery
✅ Method Design Patterns
✅ Collections & Data Handling
✅ String Performance Optimization
✅ Memory & Null Safety Concepts
✅ Debugging & Problem Solving
✅ Testing Fundamentals
```

---

## 🚀 Getting Started

### Prerequisites

- **.NET 8 LTS** (or later)
  - Download: https://dotnet.microsoft.com/download
- **C# IDE**:
  - Visual Studio 2022+
  - VS Code + C# Dev Kit
  - JetBrains Rider
- **Git** (basic familiarity)

### Setup

```bash
# 1. Clone/navigate to this folder
cd 01-csharp-basics

# 2. Verify .NET version
dotnet --version           # Should be 8.0 or later

# 3. Restore packages & build
dotnet build

# 4. Run main demo app
dotnet run --project src/CSharpBasics.ConsoleApp

# 5. Run specific playground
dotnet run --project src/CSharpBasics.Playground

# 6. Run all tests
dotnet test

# 7. Run benchmarks (optional)
cd benchmarks/StringBenchmark
dotnet run -c Release
```





---

## 📚 Documentation Structure

All detailed learning content lives in `docs/`:

| File | Purpose |
|------|---------|
| `01-dotnet-ecosystem.md` | CLR, JIT, assemblies explained |
| `02-variables-types.md` | Deep type system knowledge |
| `03-operators-control-flow.md` | Complete operator reference |
| `04-methods.md` | Method patterns & best practices |
| `05-collections-deep-dive.md` | Collections internals |
| `06-strings.md` | String performance & techniques |
| `07-nullability.md` | Null safety mechanisms |
| `cheatsheet.md` | Quick syntax reference |
| `common-pitfalls.md` | **Must read** — avoid 80% of bugs |
| `troubleshooting.md` | Debugging common issues |

---

## ⚠️ Common Pitfalls

**Quick fixes** (see `docs/common-pitfalls.md` for full details):

❌ **Using `dynamic` unnecessarily**
```csharp
dynamic var = GetInput();  // Runtime errors possible
```

✅ **Use `var` with type inference**
```csharp
var value = GetInput();    // Compiler knows the type
```

---

❌ **String concatenation in loops**
```csharp
string result = "";
for (int i = 0; i < 1000; i++)
    result += i;  // Creates 1000 objects!
```

✅ **Use StringBuilder**
```csharp
var sb = new StringBuilder();
for (int i = 0; i < 1000; i++)
    sb.Append(i);
```

---

❌ **Not checking nulls**
```csharp
string text = GetValue();
int len = text.Length;  // NullReferenceException?
```

✅ **Use safe operators**
```csharp
string? text = GetValue();
int len = text?.Length ?? 0;
```

👉 **Many more in `docs/common-pitfalls.md`** — review it early!



---

## 🔧 Code Style

This project follows **consistent C# standards** via `.editorconfig`:

```bash
# Format code automatically
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
