---
title: "Phase 10 — Object Design and GoF Patterns"
description: "Learn object collaboration through focused baseline-to-refactor examples, contract tests, modern C# alternatives, and explicit trade-offs."
phase: 10
status: in-progress
target-framework: net10.0
prerequisites: [phase-02-oop, phase-06-async-concurrency]
previous-phase: ../09-auth/README.md
next-phase: ../11-architecture/README.md
---

# Object Design and GoF Patterns

This independent phase studies collaboration between classes and objects. System/module boundaries
belong to [Software Architecture](../11-architecture/README.md), the next phase. Read one complete
lesson, run its named demo, inspect its tests, and
only then move to the next pattern. The catalog is being delivered incrementally; planned entries are
not implemented examples.

## Start Here

1. Read the [guided roadmap and delivery ledger](docs/00-roadmap.md).
2. Read [pattern thinking and selection](docs/01-pattern-thinking-and-selection.md).
3. Study [Strategy](docs/behavioral/strategy.md), the first complete lesson.
4. Continue with [Factory Method](docs/creational/factory-method.md) and its vocabulary comparison.
5. Study [Adapter](docs/structural/adapter.md) for a semantic SDK integration boundary.
6. Study [Decorator](docs/structural/decorator.md) for wrapper order, bounded retry, and required audit.
7. Study [Observer](docs/behavioral/observer.md) for subscription ownership, terminal signals, and callback failures.
8. Use the catalog/problem index in the roadmap to find delivered alternatives and remaining topics.

## Run One Lesson

From the repository root:

```powershell
dotnet restore 10-design-patterns/10-design-patterns.slnx --locked-mode
dotnet run --project 10-design-patterns/src/Learning.Patterns.ConsoleApp -- --list
dotnet run --project 10-design-patterns/src/Learning.Patterns.ConsoleApp -- --pattern behavioral.strategy
dotnet run --project 10-design-patterns/src/Learning.Patterns.ConsoleApp -- --pattern creational.factory-method
dotnet run --project 10-design-patterns/src/Learning.Patterns.ConsoleApp -- --pattern structural.adapter
dotnet run --project 10-design-patterns/src/Learning.Patterns.ConsoleApp -- --pattern structural.decorator
dotnet run --project 10-design-patterns/src/Learning.Patterns.ConsoleApp -- --pattern behavioral.observer
dotnet test 10-design-patterns/solutions/behavioral.slnx --configuration Release
dotnet test 10-design-patterns/solutions/creational.slnx --configuration Release
dotnet test 10-design-patterns/solutions/structural.slnx --configuration Release
```

The runner shows help by default. It never executes an entire growing catalog without an explicit
selection, and unknown arguments return exit code 2. The behavioral solution opens only its library
and tests; the catalog solution also includes the demo host. The root `learn-dotnet.slnx` remains the
complete maintained workspace.

## Delivered Structure

```text
10-design-patterns/
  10-design-patterns.slnx
  solutions/behavioral.slnx
  solutions/creational.slnx
  solutions/structural.slnx
  src/
    Learning.Patterns.Behavioral/
      Strategy/
        Baseline/          # Correct switch-based implementation
        Refactored/        # Named policies, context, explicit selection
        ModernCSharp/      # Delegate alternative
        PricingRequest.cs
        PricingQuote.cs
        StrategyDemo.cs
      Observer/
        Baseline/          # Fixed pair of known consumers
        Refactored/        # Disposable bounded subscriptions and explicit terminal/failure rules
        Observers/         # Latest-price display and threshold counter
        ModernCSharp/      # Smaller event-based alternative
        PriceUpdate.cs
        ObserverDemo.cs
    Learning.Patterns.ConsoleApp/
    Learning.Patterns.Creational/FactoryMethod/
      Baseline/            # Closed-list selection in the original workflow
      Products/            # Text/JSON formatter contracts and implementations
      Refactored/          # Creator workflow and subclass creation hook
      Alternatives/        # Constructor-injection comparison
    Learning.Patterns.Structural/Adapter/
      Contracts/           # Client-owned request, result, and capability
      Legacy/              # Incompatible SDK types and offline simulator
      Baseline/            # Application coupled directly to provider details
      Refactored/          # Translation of units, outcomes, failures, and cancellation
    Learning.Patterns.Structural/Decorator/
      Baseline/            # Correct fixed retry-then-audit workflow
      Contracts/           # Required audit and retry-delay boundaries
      Refactored/          # Independent wrappers with observable composition order
      DecoratorDemo.cs     # Offline adapter plus controlled audit/delay collaborators
  tests/Learning.Patterns.Behavioral.Tests/Strategy/
  tests/Learning.Patterns.Behavioral.Tests/Observer/
  tests/Learning.Patterns.Creational.Tests/FactoryMethod/
  tests/Learning.Patterns.Structural.Tests/Adapter/
  tests/Learning.Patterns.Structural.Tests/Decorator/
```

All three categories now have a complete first lesson, focused solution, and test project. There are
seven runnable projects and 133 pattern specifications; the remaining 18 GoF patterns are planned, not
scaffolded placeholders. Articles live under
`10-design-patterns/docs/`, with metadata checked by the
existing publishing gates. Each new pattern has its own topic branch and focused code/docs commits.

## Learning Contract

Each lesson includes a concrete requirement, a correct baseline, change pressure, named collaborators,
execution diagram where helpful, failure/ownership rules, deterministic tests, trade-offs, and exercises.
Catalog types are teaching examples, not a framework for other phases to depend on.

The goal is complete understanding of all 23 GoF patterns, not maximum class count. Repository, CQRS,
Outbox/Inbox, and Clean Architecture remain distinct concepts in the [architecture track](../11-architecture/README.md).
See the [approved curriculum blueprint](../docs/design-patterns-curriculum-plan.md) for delivery gates.
