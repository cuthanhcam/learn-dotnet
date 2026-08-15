---
title: "Phase 01 — C# Fundamentals"
description: "A detailed foundation in the .NET execution model, C# syntax, types, control flow, methods, collections, strings, nullability, and introductory memory concepts."
phase: 1
status: complete
target-framework: net8.0
prerequisites: [terminal-basics, git-basics]
next-phase: ../02-oop/README.md
---

# C# Fundamentals

> Build a precise mental model of C# and .NET before moving into object-oriented design and higher-level framework APIs.

This phase is both a guided course and a long-term reference. It combines detailed reading, small runnable examples, progressive exercises, automated tests, and optional performance labs. The examples favor clarity and observable behavior. Deeper runtime and optimization topics are introduced here, then developed fully in [Phase 04 — Memory & Performance](../04-memory-performance/README.md).

## Learning outcomes

After completing this phase, you should be able to:

- distinguish C#, the .NET SDK, the runtime, CLR, IL, JIT compilation, assemblies, namespaces, and the Base Class Library;
- select appropriate built-in types and explain value semantics versus reference semantics;
- explain why `var` remains statically typed and when `dynamic` moves checks to runtime;
- use operators, branching, loops, pattern matching, and guard clauses clearly;
- design methods with explicit inputs, outputs, validation, and error behavior;
- use `ref`, `out`, `in`, optional parameters, named arguments, overloads, and tuples intentionally;
- choose among arrays, `List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`, and `IEnumerable<T>`;
- compare and normalize strings correctly and use `StringBuilder` when repeated mutation justifies it;
- use nullable annotations and flow analysis to express null contracts;
- explain managed memory, garbage collection, disposal, boxing, and allocation at an introductory level;
- validate behavior with tests and interpret a microbenchmark cautiously.

## Prerequisites and SDK policy

- Basic terminal and Git familiarity.
- The SDK selected by the repository-level [`global.json`](../global.json).
- An editor with C# support: Visual Studio, VS Code with C# Dev Kit, or JetBrains Rider.

The projects target .NET 8 so the learning material remains usable on the current long-term-support baseline. The SDK may be newer because it can build earlier target frameworks. Do not confuse the installed SDK version with a project's target framework.

## Recommended learning path

| Step | Topic | Main questions | Practice |
|---:|---|---|---|
| 0 | [Module roadmap](docs/00-roadmap.md) | How should this phase be studied? | Choose a schedule and verify the toolchain |
| 1 | [.NET ecosystem](docs/01-dotnet-ecosystem.md) | What turns C# source into a running process? | Build and inspect the produced assembly |
| 2 | [Variables and types](docs/02-variables-types.md) | What is known at compile time and runtime? | `VariablesExample`, `DynamicVsTypedExample` |
| 3 | [Operators and control flow](docs/03-operators-control-flow.md) | How should decisions and repetition be expressed? | If/else, switch, loops, patterns |
| 4 | [Methods](docs/04-methods.md) | How do method signatures communicate contracts? | Parameters, overloads, tuples, validation |
| 5 | [Collections](docs/05-collections.md) | Which access pattern does each collection optimize? | Arrays, lists, dictionaries, sets, enumeration |
| 6 | [Strings](docs/06-strings.md) | How do immutability, comparison, and formatting interact? | String APIs and `StringBuilder` |
| 7 | [Nullability](docs/07-nullability.md) | How can null intent be made explicit? | Annotations, operators, guards, flow analysis |
| 8 | [Memory fundamentals](docs/08-memory.md) | What are value/reference semantics, GC, and disposal? | Memory examples and optional labs |
| 9 | [Common pitfalls](docs/common-pitfalls.md) | Which beginner assumptions create subtle defects? | Review, explain, and repair each example |

## How to study each topic

Use the same loop for every lesson:

1. Read the topic document and write down the mental model in your own words.
2. Open the matching source example and predict its output before running it.
3. Run the example, then change one assumption or boundary value.
4. Read the tests as executable specifications. Add a boundary case of your own.
5. Solve the related exercise without copying the existing implementation.
6. Revisit the common-pitfalls guide and explain the trade-off, not merely the rule.

The goal is not to memorize syntax. It is to understand what the compiler guarantees, what the runtime decides, and what contract another developer can infer from the code.

## Project structure

```text
01-csharp-basics/
├── docs/
│   ├── 00-roadmap.md
│   ├── 01-dotnet-ecosystem.md
│   ├── 02-variables-types.md
│   ├── 03-operators-control-flow.md
│   ├── 04-methods.md
│   ├── 05-collections.md
│   ├── 06-strings.md
│   ├── 07-nullability.md
│   ├── 08-memory.md
│   └── common-pitfalls.md
├── src/
│   ├── CSharpBasics.Examples/
│   │   ├── Variables/
│   │   ├── ControlFlow/
│   │   ├── Methods/
│   │   ├── Collections/
│   │   ├── Strings/
│   │   ├── Nullability/
│   │   └── Memory/
│   ├── CSharpBasics.ConsoleApp/   # Runs the curated examples
│   └── CSharpBasics.Playground/   # Disposable experiments
├── exercises/
│   └── CSharpBasics.Exercises/
│       ├── Easy/
│       ├── Medium/
│       └── Hard/
├── tests/CSharpBasics.Tests/
├── benchmarks/
│   ├── StringBenchmark/
│   ├── CollectionsBenchmark/
│   └── MemoryBenchmarks/
├── 01-csharp-basics.slnx
└── README.md
```

The older `.sln` file is retained for compatibility. The `.slnx` file is the canonical solution used by the commands in this guide.

## Build, run, and test

Run these commands from `01-csharp-basics`:

```powershell
# Restore all NuGet dependencies in the phase.
dotnet restore 01-csharp-basics.slnx

# Compile examples, exercises, tests, and benchmark projects.
dotnet build 01-csharp-basics.slnx --no-restore

# Execute the automated behavior checks.
dotnet test 01-csharp-basics.slnx --no-build

# Run the curated demonstration sequence.
dotnet run --project src/CSharpBasics.ConsoleApp

# Run completed exercise implementations with sample inputs.
dotnet run --project exercises/CSharpBasics.Exercises

# Use the playground for temporary experiments.
dotnet run --project src/CSharpBasics.Playground
```

You can also run the commands from the repository root by prefixing project and solution paths with `01-csharp-basics/`.

## Topic map

### 1. The .NET execution model

The first lesson separates language, toolchain, and runtime responsibilities. It follows the path from C# source through Roslyn compilation to IL in an assembly, then through runtime loading and JIT compilation to native instructions. This vocabulary becomes essential when diagnosing build, deployment, compatibility, and performance problems later.

Key distinctions:

- C# is a language; .NET is a platform and runtime ecosystem.
- An SDK provides build tooling and can target compatible earlier frameworks.
- A target framework declares the APIs and runtime contract expected by a project.
- An assembly contains IL and metadata; it is not merely a renamed native executable.
- The CLR provides services such as type safety, exception handling, garbage collection, and JIT compilation.

### 2. Variables and the type system

This section covers numeric types, `bool`, `char`, `string`, arrays, enums, structs, classes, nullable value types, constants, conversions, type inference, and runtime type inspection.

Important mental models:

- `var` asks the compiler to infer a concrete static type; it does not make a variable dynamic.
- `dynamic` delays member binding until runtime and therefore changes when errors are detected.
- A value type has value-copy semantics. Its storage location depends on context and runtime optimization; “value type means stack” is not a reliable rule.
- A reference-type variable contains a reference that may identify an object on the managed heap or be `null`.
- `decimal` is usually suitable for base-10 financial arithmetic; `double` is usually suitable for scientific and general floating-point calculations.

### 3. Operators and control flow

The examples progress from arithmetic and comparison through short-circuit logic, null-coalescing operators, branching, loops, switch expressions, relational patterns, property patterns, and guard clauses.

Prefer control flow that makes boundary conditions visible. A compact expression is useful only when its intent remains obvious. Exhaustive enum handling and explicit fallback behavior often communicate more than a clever chain of conditions.

### 4. Methods and parameter design

Method signatures are contracts. The material covers return types, validation, optional and named arguments, overload resolution, `params`, local functions, recursion, tuple returns, and the `ref` family.

Use parameter modifiers deliberately:

- `ref` exposes caller-owned storage for reading and writing;
- `out` represents an additional output and is common in the `Try...` pattern;
- `in` passes a readonly reference and is mainly useful after measurement for sufficiently large structs;
- tuples work well for small, local groups of results, while a named type communicates a reusable domain concept better.

### 5. Collections

Collection choice should follow access patterns:

| Need | Typical starting point | Important cost |
|---|---|---|
| Fixed-size indexed data | Array | Resizing requires another container |
| Ordered, resizable sequence | `List<T>` | Insert/remove near the front shifts elements |
| Lookup by unique key | `Dictionary<TKey, TValue>` | Requires stable equality and hash behavior |
| Unique values or membership tests | `HashSet<T>` | Does not represent sequence order as a contract |
| Read-only iteration contract | `IEnumerable<T>` | May be lazy and may enumerate more than once |

Complexity describes growth, not an exact duration. Input size, allocation, locality, comparer cost, and runtime behavior still matter.

### 6. Strings

Strings are immutable sequences of UTF-16 code units. The lessons cover literals, interpolation, escaping, common transformations, comparison modes, parsing, splitting, joining, interning, and repeated construction.

Specify comparison semantics explicitly at system boundaries. `StringComparison.Ordinal` and `OrdinalIgnoreCase` are common for identifiers and protocol-like values; culture-aware comparisons are appropriate for human-language text. Use `StringBuilder` for sufficiently large or repeated mutation, but keep simple interpolation or a small fixed concatenation when it is clearer.

### 7. Nullability

Nullable reference types add compile-time annotations and flow analysis; they do not change the runtime representation of reference types. The examples cover `?`, `?.`, `?[]`, `??`, `??=`, pattern matching, guards, `TryParse`, and the null-forgiving operator.

Treat `!` as an assertion to the compiler, not a runtime null check. Prefer APIs whose signatures communicate whether absence is expected, and validate untrusted input at boundaries.

### 8. Memory and lifetime fundamentals

This phase introduces value/reference semantics, stack frames, the managed heap, garbage collection, boxing, string interning, and deterministic disposal. These concepts are intentionally revisited with more rigor in Phase 04.

Avoid two common oversimplifications:

- value types are not guaranteed to live on the stack; they can be fields inside heap objects, array elements, boxed values, or optimized into registers;
- garbage collection manages memory, while `IDisposable` represents deterministic release of resources or other lifetime-sensitive state.

## Exercises

Exercises are grouped by difficulty, but difficulty is secondary to the concept being practiced.

### Easy

- `SumNumbers`: iteration and `params`;
- `MaxOfThree`: conditional reasoning and boundaries;
- `MethodBasics`: method input and fallback behavior;
- `EvenOdd`: arithmetic predicates;
- `TemperatureConverter`: numeric conversion;
- `SimpleLoop`: validated range generation;
- `VariableTypes`: runtime type information.

### Medium

- `ReverseString`: array conversion and reversal;
- `Palindrome`: normalization and two-pointer comparison;
- `CountWords`: tokenization and dictionary counting;
- `FibonacciSequence`: iterative sequence construction;
- `PrimeNumbers`: sieve-based filtering;
- `RemoveDuplicates`: set semantics through LINQ;
- `NullDisplay`: nullable input and normalization.

### Hard

- `BasicCalculator`: switch expressions, validation, and error contracts;
- `NestedCollections`: flattening, uniqueness, and ordering;
- `StudentReport`: collections plus nullable values;
- `MemoryBucket`: value-copy and shared-reference behavior.

For deliberate practice, hide the implementation, preserve the public method signature, write at least one boundary test, and then compare approaches. Passing the existing tests is the baseline, not proof that every possible input has been specified.

## Tests as executable documentation

The test project covers examples and exercises. Read a test using Arrange–Act–Assert:

- Arrange describes the relevant initial state and input.
- Act performs one observable behavior.
- Assert states the contract in a form future changes must preserve.

Good additions include empty input, a single element, duplicate values, numeric boundaries, invalid arguments, and null when the signature permits it. Avoid testing implementation details that callers cannot observe.

## Benchmark labs

The benchmark folders are optional learning labs. Run them in Release mode on an otherwise quiet machine:

```powershell
dotnet run -c Release --project benchmarks/StringBenchmark
dotnet run -c Release --project benchmarks/CollectionsBenchmark
dotnet run -c Release --project benchmarks/MemoryBenchmarks
```

Treat a stopwatch demo as a demonstration, not statistically rigorous evidence. Warm-up, tiered JIT compilation, dead-code elimination, GC activity, CPU frequency, and background processes can all affect results. Phase 04 introduces BenchmarkDotNet and a more disciplined measurement workflow.

## Code and documentation conventions

- One focused concept per example file.
- Public teaching methods expose behavior that tests can verify.
- Comments explain intent, surprising runtime behavior, or educational trade-offs; they do not repeat self-explanatory syntax.
- Examples may show a deliberately inferior approach when comparison is the lesson, but the trade-off must be labeled.
- Nullable annotations remain enabled repository-wide.
- Warnings are treated as errors during normal builds.
- Markdown lessons use YAML front matter with `title`, `description`, `phase`, `order`, and `topics`.
- Relative links must work when rendered on GitHub.

Before committing changes to this phase:

```powershell
dotnet format 01-csharp-basics.slnx --verify-no-changes
dotnet build 01-csharp-basics.slnx
dotnet test 01-csharp-basics.slnx --no-build
```

## Common pitfalls checklist

Before moving on, make sure you can explain why each item can fail:

- using `dynamic` when the shape is known at compile time;
- assuming every value type is stored on the stack;
- relying on default string comparison semantics at a boundary;
- repeatedly concatenating a growing string in a large loop;
- modifying a collection during `foreach` enumeration;
- using an unstable `GetHashCode()` implementation for dictionary keys;
- ignoring nullable warnings or suppressing them with `!` without proof;
- catching `Exception` without a recovery strategy;
- treating one benchmark run as a general performance conclusion;
- assuming GC also provides timely release of files, sockets, or handles.

See [Common C# Pitfalls](docs/common-pitfalls.md) for worked examples and repairs.

## Completion checklist

- [ ] Read every topic document in order and answer its review questions.
- [ ] Run the console app and predict the major output sections.
- [ ] Reimplement at least one Easy, one Medium, and one Hard exercise.
- [ ] Add at least three meaningful boundary tests.
- [ ] Explain `var` versus `dynamic` without using the phrase “both infer a type.”
- [ ] Explain value versus reference semantics without equating them directly to stack versus heap.
- [ ] Choose a collection for three different access patterns and justify the choice.
- [ ] Explain the difference between garbage collection and deterministic disposal.
- [ ] Run the complete phase test suite successfully.

## Next phase

Continue with [Phase 02 — Object-Oriented Programming](../02-oop/README.md), where these language fundamentals are used to design types that protect invariants and expose clear behavior.
