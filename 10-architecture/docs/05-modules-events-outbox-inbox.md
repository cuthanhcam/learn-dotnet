---
title: "Module Boundaries, Events, Outbox, and Inbox"
description: "Connect enrollment and notifications through a versioned contract, then test at-least-once delivery and durable deduplication across separate databases."
slug: architecture-modules-events-outbox-inbox
phase: 10
order: 5
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 30
topics: [modular-monolith, integration-events, outbox, inbox, reliability]
prerequisites: [architecture-persistence-transactions-idempotency]
status: maintained
last-reviewed: 2026-10-10
---

# Module Boundaries, Events, Outbox, and Inbox

## A Second Capability Creates a Real Boundary

Enrollment owns offering capacity and learner membership. Notifications owns preparing a welcome work
item. A notification failure should not remove a successfully committed enrollment, and enrollment
should not depend on notification tables or provider-specific delivery logic.

This is a useful reason to introduce a boundary. Creating five projects around a single arithmetic
function would not teach the same lesson. Here, separate ownership and failure behavior can be observed.

The system remains a modular monolith: both capabilities execute in one host and deploy together.
Separate projects and databases do not automatically create microservices. Independent service deployment
would additionally require transport, compatibility, security, observability, and operational decisions.

## Dependency and Ownership Map

```text
Enrollment domain <- application <- infrastructure
                                      |
                                      v
                             Contracts (LearnerEnrolledV1)
                                      ^
                                      |
                                Notifications

API composition root selects both implementations and connects them.
Enrollment database and notifications database do not share a transaction.
```

`Learning.Architecture.Contracts` references no implementation project. `Notifications` references only
that contract project and its own EF dependency. It cannot query enrollment tables through an infrastructure
reference. Assembly boundary tests protect the currently used compile-time dependency directions.

The contracts project is deliberately tiny. Do not grow it into a shared domain model containing every
module's entities, repositories, and helpers; that would turn ostensibly independent modules into a
single coupled implementation. In a larger modular monolith, publisher-owned contract packages or
explicit module APIs may provide more precise ownership than one global contracts project.

## Domain Events Are Not Integration Events

A domain event describes a business fact within a model and can coordinate reactions inside its
consistency rules. An integration event is a stable contract that crosses a module/process boundary
after the source change becomes durable. Neither is the same as a C# `event` delegate merely because
all use the word event.

This small model returns an explicit `EnrollmentDecision`; the durable coordinator translates a newly
committed enrollment intent into an outbox record. It does not introduce a generic domain-event collection,
reflection dispatcher, or mediator pipeline for one reaction. If several in-domain reactions later need
coordination, introduce domain events with explicit dispatch timing and transaction semantics. Keep the
public integration contract separate from the aggregate's internal representation.

`LearnerEnrolledV1` carries a message ID, offering ID, and learner ID. It intentionally excludes the
learner email address, access token, full aggregate, and database row. A consumer needing contact details
must use an authorized lookup or its own projection with a defined privacy policy.

## The Dual-Write Trap

Consider these tempting implementations:

1. Commit enrollment, then publish: a crash between them loses notification intent.
2. Publish, then commit enrollment: a rollback leaves a notification for an enrollment that never existed.
3. Keep a database transaction open while calling a broker: the broker is not automatically enlisted,
   and slow network work extends lock duration without making the two writes atomic.

The outbox instead commits enrollment and a local delivery-intent row together. Delivery happens later.
This solves the source-side atomicity problem; it does not make the entire cross-module workflow one
distributed transaction or guarantee immediate completion.

## Delivery and Acknowledgement Order

`OutboxDispatcher` selects a bounded pending batch, checks schema version, calls `IEnrollmentEventSink`,
and only then marks the row dispatched. The current sink is an in-process call into notifications.
No broker or network transport is claimed by the sample.

| Failure point | Durable state | Recovery |
|---|---|---|
| Before enrollment commit | no new seat or outbox | repeat request under its key |
| After source commit, before dispatch | pending outbox | later iteration delivers |
| Before notification commit | pending source; no welcome item | retry delivery |
| After notification commit, before source acknowledgement | welcome exists; source pending | redeliver and deduplicate |
| After acknowledgement | source marked dispatched | no delivery in later normal batches |

The fourth row is why the receiver needs an inbox-style deduplication mechanism. Acknowledging before
the receiver commits would avoid duplicates by accepting message loss instead, which is not the desired
trade-off.

## Inbox and Local Effect Must Be Atomic

The notification module creates one `WelcomeWorkItem` keyed by `MessageId`. For this one-effect consumer,
the work item itself is also the inbox record: one database insert represents both “seen this message”
and “prepared the local work”. They cannot commit independently.

On repeat delivery, the consumer checks that offering and learner match the existing row. Identical
data is a no-op. A reused message ID with changed data is rejected rather than hidden as a duplicate.
The primary key remains the final defense against concurrent inserts after both deliveries initially
see no row. A competing database failure is allowed to surface; the next attempt uses a fresh context
and sees the committed winner. The code does not label every constraint/connection failure as success.

If the consumer later performs several local writes, use an inbox record and those writes in one local
transaction. If it sends email, that external effect needs its own delivery protocol and possibly a
provider idempotency key. A stored welcome work item is not evidence that an email reached an inbox.

## Read the Failure-Injection Test

`FailureAfterConsumerCommit_RedeliversWithoutDuplicatingLocalEffect` uses separate real SQLite databases.
The receiver commits a work item, then a decorator throws before the sender can acknowledge delivery.
The test verifies that the source row remains pending, another dispatcher attempt succeeds, and the
notification database still contains one work item.

The decorator is not the production consumer. It models a lost acknowledgement deterministically without
sleeping, killing a process, or relying on a scheduler race. This proves a particular failure protocol,
not the reliability of an unimplemented network transport.

Another test inserts an unknown schema version and checks that it remains pending. Silently ignoring an
unknown message and acknowledging it would turn a compatibility issue into permanent data loss.

## Hosting and Shutdown

The API worker waits between bounded iterations and creates a fresh DI scope for each one. Scoped EF
contexts and failed transaction state do not survive into the next attempt. It passes the host stopping
token to queries, delivery, and acknowledgement, and treats shutdown cancellation as normal.

A shutdown can interrupt delivery between receiver commit and acknowledgement. The same deduplication
protocol applies after restart. Graceful shutdown improves behavior but cannot replace crash-safe data
semantics; a process can terminate without receiving its shutdown token.

The worker logs failures using a fixed structured message. It does not interpolate request headers or
learner identity. Production telemetry should track pending count, oldest pending age, delivery latency,
retry count, and permanent failures without exposing sensitive message payloads.

## Explicit Production Gaps

The dispatcher is a **single-worker lab**. Its pending selection is not a distributed claim or lease.
Multiple hosts can select the same message and rely on receiver deduplication, but the sample does not
provide efficient multi-worker coordination or ordering guarantees.

Before scaling it, design:

- A claim/lease protocol with expiry and recovery for crashed owners.
- Bounded retry schedules and jitter for transient failures.
- Poison-message quarantine and an authorized operator replay workflow.
- Per-aggregate ordering only where business semantics actually require it.
- Retention and privacy policies for source messages and receiver deduplication records.
- Queue/broker authentication, payload validation, tracing, and transport-specific acknowledgement.
- Health signals that distinguish temporary receiver failure from a permanently blocked oldest batch.

The current worker retries failed iterations but does not quarantine poison messages. A permanently
invalid message can repeatedly block later messages in its batch. This is documented teaching scope,
not a hidden claim of production readiness.

## Modular Monolith or Microservices?

Choose modular boundaries first when a single deployment and direct contracts meet the workload's needs.
Consider service extraction when independent deployment, scaling, ownership, or isolation has a concrete
benefit large enough to justify network failure modes and operating cost. Do not distribute the database
because the project folder contains the word module.

An extraction plan should preserve contract ownership, define data migration, stop cross-module joins,
add transport security, and retain the tested at-least-once/deduplication behavior. It should also define
how operators observe a completed enrollment whose downstream welcome work remains pending.

## Exercises

1. Add a second notification effect and make its local transaction explicit.
2. Introduce retry timestamps without falsely claiming they are a multi-worker lease.
3. Design a V2 event that changes meaning rather than just adding an optional field.
4. Explain why deleting inbox records too early can allow an old redelivery to duplicate work.
5. Specify a poison-message workflow and its effect on ordering and later messages.

## Official References

- [Domain events: design and implementation](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation).
- [Subscribing to integration events](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/subscribe-events).
- [Transactional outbox with Azure Cosmos DB](https://learn.microsoft.com/en-us/azure/architecture/databases/guide/transactional-out-box-cosmos): a provider-specific reference, not the storage implementation used here.
- [Publisher-subscriber pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/publisher-subscriber).

## Navigation

- Previous: [Persistence and durable idempotency](04-persistence-transactions-idempotency.md)
- Next: [HTTP security, errors, and version evolution](06-http-security-errors-versioning.md)
