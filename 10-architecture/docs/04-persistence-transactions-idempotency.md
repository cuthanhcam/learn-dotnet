---
title: "Persistence, Transactions, and Durable Idempotency"
description: "Implement an EF Core adapter with conditional SQL updates and a transaction that commits enrollment, replay receipts, and outbox intent together."
slug: architecture-persistence-transactions-idempotency
phase: 10
order: 4
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 35
topics: [ef-core, transactions, optimistic-concurrency, idempotency, persistence]
prerequisites: [architecture-queries-dtos-mapping-validation]
status: maintained
last-reviewed: 2026-10-10
---

# Persistence, Transactions, and Durable Idempotency

## Start with the Failure We Need to Prevent

Two callers load an offering with one remaining seat and version zero. Each independently produces a
valid aggregate containing its learner and version one. If persistence writes without a version check,
the later snapshot overwrites the earlier one. Both callers may receive success although only one
membership remains stored. Domain validation cannot close a race between separate snapshots.

Now consider a different failure: the database commits enrollment, but the response never reaches the
client. A retry must discover the committed outcome. “Try again with a different request ID” loses
that evidence and may repeat effects. Capacity correctness, business idempotency, request replay, and
message delivery are related but distinct requirements.

## Files and Responsibilities

- `Domain/Courses/CourseOffering.cs`: invariant checks and restoration of detached snapshots.
- `Application/Abstractions/ICourseOfferingStore.cs`: load and compare-and-save contract.
- `Application/Enrollments/IEnrollmentRequests.cs`: atomic request workflow contract.
- `Infrastructure/Persistence/EnrollmentDbContext.cs`: persistence records and schema constraints.
- `Infrastructure/Persistence/EfCourseOfferingStore.cs`: mapping, SQL projection, conditional update.
- `Infrastructure/Persistence/DurableEnrollmentRequests.cs`: transaction, receipt lookup, and outbox intent.

These paths are relative to their corresponding source projects. Read the introductory handler first;
the durable coordinator reuses it rather than introducing a competing implementation of capacity rules.

## An Explicit Persistence Model

The aggregate is immutable. EF records are mutable, storage-oriented objects. Mapping between them is
deliberate: the domain does not need EF attributes, navigation fix-up, or a constructor shaped for ORM
materialization. Conversely, the adapter does not pretend that a database row is automatically valid.

`Restore` validates identifiers, title, capacity, unique learner IDs, membership count, and the version
floor. The adapter additionally compares the stored count to the deserialized learner set. Corruption
fails visibly instead of silently reconstructing a plausible but incorrect aggregate.

Learner IDs are stored as JSON in this bounded lab aggregate. The capacity ceiling limits snapshot size.
This makes whole-snapshot compare-and-save easy to inspect, but it is not a universal enrollment schema.
A normalized membership table can provide per-learner indexes and a unique `(OfferingId, LearnerId)`
constraint. Its insertion and aggregate count/version update must share a transaction. Choose based on
query patterns, write contention, privacy retention, and expected aggregate size—not ORM convenience.

## Comparison Must Happen in the Database

The adapter's update is conceptually:

```sql
UPDATE Offerings
SET EnrolledCount = @count, LearnersJson = @learners, Version = @nextVersion
WHERE Id = @id AND Version = @expectedVersion;
```

One affected row means the caller owned the expected version at mutation time. Zero means missing or
changed state and returns `false` to the application contract. The handler exposes `Conflict`; it does
not relabel a lost update as a full offering.

`ExecuteUpdateAsync` executes immediately and bypasses the change tracker. The explicit `WHERE Version`
predicate is therefore essential. Marking a property as a concurrency token alone does not make this
bulk-style operation apply tracked-entity concurrency handling. Do not subsequently save a stale tracked
copy of the same row in the same context.

The introductory in-memory adapter uses atomic dictionary comparison instead. Both adapters enforce
the same behavioral port, but neither its implementation nor its performance characteristics are identical.

## What the Durable Workflow Commits

```text
begin local database transaction
  find receipt by RequestId
  if present: verify OfferingId + LearnerId, return original result
  otherwise: load aggregate, decide, conditional SQL update
  if conflict: leave without committing a receipt
  add receipt for the stable business result
  if newly enrolled: add outbox row
  SaveChanges (receipt + outbox)
commit
```

The seat update runs before `SaveChanges`, but is inside the explicitly opened transaction. If receipt
or outbox persistence fails, disposing the transaction rolls back that earlier SQL update too. The
fault-injection integration test proves this precise boundary; merely asserting `SaveChanges` was called
would not demonstrate atomicity.

No email, HTTP callback, or broker publish runs inside the transaction. These external operations cannot
be rolled back by the enrollment database. The outbox captures an intent that can be delivered later.

## Three Different Kinds of Repetition

| Situation | Result | Seat/event behavior |
|---|---|---|
| Same key, same offering and learner | replay original receipt | no additional mutation or event |
| Same key, changed offering or learner | `KeyReused` | no mutation; HTTP 409 |
| New key, already enrolled learner | new `AlreadyEnrolled` receipt | no additional seat or event |
| New key, another learner on full offering | `Full` receipt | no enrollment event |
| Conditional write loses a race | `Conflict`, no receipt | caller may retry same key in a fresh scope |

The receipt stores a semantic result and aggregate version, not every HTTP byte. The adapter reconstructs
the response contract from that result. If a production API promises byte-for-byte replay including
headers/status, store those values explicitly and define their compatibility/retention policy.

`Full` and `NotFound` are receipted in this lab. Replaying that key returns the original answer even if
the world later changes. A caller requesting a genuinely new attempt needs a new key. This must be
documented; silently reevaluating only some receipt statuses makes retry behavior unpredictable.

## Idempotency Keys Are Not Authorization

The GUID key is bound to offering and learner identity. The HTTP host derives learner identity from
validated claims and applies the write policy before calling the workflow. Knowing a request key does
not grant access to another learner's result. A mismatched learner gets `KeyReused`, never the stored
success payload.

In a multitenant system, include tenant identity in the receipt namespace and authorization checks.
If new request fields can affect behavior, include them in semantic comparison or a canonical request
fingerprint. Do not hash arbitrary raw JSON and assume different property order means a different intent.

Receipts also need a retention budget, privacy classification, and expiry semantics. This lab retains
them indefinitely to keep the evidence visible. It does not implement a cleanup worker or silently
reuse expired keys.

## Cancellation and Uncertain Commit Outcomes

Cancellation before work should prevent writes. Cancellation during work should propagate through EF
calls, and an uncommitted transaction should be disposed. Cancellation near commit is more subtle:
the caller may not know whether storage committed. Never tell a user “nothing happened” merely because
the HTTP request was canceled.

The recovery protocol is to retry the same request key in a fresh scope. A receipt proves the durable
outcome; absence permits another attempt under the concurrency rules. Do not reuse a failed context,
tracked pending inserts, or a half-finished transaction for an application-level retry.

Provider busy/lock failures are infrastructure conditions, not domain conflicts. SQLite may serialize
writes or report lock contention differently from SQL Server/PostgreSQL. The API maps known SQLite
busy/locked codes to 503 with `Retry-After`; other failures remain errors. The coordinator does not catch
all `DbUpdateException` instances and call them duplicate requests.

## Deterministic Evidence and What It Does Not Prove

`IndependentContexts_RejectTheSecondStaleSnapshot` loads two version-zero snapshots before writing
either. It writes them sequentially to deterministically test the database predicate. It does not claim
to benchmark simultaneous SQLite connections or model every production isolation level.

Other specifications verify same-key replay across fresh contexts, changed-payload rejection, business
idempotency under a new key, full results without events, cancellation before transaction, and rollback
after an injected post-update failure. The database is real SQLite held in memory by a test connection,
not EF's nonrelational InMemory provider.

For production database adoption, run equivalent tests against that provider with independent connections,
real migrations, concurrent requests, lock timeouts, deadlocks, and deployment-compatible isolation settings.
Phase 11 will build on this evidence with broader testing infrastructure.

## Run the Offline Lab

```powershell
dotnet run --project 10-architecture/src/Learning.Architecture.ConsoleApp -- --relational
```

Expect one new enrollment, a replay of that same result, one delivered integration message, zero
messages on the second dispatch, zero available seats, and one welcome work item. The console's SQLite
databases disappear at exit; the HTTP host's configured file-backed databases are persistent.

## Deployment Boundary

`EnsureCreated` is used only for disposable learning/test databases. It does not upgrade an existing
schema and is not interchangeable with migrations. The API refuses automatic initialization outside
Development/Testing. A real deployment needs reviewed migration artifacts, backup/recovery, receipt and
outbox retention, security for database files, and a rollback/roll-forward plan. Phase 08 provides the
migrations foundation; this phase focuses on application-level transaction ownership.

## Exercises

1. Normalize membership while preserving the existing compare-and-save tests.
2. Add an integration test that injects failure immediately before commit.
3. Define receipt expiry and explain how it affects retries after the retention window.
4. Add bounded retries only for classified transient failures; keep the request key unchanged.
5. Introduce withdrawal and decide how old enrollment receipts should behave after a new business state.

## Official References

- [EF Core concurrency](https://learn.microsoft.com/en-us/ef/core/saving/concurrency).
- [EF Core transactions](https://learn.microsoft.com/en-us/ef/core/saving/transactions).
- [ExecuteUpdate and ExecuteDelete](https://learn.microsoft.com/en-us/ef/core/saving/execute-insert-update-delete).
- [Choosing a testing strategy](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy).

## Navigation

- Previous: [Queries, DTOs, and validation](03-queries-dtos-mapping-validation.md)
- Next: [Modules, events, outbox, and inbox](05-modules-events-outbox-inbox.md)
