---
title: "Phase 03 — Core .NET and the Standard Library"
description: "A detailed guide to collections, generics, exceptions, LINQ, delegates, events, files, time, attributes, reflection, and nullable API contracts."
phase: 3
status: complete
target-framework: net8.0
prerequisites: [phase-01-csharp-fundamentals, phase-02-oop]
previous-phase: ../02-oop/README.md
next-phase: ../04-memory-performance/README.md
---

# Core .NET and the Standard Library

> Learn the library contracts and execution behaviors used in everyday .NET applications.

Phase 01 introduced language fundamentals and Phase 02 focused on object design. This phase connects those skills to the .NET Base Class Library: choosing collection contracts, designing generic APIs, composing LINQ queries, handling failures, representing callbacks and events, managing files and streams, working with time, reading metadata, and expressing nullability.

## Learning outcomes

After completing this phase, you should be able to:

- select a collection by access pattern, ordering, uniqueness, and mutation requirements;
- design generic types and methods with useful constraints and correct variance;
- distinguish expected result states from exceptional failures;
- preserve exception context and place recovery at an appropriate boundary;
- predict deferred LINQ execution, materialization, repeated enumeration, and side effects;
- use delegates, lambdas, closures, and events while managing subscriber lifetime;
- read and write files with explicit encoding, ownership, disposal, and path boundaries;
- choose among `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, and `TimeSpan`;
- convert time zones without assuming local machine configuration;
- define and inspect attributes while accounting for reflection cost;
- design nullable APIs whose annotations match runtime behavior;
- measure before optimizing and separate algorithmic improvements from micro-optimizations.

## Prerequisites

- [Phase 01 — C# Fundamentals](../01-csharp-basics/README.md)
- [Phase 02 — Object-Oriented Programming](../02-oop/README.md)
- The SDK selected by the repository-level [`global.json`](../global.json)

Projects target .NET 8. A newer compatible SDK may build that target framework; the SDK version and target framework describe different parts of the toolchain.

## Study path

| Order | Lesson | Questions to answer |
|---:|---|---|
| 0 | [Roadmap](docs/00-roadmap.md) | How should the phase be practiced and verified? |
| 1 | [Collections](docs/01-collections.md) | Which data structure matches the access pattern? |
| 2 | [Generics](docs/02-generics.md) | Which requirements belong in compile-time constraints? |
| 3 | [Exception handling](docs/03-exception-handling.md) | Where can a failure be recovered or translated? |
| 4 | [LINQ](docs/04-linq.md) | When does a query execute, and how often? |
| 5 | [Delegates and events](docs/05-delegates-events.md) | Who invokes the callback and owns the subscription? |
| 6 | [File I/O and streams](docs/06-file-io.md) | Who owns the resource, encoding, path, and failure policy? |
| 7 | [Date, time, and time zones](docs/07-datetime-timezone.md) | Is this an instant, local civil time, date, or duration? |
| 8 | [Attributes and metadata](docs/08-attributes.md) | Is metadata declarative, and when is it inspected? |
| 9 | [Nullable reference types](docs/09-nullable-reference-types.md) | Does the signature state the real null contract? |
| 10 | [Performance guidelines](docs/performance-guidelines.md) | What evidence justifies an optimization? |
| 11 | [Common pitfalls](docs/common-pitfalls.md) | Which hidden behavior makes the code incorrect? |

## Project structure

```text
03-core-dotnet/
├── docs/                         # Ordered deep-dive lessons
├── src/
│   ├── CoreDotNet.Examples/      # One focused example group per topic
│   └── CoreDotNet.ConsoleApp/    # Curated runnable demonstration
├── tests/CoreDotNet.Tests/       # Behavioral and smoke tests
├── benchmarks/CoreDotNet.Benchmarks/ # BenchmarkDotNet LINQ labs
├── 03-core-dotnet.slnx
└── README.md
```

This tree intentionally describes the filesystem that exists. New exercise projects must be added to the solution and test project rather than documented as hypothetical folders.

## Build, run, and test

From `03-core-dotnet`:

```powershell
dotnet restore 03-core-dotnet.slnx
dotnet build 03-core-dotnet.slnx --no-restore
dotnet test 03-core-dotnet.slnx --no-build
dotnet run --project src/CoreDotNet.ConsoleApp
```

Run BenchmarkDotNet labs only in Release mode:

```powershell
dotnet run -c Release --project benchmarks/CoreDotNet.Benchmarks
```

Use filters during study so one benchmark family runs at a time. Results depend on hardware, runtime, data shape, and configuration.

## Core mental models

### Collections are contracts plus costs

Program to the narrowest useful abstraction at API boundaries, but choose a concrete implementation based on required behavior. `List<T>` provides indexed ordered storage, `Dictionary<TKey,TValue>` maps unique keys, `HashSet<T>` models uniqueness, and queue/stack types make processing order explicit. Big-O describes growth; comparer cost, allocation, data locality, and input size still affect real performance.

Equality is part of collection correctness. Dictionary and set keys require equality and hash codes that remain stable while stored. Exposing `IEnumerable<T>` communicates iteration, not count, indexing, repeatability, or deferred execution.

### Generics encode reusable type relationships

Generics preserve static type information and avoid casts. Constraints should express operations the implementation genuinely requires. Covariance supports safe output substitution; contravariance supports safe input substitution. Invariance is correct when a type both consumes and produces `T`.

Avoid creating a generic abstraction merely because syntax permits it. A useful generic API captures behavior shared across types without erasing domain meaning.

### Exceptions represent failed operations

Throw exceptions when an operation cannot honor its contract. Validate public arguments near the boundary, catch only where the code can recover, add context, translate to another boundary contract, or perform required cleanup. Preserve the original stack trace with `throw;`, not `throw ex;`.

Do not use exceptions as the normal result of a high-frequency expected branch when a `Try...` API or explicit result type communicates the outcome better. Do not swallow an exception merely to keep a process running in an unknown state.

### LINQ describes a query pipeline

Most operators over `IEnumerable<T>` are deferred: creating a query does not consume its source. Enumeration executes the pipeline, and repeated enumeration can repeat work or observe changed state. Materialize with `ToList`, `ToArray`, or another terminal operation when a stable snapshot is required.

Keep side effects outside query operators. Select an algorithm first: replacing nested scans with a lookup can matter far more than replacing readable LINQ with a manual loop.

### Delegates carry behavior; events restrict publication

A delegate is a type-safe reference to callable behavior. Lambdas can capture variables; captured state may extend lifetime and can create surprising mutation. An event exposes subscription and unsubscription while reserving invocation for the publisher.

Long-lived publishers can retain subscribers through delegate references. Unsubscribe deterministically when subscriber lifetime is shorter, or design ownership so the publisher cannot outlive the subscriber unexpectedly.

### Files and streams require explicit ownership

Paths are data from a trust boundary. Normalize and validate user-controlled paths before combining them with an allowed root. Specify text encoding when interoperability matters. Dispose streams deterministically, avoid loading unbounded files entirely into memory, and distinguish a successful write call from an atomic or durable update.

### Time requires a semantic type

Use `DateTimeOffset` for an instant with an offset, UTC for storage and cross-system exchange, `DateOnly` for a calendar date, `TimeOnly` for a wall-clock time, and `TimeSpan` for a duration. A time-zone identifier and rules are required to interpret recurring local civil time. Daylight-saving transitions create ambiguous and invalid local times.

Inject `TimeProvider` or a domain clock when behavior depends on the current time. Tests should not race the wall clock.

### Attributes are metadata, not automatic behavior

An attribute records metadata. A framework or your code must inspect it and decide what it means. Reflection is powerful but bypasses some compile-time discoverability, may allocate, and should often be cached when used in a hot path.

### Nullability is an API contract

Nullable reference types provide compile-time annotations and flow analysis; they do not add a runtime null check. A `string?` return explicitly permits absence. A non-nullable return promises callers a value, so implementations and tests must honor that promise. Use `!` only when external reasoning proves what the compiler cannot see.

## Practice workflow

For every topic:

1. Read the lesson and write down the relevant API contract.
2. Predict the example output and deferred side effects.
3. Run the console example and change one boundary condition.
4. Read the tests as executable specifications.
5. Add a test for empty, invalid, repeated, culture-sensitive, or time-sensitive input.
6. Explain the correctness trade-off before discussing performance.

## Testing guidance

Useful tests in this phase verify:

- collection ordering, equality, duplicate handling, and empty input;
- generic constraint behavior through concrete implementations;
- exact exception type, parameter name, and preserved state;
- LINQ behavior before and after materialization;
- event count, payload, and unsubscription;
- file content, encoding, cleanup, and invalid paths using isolated temporary directories;
- fixed clock behavior and time-zone edge cases;
- attribute discovery and missing metadata;
- nullable fallback and validation contracts.

Tests that touch global console output or filesystem state need isolation. The existing test suite uses console coordination and temporary locations to avoid cross-test interference.

## Performance checklist

- Measure a representative workload before changing code.
- Compare algorithms and data structures before syntax-level rewrites.
- Include warm-up and use BenchmarkDotNet for microbenchmarks.
- Confirm that deferred queries are not enumerated accidentally.
- Avoid reflection discovery on every request when metadata can be cached.
- Stream large data rather than loading it all when the consumer supports streaming.
- Record input size, runtime version, environment, and benchmark configuration.

## Completion checklist

- [ ] Choose collections for at least five access patterns and justify each choice.
- [ ] Explain generic covariance, contravariance, and invariance with one API example each.
- [ ] Preserve stack traces and place an exception translation at a real boundary.
- [ ] Demonstrate deferred LINQ execution and deliberate materialization.
- [ ] Build an event publisher and prove unsubscription with a test.
- [ ] Read and write a temporary file with explicit encoding and deterministic cleanup.
- [ ] Model an instant, a civil date, and a duration with appropriate types.
- [ ] Read a custom attribute and explain who gives it behavior.
- [ ] Design a nullable signature that matches runtime behavior.
- [ ] Pass `dotnet test 03-core-dotnet.slnx`.

## Next phase

Continue with [Phase 04 — Memory and Performance](../04-memory-performance/README.md) to deepen the allocation, GC, span, pooling, profiling, and benchmarking models introduced here.
