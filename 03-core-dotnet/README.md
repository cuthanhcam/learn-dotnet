# Core .NET & Standard Library (03-core-dotnet)

> Reference documentation for essential .NET runtime concepts, standard libraries, and advanced C# programming patterns

## Overview

Comprehensive guide covering:

* Collections and generics
* LINQ and functional-style programming
* Delegates, events, and callbacks
* Exception handling strategies
* File I/O and streams
* Date/time and timezone handling
* Attributes and reflection metadata
* Nullable reference types and null safety
* Performance considerations and best practices

## Setup

```bash
# Prerequisites
dotnet --version  # 8.0 or later

# Commands
dotnet build
dotnet run --project src/CoreDotNet.ConsoleApp
dotnet test
```

## 🏗️ Project Structure

```text
03-core-dotnet/
│
├── 03-core-dotnet.sln                # Main solution file
├── 03-core-dotnet.slnx               # Alternative solution format
├── global.json                       # .NET SDK version lock
├── .gitignore                        # Git ignore rules
│
├── src/                              # Main source code
│   │
│   ├── CoreDotNet.ConsoleApp/        # Entry point for demonstrations
│   │   ├── Program.cs
│   │   └── CoreDotNet.ConsoleApp.csproj
│   │
│   ├── CoreDotNet.Examples/          # Organized examples by topic
│   │   │
│   │   ├── Collections/
│   │   │   ├── ListExample.cs
│   │   │   ├── DictionaryExample.cs
│   │   │   ├── HashSetExample.cs
│   │   │   ├── QueueStackExample.cs
│   │   │   ├── CollectionInterfacesExample.cs
│   │   │   └── CustomCollectionExample.cs
│   │   │
│   │   ├── Generics/
│   │   │   ├── GenericClassExample.cs
│   │   │   ├── GenericMethodExample.cs
│   │   │   ├── ConstraintsExample.cs
│   │   │   ├── CovarianceContravarianceExample.cs
│   │   │   └── GenericPerformanceExample.cs
│   │   │
│   │   ├── ExceptionHandling/
│   │   │   ├── TryCatchExample.cs
│   │   │   ├── FinallyExample.cs
│   │   │   ├── CustomExceptionExample.cs
│   │   │   ├── ExceptionPropagationExample.cs
│   │   │   └── BestPracticesExample.cs
│   │   │
│   │   ├── LINQ/
│   │   │   ├── QuerySyntaxExample.cs
│   │   │   ├── MethodSyntaxExample.cs
│   │   │   ├── ProjectionExample.cs
│   │   │   ├── GroupingExample.cs
│   │   │   ├── JoinExample.cs
│   │   │   ├── DeferredExecutionExample.cs
│   │   │   └── LINQPerformanceExample.cs
│   │   │
│   │   ├── DelegatesAndEvents/
│   │   │   ├── DelegateBasicsExample.cs
│   │   │   ├── FuncActionPredicateExample.cs
│   │   │   ├── EventPublisherSubscriberExample.cs
│   │   │   ├── CustomEventExample.cs
│   │   │   └── LambdaExpressionExample.cs
│   │   │
│   │   ├── FileIO/
│   │   │   ├── FileReadWriteExample.cs
│   │   │   ├── StreamExample.cs
│   │   │   ├── BinaryFileExample.cs
│   │   │   ├── AsyncFileIOExample.cs
│   │   │   └── PathUtilitiesExample.cs
│   │   │
│   │   ├── DateTime/
│   │   │   ├── DateTimeExample.cs
│   │   │   ├── DateTimeOffsetExample.cs
│   │   │   ├── TimeSpanExample.cs
│   │   │   ├── TimeZoneExample.cs
│   │   │   └── FormattingParsingExample.cs
│   │   │
│   │   ├── Attributes/
│   │   │   ├── BuiltInAttributesExample.cs
│   │   │   ├── CustomAttributesExample.cs
│   │   │   ├── ReflectionAttributeExample.cs
│   │   │   └── SerializationAttributesExample.cs
│   │   │
│   │   ├── NullableReferenceTypes/
│   │   │   ├── NullableAnnotationsExample.cs
│   │   │   ├── NullConditionalExample.cs
│   │   │   ├── NullForgivingOperatorExample.cs
│   │   │   └── NullSafetyPatternsExample.cs
│   │   │
│   │   └── CoreDotNet.Examples.csproj
│   │
│   └── CoreDotNet.Playground/        # Experimental sandbox
│       ├── Program.cs
│       └── CoreDotNet.Playground.csproj
│
├── exercises/                        # Practice problems
│   │
│   ├── Easy/
│   │   ├── GenericSwap/
│   │   ├── BasicLINQ/
│   │   ├── FileReader/
│   │   └── SafeNullChecks/
│   │
│   ├── Medium/
│   │   ├── CustomCollection/
│   │   ├── EventDrivenCounter/
│   │   ├── ExceptionHierarchy/
│   │   └── LINQDataProcessor/
│   │
│   └── Hard/
│       ├── MiniFileDatabase/
│       ├── GenericRepository/
│       └── ReflectionSerializer/
│
├── tests/
│   └── CoreDotNet.Tests/
│       ├── CollectionsTests.cs
│       ├── GenericsTests.cs
│       ├── ExceptionHandlingTests.cs
│       ├── LINQTests.cs
│       ├── DelegatesAndEventsTests.cs
│       ├── FileIOTests.cs
│       ├── DateTimeTests.cs
│       ├── AttributesTests.cs
│       ├── NullableReferenceTypesTests.cs
│       ├── ExamplesRunSmokeTests.cs
│       └── CoreDotNet.Tests.csproj
│
├── benchmarks/
│   ├── CollectionsBenchmark/
│   │   ├── Program.cs
│   │   ├── CollectionsBenchmark.cs
│   │   └── CollectionsBenchmark.csproj
│   │
│   ├── LINQBenchmark/
│   │   ├── Program.cs
│   │   ├── LINQBenchmark.cs
│   │   └── LINQBenchmark.csproj
│   │
│   └── FileIOBenchmark/
│       ├── Program.cs
│       ├── FileIOBenchmark.cs
│       └── FileIOBenchmark.csproj
│
├── docs/
│   ├── 00-roadmap.md
│   ├── 01-collections.md
│   ├── 02-generics.md
│   ├── 03-exception-handling.md
│   ├── 04-linq.md
│   ├── 05-delegates-events.md
│   ├── 06-file-io.md
│   ├── 07-datetime-timezone.md
│   ├── 08-attributes.md
│   ├── 09-nullable-reference-types.md
│   ├── performance-guidelines.md
│   └── common-pitfalls.md
│
└── README.md
```

---

## 🎬 What's Inside

| Topic                        | Focus                                        |
| ---------------------------- | -------------------------------------------- |
| **Collections**              | Generic collections, interfaces, performance |
| **Generics**                 | Type-safe reusable programming               |
| **Exception Handling**       | Robust error management patterns             |
| **LINQ**                     | Querying and transforming data               |
| **Delegates & Events**       | Event-driven and callback programming        |
| **File I/O**                 | Streams, files, async operations             |
| **Date & Time**              | Timezones, formatting, calculations          |
| **Attributes**               | Metadata and reflection                      |
| **Nullable Reference Types** | Null safety and defensive coding             |
| **Benchmark Labs**           | LINQ, collections, and I/O performance       |

**👉 Deep dives** are in `docs/` — this README is your **entry point**, not a textbook.

---

## 🏛️ Design Principles

This module follows:

* **Concept isolation** — one core concept per example
* **Runnable examples** — every example compiles and executes
* **Performance awareness** — understand costs and trade-offs
* **Idiomatic C#** — modern .NET coding standards
* **Testability first** — examples designed for verification

---

## 📋 Module Rules

1. Each example exposes a `Run()` method or executable entry point
2. Playground projects are for experimentation only
3. Exercises remain self-contained and independent
4. Tests validate expected behavior and edge cases
5. Benchmarks focus on real-world performance characteristics

---

## 💡 Skills Covered

```text
✅ Advanced Collection Usage
✅ Generic Programming
✅ LINQ Query Mastery
✅ Exception Management
✅ Event-Driven Architecture
✅ File & Stream Operations
✅ Timezone & Date Handling
✅ Reflection & Attributes
✅ Null Safety Techniques
✅ Performance Optimization
```

---

## 🚀 Getting Started

### Prerequisites

* **.NET 8 LTS** (or later)

  * Download: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
* **C# IDE**

  * Visual Studio 2022+
  * VS Code + C# Dev Kit
  * JetBrains Rider
* Basic understanding of:

  * C# syntax
  * OOP fundamentals
  * Methods and classes

### Setup

```bash
# 1. Navigate to the module
cd 03-core-dotnet

# 2. Verify .NET version
dotnet --version

# 3. Restore & build
dotnet build

# 4. Run demo application
dotnet run --project src/CoreDotNet.ConsoleApp

# 5. Run playground
dotnet run --project src/CoreDotNet.Playground

# 6. Run all tests
dotnet test

# 7. Run benchmarks (optional)
dotnet run -c Release --project benchmarks/CollectionsBenchmark/CollectionsBenchmark.csproj
dotnet run -c Release --project benchmarks/LINQBenchmark/LINQBenchmark.csproj
dotnet run -c Release --project benchmarks/FileIOBenchmark/FileIOBenchmark.csproj
```

---

## 📚 Documentation Structure

All detailed learning material lives in `docs/`:

| File                             | Purpose                           |
| -------------------------------- | --------------------------------- |
| `01-collections.md`              | Collection internals and usage    |
| `02-generics.md`                 | Generic programming deep dive     |
| `03-exception-handling.md`       | Error handling strategies         |
| `04-linq.md`                     | LINQ operators and execution      |
| `05-delegates-events.md`         | Delegates, events, and callbacks  |
| `06-file-io.md`                  | Streams and filesystem operations |
| `07-datetime-timezone.md`        | Time and timezone correctness     |
| `08-attributes.md`               | Metadata and reflection           |
| `09-nullable-reference-types.md` | Null safety practices             |
| `performance-guidelines.md`      | Optimization and benchmarking     |
| `common-pitfalls.md`             | Common mistakes and fixes         |
| `00-roadmap.md`                  | Suggested study order             |

---

## ⚠️ Common Pitfalls

❌ **Enumerating LINQ multiple times**

```csharp
var query = numbers.Where(x => x > 10);

Console.WriteLine(query.Count());
Console.WriteLine(query.Count()); // Executes twice
```

✅ **Materialize when needed**

```csharp
var results = numbers
    .Where(x => x > 10)
    .ToList();

Console.WriteLine(results.Count);
Console.WriteLine(results.Count);
```

---

❌ **Catching all exceptions blindly**

```csharp
try
{
    DangerousOperation();
}
catch
{
    // Hides real issues
}
```

✅ **Catch specific exceptions**

```csharp
try
{
    DangerousOperation();
}
catch (IOException ex)
{
    Console.WriteLine(ex.Message);
}
```

---

❌ **Using DateTime without timezone awareness**

```csharp
DateTime now = DateTime.Now;
```

✅ **Use DateTimeOffset when correctness matters**

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;
```

👉 More examples available in `docs/common-pitfalls.md`

---

## 🔧 Code Style

This project follows consistent .NET coding conventions using `.editorconfig`.

```bash
# Format all code
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

1. Build every example yourself
2. Benchmark assumptions before optimizing
3. Read exception stack traces carefully
4. Experiment with LINQ execution behavior
5. Practice null-safe coding constantly
6. Use tests to validate understanding
7. Refactor old code using modern patterns
