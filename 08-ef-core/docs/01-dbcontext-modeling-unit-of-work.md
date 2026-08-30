---
title: "DbContext, Relational Modeling, and the Unit of Work"
description: "Configure an EF Core 10 model with short-lived contexts, domain invariants, keys, indexes, precision, relationships, delete behavior, and relational SQLite specifications."
slug: ef-core-dbcontext-modeling-unit-of-work
phase: 8
order: 1
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 44
topics: [ef-core, dbcontext, modeling, relationships, constraints, sqlite]
prerequisites: [ef-core-roadmap]
status: maintained
last-reviewed: 2026-08-30
---

# DbContext, Relational Modeling, and the Unit of Work

`DbContext` is EF Core's short-lived unit-of-work boundary. It combines model metadata, an identity
map, change tracking, query execution, persistence, and transaction coordination. It is not a
thread-safe database singleton and should not become an application-wide cache.

## A Typical Unit of Work

One unit of work normally performs this sequence:

1. create or resolve a context;
2. query or add entity instances;
3. apply business behavior to those instances;
4. call `SaveChanges` or `SaveChangesAsync` once at the intended consistency boundary;
5. dispose the context.

ASP.NET Core's `AddDbContext` registers a scoped context by default, which often aligns one HTTP
request with one context. That is a default, not a rule that every request must perform one transaction.
Background jobs and multiple independent units inside one request should create explicit scopes or use
`IDbContextFactory<TContext>`.

Never execute parallel operations on one context. Await an EF operation before using that context
again. If independent database operations truly need concurrency, use independent context instances
and reason about their transaction and consistency boundaries.

## Context as Composition Point

`LearningDbContext` exposes `DbSet<T>` properties and calls
`ApplyConfigurationsFromAssembly`. Entity configurations live in focused
`IEntityTypeConfiguration<T>` types instead of one enormous `OnModelCreating` method.

`DbSet<T>` does not mean that every application layer should receive the context and build arbitrary
queries. It represents an entry point into the mapped model. Query ownership, reuse, and test strategy
should follow application boundaries rather than spreading provider-dependent LINQ everywhere.

## Domain Rules and Database Rules

The entity constructors and methods reject obviously invalid in-memory state: empty category IDs,
blank titles, negative prices, and invalid names. That gives application code immediate feedback.

The relational model independently enforces durable invariants:

- primary keys for every table;
- unique category name and course slug indexes;
- maximum string lengths;
- decimal precision;
- required foreign keys;
- unique module ordering within a course;
- explicit delete behavior;
- a concurrency-token property.

Application checks improve user experience but cannot replace constraints. Concurrent processes can
both pass an existence check; only an authoritative unique index resolves the race at commit time.

## Relationships and Delete Behavior

The model contains two one-to-many relationships:

```text
Category 1 ── * Course 1 ── * CourseModule
```

Deleting a category is restricted while courses reference it. Categories are shared classification
data, so silently deleting every course would be dangerous. Modules belong to a course aggregate and
cascade when that course is deleted.

Delete behavior is domain policy, not a convention to accept without review. Compare `Cascade`,
`Restrict`, `NoAction`, `SetNull`, and client-side variants against optionality and the database's
actual foreign-key action.

## Encapsulation and Materialization

Entities use private parameterless constructors for EF materialization and public constructors for
application creation. Properties have private setters. `Course` exposes modules as a read-only
collection backed by a private list and adds modules through behavior that assigns deterministic
ordering.

Encapsulation does not remove the need for database constraints. EF can materialize persisted state
without calling the public constructor, and external writers may modify the database. Model both
in-memory behavior and durable invariants deliberately.

## Precision and Type Mapping

`HasPrecision(18, 2)` documents the intended monetary column. SQLite's storage system differs from
SQL Server and PostgreSQL numeric types, so provider-specific migrations and tests must confirm the
actual schema. `decimal` avoids binary floating-point representation errors but does not define
currency, rounding, tax, or exchange-rate policy.

Date/time mapping also differs by provider. The sample stores `DateTimeOffset` in the domain model;
later provider slices define UTC conventions, indexes, and query behavior explicitly.

## Concurrency Token Foundation

`Course.Version` is marked as a concurrency token. Later updates will include the original version in
the SQL predicate and increment the value intentionally. If another unit of work changed the row, zero
rows are affected and EF raises `DbUpdateConcurrencyException`.

Marking a property is only the model foundation. The application must define who increments it, how
conflicts are translated, whether values become HTTP ETags, and whether clients retry, merge, or reject.

## Relational SQLite Test Lifetime

An SQLite `:memory:` database exists only while its low-level connection remains open. The
`SqliteTestDatabase` fixture opens one owner connection, creates the schema, and creates multiple
short-lived contexts over that connection. This lets a test prove persistence across context disposal
without a temporary file.

`EnsureCreated` is suitable for this initial model test, not a migration test or production deployment
strategy. It creates a schema directly from the current model and bypasses migration history. The next
slice introduces migrations and verifies upgrade scripts separately.

## What the Tests Prove

The first specifications verify:

- an aggregate saved in one context loads from another context;
- category and ordered module relationships materialize correctly;
- the unique category index rejects duplicate durable state;
- the database foreign key restricts deletion of a referenced category.

The restriction test deliberately uses a fresh context that loads only the category. If the complete
relationship graph were tracked, EF's conceptual-null detection could reject the operation before SQL.
The fresh unit proves the relational constraint itself, not only change-tracker behavior.

## Testing Caveat

SQLite is a genuine relational provider, but it remains a test double when production uses another
database. Collation, case sensitivity, SQL functions, transaction isolation, locking, raw SQL, and
migration operations can differ. Important queries and migrations must eventually run against the
production provider.

Avoid the EF Core InMemory provider for relational behavior tests. It does not enforce relational
foreign keys or transactions and executes queries with different semantics. Mocking `DbSet` query
behavior similarly evaluates LINQ over objects rather than testing SQL translation.

## Review Checklist

- Is each context short-lived, disposed, and used by one asynchronous flow at a time?
- Which invariants belong in entity behavior, database constraints, or both?
- Are maximum lengths, precision, requiredness, indexes, and uniqueness explicit?
- Is delete behavior reviewed for every relationship?
- Can EF materialize encapsulated entities without weakening application construction?
- Does the context stay a composition point rather than a global service locator?
- Do tests distinguish change-tracker rejection from database constraint enforcement?
- Is the provider difference documented for every test-double strategy?

## Further Reading

- [DbContext lifetime and configuration](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/)
- [Creating and configuring a model](https://learn.microsoft.com/en-us/ef/core/modeling/)
- [Relationships](https://learn.microsoft.com/en-us/ef/core/modeling/relationships)
- [Choosing an EF Core testing strategy](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy)
- [What's new in EF Core 10](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/whatsnew)
