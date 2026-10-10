---
title: "Architecture Learning Roadmap"
description: "Follow an enrollment scenario through dependency direction, aggregates, application services, persistence, modules, HTTP contracts, and architecture decisions."
slug: architecture-learning-roadmap
phase: 10
order: 0
difficulty: advanced
article-type: roadmap
estimated-reading-minutes: 22
topics: [architecture, dependency-inversion, domain-modeling, concurrency]
prerequisites: [auth-phase-09-completion-audit]
status: maintained
last-reviewed: 2026-10-10
---

# Architecture Learning Roadmap

## The Scenario

A training organization publishes scheduled offerings. Learners request enrollment, each offering has
limited capacity, and retries must not consume a second seat for the same learner. Two callers may
compete for the final seat. Administrative changes and later billing interactions introduce separate
authority and consistency boundaries.

The initial scenario deliberately uses one business capability. Decomposing a trivial workflow into
services before understanding its invariants obscures the reason for each boundary.

## Delivery Sequence

1. Establish inward project references and an executable composition root.
2. Model an offering as the unit that protects capacity and learner uniqueness.
3. Implement a command handler with explicit persistence conflicts.
4. Add read projections, DTO mapping, transport validation, and query contracts.
5. Add relational persistence, optimistic concurrency, transactions, and durable idempotency.
6. Introduce module contracts and domain/integration events when a second capability needs them.
7. Add authenticated HTTP entry points, resource authorization, error mapping, and API versioning.
8. Record architecture decisions and enforce dependency rules with executable checks.
9. Audit every promised outcome and hand off the finished system to Phase 11 testing.

Each slice must state the problem, show its observable behavior, test failure paths, and record the
production boundary. Do not declare the whole phase complete when only the directory scaffold exists.

## What to Compare

| Decision | Questions to answer |
|---|---|
| Layered versus clean architecture | Which dependencies need inversion, and which can remain direct? |
| Feature folders | Can a reader find one use case without navigating unrelated technical concerns? |
| Repository versus DbContext | Does an abstraction express a behavioral contract or only rename CRUD? |
| Domain versus application service | Who decides validity, and who coordinates external work? |
| Commands versus queries | Do reads need aggregate behavior or only an efficient projection? |
| Monolith versus modular monolith | Where should ownership and compile-time isolation exist? |
| Microservices | Is independent deployment worth distributed consistency and operations? |

## Reading and Coding Loop

Read the article, run the console, follow one command from host to aggregate and back, then change an
invariant and observe the relevant tests. Trace compile-time references separately from runtime calls:
the infrastructure object executes persistence work at runtime while depending on an interface owned
by the application at compile time.

Review the earlier EF Core transaction and security authorization articles when those boundaries are
introduced. Architecture does not replace their correctness requirements.

## Progress

- [x] Domain/application/infrastructure/host boundaries.
- [x] Enrollment invariant and immutable aggregate snapshots.
- [x] Explicit optimistic concurrency and deterministic negative specifications.
- [ ] Read projection and DTO mapping.
- [ ] Relational persistence and durable idempotency.
- [ ] Module contracts and events.
- [ ] HTTP security, validation, and versioning.
- [ ] Architecture fitness checks and completion audit.

## References

- [Common web application architectures](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Architectural principles](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles)

## Navigation

- Previous: [Phase 09 completion audit](../../09-auth/docs/10-completion-audit.md)
- Next: [Dependency direction and project boundaries](01-dependency-direction-project-boundaries.md)
