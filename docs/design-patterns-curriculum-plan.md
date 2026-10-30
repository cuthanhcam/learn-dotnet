---
title: "Design Patterns Curriculum and Solution Blueprint"
description: "Define the independent GoF phase, its learning order, focused solutions, complete catalog, and delivery gates."
slug: advanced-design-patterns-curriculum-plan
phase: 0
order: 2
difficulty: advanced
article-type: roadmap
estimated-reading-minutes: 12
topics: [curriculum, design-patterns, gof, solution-structure]
prerequisites: [oop-composition-and-patterns]
status: reviewed
last-reviewed: 2026-10-31
---

# Design Patterns Curriculum and Solution Blueprint

## Placement Decision

Design Patterns is independent **Phase 10**, followed by **Phase 11 Software Architecture** and
**Phase 12 Testing**. The earlier nested 10B placement is superseded by this curriculum reorganization.
See the [path/number migration note](phase-reorganization.md) for older checkouts and saved links.

Design patterns address collaboration among classes and objects. Software architecture addresses
system/module responsibilities, dependencies, deployment and consistency trade-offs. They inform
one another but have distinct learning outcomes and separate solutions. The pattern catalog is not
a library that the enrollment architecture application must import.

Keep Phase 02 focused on the object-model foundation: encapsulation, composition, substitution,
equality, lifetime, and small introductory examples. Async and DI material are prerequisites when a
lesson uses those mechanisms. The complete 23-pattern catalog does not belong in a beginner chapter.

## Phase Ownership

| Phase | Owns |
|---|---|
| 02 OOP | language/object-model foundation and introductory pattern vocabulary |
| 03 Core .NET | DI, configuration, and runtime/lifetime mechanisms |
| 06 Async/concurrency | cancellation, synchronization, and concurrent state |
| 07 ASP.NET Core | concrete HTTP/middleware/framework applications |
| 08 EF Core | persistence behavior; Repository/Unit of Work distinguished from GoF |
| 09 Auth | security boundaries; a wrapper is not authorization by itself |
| 10 Design Patterns | construction, composition, object collaboration, all 23 GoF patterns |
| 11 Architecture | application/module boundaries, aggregates, transactions, delivery protocols, ADRs |
| 12 Testing | contracts, doubles, fixtures, coverage interpretation, provider tests |

Phase order is a guided path, not a demand to finish every reference entry before running the next
available phase. Learn the delivered pattern foundations before architecture; unfinished catalog
entries remain explicit in the [active ledger](../10-design-patterns/docs/00-roadmap.md).

## Catalog and Learning Sequence

| Category | Canonical scope |
|---|---|
| Creational | Abstract Factory, Builder, Factory Method, Prototype, Singleton |
| Structural | Adapter, Bridge, Composite, Decorator, Facade, Flyweight, Proxy |
| Behavioral | Chain of Responsibility, Command, Interpreter, Iterator, Mediator, Memento, Observer, State, Strategy, Template Method, Visitor |

Begin with pattern thinking, Strategy, Factory Method and its vocabulary comparison, Adapter, and
Decorator, then Observer. Command/State are next, followed by the remaining category lessons. Finish
with category comparisons, assessed exercises, a small refactoring case study, and a whole-catalog audit.

The current phase delivers five patterns and 133 executable cases. This is not a coverage percentage;
the other eighteen patterns and final comparative/case-study work remain planned. Do not create empty
directories, `NotImplementedException` examples, or skipped placeholder tests to imply delivery.

## Physical Structure

```text
10-design-patterns/
  README.md
  10-design-patterns.slnx
  docs/
    00-roadmap.md
    01-pattern-thinking-and-selection.md
    behavioral/
    creational/
    structural/
    comparisons/
  solutions/
    behavioral.slnx
    creational.slnx
    structural.slnx
  src/
    Learning.Patterns.Behavioral/
    Learning.Patterns.Creational/
    Learning.Patterns.Structural/
    Learning.Patterns.ConsoleApp/
  tests/
    Learning.Patterns.Behavioral.Tests/
    Learning.Patterns.Creational.Tests/
    Learning.Patterns.Structural.Tests/
```

Seven projects provide focused category views without creating one project per pattern. Root
`learn-dotnet.slnx` includes all maintained projects. Examples, test namespaces, and assembly names
remain concept-based; moving a phase is not a reason to rename public types or churn package locks.

Articles live in this phase's `docs/` and are checked by metadata/link/Markdown gates. Add exercises
or case-study projects only when actual runnable material justifies them. The tree above describes
delivered category infrastructure, not empty future pattern folders.

## Lesson and Delivery Contract

Each topic needs a concrete requirement, a correct baseline, actual change pressure, named roles,
an execution diagram where useful, explicit ownership/failure rules, a focused implementation map,
behavioral tests, runnable output, simpler alternatives, trade-offs, and exercises with acceptance criteria.
Explain modern C# alternatives when they change the design decision rather than merely the syntax.

Use one small branch per topic from verified `develop`, such as `feature/patterns-observer`.
Run the focused category suite and named demo, then the shared repository gate before push. Inspect
all hosted checks before merging into `develop`; collect verified slices in a promotion PR to `main`.
Dependency changes additionally require a complete lock update and the existing package audit.

Tests should assert observable contracts: copy isolation for Prototype/Memento, compatible families
for Abstract Factory, subscription lifetime for Observer, and order/cancellation for Decorator.
Use deterministic synchronization rather than timing sleeps. Bound parser/traversal/history/cache
resources where relevant. Keep providers simulated when object collaboration is the lesson; use
real provider integration tests when the external behavior itself is the subject.

The case study should preserve behavior while introducing a small justified subset of patterns.
Record rejected patterns and why. It is not an exercise in inserting every pattern into one app,
nor should it make unrelated phases depend on teaching examples as a shared framework.

## Whole-Phase Completion Gate

Completion requires all 23 source/test/article/demo entries, consistent guided/catalog/problem
indexes, comparison lessons, meaningful exercises, the refactoring case study, a final evidence
audit, and passing repository/hosted checks. Until then the phase remains in progress.

## References and Navigation

- [GoF book and canonical catalog](https://www.informit.com/store/design-patterns-elements-of-reusable-object-oriented-9780321770462)
- [Phase 10 entry point](../10-design-patterns/README.md)
- [Architecture boundary comparison](../11-architecture/README.md)
- [Active pattern delivery ledger](../10-design-patterns/docs/00-roadmap.md)
