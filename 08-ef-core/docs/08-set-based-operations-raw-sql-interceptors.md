---
title: "Set-Based Updates, Raw SQL, and Command Interceptors"
description: "Use EF Core bulk update and delete APIs safely, understand their change-tracker and transaction boundaries, parameterize raw SQL, and inspect generated commands without coupling application behavior to SQL text."
slug: ef-core-set-based-operations-raw-sql-interceptors
phase: 8
order: 8
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 52
topics: [ef-core, execute-update, execute-delete, raw-sql, sql-injection, interceptors]
prerequisites: [ef-core-transactions-savepoints-retries-outbox]
status: maintained
last-reviewed: 2026-08-30
---

# Set-Based Updates, Raw SQL, and Command Interceptors

Normal tracked updates are aggregate-oriented: load an entity, invoke behavior, detect changes, and
persist the modified rows. Set-based operations are statement-oriented: describe a predicate and let
the database update or delete every matching row without materializing entities.

Neither style is universally better. The important decision is whether a change requires per-entity
domain behavior or whether it can be expressed completely and safely as a database statement.

## Choosing the Write Model

Use tracked entities when the operation needs:

- domain methods and invariants that cannot be expressed in SQL;
- navigation graph decisions;
- per-entity validation or domain events;
- automatic optimistic-concurrency checking from original values;
- a small unit of work containing different changes to different objects.

Consider `ExecuteUpdate` or `ExecuteDelete` when:

- all matching rows receive the same database-expressible transformation;
- loading the entities would add memory and network cost without adding correctness;
- the predicate fully describes which rows may change;
- skipped domain methods and tracker synchronization are understood;
- affected-row count is checked when it carries business meaning.

Typical examples include expiring sessions, publishing a prepared catalog batch, applying a status
transition selected by an operational rule, and deleting processed outbox records.

## `ExecuteUpdate` Is Immediate and Set-Based

`CourseBulkOperations.PublishReadyCoursesAsync` translates one LINQ query into one `UPDATE`. Its
predicate limits the write to the requested category, unpublished courses, and courses with at least
one module. Its setters update publication state and increment the application-managed version.

```csharp
return await dbContext.Courses
    .Where(course => course.CategoryId == categoryId)
    .Where(course => !course.IsPublished)
    .Where(course => course.Modules.Any())
    .ExecuteUpdateAsync(
        setters => setters
            .SetProperty(course => course.IsPublished, true)
            .SetProperty(course => course.PublishedAt, publishedAt)
            .SetProperty(course => course.Version, course => course.Version + 1),
        cancellationToken);
```

The command executes immediately. It is not queued until `SaveChanges`, and calling `SaveChanges`
afterward is neither required nor useful for that bulk statement.

### Domain Methods Are Not Invoked

`ExecuteUpdate` does not call `Course.Publish`. Therefore the query must reproduce the invariants that
belong to this maintenance operation, while durable invariants should also be protected by relational
constraints where practical.

Do not duplicate complex domain policy casually. If publication requires authorization, pricing
rules, event creation, or a different decision for each course, load aggregates or design a dedicated
database workflow instead.

### The Change Tracker Is Not Synchronized

Suppose a context tracks an unpublished course and then issues a bulk publish. The database row is
published, but the tracked object still says `IsPublished == false`. A later `SaveChanges` can even
overwrite bulk changes if that entity has conflicting modified properties.

Safe boundaries include:

- use a dedicated short-lived context for bulk maintenance;
- complete tracked work before the bulk statement;
- call `Entry(entity).ReloadAsync()` for the few objects that must remain in use;
- clear the tracker only when discarding all pending tracked work is intentional;
- never assume another query refreshes an entity already present in the identity map.

The executable test demonstrates stale tracked state and explicit reload rather than merely describing
the hazard.

### Concurrency Is Manual

Set-based APIs do not automatically add the original concurrency token predicate used by tracked
`SaveChanges`. When a command targets one expected version, include it explicitly and interpret the
affected row count:

```csharp
int affected = await context.Courses
    .Where(course => course.Id == id && course.Version == expectedVersion)
    .ExecuteUpdateAsync(
        setters => setters
            .SetProperty(course => course.Price, newPrice)
            .SetProperty(course => course.Version, course => course.Version + 1));

if (affected == 0)
{
    // Distinguish not-found from conflict only if the API contract requires it.
}
```

For a many-row maintenance command, version increments make later tracked writers notice that their
original snapshot is stale. Whether concurrent rows should be skipped, retried, or treated as an
operation failure is an application decision.

## `ExecuteDelete` and Bounded Cleanup

`ExecuteDelete` sends a direct `DELETE` for the query predicate. It does not load entities and does not
apply client-side cascade behavior. Database-configured cascades and foreign keys still apply.

Large retention jobs should usually operate in batches. One enormous delete may hold locks for too
long, grow the transaction log, cause replication lag, or make cancellation expensive. The sample:

1. selects at most the configured number of processed outbox IDs;
2. returns immediately when no work exists;
3. deletes that key set with one `ExecuteDelete` statement;
4. reports the affected row count.

This is two commands, so another worker can race between selection and deletion. Deleting by stable
primary keys remains safe, but the returned count may be smaller than the selected count. Stronger
worker coordination may require a transaction, row locking, lease columns, or provider-specific SQL.

### Time Retention and SQLite

SQLite has no native `DateTimeOffset` type and the provider does not translate every comparison or
ordering operation for it. The learning model intentionally surfaced this during an executable test.
It does not fall back to client evaluation, which could load an unbounded outbox into memory.

For production retention by time, choose a storage contract appropriate to the provider, for example:

- store UTC instants in a natively comparable timestamp type on PostgreSQL or SQL Server;
- convert UTC timestamps to an integer epoch representation for SQLite;
- map `DateTime` values whose `Kind` and UTC policy are enforced consistently;
- test the exact predicate, ordering, and migration on the production provider.

Provider limitations are architectural inputs, not test inconveniences to suppress.

## Transaction Boundaries

Each `ExecuteUpdate` or `ExecuteDelete` call executes immediately as one database command. EF does not
automatically wrap several separate bulk calls in one shared transaction. If two statements must be
atomic, create an explicit transaction and follow the execution-strategy guidance from the previous
article.

Mixing tracked and bulk changes in one context is especially easy to misunderstand:

```text
tracked modifications -> ExecuteUpdate executes now -> SaveChanges executes later
```

The order in code is the database order, but tracked snapshots are not refreshed by the middle step.
Prefer a clear orchestration boundary rather than relying on subtle tracker state.

## Raw SQL Is an Escape Hatch

LINQ should remain the default because it composes with the model, provider translation, and refactoring.
Raw SQL is appropriate for a query that cannot be represented efficiently, a database feature exposed
only through SQL, or carefully reviewed provider-specific behavior.

`FindBySlugWithSqlAsync` uses `FromSqlInterpolated`:

```csharp
string normalizedSlug = slug.Trim().ToLowerInvariant();

return dbContext.Courses
    .FromSqlInterpolated($"SELECT * FROM courses WHERE Slug = {normalizedSlug}")
    .AsNoTracking()
    .SingleOrDefaultAsync(cancellationToken);
```

Although this looks like string interpolation, EF turns the value into a `DbParameter`. The hostile
test input remains parameter data and cannot change the SQL grammar.

### Unsafe String Composition

Never concatenate or interpolate untrusted values into a plain SQL string:

```csharp
// Unsafe: input becomes executable SQL text.
string sql = "SELECT * FROM courses WHERE Slug = '" + input + "'";
```

Parameterization protects values, not SQL identifiers. Table names, column names, sort directions,
and SQL keywords cannot normally be parameters. If an API permits dynamic ordering or field selection,
map a small allowlist of public choices to known identifiers. Do not accept arbitrary identifier text
and attempt to escape it yourself.

### Entity Shape and Composition

A raw SQL entity query must return the columns EF needs to materialize that entity. Provider rules also
determine whether additional LINQ can compose over the SQL. Stored procedures and non-composable SQL
often require a different boundary.

Prefer projections or keyless entity types for reporting shapes rather than pretending an arbitrary
result set is a complete tracked aggregate. Add `AsNoTracking` for read-only entity materialization.

Use `Database.ExecuteSqlInterpolatedAsync` for non-query SQL whose values should be parameterized.
Remember that direct SQL bypasses tracked state just like set-based APIs.

## Command Interceptors

`CommandCaptureInterceptor` records command text and parameter snapshots in a thread-safe queue. The
tests use it to prove properties that matter:

- bulk publication emits one `UPDATE`;
- the readiness predicate reaches SQL;
- hostile input is absent from command text;
- normalized input is present as a parameter.

Interceptors are useful for diagnostics, auditing metadata, correlation, controlled fault injection,
and enforcing narrow technical policies. They are not a replacement for domain authorization or
application orchestration.

Production interceptors should:

- be thread-safe because singleton instances may observe concurrent contexts;
- avoid logging secrets, credentials, personal data, or full payloads;
- avoid expensive synchronous work on the command path;
- implement sync and async hooks when the application uses both;
- preserve cancellation and exceptions;
- avoid changing SQL unless the transformation is rigorously tested per provider.

SQL text assertions are inherently provider-sensitive. Assert only the property the test owns, such as
command count or parameterization, instead of snapshotting every quote, alias, and whitespace choice.

## Failure Modes to Review

| Failure | Cause | Safer design |
|---|---|---|
| Tracked entity overwrites bulk result | tracker retains an older snapshot | separate context or reload explicitly |
| Domain invariant silently skipped | bulk API bypasses entity method | encode a complete predicate or use aggregates |
| Lost-update detection disappears | no original concurrency predicate | filter by version and inspect affected rows |
| Cleanup causes long locks | unbounded delete | bounded, observable batches |
| Query works on SQLite but fails in production | provider translation differs | run production-provider integration tests |
| Injection through raw SQL | values concatenated into command text | interpolated/parameter APIs and allowlisted identifiers |
| Sensitive data reaches logs | interceptor records parameter values | redact or omit values in production telemetry |
| Several bulk statements partially succeed | no shared transaction | explicit retry-aware transaction when atomicity is required |

## Review Checklist

- [ ] Is set-based execution appropriate, or must domain behavior run per entity?
- [ ] Does the predicate encode every required eligibility rule?
- [ ] Are affected rows checked where zero has business meaning?
- [ ] Is optimistic concurrency represented explicitly?
- [ ] Can tracked instances become stale in the same context?
- [ ] Is a large update or delete bounded and observable?
- [ ] Are multiple statements enclosed in a transaction when they must be atomic?
- [ ] Are all raw SQL values parameterized?
- [ ] Are dynamic identifiers restricted to an allowlist?
- [ ] Does the raw result provide the full required shape?
- [ ] Does the interceptor avoid sensitive data and expensive work?
- [ ] Has the exact SQL path been tested on the production provider?

## Executable Specifications

Run the Phase 08 suite:

```powershell
dotnet test 08-ef-core/08-ef-core.slnx -c Release
```

`BulkOperationsAndRawSqlTests` verifies single-command bulk publication, tracker staleness and reload,
bounded outbox cleanup, raw SQL parameterization, and no-tracking materialization. These tests make the
dangerous boundaries visible while leaving provider-specific production verification as an explicit
later slice.
