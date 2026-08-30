---
title: "Optimistic Concurrency and Conflict Resolution"
description: "Protect EF Core updates and deletes with application-managed versions, database predicates, disconnected expected versions, conflict snapshots, and explicit store-wins or client-wins policies."
slug: ef-core-optimistic-concurrency-conflict-resolution
phase: 8
order: 6
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 52
topics: [ef-core, optimistic-concurrency, concurrency-token, conflict-resolution, etag]
prerequisites: [ef-core-relationships-graph-loading]
status: maintained
last-reviewed: 2026-08-30
---

# Optimistic Concurrency and Conflict Resolution

Optimistic concurrency assumes conflicts are uncommon and detects them when a write reaches the
database. It does not lock a row for the complete read–think–write interval. Instead, an update or
delete includes the original concurrency token in its predicate. If another writer changed the token,
zero rows match and EF Core raises `DbUpdateConcurrencyException`.

## The Lost-Update Timeline

Two units of work read course version 1:

1. Writer A changes price, advances version to 2, and commits.
2. Writer B changes title based on version 1.
3. Without a token predicate, B overwrites some or all newer state.
4. With optimistic concurrency, B's update requires `Version = 1`; the database now stores 2, so it
   affects zero rows and EF reports a conflict.

The database predicate closes the race window. Checking a version in application memory before saving
improves feedback for already-stale requests but cannot replace the predicate: another transaction can
commit immediately after the check.

## Application-Managed Token

`Course.Version` is a `long` configured with `IsConcurrencyToken`. Application behavior increments it
for each meaningful course update. The generated command conceptually contains:

```sql
UPDATE courses
SET Title = ..., Price = ..., Version = 2
WHERE Id = ... AND Version = 1;
```

The test captures the executed SQL and asserts both primary-key and original-version predicates. This
is stronger evidence than checking model metadata alone.

Application-managed versions work consistently across providers, including SQLite. They require every
write path—tracked update, set-based update, raw SQL, import, and external writer—to advance the token.
Forgetting one path makes changes invisible to concurrency detection.

## Database-Generated Tokens

SQL Server `rowversion` automatically changes when a row is updated. Other providers offer system
columns such as PostgreSQL's transaction metadata with different semantics and mappings. Database-
generated tokens reduce application increment mistakes but are provider-specific and may change for
columns the application considers irrelevant.

Choose based on conflict granularity and provider. A token can protect the whole row or selected
properties can individually participate in concurrency. Fine-grained tokens reduce false conflicts
but make merge semantics more complex.

## Disconnected Expected Version

`UpdateCourseCommand` includes `ExpectedVersion`. In an HTTP API this value can come from the strong
ETag/`If-Match` contract built in Phase 07. A message consumer may carry an expected aggregate version
or idempotency state.

`CourseEditor`:

1. loads the authoritative tracked course;
2. returns `NotFound` if the row is gone;
3. returns `Conflict` immediately if the loaded version differs from the caller expectation;
4. applies allowed changes and advances the version;
5. attempts `SaveChangesAsync`;
6. still catches `DbUpdateConcurrencyException` for a commit occurring after step 3;
7. obtains current database values and returns a safe conflict snapshot.

The command never attaches a client-supplied entity graph. Expected version is a precondition, not
permission to overwrite every submitted property.

## Original, Current, and Database Values

On conflict, EF exposes three views:

- `OriginalValues`: values captured when tracking began and used in concurrency predicates;
- `CurrentValues`: local attempted values;
- `GetDatabaseValuesAsync`: a fresh snapshot currently persisted, or `null` if deleted.

The test proves original version 1, attempted current version 2, and database version 2 can coexist
while titles differ. Conflict UI, API response, or merge logic can compare these sets property by
property.

Never serialize raw EF `PropertyValues` or `DbUpdateConcurrencyException` to clients. Map an allowlisted
application contract. Database values may include internal, tenant-private, or sensitive columns.

## Conflict Policies

### Reject and Re-read

The safest general API policy returns a conflict/precondition failure plus current version, then asks
the client to fetch, reconcile, and resubmit. Do not automatically retry the same stale values; that
merely converts optimistic concurrency into delayed last-write-wins.

### Store Wins

Discard local changes by copying database values into current and original values and marking the entry
unchanged. The test confirms no second database write occurs. This can be appropriate for refresh or
low-priority local edits.

### Client Wins

Explicitly overwrite the newer store state:

1. fetch database values;
2. set them as new original values so the retry predicate uses the current token;
3. preserve/reapply intended client values;
4. advance version from the persisted version;
5. save again.

The test advances from version 2 to 3. Client-wins must be an authorized business decision, not a
generic catch-and-retry loop. It can erase another user's valid change.

### Merge

Compare original, current, and database values. Keep database changes for properties the client did
not edit; apply non-overlapping client changes; require human resolution when both changed the same
field. Collection and invariant merges are domain-specific and often need aggregate revalidation.

## Concurrent Delete

EF applies concurrency tokens to deletes as well. A stale entity deletion includes its original
version. If another writer updated the course first, the delete affects zero rows and throws instead of
silently removing newer state.

If `GetDatabaseValuesAsync` returns `null`, another transaction deleted the row. Whether the application
returns not-found, precondition-failed, idempotent success, or a domain conflict depends on operation
semantics. `CourseEditor` returns `NotFound` for its update workflow.

## Context State After a Conflict

Unlike many programming-error `InvalidOperationException` cases, concurrency exceptions are designed
for resolution. Entries remain modified/deleted and can be reconciled. However, do not keep retrying an
arbitrary context indefinitely. Bound retry attempts, reload all affected entries, rerun business
rules, and consider creating a fresh unit of work for complex aggregates.

`SaveChanges` can contain multiple commands. A transaction normally prevents a partially committed
unit when a concurrency exception occurs, but external side effects outside the database require their
own consistency design.

## Isolation Levels and Pessimistic Alternatives

Optimistic tokens protect writes after a disconnected interval. Transaction isolation controls what a
transaction observes while it is active. Repeatable-read, snapshot, or serializable isolation may
detect/prevent other anomalies but can hold locks, abort transactions, or increase contention.

Do not keep a database transaction open while a human edits a page or an external API responds.
Optimistic versions are designed for long disconnected intervals. Short critical allocation workflows
may use provider-specific locks or serializable transactions after measuring contention and deadlocks.

## HTTP Mapping

Phase 07 maps stale `If-Match` to `412 Precondition Failed` and missing preconditions to `428`. Phase 08
now supplies the authoritative persistence behavior behind that contract:

```text
HTTP ETag "7"
  -> expected version 7
  -> EF original/concurrency predicate Version = 7
  -> successful row count 1 or conflict row count 0
```

Keep ETags opaque at the HTTP boundary even if the implementation currently encodes an integer.

## Executable Scenarios

The suite verifies:

- two contexts updating the same row cause the second save to throw;
- original/current/database versions and values are distinguishable;
- an already-stale disconnected command returns conflict without issuing an update;
- client-wins adopts database originals and advances to version 3;
- store-wins discards the local edit and performs no second write;
- a stale delete conflicts after another writer updates;
- generated update SQL includes ID and original version in `WHERE`.

## Review Checklist

- Which field is the concurrency token and who advances it on every write path?
- Does the database command include the original token, not only the key?
- Does disconnected input carry an explicit expected version?
- Is the pre-save version check treated only as an optimization, not the authoritative guard?
- Are conflict responses allowlisted and free from raw database/EF details?
- What does a missing database row mean for update and delete?
- Is store-wins, client-wins, merge, or reject an explicit business policy?
- Are retries bounded and do they revalidate invariants against current state?
- Does HTTP map the database conflict to the documented ETag/precondition contract?
- Are production-provider concurrency mechanisms tested where provider-specific?

## Further Reading

- [Handling concurrency conflicts](https://learn.microsoft.com/en-us/ef/core/saving/concurrency)
- [Saving data overview](https://learn.microsoft.com/en-us/ef/core/saving/)
- [Transactions and isolation](https://learn.microsoft.com/en-us/ef/core/saving/transactions)
