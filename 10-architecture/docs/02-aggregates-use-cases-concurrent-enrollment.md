---
title: "Aggregates, Use Cases, and Concurrent Enrollment"
description: "Protect course capacity with immutable aggregate transitions, idempotent enrollment behavior, and atomic optimistic persistence."
slug: architecture-aggregates-use-cases-concurrent-enrollment
phase: 10
order: 2
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 22
topics: [architecture, dependency-inversion, domain-modeling, concurrency]
prerequisites: [architecture-dependency-direction-project-boundaries]
status: maintained
last-reviewed: 2026-10-10
---

# Aggregates, Use Cases, and Concurrent Enrollment

## Define the Consistency Boundary

A course catalog definition and a scheduled offering are different concepts. The offering owns capacity
and the set of learners enrolled in that occurrence. Those facts must change together: checking capacity
in an endpoint and inserting a learner later leaves a race between two requests.

The aggregate protects these invariants:

- capacity is between one and one thousand;
- an offering and learner have nonempty stable identifiers;
- one learner occupies at most one seat in an offering;
- enrolled count never exceeds capacity;
- a successful new enrollment advances the version exactly once.

`CourseOffering.Create` establishes valid initial state. Its constructor is private, its properties
are read-only, and its learner set is immutable and never exposed. Callers cannot bypass enrollment by
assigning a count or appending to a mutable collection.

## A Domain Transition Returns a Decision

`Enroll` first checks duplicate enrollment, then remaining capacity. That order matters: a repeated
request from an already enrolled learner remains idempotent even when the offering is full.

Successful enrollment returns a new aggregate instance with a new immutable set and incremented
version. Failed or duplicate attempts return the original state. A concurrent reader can safely hold
the previous snapshot; it cannot observe an object being mutated by another request.

This is a deliberate teaching model, not a requirement to make every EF entity immutable. Mutable
tracked aggregates are also valid when their lifetime and concurrency rules are explicit.

## Application Coordination

The handler validates command identifiers, loads the offering, asks the aggregate to decide, then saves
only a successful new transition. Expected outcomes have named statuses:

| Status | Meaning | Persistence |
|---|---|---|
| Enrolled | a new seat was committed | one conditional save |
| AlreadyEnrolled | the learner already occupies a seat | none |
| Full | no seat is available | none |
| NotFound | the offering does not exist | none |
| Conflict | the loaded version is stale at save | no overwrite |

Transport validation and domain validation solve different problems. HTTP can validate a request shape,
but the domain must remain valid when called by a console, worker, test, or future transport. Invalid
identifiers are programmer/input contract errors; full and duplicate outcomes are normal business
decisions. Later HTTP mapping will preserve that distinction.

## Why an Immutable Aggregate Still Needs Concurrency Control

Two callers can both load version zero with one available seat. Each produces a valid version-one
snapshot containing a different learner. Both domain transitions are individually valid. If storage
accepts both with last-write-wins semantics, the earlier success disappears.

The store must compare and update atomically. The in-memory adapter reads a candidate current object,
checks its version, and uses `ConcurrentDictionary.TryUpdate` to replace exactly that original object.
Only one caller wins. The loser sees an explicit conflict and must reload before making another decision.

A database adapter needs the equivalent guarantee through a concurrency token or conditional update.
A transaction alone is insufficient if its isolation and write conditions permit lost updates.

## Retry and Idempotency

This use case's duplicate learner rule is a business idempotency property. It does not yet implement a
durable HTTP idempotency key with request hashing and stored response. That later contract is needed
when clients repeat requests after an uncertain network response.

The handler does not automatically retry conflicts. A bounded caller retry can reload and decide that
the course is now full or that the same learner was already enrolled. Blind retries become unsafe once
a handler sends email, charges money, or publishes events. Such effects require transaction/outbox and
idempotency decisions before retries are introduced.

## Cancellation and Partial Effects

The store checks cancellation before reading or changing state. A cancellation before save leaves the
snapshot uncommitted. Cancellation after a successful database commit does not imply rollback: clients
may receive no response even though the operation succeeded. A later durable idempotency contract must
handle that uncertainty.

Do not use cancellation as proof that an external operation did not happen. Query authoritative state
or use an operation identifier before issuing a duplicate effect.

## Executable Evidence

`EnrollmentTests` exercises invalid capacity, snapshot immutability, duplicate enrollment while full,
two different snapshots competing for the last seat, handler conflict mapping, missing offerings, and
cancellation without mutation. The race test creates both snapshots before launching competing saves;
it does not rely on sleep or assume which thread wins.

The handler conflict test uses a narrow test adapter to force a stale-write outcome. The separate
concrete-store race test verifies actual atomic persistence mechanics. These tests answer different
questions and together protect the behavior at the port boundary.

## Production Extensions

The in-memory adapter loses data on restart and does not coordinate processes. The next persistence
slice will need a schema, normalized learner/offering uniqueness, transaction boundary, concurrency
token, migration history, and relational integration tests. Identity-derived learner IDs and resource
authorization belong at the entry point, with trusted authority passed into the application.

## Exercises

1. Implement a bounded reload/retry wrapper and test the final full or duplicate decision.
2. Explain how a unique learner/offering constraint complements aggregate checks.
3. Add cancellation after a simulated commit and specify how the client discovers the outcome.
4. Model withdrawal and decide whether capacity becomes immediately available.
5. Introduce a waitlist without allowing it to consume confirmed enrollment capacity.

## References

- [Designing a DDD-oriented microservice](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)
- [Handling concurrency conflicts in EF Core](https://learn.microsoft.com/en-us/ef/core/saving/concurrency)

## Navigation

- Previous: [Dependency direction and project boundaries](01-dependency-direction-project-boundaries.md)
- Next: Queries, DTOs, and validation (planned)
