---
title: "Migrations and Safe Schema Evolution"
description: "Create, inspect, test, script, apply, and deploy EF Core 10 migrations while preserving schema history and provider-specific safety."
slug: ef-core-migrations-schema-evolution
phase: 8
order: 2
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 46
topics: [ef-core, migrations, schema, deployment, sqlite]
prerequisites: [ef-core-dbcontext-modeling-unit-of-work]
status: maintained
last-reviewed: 2026-08-30
---

# Migrations and Safe Schema Evolution

An EF Core migration is reviewed source code describing how one model version transitions to another.
It is not a disposable build artifact and not permission for every application instance to modify a
production database during startup. Migration source, model snapshot, deployment scripts, and database
history together form the schema evolution chain.

## Pin the Toolchain

The repository commits a local tool manifest containing `dotnet-ef` 10.0.11 and references matching
EF Core runtime/design packages. Contributors restore the exact tool with:

```powershell
dotnet tool restore
```

Pinning avoids machine-global tool drift. Keep the EF tool, design package, runtime packages, target
framework, and SDK on compatible major versions. Review patch upgrades normally; do not assume a
generated migration remains identical across tooling changes.

## Design-Time Context Creation

Migration commands must create `LearningDbContext` without running an application host.
`DesignTimeLearningDbContextFactory` supplies deterministic SQLite options for model discovery. Its
connection string is explicitly local and contains no production credentials.

Production configuration remains owned by the deployed application's composition root. A design-time
factory should not read hidden developer state that makes generation irreproducible on CI.

## Generate and Inspect

The initial migration was created with:

```powershell
dotnet ef migrations add InitialCreate `
  --project src/Learning.Persistence/Learning.Persistence.csproj `
  --output-dir Migrations
```

Generation produces:

- a migration with `Up` and `Down` operations;
- designer metadata for that migration;
- `LearningDbContextModelSnapshot`, representing the latest model known to migrations.

Inspect every operation. Verify column types, lengths, nullability, defaults, computed values, indexes,
foreign keys, delete actions, and provider annotations. A migration can compile while dropping data,
locking a large table, rebuilding an SQLite table, or creating an expensive index.

Never hand-edit only the snapshot to make a diff disappear. The snapshot and migration chain must
describe the same history. If an unshared migration is incorrect, remove and regenerate it; once a
migration is deployed, create a forward corrective migration rather than rewriting shared history.

## Apply Through Migrations, Not `EnsureCreated`

The relational test fixture now calls `Database.MigrateAsync`. This applies pending migrations and
creates `__EFMigrationsHistory`. `EnsureCreated` builds the current model directly, bypasses migration
history, and is not a substitute for verifying incremental upgrades.

`EnsureCreated` remains useful for disposable prototypes with no migration lifecycle. Mixing it with
migrations on the same database leads to incompatible schema ownership.

## Test the Schema Chain

The migration specifications verify:

- the complete chain applies to an empty relational database;
- exactly the expected initial migration is recorded;
- no pending migrations remain after application;
- the generated upgrade SQL contains tables, foreign keys, and unique indexes;
- SQLite explicitly rejects idempotent script generation.

The last case is a provider lesson, not a framework defect hidden by the test. SQLite lacks the
procedural existence checks EF needs for general idempotent scripts. SQL Server or PostgreSQL deployment
tests must generate and inspect scripts using their own provider.

## Deployment Strategies

Common strategies include:

| Strategy | Strength | Risk/ownership |
|---|---|---|
| Reviewed SQL scripts | DBA review, audit, deployment control | Must generate for correct provider/from-version |
| Migration bundle | Self-contained, repeatable EF runner | Still needs credentials, locking, rollout policy |
| Dedicated deployment job | One controlled actor before application rollout | Pipeline must gate application compatibility |
| Application startup migration | Simple for small controlled systems | Concurrent instances, permissions, startup failure, long locks |

EF Core 9+ adds migration locking, which reduces concurrent migrator races but does not solve least
privilege, long-running operations, rollback, zero-downtime compatibility, or deployment ownership.
Production application identities often should not have schema-alter permissions.

## Expand-and-Contract Changes

Deploying code and schema atomically is difficult in rolling systems. Prefer compatible stages:

1. add a nullable/new column or table;
2. deploy code that writes old and new representations where needed;
3. backfill in bounded, observable batches;
4. switch reads to the new representation;
5. stop old writes;
6. enforce required constraints and remove obsolete schema in a later deployment.

Renames are especially dangerous because EF may infer drop-and-add. Replace generated operations with
an explicit rename when that matches reality, and test upgrade from a database containing representative
data.

## `Down` Is Not Always Production Rollback

A generated `Down` method documents a reverse schema transition, but destructive data changes cannot
always be reversed. Production rollback may mean rolling application code forward to compatibility,
restoring a backup, or applying a corrective migration. Define rollback and backup verification before
executing a risky migration.

## Transactions and Long Operations

EF generally wraps migration operations in transactions where the provider supports it. Some database
operations cannot run in a transaction, and EF Core 10 no longer assumes one transaction must span an
entire migration chain. Understand provider behavior before mixing data movement, index creation, or
raw SQL with schema changes.

Avoid large backfills inside a deployment migration when they can exceed lock or command timeouts.
Use resumable application jobs with progress tracking when operational control matters.

## Secrets and Generated Artifacts

Migration C# files must be committed. Local `.db`, `.sqlite`, MDF, and LDF database files remain
ignored. Connection strings belong in environment-specific secure configuration, not the design-time
factory, migration source, command history, or generated script checked into a public repository.

## Review Checklist

- Are EF runtime, design package, tool, SDK, and provider versions compatible and pinned?
- Does the design-time factory avoid production secrets and host side effects?
- Was every generated operation reviewed for data loss, locks, types, defaults, and indexes?
- Does a test apply migrations from an empty database and report no pending migrations?
- Are production-provider scripts generated and validated separately?
- Is schema application owned by one controlled deployment actor?
- Can old and new application versions coexist during rolling deployment?
- Is rollback realistic for destructive changes and are backups verified?
- Are migration sources committed while database files remain ignored?

## Further Reading

- [Managing database schemas with migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Applying migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying)
- [Migration bundles](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying#bundles)
- [EF Core tools reference](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
