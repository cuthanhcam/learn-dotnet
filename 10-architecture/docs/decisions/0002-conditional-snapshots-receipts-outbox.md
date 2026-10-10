---
title: "ADR 0002: Conditional Snapshots, Durable Receipts, and Outbox"
description: "Record the storage and failure-recovery decisions that preserve capacity, reconstruct request outcomes, and avoid source-side event loss."
slug: architecture-adr-conditional-snapshots-receipts-outbox
phase: 10
order: 11
difficulty: advanced
article-type: reference
estimated-reading-minutes: 10
topics: [adr, concurrency, idempotency, outbox, transactions]
prerequisites: [architecture-persistence-transactions-idempotency]
status: maintained
last-reviewed: 2026-10-10
---

# ADR 0002: Conditional Snapshots, Durable Receipts, and Outbox

## Decision Status

Accepted for the bounded Phase 10 workload on 2026-10-10. SQLite is the executable relational learning
provider, not a claim that it fits every deployment or contention profile.

## Context

Capacity and learner uniqueness must be decided together. Detached snapshots may compete for the final
seat. Clients can lose a response after commit. Notification delivery can fail independently. The
example needs visible transaction mechanics and deterministic failure tests without requiring a broker.

## Decision

Keep the aggregate immutable and persist a bounded learner snapshot plus count/version. Save only
through a SQL predicate that compares the expected version. Use one local transaction for the seat
update, semantic request receipt, and enrollment outbox intent. Replay the same key only when offering
and learner match. Deliver outbox messages after commit and deduplicate the notification work item by
message ID in a separate database.

Stable business outcomes are receipted; a write conflict is not. Persisted enrollment status values
have explicit numeric assignments. Use a fresh context for retries after failures.

## Alternatives Considered

| Alternative | Benefit | Trade-off |
|---|---|---|
| Tracked immutable aggregate mapping | less explicit adapter mapping | requires ORM model compromises or more complex configuration for this lesson |
| Normalized enrollment rows | indexed membership and database uniqueness | more schema/transaction code; useful next step for larger membership workloads |
| Pessimistic provider-specific locks | serialize a contested decision | provider coupling and lock-duration/deadlock concerns |
| Membership check without receipts | simple duplicate prevention | cannot recover the original request outcome or detect key misuse |
| Publish directly after SaveChanges | fewer source tables | source commit/publish crash window loses intent |
| Distributed transaction | broad atomicity where supported | deployment/provider constraints and operating cost outside this workload |

## Consequences

The compare-and-save contract can be exercised by both dictionary and SQL adapters. Snapshot JSON makes
the SQL mutation readable, but does not provide normalized per-learner indexing or a relational membership
unique constraint. Bounded capacity limits snapshot size but is not a universal scaling strategy.

Receipts and outbox rows require retention policies before real adoption. Notifications are eventually
prepared, not synchronously guaranteed. At-least-once delivery requires receiver deduplication; the
single dispatcher does not implement leasing, broker transport, quarantine, or external email delivery.

## Reconsideration Triggers

- Aggregate membership or query needs outgrow bounded JSON snapshots.
- Sustained contention requires a different reservation or locking model.
- Multitenancy or additional input changes the request-key namespace/fingerprint.
- Delivery scales to several hosts or an external broker.
- Retained receipts must survive a semantic/API contract migration.

## Related Material

- [Persistence, transactions, and idempotency](../04-persistence-transactions-idempotency.md).
- [Modules, outbox, and inbox](../05-modules-events-outbox-inbox.md).
- [EF Core concurrency](https://learn.microsoft.com/en-us/ef/core/saving/concurrency).
