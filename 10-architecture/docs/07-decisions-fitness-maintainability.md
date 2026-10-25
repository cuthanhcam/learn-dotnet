---
title: "Architecture Decisions, Fitness Checks, and Maintainability"
description: "Explain the selected project graph, enforce declared and compiled boundaries, and evaluate alternatives using change scenarios rather than template popularity."
slug: architecture-decisions-fitness-maintainability
phase: 10
order: 7
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 25
topics: [architecture-decisions, fitness-functions, maintainability, dependency-inversion]
prerequisites: [architecture-http-security-errors-versioning]
status: maintained
last-reviewed: 2026-10-10
---

# Architecture Decisions, Fitness Checks, and Maintainability

## A Folder Diagram Is Not a Decision Record

An architecture review needs context: what must remain correct, what changes are expected, which
constraints apply, and what alternatives were rejected. “We use clean architecture because it is a
best practice” does not explain why a repository interface exists or who owns a transaction.

This phase's decisions are driven by two requirements: capacity must remain correct under competing
snapshots, and a notification failure must not undo enrollment. These explain the aggregate boundary,
compare-and-save port, durable workflow, and versioned integration contract better than a pattern list.

Read the two concrete decision records:

- [ADR 0001: feature-oriented layers and a modular monolith](decisions/0001-feature-oriented-modular-monolith.md).
- [ADR 0002: conditional snapshots, receipts, and outbox](decisions/0002-conditional-snapshots-receipts-outbox.md).

Each states the status, context, decision, alternatives, consequences, and triggers for reconsideration.
Do not rewrite an accepted historical decision to imply it was always obvious. When conditions change,
add a superseding decision and link it to the older one.

## Structure by Responsibility and Feature

The main enrollment capability has Domain, Application, and Infrastructure projects. Inside each,
feature names such as Courses, Enrollments, and Offerings tell a reader what the code does. The API
and console are independent composition roots, not places where business rules are duplicated.

Notifications is a second capability with its own implementation/schema, rather than a folder that
directly modifies enrollment state. Contracts is deliberately independent. Tests are separated into
core/relational specifications and HTTP integration specifications so the required infrastructure is clear.

This is a teaching structure, not a mandate that every future phase needs nine projects. Reuse the
principles and naming conventions; add physical boundaries only where they protect something meaningful.
A small utility can remain one project. A large module may later need its own application/domain split.

## What Each Pattern Does Here

| Pattern or technique | Concrete purpose | What is deliberately absent |
|---|---|---|
| Layered responsibilities | separate domain decisions, orchestration, adapters, hosts | a forced network tier per layer |
| Dependency inversion | application owns the persistence contract | domain depending on EF |
| Purpose-specific repository/store | atomic aggregate load/save semantics | generic CRUD repository hierarchy |
| Application service | coordinate one enrollment use case | capacity rules duplicated in the handler |
| CQRS | project catalog reads independently of mutation | broker, separate database, event sourcing requirement |
| DDD aggregate | protect capacity and learner uniqueness together | a base entity for every scalar/record |
| Modular monolith | isolate notification ownership in one deployment | claims of independently operated services |
| Outbox/inbox | recover from partial cross-module delivery | exactly-once transport promises |
| API versioning | make the current v1 route contract explicit | speculative v2 endpoints |

The absence column matters. Patterns introduce operating and reasoning costs. A smaller design that
meets the requirement can be the more maintainable choice.

## Executable Fitness Checks

`ArchitectureBoundaryTests` checks both compiled assembly references and source project declarations.
Assembly checks catch actual use of a forbidden implementation dependency. Source checks also catch
an unused `ProjectReference` that may not appear in compiled metadata.

Rules currently enforced include:

- Domain has no project, EF, or ASP.NET dependency.
- Application directly references only Domain.
- Contracts has no project/package/framework dependency.
- Notifications directly references only Contracts among solution projects.
- Infrastructure directly references Application and Contracts.
- Hosts reference the implementations they compose, not additional hidden module internals.
- Domain, Application, and Contracts have no NuGet package or framework references.

The no-package rule is a deliberate current constraint, not an assertion that a pure library can never
use an appropriate dependency. Change the decision and its test together if a justified dependency is
introduced. Do not weaken the test with a broad allow-all prefix to silence a failure.

The source graph test requires a repository checkout and locates its root through
`Directory.Packages.props`. It is a repository fitness check, not a test intended to run against a
published application package with source files removed.

## Behavior Is Also Architecture Evidence

Dependency rules alone cannot prove correctness. The relational tests check atomic conditional writes,
replay semantics, and rollback. Delivery tests check duplicate recovery across separate databases.
HTTP tests check identity binding and error/status contracts. These properties protect the decisions
that motivated the graph.

Consider a refactor that preserves every project reference but moves all capacity logic into an endpoint.
An assembly rule might still pass; domain/use-case tests would expose the lost reusable behavior. Use
structural and behavioral evidence together.

## Evaluate Changes by Their Blast Radius

### Replace SQLite

Domain and Application should not change merely because a database provider changes. Infrastructure,
schema/migrations, transaction/error classification, and provider-specific tests will change. A new
provider does not automatically preserve lock behavior or JSON schema choices.

### Add another public host

A CLI or job can call the same application behavior with its own authorization/input boundary. It
should not invoke the HTTP endpoint in process or copy capacity checks. If it needs durable replay,
use `IEnrollmentRequests`, not just the introductory nontransactional handler.

### Extract notifications

Preserve the integration contract but replace the in-process sink with a transport adapter. Add broker
security, serialization compatibility, operations, and delivery semantics. Do not let the extracted
consumer acquire a reference to enrollment infrastructure as a shortcut.

### Introduce billing

Payment is not one more field in the enrollment aggregate. Define when a seat is reserved/confirmed,
which capability owns money state, and how timeouts/compensation work. A saga/process manager might
be justified, but the current two-database outbox is not already a full billing workflow.

## Review Checklist

1. Can a newcomer trace one use case without reading every infrastructure class?
2. Is the source of each business decision explicit?
3. Does a port name a required behavior, including failure semantics?
4. Does a transaction have one visible owner?
5. Are retries scoped to idempotent operations with classified failures?
6. Do public contracts omit domain/persistence implementation types?
7. Are module data ownership and consistency boundaries documented?
8. Do tests fail when a motivating guarantee is removed?
9. Are production gaps visible rather than hidden behind pattern names?
10. Is the operational cost of a new abstraction justified by an actual change scenario?

## Official References

- [Common web application architectures](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures).
- [Architectural principles](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles).
- [Maintain an architecture decision record](https://learn.microsoft.com/en-us/azure/well-architected/architect-role/architecture-decision-record).

## Navigation

- Previous: [HTTP security and version evolution](06-http-security-errors-versioning.md)
- Next: [Common architecture pitfalls](08-common-pitfalls.md)
