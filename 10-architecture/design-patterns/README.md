---
title: "Phase 10B — Object Design and GoF Patterns"
description: "Learn object collaboration through focused baseline-to-refactor examples, contract tests, modern C# alternatives, and explicit trade-offs."
phase: 10
status: in-progress
target-framework: net10.0
prerequisites: [phase-02-oop, phase-10-architecture]
---

# Object Design and GoF Patterns

This is the advanced object-design track inside Phase 10. The existing enrollment architecture example
remains separate and unchanged. Read one complete lesson, run its named demo, inspect its tests, and
only then move to the next pattern. The catalog is being delivered incrementally; planned entries are
not implemented examples.

## Start Here

1. Read the [guided roadmap and delivery ledger](../docs/design-patterns/00-roadmap.md).
2. Read [pattern thinking and selection](../docs/design-patterns/01-pattern-thinking-and-selection.md).
3. Study [Strategy](../docs/design-patterns/behavioral/strategy.md), the first complete lesson.
4. Continue with [Factory Method](../docs/design-patterns/creational/factory-method.md) and its vocabulary comparison.
5. Use the catalog/problem index in the roadmap to find delivered alternatives and remaining topics.

## Run One Lesson

From the repository root:

```powershell
dotnet restore 10-architecture/design-patterns/design-patterns.slnx --locked-mode
dotnet run --project 10-architecture/design-patterns/src/Learning.Patterns.ConsoleApp -- --list
dotnet run --project 10-architecture/design-patterns/src/Learning.Patterns.ConsoleApp -- --pattern behavioral.strategy
dotnet run --project 10-architecture/design-patterns/src/Learning.Patterns.ConsoleApp -- --pattern creational.factory-method
dotnet test 10-architecture/design-patterns/solutions/behavioral.slnx --configuration Release
dotnet test 10-architecture/design-patterns/solutions/creational.slnx --configuration Release
```

The runner shows help by default. It never executes an entire growing catalog without an explicit
selection, and unknown arguments return exit code 2. The behavioral solution opens only its library
and tests; the catalog solution also includes the demo host. The root `learn-dotnet.slnx` remains the
complete maintained workspace.

## Delivered Structure

```text
design-patterns/
  design-patterns.slnx
  solutions/behavioral.slnx
  src/
    Learning.Patterns.Behavioral/
      Strategy/
        Baseline/          # Correct switch-based implementation
        Refactored/        # Named policies, context, explicit selection
        ModernCSharp/      # Delegate alternative
        PricingRequest.cs
        PricingQuote.cs
        StrategyDemo.cs
    Learning.Patterns.ConsoleApp/
  tests/Learning.Patterns.Behavioral.Tests/Strategy/
```

Creational now contains the Factory Method lesson, its own category solution, and tests. Structural
projects will appear with their first complete lesson, not as empty shells. Articles live under
`10-architecture/docs/design-patterns/`, with metadata checked by the
existing publishing gates. Each new pattern has its own topic branch and focused code/docs commits.

## Learning Contract

Each lesson includes a concrete requirement, a correct baseline, change pressure, named collaborators,
execution diagram where helpful, failure/ownership rules, deterministic tests, trade-offs, and exercises.
Catalog types are teaching examples, not a framework for other phases to depend on.

The goal is complete understanding of all 23 GoF patterns, not maximum class count. Repository, CQRS,
Outbox/Inbox, and Clean Architecture remain distinct concepts in the [architecture track](../README.md).
See the [approved curriculum blueprint](../../docs/design-patterns-curriculum-plan.md) for delivery gates.
