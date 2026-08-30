---
title: "Compiled Queries, Safe Diagnostics, and Provider-Accurate Testing"
description: "Optimize measured EF Core hot paths, emit low-cardinality command observations without leaking data, and design a layered test suite that verifies behavior against the production database provider."
slug: ef-core-compiled-queries-diagnostics-provider-testing
phase: 8
order: 9
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 55
topics: [ef-core, compiled-queries, diagnostics, interceptors, performance, integration-testing]
prerequisites: [ef-core-set-based-operations-raw-sql-interceptors]
status: maintained
last-reviewed: 2026-08-30
---

# Compiled Queries, Safe Diagnostics, and Provider-Accurate Testing

Performance work starts with evidence. A faster query-compilation path cannot rescue an unindexed
predicate, an N+1 workflow, an oversized graph, or a database reached through excessive network
round trips. Diagnostics must reveal those costs without leaking user data, and tests must exercise the
provider that will interpret the generated SQL in production.

This slice connects those three concerns:

1. optimize one stable, measured query shape;
2. observe command outcomes through a safe telemetry contract;
3. assign each database behavior to the correct test layer.

## EF Already Caches Query Compilation

EF translates a LINQ expression tree into a database command and caches the result by query-tree shape.
Calls with the same shape and different parameter values normally reuse that cache. Keep runtime values
as parameters instead of embedding a different constant expression into every query.

Explicit compiled queries bypass the remaining expression-tree cache lookup. That can reduce CPU and
allocation on a frequently executed hot path, but the database operation and network usually dominate
the request. Benchmark before and after introducing the extra static delegate and constraints.

Use a compiled query when all of these are true:

- profiling identifies query compilation as a meaningful cost;
- the expression shape is stable and invoked at high frequency;
- inputs can be represented by simple scalar parameters;
- every context uses the same EF model;
- the specialized code remains understandable and tested.

Do not mechanically compile every query. Dynamic filters, infrequent administration screens, and
database-heavy analytical queries rarely benefit enough to justify a separate delegate.

## The Compiled Course Hot Path

`CourseCompiledQueries` stores one static `EF.CompileAsyncQuery` delegate. It returns published courses
for one category, uses a bounded result size, projects only list columns, includes a diagnostic query
tag, and does not track entities.

```csharp
private static readonly Func<LearningDbContext, string, int, IAsyncEnumerable<CourseListItem>> Query =
    EF.CompileAsyncQuery(
        (LearningDbContext context, string categoryName, int take) =>
            context.Courses
                .AsNoTracking()
                .Where(course => course.IsPublished && course.Category.Name == categoryName)
                .OrderBy(course => course.Slug)
                .Take(take)
                .Select(course => new CourseListItem(/* projected columns */)));
```

The delegate is reusable across different context instances and can be invoked concurrently only when
each invocation has its own context. A `DbContext` itself must never execute concurrent operations.

### Cancellation During Async Enumeration

An async compiled query returning several rows produces `IAsyncEnumerable<T>`. Cancellation belongs to
enumeration, not to a normal delegate token parameter:

```csharp
await foreach (CourseListItem item in Query(context, categoryName, take)
                   .WithCancellation(cancellationToken)
                   .ConfigureAwait(false))
{
    items.Add(item);
}
```

Validate input before invoking the delegate. The executable specification proves an invalid page size
causes no database command.

### Compiled Query Limitations

A compiled delegate is tied to one EF model. Do not use it where the same context type builds different
models per tenant or runtime configuration. Keep parameters scalar; member access over complex input
objects may not be supported as a compiled-query parameter expression.

Changing mapping, filters, collation, or provider still requires the same migration and provider tests
as a normal query. Compilation optimizes the client pipeline; it does not make the SQL correct.

## Query Tags as Operational Context

`TagWith` places a stable comment in generated SQL. A tag such as
`CourseCompiledQueries.PublishedByCategory` helps correlate database traces with an application use
case without using the entire SQL string as a metric label.

Good tags are:

- stable across deployments;
- low-cardinality;
- free of user, tenant, order, email, or request data;
- named after the use case rather than a controller implementation detail.

Never interpolate request values into a query tag. Tags reach database logs and monitoring systems.

## A Safe Command Observation Contract

`CommandMetricsInterceptor` converts EF command lifecycle callbacks into this deliberately small event:

```csharp
public sealed record CommandObservation(
    string Operation,
    TimeSpan Duration,
    bool Succeeded,
    string? ErrorType);
```

It records execution method, provider-reported duration, success, and exception type. It intentionally
cannot carry SQL or parameter values. Structural omission is safer than asking every logging call to
remember redaction.

One interceptor instance can observe concurrent contexts. The implementation therefore keeps no
mutable per-command state and sends each completed observation to an injected thread-safe sink.

The interceptor covers reader, scalar, and non-query commands in both synchronous and asynchronous
pipelines. Failure callbacks record the failure and allow the original provider exception to continue;
observability must not change persistence semantics.

### What Metrics Should Look Like

Export observations into low-cardinality instruments such as:

- command duration histogram by provider, operation, and stable query tag;
- command failure counter by provider and exception category;
- database round trips per request or background-job execution;
- connection-pool saturation from the database driver;
- retry count from the provider execution strategy.

Do not label metrics with raw SQL, parameter values, exception messages, IDs, URLs, or arbitrary tenant
names. High-cardinality labels make metrics expensive and unreliable, while sensitive values create a
security and compliance risk.

### Logging, Diagnostic Listeners, and Interceptors

Use the simplest mechanism that serves the need:

| Mechanism | Appropriate use |
|---|---|
| `Microsoft.Extensions.Logging` | normal structured application logging |
| `LogTo` | simple local debugging and learning |
| .NET diagnostic listeners | broad tracing integrations and libraries |
| interceptors | observing or deliberately influencing a specific EF operation |

Interceptors can suppress or modify commands, but production command rewriting is high risk. A logging
requirement alone does not justify changing command behavior.

## Context Pooling Is Not Connection Pooling

Database drivers commonly pool physical connections. EF context pooling reuses initialized
`DbContext` instances. These are independent optimizations:

```text
DbContext pool      -> reduces context setup and allocation
connection pool     -> reduces physical database connection setup
```

Most applications should begin with normal scoped contexts and driver connection pooling. Consider
`AddDbContextPool` only after measuring context setup overhead.

A pooled context behaves like reused infrastructure, even though DI gives it to one scope at a time.
Per-request state must be reset or supplied through a safe scoped factory. Never store an authenticated
user, mutable tenant ID, request correlation object, or disposable request service in context fields
that can survive return to the pool.

Pooling does not make a context thread-safe and does not permit concurrent commands on one instance.

## Compiled Models Solve a Different Problem

Compiled models reduce first-use model initialization for very large models, commonly hundreds or
thousands of entity types and relationships. They are generated with EF tooling and selected in context
configuration. This learning model is intentionally small, so generating a compiled model would add
maintenance without a credible measured benefit.

Re-evaluate compiled models when startup traces show model building is material—for example, large
serverless workloads or tools that create many short processes. Regenerate the compiled model whenever
the EF model changes and verify the generation step in CI.

## Provider-Accurate Testing

SQLite in-memory is fast, relational, and naturally isolated while its owner connection remains open.
It is excellent for many model, migration-chain, transaction, and query-shape specifications in this
repository. It is not SQL Server or PostgreSQL.

Provider differences include:

- case sensitivity and collation;
- supported CLR type operations;
- SQL syntax and raw SQL;
- generated keys and concurrency tokens;
- isolation levels, locking, deadlocks, and retry classification;
- migration SQL and online schema capabilities;
- indexes, query plans, JSON, temporal, full-text, and provider functions.

A passing SQLite test does not certify any of these production behaviors.

### Recommended Test Layers

| Layer | Database | Purpose | Frequency |
|---|---|---|---|
| Domain unit | none | entity invariants and pure conflict policy | every change |
| Fast relational | isolated SQLite | relational baseline and most workflows | every change |
| Production-provider integration | ephemeral actual engine | translation, migration, constraints, transactions | pull request/CI |
| Performance and operational | representative engine/data | plans, contention, pool and retry behavior | scheduled/release |

Do not replace query tests with mocked `DbSet` objects. That executes neither EF translation nor the
database. If application logic needs a test double, place an abstraction above the data-access query
and stub its returned result, while retaining integration tests for the real implementation.

### Production-Provider Fixture Design

A production-provider suite should:

1. start or connect to an approved disposable database instance;
2. generate a unique database/schema name per parallel test collection;
3. apply the committed migration chain rather than `EnsureCreated`;
4. seed only the data owned by the test;
5. execute the exact LINQ/raw SQL/application workflow;
6. assert behavior, constraints, and important generated SQL characteristics;
7. remove the isolated database in guaranteed cleanup;
8. capture container/database logs when setup or cleanup fails.

Pin the database engine image/version in CI. “Latest” makes a previously green build change behavior
without a repository change. Keep credentials outside source control and use a least-privileged test
principal where practical.

This environment currently has no Docker engine, so Phase 08 does not pretend that a container-backed
provider test ran. The suite remains explicit about that infrastructure requirement rather than silently
substituting SQLite and labeling the result production-compatible.

## Performance Investigation Order

Investigate in an order that targets dominant costs:

1. verify correctness and capture request-level latency;
2. count database round trips;
3. inspect generated SQL and actual query plans;
4. fix missing indexes, excessive rows/columns, and N+1 workflows;
5. measure database CPU, I/O, locks, waits, and connection saturation;
6. stabilize dynamic expression-tree shapes;
7. benchmark context pooling or compiled queries;
8. consider compiled models for demonstrably large startup models.

Micro-optimizing EF client overhead before fixing database work usually optimizes the smaller term.

## Review Checklist

- [ ] Was a hot path measured before introducing an explicit compiled query?
- [ ] Are compiled-query parameters simple scalars and result counts bounded?
- [ ] Is cancellation propagated through async enumeration?
- [ ] Does every invocation use a non-concurrent context instance with the same model?
- [ ] Are query tags stable, low-cardinality, and free of request data?
- [ ] Does telemetry exclude SQL text and parameter values by default?
- [ ] Are sync, async, success, and failure command paths considered?
- [ ] Does the interceptor preserve the original result, cancellation, and exception?
- [ ] Are context pooling and connection pooling configured independently?
- [ ] Is mutable request state prevented from leaking through pooled contexts?
- [ ] Are important translations and migrations executed on the actual production provider?
- [ ] Are production databases isolated, version-pinned, migrated, and reliably cleaned up?

## Executable Specifications

```powershell
dotnet test 08-ef-core/08-ef-core.slnx -c Release
```

`CompiledQueriesAndDiagnosticsTests` proves projection and tagging of the compiled hot path, input
validation before database execution, no tracked entity residue, safe success observations, and failure
observation without swallowing the provider exception.
