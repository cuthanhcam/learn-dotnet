---
title: "Phase 08 — Entity Framework Core"
description: "A relational-data learning path through EF Core 10 modeling, querying, change tracking, migrations, transactions, concurrency, and performance."
phase: 8
status: completed
target-framework: net10.0
prerequisites: [phase-07-aspnet-core]
previous-phase: ../07-aspnet-core/README.md
next-phase: ../09-auth/README.md
---

# Entity Framework Core

> Turn relational data into explicit, testable units of work without hiding SQL, database constraints,
> provider behavior, or transaction boundaries behind ORM convenience.

## Learning Outcomes

After this phase, you should be able to design an EF Core model, configure relationships and indexes,
reason about `DbContext` lifetime and change tracking, write translatable and efficient queries,
manage schema migrations, implement optimistic concurrency, coordinate transactions, diagnose N+1
and cartesian explosion, use set-based updates, and test against appropriate database providers.

## Study Path

| Order | Article | Executable focus |
|---:|---|---|
| 0 | [Roadmap](docs/00-roadmap.md) | Scope, provider strategy, and progressive workflow |
| 1 | [DbContext, modeling, and unit of work](docs/01-dbcontext-modeling-unit-of-work.md) | relational model, constraints, relationships, and SQLite tests |
| 2 | [Migrations and schema evolution](docs/02-migrations-schema-evolution.md) | pinned tooling, generated history, scripts, and deployment safety |
| 3 | [Change tracking and disconnected updates](docs/03-change-tracking-disconnected-updates.md) | identity map, read models, state conflicts, and safe commands |
| 4 | [Querying, loading, and performance](docs/04-querying-loading-performance.md) | N+1, projections, SQL capture, and keyset pagination |
| 5 | [Advanced relationships and graph loading](docs/05-relationships-graph-loading.md) | join entities, cascades, split queries, and fixup pitfalls |
| 6 | [Optimistic concurrency and conflict resolution](docs/06-optimistic-concurrency-conflict-resolution.md) | version predicates, database values, merge policies, and stale deletes |
| 7 | [Transactions, savepoints, retries, and outbox](docs/07-transactions-savepoints-retries-outbox.md) | atomic publication, idempotency, rollback, and side-effect boundaries |
| 8 | [Set-based updates, raw SQL, and interceptors](docs/08-set-based-operations-raw-sql-interceptors.md) | bulk writes, tracker staleness, parameterization, and command inspection |
| 9 | [Compiled queries, safe diagnostics, and provider-accurate testing](docs/09-compiled-queries-diagnostics-provider-testing.md) | measured hot paths, telemetry boundaries, pooling, and real-provider suites |
| 10 | [Completion audit](docs/10-completion-audit.md) | coverage matrix, provider boundary, verification, and Phase 09 handoff |

The completed phase covers modeling, migrations, tracking, translation, graph loading, concurrency,
transactions, performance diagnostics, bulk operations, raw SQL, compiled queries, and a deliberately
explicit production-provider testing boundary.

## Structure

```text
08-ef-core/
├── docs/
├── src/Learning.Persistence/
│   ├── Configurations/
│   ├── Domain/
│   └── LearningDbContext.cs
├── tests/Learning.Persistence.Tests/
│   └── Infrastructure/
└── 08-ef-core.slnx
```

## Provider Strategy

The initial executable model uses SQLite because it is a relational provider, runs locally, and makes
foreign keys, unique indexes, transactions, and generated SQL observable. SQLite is not a behavioral
substitute for SQL Server or PostgreSQL. Later tests explicitly separate provider-independent model
rules from production-provider query and migration behavior.

The EF Core InMemory provider is intentionally not used as a relational database fake: it does not
provide relational constraints, transactions, raw SQL, or production query translation semantics.

## Run and Test

```powershell
dotnet restore 08-ef-core.slnx
dotnet build 08-ef-core.slnx --configuration Release --no-restore
dotnet test 08-ef-core.slnx --configuration Release --no-build
```

## Initial Code Map

| Concern | Implementation |
|---|---|
| Unit-of-work boundary | `LearningDbContext.cs` |
| Domain invariants | `Domain/Category.cs`, `Domain/Course.cs` |
| One-to-many dependent | `Domain/CourseModule.cs` |
| Table and relationship mapping | `Configurations/` |
| Relational test database lifetime | `tests/Learning.Persistence.Tests/Infrastructure/SqliteTestDatabase.cs` |
| Constraint and persistence specifications | `tests/Learning.Persistence.Tests/ModelAndUnitOfWorkTests.cs` |
| Read and write use cases | `Courses/CourseQueries.cs`, `Courses/CourseEditor.cs` |
| Change-tracker specifications | `tests/Learning.Persistence.Tests/ChangeTrackingTests.cs` |
| Query round-trip instrumentation | `tests/Learning.Persistence.Tests/Infrastructure/CommandCaptureInterceptor.cs` |
| Relationship and graph specifications | `tests/Learning.Persistence.Tests/RelationshipLoadingTests.cs` |
| Concurrency specifications | `tests/Learning.Persistence.Tests/ConcurrencyTests.cs` |
| Transaction and outbox specifications | `tests/Learning.Persistence.Tests/TransactionTests.cs` |
| Bulk operations and raw SQL specifications | `tests/Learning.Persistence.Tests/BulkOperationsAndRawSqlTests.cs` |
| Compiled-query and diagnostics specifications | `tests/Learning.Persistence.Tests/CompiledQueriesAndDiagnosticsTests.cs` |
| Finalized model contract | `tests/Learning.Persistence.Tests/ModelMetadataTests.cs` |

## Design Rules

- Treat `DbContext` as a short-lived unit of work and never share it across concurrent operations.
- Keep business invariants in domain behavior and authoritative relational invariants in the database.
- Configure indexes, lengths, precision, delete behavior, and concurrency explicitly.
- Await every EF Core asynchronous operation before reusing or disposing the context.
- Project only required columns for reads; do not materialize graphs by habit.
- Inspect generated SQL and query plans before labeling a query optimized.
- Apply migrations through a deliberate deployment workflow rather than every application instance.
- Test important queries and migrations against the actual production provider.

## Completion Criteria

- [x] Explain `DbContext` lifetime, identity map, tracking, and unit-of-work semantics.
- [x] Configure keys, indexes, value generation, precision, relationships, and delete behavior.
- [x] Create, review, script, apply, and roll back migrations safely.
- [x] Compare tracking, no-tracking, identity resolution, projection, and compiled queries.
- [x] Detect N+1, cartesian explosion, client evaluation, and inefficient pagination.
- [x] Implement optimistic concurrency and translate conflicts intentionally.
- [x] Use implicit and explicit transactions, savepoints, execution strategies, and idempotency.
- [x] Apply `ExecuteUpdate`/`ExecuteDelete` and raw SQL with safe parameterization.
- [x] Separate fast relational specifications from the mandatory live-provider application gate.
- [x] Pass the complete Phase 08 suite and content audit.

## Previous Phase

Revisit [Phase 07 — ASP.NET Core](../07-aspnet-core/README.md) for HTTP concurrency contracts,
cancellation propagation, dependency health checks, and persistence-boundary integration testing.
