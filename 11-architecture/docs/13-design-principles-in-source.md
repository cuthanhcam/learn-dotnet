---
title: "Design Principles in the Enrollment Source"
description: "Read SOLID, DRY, KISS, and YAGNI as concrete responsibility and contract decisions, with source pointers and review exercises."
slug: architecture-design-principles-in-source
phase: 11
order: 13
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 15
topics: [solid, maintainability, contracts, code-review]
prerequisites: [architecture-decisions-fitness-maintainability]
status: maintained
last-reviewed: 2026-10-30
---

# Design Principles in the Enrollment Source

Names such as SOLID are prompts for a design discussion, not a score obtained by adding interfaces.
Start with the [source tour](12-guided-source-tour.md) and one requirement: concurrent enrollments must
respect capacity, and a notification failure must not undo a committed enrollment.

## Trace SOLID Through Actual Responsibilities

| Principle | Concrete decision | Review question |
|---|---|---|
| Single responsibility | The aggregate decides membership/capacity; the handler coordinates ports; persistence owns SQL; the endpoint owns HTTP mapping | Would changing an HTTP status force a domain change? |
| Open/closed | Another store can satisfy the application port without changing the handler | Is this a real variation point, or are we inventing extensions? |
| Liskov substitution | Dictionary, SQLite, and PostgreSQL stores must obey the same snapshot/version contract | Does replacing the adapter preserve rejection and cancellation behavior? |
| Interface segregation | Writes use `ICourseOfferingStore`; catalog reads use `IOfferingQueries` | Does the consumer need every operation it depends on? |
| Dependency inversion | Application owns ports; infrastructure implements them; hosts compose the implementations | Which project would need to reference EF if the boundary were reversed? |

Read [CourseOffering](../src/Learning.Architecture.Domain/Courses/CourseOffering.cs),
[EnrollLearnerHandler](../src/Learning.Architecture.Application/Enrollments/EnrollLearner.cs), and
[the store contract](../src/Learning.Architecture.Application/Abstractions/ICourseOfferingStore.cs) in that
order. Then inspect [the EF adapter](../src/Learning.Architecture.Infrastructure/Persistence/EfCourseOfferingStore.cs)
and [the endpoint](../src/Learning.Architecture.Api/Features/EnrollmentEndpoints.cs).

Single responsibility does not mean one statement per class. The handler's load/decide/save sequence
is one orchestration responsibility. Open/closed does not forbid editing a class when its requirement
changes: changing a business rule belongs in the domain rather than another unnecessary plugin hook.
Dependency injection is the wiring technique; dependency inversion describes ownership/direction.

## Substitution Needs Behavioral Evidence

The store port is stronger than matching method signatures. A save advances one version, preserves
immutable offering metadata, rejects stale snapshots, and does not silently insert a missing offering.
A SQL adapter that accepts a changed title while the dictionary adapter rejects it violates the
shared promise even though both compile.

Run the [shared contracts](../tests/Learning.Architecture.Tests/Contracts/CourseOfferingStoreContractTests.cs):

```powershell
dotnet test 11-architecture/tests/Learning.Architecture.Tests --configuration Release --filter FullyQualifiedName~CourseOfferingStoreContractTests
pwsh -NoProfile -File scripts/Test-PostgresProvider.ps1
```

The first command uses dictionary/SQLite. The second requires Docker and reuses those contract
specifications against PostgreSQL. See the [provider lesson](../../12-testing/docs/05-postgresql-provider-tests.md)
for its additional connection/transaction cases and migration limits. These tests give evidence for
the specified behaviors; they do not prove equivalence for every provider failure or deployment.

## DRY Without Coupling Unrelated Boundaries

The aggregate is the authority for capacity and learner uniqueness. Avoid repeating those rules in
controllers, SQL mapping helpers, and UI code. The handler asks the aggregate for the decision.

Validation at multiple boundaries can still be appropriate: transport checks request shape, application
checks identifiers/ranges, domain protects invariants, and the database constrains stored data.
Those are different responsibilities and trust boundaries. Removing a database check merely because
the domain also validates capacity weakens defense against direct persistence writes.

The baseline and refactored GoF examples intentionally coexist for comparison. That duplication is
part of a lesson, not a production recommendation to maintain two competing implementations forever.
Shared contract source is linked across test projects because it expresses one common promise; the
fixture-specific setup remains separate because the providers have different ownership requirements.

## KISS and YAGNI as Scope Decisions

The application uses a direct command handler, explicit project references, and small composition
roots. There is no generic repository hierarchy, mediator requirement, reflection-based registration,
or network service per layer. Add one only when a concrete change makes its benefits reviewable.

Missing production capabilities are not automatically unnecessary. Migrations, retention, distributed
worker leases, and outage recovery matter when the deployment requires them. Here they remain visible
extensions in the [evidence audit](09-completion-audit.md). YAGNI is not permission to label them delivered
or omit invariants that the current scenario already promises.

## Review One Change

For a proposed enrollment change, identify the rule owner, affected caller contract, persistence and
transaction effect, failure/cancellation behavior, and evidence that could reject an incorrect version.
Prefer a small feature-focused change. Explain a non-obvious invariant near its implementation rather
than adding comments that repeat every assignment.

Exercise: propose a waitlist after capacity is reached. Before coding, specify whether it consumes a
seat, whether repeated request keys replay the original result, and whether notification intent commits
with waitlist state. Acceptance is a written state/ownership model and tests that distinguish waiting,
enrolled, duplicate, and stale-write outcomes. A new folder alone is not a completed feature.

## References and Navigation

- [Microsoft architectural principles](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles)
- Related: [Architecture decisions and fitness](07-decisions-fitness-maintainability.md)
- Return: [Phase 11 study path](../README.md)
