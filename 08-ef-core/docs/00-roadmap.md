---
title: "Entity Framework Core Learning Roadmap"
description: "An ordered path from relational modeling and DbContext lifetime to migrations, query performance, transactions, concurrency, and provider-accurate testing."
slug: ef-core-roadmap
phase: 8
order: 0
difficulty: intermediate
article-type: roadmap
estimated-reading-minutes: 16
topics: [ef-core, relational-databases, orm, roadmap]
prerequisites: [aspnet-core-phase-07-completion-audit]
status: maintained
last-reviewed: 2026-08-30
---

# Entity Framework Core Learning Roadmap

## Goal

Build persistence code whose schema, generated SQL, tracking behavior, transaction ownership,
concurrency semantics, performance, and failure modes remain understandable under real relational
database behavior.

## Progressive Layers

1. `DbContext`, model metadata, and unit-of-work lifetime.
2. Keys, indexes, value generation, relationships, and delete behavior.
3. Migrations, schema review, scripts, bundles, and deployment ownership.
4. Change tracker, entity states, identity resolution, and disconnected updates.
5. LINQ translation, projection, pagination, loading strategies, and generated SQL.
6. N+1 diagnosis, cartesian explosion, split queries, and performance measurement.
7. Optimistic concurrency tokens, conflict resolution, and HTTP integration.
8. Transactions, savepoints, execution strategies, retries, and idempotency.
9. Set-based mutations, raw SQL, interceptors, and auditing boundaries.
10. Provider-accurate testing, migration tests, diagnostics, and completion audit.

## Study Loop

For each slice:

1. State the relational invariant and which layer owns it.
2. Predict the SQL and number of database round trips.
3. Execute the test and inspect generated commands where relevant.
4. Separate tracking behavior from persisted database state.
5. Introduce one constraint, cancellation, concurrency, or failure case.
6. Compare SQLite behavior with the intended production provider.
7. Record migration and deployment implications before moving on.

## Provider Honesty

EF Core provides a common programming model, not identical database behavior. Providers differ in
type mappings, collations, SQL translation, locking, transaction semantics, generated values,
concurrency mechanisms, and migration operations. A passing SQLite test cannot certify a SQL Server
or PostgreSQL query.

This phase uses SQLite for fast relational learning tests and later introduces a production-provider
test layer. It does not use EF Core InMemory as a substitute for a relational database.

## Navigation

- Previous: [Phase 07 — ASP.NET Core](../../07-aspnet-core/README.md)
- Next: [DbContext, modeling, and unit of work](01-dbcontext-modeling-unit-of-work.md)
- Completion: [Phase 08 completion audit](10-completion-audit.md)
