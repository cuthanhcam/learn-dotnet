---
title: "GoF Patterns Guided Roadmap and Delivery Ledger"
description: "Navigate the advanced object-design track by learning sequence, canonical catalog, problem pressure, and topic-sized delivery branches."
slug: patterns-guided-roadmap
phase: 10
order: 100
difficulty: advanced
article-type: roadmap
estimated-reading-minutes: 15
topics: [design-patterns, gof, roadmap, learning-path]
prerequisites: [oop-composition-and-patterns, architecture-phase-10-completion-audit]
status: maintained
last-reviewed: 2026-10-10
---

# GoF Patterns Guided Roadmap and Delivery Ledger

## Scope and Prerequisites

Track 10A addresses application boundaries and module ownership. Track 10B addresses object construction
and collaboration inside those boundaries. Understand interfaces, composition, substitutability,
immutability, exceptions, and basic tests before starting. Async and DI chapters are prerequisites only
when a particular example actually needs them.

The track targets the complete 23-pattern GoF catalog. Delivery is incremental: a pattern becomes
delivered only after its examples, test cases, article, demo, and workspace integration pass validation.
A folder name or an interface with an unfinished method does not count.

## Guided Learning Path

1. [Pattern thinking and selection](01-pattern-thinking-and-selection.md).
2. [Strategy](behavioral/strategy.md): interchangeable algorithms and delegate comparison.
3. Factory Method: extensible creation in a real creator workflow (next planned topic).
4. Adapter: translate an incompatible provider contract without changing the client.
5. Decorator: compose behavior while preserving a capability contract.
6. Observer: subscriptions and lifecycle, not reliable cross-process delivery.
7. Command and State: explicit requests and legal transitions.
8. Complete the category catalogs and compare look-alike patterns.
9. Apply a small justified subset in a refactoring case study.

The order is pedagogical, not alphabetical or a claim that Strategy is required for every other pattern.
Once the foundations are clear, the catalog is also a reference index.

## Canonical Catalog and Current Status

| Category | Delivered | Planned |
|---|---|---|
| Creational (5) | none yet | Factory Method, Abstract Factory, Builder, Prototype, Singleton |
| Structural (7) | none yet | Adapter, Bridge, Composite, Decorator, Facade, Flyweight, Proxy |
| Behavioral (11) | Strategy | Chain of Responsibility, Command, Interpreter, Iterator, Mediator, Memento, Observer, State, Template Method, Visitor |

Do not mistake the number of patterns delivered for a coverage percentage of software design. Each
pattern has a different depth and failure surface. The completion audit will map all 23 to executable evidence.

## Problem Index

| Change pressure | Start with | First consider the simpler alternative |
|---|---|---|
| Algorithm varies independently | Strategy (delivered) | a small switch or delegate |
| Subclasses need control over products used by a workflow | Factory Method (planned) | constructor injection or a simple factory |
| Provider API does not match client expectations | Adapter (planned) | a direct mapping function for a tiny stable contract |
| Behavior must wrap an existing capability | Decorator (planned) | a direct explicit call sequence |
| Many operations need notification | Observer (planned) | a direct callback with clear lifetime |
| Behavior changes with legal lifecycle state | State (planned) | an enum and transition table |

This index should grow with delivered articles. Do not link to nonexistent pages or imply a planned
pattern has runnable code.

## Branch and Review Workflow

Each topic uses a branch such as `feature/10b-strategy`, `feature/10b-factory-method`, or
`feature/10b-adapter`. Keep pattern implementation/testing and its article/navigation in focused commits.
An integration branch, `feature/10b-design-patterns`, collects only verified slices. Develop/main are not
rewritten, and the existing architecture solution is not repurposed as the pattern catalog.

| Topic | Branch | Delivery evidence |
|---|---|---|
| Curriculum blueprint | `docs/design-patterns-curriculum` | approved placement, solution and article contracts |
| Strategy | `feature/10b-strategy` | baseline/interface/delegate, boundary tests, deterministic demo, detailed article |
| Factory Method | `feature/10b-factory-method` | planned |
| Adapter | `feature/10b-adapter` | planned |

Create the next topic from the verified integration tip, not from an unrelated unreviewed feature.
This avoids a long hidden dependency chain between topic PRs. When a topic introduces a category,
add only the library/test projects that now contain working material.

## Study Loop and Completion Gate

1. Read the original requirement and run the baseline mentally.
2. Identify the actual variation point and predict the smallest change.
3. Read the collaboration diagram and trace one request in source.
4. Run the named demo and compare baseline/refactored output.
5. Read failure and boundary tests before adding another implementation.
6. Solve an exercise and explain when you would remove the abstraction.

Before closing a topic: locked restore, Release build/tests, deterministic demo, formatting, metadata,
Markdown links/structure, and master inventory must pass. Package changes additionally require the
repository dependency audit. No skipped placeholder tests or unsafe executable counterexamples are used
to simulate completeness.

## References and Navigation

- [GoF canonical catalog](https://www.informit.com/store/design-patterns-elements-of-reusable-object-oriented-9780321770462).
- [Track entry point](../../design-patterns/README.md).
- [Curriculum blueprint](../../../docs/design-patterns-curriculum-plan.md).
- Next: [Pattern thinking and selection](01-pattern-thinking-and-selection.md).
