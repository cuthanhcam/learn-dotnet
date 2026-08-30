---
title: "Transactions, Savepoints, Retries, and the Outbox Pattern"
description: "Coordinate EF Core units of work with implicit and explicit transactions, savepoint recovery, execution strategies, idempotent operation IDs, cancellation rollback, and atomic outbox messages."
slug: ef-core-transactions-savepoints-retries-outbox
phase: 8
order: 7
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 58
topics: [ef-core, transactions, savepoints, retries, idempotency, outbox]
prerequisites: [ef-core-optimistic-concurrency-conflict-resolution]
status: maintained
last-reviewed: 2026-08-30
---

# Transactions, Savepoints, Retries, and the Outbox Pattern

A transaction defines which database changes commit or roll back as one unit. It does not make remote
HTTP calls, message brokers, files, or emails atomic with the database. Real applications need clear
transaction ownership, bounded duration, retry-safe operations, and a strategy for side effects after
commit.

## Implicit `SaveChanges` Transaction

When a provider supports transactions, one `SaveChanges` call normally wraps all generated commands in
a transaction. The test adds a valid category and a duplicate unique category in one save. The later
constraint failure rolls back the earlier insert.

Prefer one `SaveChanges` when all changes can be staged together. It gives atomicity with less code and
usually one transaction. An explicit transaction is not “more correct” when one save already expresses
the unit.

## When an Explicit Transaction Is Needed

An explicit transaction is appropriate when one consistency boundary contains:

- multiple `SaveChanges` calls;
- EF commands plus safe parameterized raw SQL;
- a database operation that must observe intermediate persisted state;
- multiple contexts sharing the same connection/transaction;
- application-managed savepoints.

Keep transactions short. Do not wait for user input, long remote calls, message delivery, or unbounded
background work while holding locks and a pooled database connection.

## Course Publication Use Case

`CoursePublicationService` models a realistic workflow:

1. start the provider execution-strategy delegate;
2. open a local database transaction;
3. detect replay by deterministic operation ID;
4. load the course and modules;
5. validate expected version and publication invariant;
6. mark the course published and save;
7. execute an injectable post-save hook;
8. insert a serialized outbox event and save;
9. commit.

Two saves are intentional so tests can fail or cancel between them and prove the explicit transaction
rolls back both logical stages. In ordinary code, stage all possible changes before one save.

## Transactional Outbox

Publishing a course and directly sending a broker message creates a dual-write problem:

- database commits, broker send fails: state changed but no event;
- broker send succeeds, database rolls back: consumers observe an event for state that does not exist;
- retry sends the same message twice.

The transactional outbox writes the aggregate change and an `OutboxMessage` to the same database
transaction. A separate dispatcher later reads unprocessed messages, publishes them, and marks them
processed.

This guarantees atomic local intent, not exactly-once external delivery. Dispatch can crash after send
but before marking processed, so consumers and messages need idempotency/deduplication.

## Stable Event Contract

The outbox stores:

- deterministic message/operation ID;
- occurrence time;
- versioned type name (`learning.course-published.v1`);
- serialized payload;
- optional processed timestamp.

Do not serialize an EF entity graph. Persistence navigations, proxies, cycles, internal fields, and
future model changes make an unstable integration contract. Serialize an explicit event record with
only intended fields and version the event type.

Payload schema evolution, retention, poison messages, dispatcher leasing, batch size, ordering, and
observability belong to the eventual outbox processor design.

## Execution Strategies

Some providers define retrying execution strategies for transient database failures. A user-created
transaction must be executed inside the strategy delegate so the provider can replay the complete
transaction:

```text
CreateExecutionStrategy
  -> ExecuteAsync
      -> BeginTransaction
      -> complete database unit
      -> Commit
```

Starting a transaction outside and then invoking independently retried commands can produce an
unsupported or partially replayed unit.

SQLite's default strategy does not retry, but the service shape remains correct for a retrying provider.
Production-provider failure tests must verify actual transient classification and retry behavior.

## Idempotency Under Replay

A retry delegate can run more than once. Client disconnects can also cause the caller to retry after a
commit whose response was lost. `OperationId` is deterministic for one publication request and is the
outbox primary key.

The service checks for that ID and returns `AlreadyProcessed` without publishing again or incrementing
course version. The test executes the same operation through a fresh context and verifies one message
and version 2.

The pre-check alone has a concurrency race if two identical operations start simultaneously. The
primary key is authoritative. A production implementation should translate its unique violation by
re-reading the operation result or use a provider-specific atomic insert/upsert strategy.

Never generate a new idempotency key inside each retry attempt. The key must identify the logical
operation, not the physical execution.

## Savepoints

A savepoint marks a recoverable position inside a transaction. The test:

1. inserts `Before Savepoint` and saves;
2. creates a named savepoint;
3. attempts a duplicate insert and receives `DbUpdateException`;
4. rolls back to the savepoint;
5. detaches the failed added entity from the change tracker;
6. inserts `After Recovery`;
7. commits both valid stages.

Rolling back database state does not automatically repair every in-memory entity state. The failed
duplicate remains `Added` until explicitly detached or corrected. Database transaction state and EF
change-tracker state are separate concerns.

EF automatically creates a savepoint before `SaveChanges` when an explicit transaction is already in
progress and the provider supports it. Manual savepoints are useful for application-defined optional
stages. SQL Server Multiple Active Result Sets can affect savepoint availability; verify provider
configuration.

## Failure Between Saves

The injected publication hook throws after the first save but before outbox insertion. The transaction
is disposed without commit and rolls back. A fresh context proves:

- course remains unpublished;
- version remains 1;
- outbox is empty.

The original context still contains in-memory published state after rollback. Do not reuse it as if its
tracked values represented the database. Dispose and begin a new unit of work, or explicitly reload all
affected entries if reuse is unavoidable.

## Cancellation and Rollback

The cancellation hook waits after the first save. Cancelling the caller token aborts the wait, unwinds
the transaction, and a fresh context proves no publication or message committed.

Pass cancellation to begin, queries, saves, hooks, commit, and rollback. Cancellation means the outcome
may be unknown if it occurs near commit; the caller should retry with the same operation ID and query
authoritative state rather than assume rollback.

## Isolation and Anomalies

Transactions provide atomicity, but isolation level controls concurrent visibility. Read committed,
snapshot, repeatable read, and serializable differ by provider and configuration. The publication
workflow also uses an optimistic version token, so a stale expected version cannot silently publish a
newer course.

Choose stronger isolation only for an identified invariant. Serializable transactions can abort or
block under contention. Unique constraints, atomic conditional updates, and idempotent operations often
protect invariants more efficiently.

## Cross-Context Transactions

Two contexts can share a relational transaction only when they share the same `DbConnection` and enlist
in the same `DbTransaction`. This couples lifetime and provider details. Prefer one context when models
belong to one database unit. Separate bounded contexts/databases require eventual consistency patterns,
not an assumed local EF transaction.

`TransactionScope` can coordinate ambient transactions but async flows require explicit enablement,
providers vary in support, and escalation to distributed transactions may be unavailable. Use it only
with a deliberate infrastructure decision.

## Commit Unknown and Retry Design

A connection can fail during commit after the database persisted changes but before the application
received acknowledgment. Blindly replaying non-idempotent inserts can duplicate effects. Stable keys,
operation records, and reconciliation queries turn an unknown outcome into a discoverable state.

Execution strategies retry transient database operations; they do not make arbitrary external calls
safe. Keep remote side effects outside the replayable transaction delegate and publish through the
outbox.

## Executable Scenarios

The suite verifies:

- course publication and outbox insert commit across two saves;
- a deterministic failure between saves rolls back both;
- replaying an operation ID creates no duplicate message/version change;
- one `SaveChanges` atomically rolls back earlier commands on a later constraint failure;
- manual savepoint recovery preserves earlier and later valid work;
- cancellation between saves rolls back transaction state;
- three migrations create publication columns and the outbox table.

## Review Checklist

- Can this unit use one implicit `SaveChanges` transaction?
- If explicit, is transaction lifetime short and ownership visible?
- Is the complete transaction inside the provider execution-strategy delegate?
- Can the delegate safely execute multiple times?
- Does one logical operation retain the same idempotency ID across retries?
- Are database changes and integration-event intent written atomically through an outbox?
- Are external side effects excluded from the transaction/retry delegate?
- After savepoint rollback, is change-tracker state repaired explicitly?
- Does cancellation propagate through every database stage, and can commit outcome be unknown?
- Are isolation level, constraint strategy, and production-provider behavior tested?

## Further Reading

- [Using transactions](https://learn.microsoft.com/en-us/ef/core/saving/transactions)
- [Connection resiliency and retries](https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency)
- [Saving data](https://learn.microsoft.com/en-us/ef/core/saving/)
