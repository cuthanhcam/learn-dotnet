---
title: "Guided Architecture Source Tour"
description: "Read the enrollment example in small steps, connect each boundary to tests, and distinguish implemented guarantees from future work."
slug: architecture-guided-source-tour
phase: 10
order: 12
difficulty: reference
article-type: reference
estimated-reading-minutes: 12
topics: [architecture, source-code, testing, maintainability]
prerequisites: [architecture-phase-10-completion-audit]
status: maintained
last-reviewed: 2026-10-25
---

# Guided Architecture Source Tour

Read one request path before studying every project. Run the offline console first using the
[phase commands](../README.md#run-and-verify). The basic path needs no identity server or database setup.

## 1. Follow a Business Decision

Start in [the console composition root](../src/Learning.Architecture.ConsoleApp/Program.cs), then open
[EnrollLearnerHandler](../src/Learning.Architecture.Application/Enrollments/EnrollLearner.cs).
The handler validates input, loads an offering, asks the domain for a decision, and attempts a save.
It coordinates the operation; it does not decide capacity rules.

Read [CourseOffering](../src/Learning.Architecture.Domain/Courses/CourseOffering.cs) next. Enrollment
produces a new immutable snapshot. A duplicate learner is recognized before checking capacity, so an
already enrolled learner can repeat the operation even when the offering is full.

Now read [the store port](../src/Learning.Architecture.Application/Abstractions/ICourseOfferingStore.cs)
and [dictionary adapter](../src/Learning.Architecture.Infrastructure/Courses/InMemoryCourseOfferingStore.cs).
The port describes the save contract; the adapter atomically compares the expected version and replaces
the snapshot. Domain validation alone cannot protect two callers that read the same old version.

**Checkpoint:** explain why a valid domain decision may still return `Conflict`. Run
[EnrollmentTests](../tests/Learning.Architecture.Tests/EnrollmentTests.cs) and the
[shared store contracts](../tests/Learning.Architecture.Tests/Contracts/CourseOfferingStoreContractTests.cs).

## 2. Add Durable Coordination

Read [EfCourseOfferingStore](../src/Learning.Architecture.Infrastructure/Persistence/EfCourseOfferingStore.cs).
Its conditional SQL write enforces the same snapshot contract as the dictionary adapter. The shared
suite runs against both implementations; it does not prove that a different provider or collation has
identical behavior.

Then read [DurableEnrollmentRequests](../src/Learning.Architecture.Infrastructure/Persistence/DurableEnrollmentRequests.cs).
It owns one local transaction: enrollment state, a semantic replay receipt, and outbox delivery intent
commit together. The receipt binds a request key to offering and learner IDs. Reusing that key for
different input is rejected. Replaying valid stored input returns the original result, including
`Full` and `NotFound`. Malformed stored results are rejected rather than treated as successes.

The transaction contains no email or remote broker call. A failed or uncertain commit requires the
same key in a fresh scope; reusing a failed context or inventing a new key can defeat recovery.
Follow the rollback and replay cases in
[RelationalEnrollmentTests](../tests/Learning.Architecture.Tests/Persistence/RelationalEnrollmentTests.cs).

## 3. Follow Delivery and HTTP Separately

Use [the messaging article](05-modules-events-outbox-inbox.md) to trace pending outbox intent into a
separately owned notification database. Delivery can repeat after acknowledgement loss; the receiver
deduplicates the durable work item. This is not a guarantee that an external email was sent exactly once.

Finally open [the API composition root](../src/Learning.Architecture.Api/Program.cs) and
[EnrollmentEndpoints](../src/Learning.Architecture.Api/Features/EnrollmentEndpoints.cs).
Authentication validates the token; authorization requires the enrollment scope and learner identity.
The endpoint derives the learner from claims rather than trusting a request body. Translate business
outcomes into HTTP only at this boundary. Use [the security article](06-http-security-errors-versioning.md)
for configuration and [HTTP tests](../tests/Learning.Architecture.IntegrationTests/EnrollmentHttpTests.cs)
for observable responses. Test-only identity headers are not an application login mechanism.

## What to Change Next

| Change | Start here | Evidence to revisit |
|---|---|---|
| Enrollment rule | Domain aggregate | domain and handler cases |
| Save semantics | Application port and both adapters | shared store contracts and relational rollback |
| Replay result format | Durable coordinator and receipt schema | malformed and valid replay cases |
| HTTP response or permissions | API feature and authorization policy | HTTP and signed-token tests |
| Module delivery contract | Contracts, sender, receiver | acknowledgement-loss and duplicate-delivery cases |
| Project dependency | project references | architecture fitness tests |

Keep the current feature folders and explicit project boundaries. A generic repository, mediator,
additional deployment, or extra layer needs a concrete learning problem before it earns a place here.

Track 10A implements the [documented learning baseline](09-completion-audit.md), with provider,
migration, concurrency, and operational limits still visible. Track 10B delivers four GoF patterns;
the other nineteen remain on [its roadmap](../design-patterns/README.md). Continue testing practice in
[Phase 11](../../11-testing/README.md). A green build verifies the checked behavior, not every possible
deployment or future modification.

## Navigation

- Return: [Phase 10 study path](../README.md)
- Related: [Completion audit](09-completion-audit.md)
