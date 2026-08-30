---
title: "Phase 08 Completion Audit"
description: "Map every Entity Framework Core learning outcome to maintained articles and executable specifications, record provider boundaries honestly, and define the handoff to authentication and application architecture."
slug: ef-core-phase-08-completion-audit
phase: 8
order: 10
difficulty: advanced
article-type: reference
estimated-reading-minutes: 25
topics: [ef-core, audit, testing, architecture, learning-path]
prerequisites: [ef-core-compiled-queries-diagnostics-provider-testing]
status: maintained
last-reviewed: 2026-08-30
---

# Phase 08 Completion Audit

Phase 08 is complete as a provider-aware EF Core foundation. Completion means each promised concept
has an article, an executable example where local verification is meaningful, explicit failure cases,
and a documented boundary where behavior depends on a production database or later application phase.

It does not mean every database feature or provider extension belongs in this phase. Spatial data,
temporal tables, full-text search, provider-specific JSON, sharding, multi-region replication, and
warehouse workloads require separate problem-driven study.

## Coverage Matrix

| Learning outcome | Primary article | Executable evidence |
|---|---|---|
| Context lifetime and unit of work | [DbContext and modeling](01-dbcontext-modeling-unit-of-work.md) | `ModelAndUnitOfWorkTests` |
| Keys, constraints, precision, indexes | [DbContext and modeling](01-dbcontext-modeling-unit-of-work.md) | `ModelMetadataTests`, `ModelAndUnitOfWorkTests` |
| Migration chain and deployment safety | [Migrations](02-migrations-schema-evolution.md) | `MigrationTests`, three committed migrations |
| Identity map and tracking state | [Change tracking](03-change-tracking-disconnected-updates.md) | `ChangeTrackingTests` |
| Safe disconnected writes | [Change tracking](03-change-tracking-disconnected-updates.md) | `CourseEditor`, `CourseWorkflowTests` |
| Projection and pagination | [Query performance](04-querying-loading-performance.md) | `CourseQueries`, `QueryPerformanceTests` |
| N+1 and loading tradeoffs | [Relationships](05-relationships-graph-loading.md) | `RelationshipLoadingTests` |
| Join entities and cascade policy | [Relationships](05-relationships-graph-loading.md) | explicit configurations and relationship tests |
| Optimistic concurrency | [Concurrency](06-optimistic-concurrency-conflict-resolution.md) | `ConcurrencyTests` |
| Transactions and savepoints | [Transactions](07-transactions-savepoints-retries-outbox.md) | `TransactionTests` |
| Idempotency and transactional outbox | [Transactions](07-transactions-savepoints-retries-outbox.md) | `CoursePublicationService`, rollback/replay tests |
| Set-based update and delete | [Set-based operations](08-set-based-operations-raw-sql-interceptors.md) | `BulkOperationsAndRawSqlTests` |
| Parameterized raw SQL | [Set-based operations](08-set-based-operations-raw-sql-interceptors.md) | hostile-input parameter snapshot test |
| Compiled hot paths | [Compiled queries](09-compiled-queries-diagnostics-provider-testing.md) | `CourseCompiledQueries`, compiled-query tests |
| Safe command diagnostics | [Compiled queries](09-compiled-queries-diagnostics-provider-testing.md) | `CommandMetricsInterceptor`, success/failure tests |
| Provider-accurate testing strategy | [Compiled queries](09-compiled-queries-diagnostics-provider-testing.md) | SQLite relational suite plus explicit live-provider gate |

## Executable Inventory

The final sample is intentionally compact enough to understand but broad enough to exercise real
persistence boundaries:

```text
Domain/
├── Category, Course, CourseModule
├── Tag, CourseTag
└── OutboxMessage

Configurations/
└── explicit tables, keys, lengths, precision, indexes, relationships, and delete behavior

Courses/
├── CourseQueries                 projected and paginated reads
├── CourseEditor                  disconnected optimistic update
├── CoursePublicationService      retry-aware transaction and outbox
├── CourseBulkOperations          ExecuteUpdate, ExecuteDelete, raw SQL
└── CourseCompiledQueries         measured stable hot path

Diagnostics/
└── safe low-cardinality command observation

Migrations/
├── InitialCreate
├── AddCourseTags
└── AddPublishingOutbox
```

The test suite covers successful behavior and failure behavior. Examples include unique constraint
violations, restricted deletes, translation failure, stale concurrency versions, concurrent deletes,
rollback between saves, cancellation rollback, savepoint recovery, stale tracked entities after bulk
updates, hostile raw SQL input, and provider command failures.

## Model Contract Audit

Workflow tests alone can miss an accidental convention change until a particular value reaches
production. `ModelMetadataTests` protects intentional mapping decisions directly:

- course table name;
- title and slug maximum lengths;
- price precision and scale;
- application-managed concurrency token;
- unique slug index;
- publication query index;
- restricted category deletion;
- cascading module lifecycle;
- relational SQLite provider selection rather than EF InMemory.

Metadata tests do not replace migration review. They prove the finalized EF model; migration tests prove
the committed history can create the expected schema artifacts.

## Provider Boundary

This repository executes its always-on relational suite against SQLite in memory. SQLite provides real
SQL, foreign keys, constraints, transactions, migrations, and raw SQL while keeping tests isolated and
fast. The suite also makes SQLite-specific limitations visible, including idempotent migration scripts
and `DateTimeOffset` ordering.

SQLite is not a compatibility certificate for another provider. A consuming application must add a
live-provider suite after choosing SQL Server, PostgreSQL, or another engine. That gate must run:

- the committed migration chain on the selected engine;
- every important LINQ translation and raw SQL statement;
- provider concurrency tokens if introduced;
- retry and transient-error classification;
- intended isolation and locking behavior;
- collation, date/time, decimal, and generated-value semantics;
- representative query plans and indexes.

The current machine has no Docker engine. The audit therefore records this as a deployment-specific
gate rather than adding skipped tests that create a misleading green count. CI should fail—not silently
skip—when a repository declares a production provider and its required database is unavailable.

## Deliberately Deferred Topics

| Topic | Destination or trigger |
|---|---|
| Authentication user/token persistence | Phase 09 — Authentication and Authorization |
| Repository/CQRS/DDD boundaries | Phase 10 — Architecture, selected per use case |
| Distributed cache and database invalidation | caching/distributed-systems phase |
| Broker delivery for outbox messages | messaging and background processing phase |
| SQL Server `rowversion` or PostgreSQL system columns | chosen production-provider implementation |
| Temporal, spatial, JSON, full-text features | provider-specific feature requirement |
| Sharding, replicas, multi-region consistency | distributed data architecture |
| BenchmarkDotNet persistence benchmarks | measured performance investigation |

Deferral is intentional scope control, not an invitation to hide important behavior. When a later phase
introduces one of these features, it should link back to the transaction, concurrency, testing, and
diagnostic contracts established here.

## Final Verification

Run the phase from the repository root:

```powershell
dotnet restore 08-ef-core/08-ef-core.slnx
dotnet build 08-ef-core/08-ef-core.slnx -c Release --no-restore
dotnet test 08-ef-core/08-ef-core.slnx -c Release --no-build
./scripts/Test-ArticleMetadata.ps1
./scripts/Test-MarkdownLinks.ps1
```

Also inspect migration status using the pinned local tool:

```powershell
dotnet tool restore
dotnet ef migrations list `
  --project 08-ef-core/src/Learning.Persistence `
  --startup-project 08-ef-core/src/Learning.Persistence
```

## Exit Checklist

- [x] Every study-path entry has complete article metadata and navigation.
- [x] The model uses explicit relational configurations and reviewed migrations.
- [x] Reads demonstrate projection, no-tracking, loading strategy, and pagination choices.
- [x] Writes demonstrate tracked, disconnected, bulk, concurrent, and transactional workflows.
- [x] Failure, cancellation, rollback, replay, and stale-state cases are executable.
- [x] SQL parameterization and low-cardinality diagnostics have executable specifications.
- [x] Provider limitations and the live-provider CI gate are stated without false equivalence.
- [x] The Phase 08 solution, repository metadata, and Markdown-link checks pass.

## Handoff to Phase 09

Phase 09 can now add authentication and authorization persistence without rediscovering foundational
EF behavior. It should preserve short-lived context ownership, parameterized queries, explicit token
and session constraints, optimistic concurrency, transactional security events, safe logging, and
provider-accurate tests.

Continue to Phase 09 — Authentication and Authorization when that phase is implemented.
