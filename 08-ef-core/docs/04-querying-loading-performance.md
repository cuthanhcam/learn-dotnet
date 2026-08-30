---
title: "Query Translation, Loading Strategies, and Performance"
description: "Build efficient EF Core queries with projection, no-tracking, eager and explicit loading, N+1 detection, keyset pagination, SQL inspection, and round-trip tests."
slug: ef-core-querying-loading-performance
phase: 8
order: 4
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 54
topics: [ef-core, linq, sql-translation, n-plus-one, projection, pagination, performance]
prerequisites: [ef-core-change-tracking-disconnected-updates]
status: maintained
last-reviewed: 2026-08-30
---

# Query Translation, Loading Strategies, and Performance

An EF Core LINQ query is a database-program description until it executes. Performance depends on the
translated SQL, selected columns, indexes, number of round trips, result cardinality, tracking work,
network latency, and query plan—not on how concise the C# expression looks.

## Keep Work in the Expression Tree

Operators applied to `IQueryable<T>` become an expression tree for the provider. Materialization
methods such as `ToArrayAsync`, `SingleAsync`, and `FirstOrDefaultAsync` execute it. Calling
`AsEnumerable`, `ToList`, or a client method too early crosses the boundary and moves later work into
.NET memory.

`CourseQueries` keeps filtering, ordering, pagination, counting, joins, and projection in the database.
An unsupported custom `HasEvenWordCount` predicate throws “could not be translated” rather than
silently loading the table. Modern EF Core generally allows client evaluation only in the top-level
projection; understand that boundary and make any client work explicit after a bounded result.

## Projection Before Materialization

Read endpoints rarely need complete mutable entities. Projecting directly to `CourseListItem` selects
title, slug, price, category name, and an aggregate module count. It does not select `CreatedAt` or
`Version`, materialize module rows, or populate the change tracker.

Projection benefits:

- fewer columns and bytes transferred;
- no accidental lazy/explicit follow-up access;
- stable read contracts separate from persistence entities;
- server-side aggregates and formatting where translatable;
- lower tracking and relationship-fixup cost.

Projection is not automatically efficient. Nested collections, correlated subqueries, complex
conditionals, and provider limitations can still produce expensive SQL. Inspect it.

## The N+1 Problem

The executable N+1 example loads two courses with one query, then explicitly loads modules inside a
loop. The command interceptor observes three commands:

```text
1 query for courses
N queries for modules (one per course)
```

With 1,000 courses this becomes 1,001 round trips. Local tests can hide the cost because database
latency is near zero. Production latency, connection-pool pressure, and database CPU make it visible.

N+1 can arise through:

- lazy-loading navigation access in a loop;
- explicit `Load` in a loop;
- per-row repository/service calls;
- serializers walking unloaded navigation properties;
- authorization or enrichment queries per item.

Count commands for representative use cases and monitor database dependency spans in production.

## Loading Strategies

### Projection

Prefer projection for API/read models. Load exactly the shape required and keep entities out of the
serialization boundary.

### Eager Loading

`Include` loads a known entity graph as part of the query. The test proves courses and modules use one
round trip with a `LEFT JOIN`. This fixes N+1 but can duplicate principal columns for every child row.

### Explicit Loading

Explicit loading is appropriate when a decision made after loading the principal determines whether a
navigation is needed. Do not place it in an unbounded loop. Batch the required keys or redesign the
projection.

### Lazy Loading

Lazy loading is convenient but hides database I/O behind property access. It makes round trips hard to
see, causes serializer surprises, requires proxies or injected loaders, and commonly produces N+1.
This learning model does not enable it. If a project chooses lazy loading, command-count tests and
strict context lifetime rules are essential.

## Cartesian Explosion and Split Queries

Including multiple sibling collections in one query creates a relational cross product. Ten modules
and ten tags can produce 100 rows for one course, duplicating course columns each time. `AsSplitQuery`
uses multiple SQL queries to avoid that multiplication; `AsSingleQuery` keeps one round trip but may
transfer far more data.

Split queries trade row explosion for additional round trips and consistency considerations between
queries. Apply deterministic ordering with pagination and inspect the generated statements. A later
relationship slice adds a second collection and tests both strategies explicitly.

## Offset Pagination

Page-number pagination translates to `Skip`/`Take`, typically `OFFSET`/`LIMIT`. It supports jumping to
an arbitrary page and total counts, but deep offsets require the database to process skipped rows.
Concurrent inserts/deletes before the offset can duplicate or omit items between pages.

Always use deterministic ordering with a unique tie-breaker. An order by non-unique title alone is not
stable.

## Keyset Pagination

`ListAfterSlugAsync` uses the unique indexed slug as a cursor:

```text
WHERE slug > @cursor
ORDER BY slug
LIMIT @take
```

It does not scan an ever-growing offset and remains stable when rows are inserted before the cursor.
The client cannot jump directly to arbitrary page numbers without additional index/navigation data.

For compound order `(CreatedAt, Id)`, the continuation predicate must be lexicographic:

```text
CreatedAt > lastCreatedAt
OR (CreatedAt = lastCreatedAt AND Id > lastId)
```

The index order must match. Cursor values should be encoded, validated, and treated as opaque by API
clients.

## Total Counts Are Separate Work

`CourseQueries.ListAsync` executes one `COUNT(*)` and one data query. That is two database round trips.
Counts over large filtered datasets can be more expensive than retrieving one page. Decide whether the
client truly needs an exact count, an approximate count, or only a `hasMore` signal obtained by reading
one extra row.

The command-count test makes the two-query contract explicit instead of pretending pagination metadata
is free.

## Inspect Generated SQL

Useful techniques include:

- `ToQueryString()` during development/tests;
- `LogTo` with safe filtering in local environments;
- structured EF command logging;
- `DbCommandInterceptor` for instrumentation or tests;
- query tags through `TagWith`;
- database execution plans and runtime statistics.

`FindAsync` and keyset queries include stable tags such as `CourseQueries.FindById`. Tags appear as SQL
comments and help connect a slow database command to a use case. Never put user input or high-cardinality
identifiers into query tags.

SQL string assertions are provider/version-sensitive. The tests assert broad intent—command count,
join presence, omitted columns, tag, `WHERE`, and absence of `OFFSET`—rather than snapshotting every
quote and alias.

## Interceptors

`CommandCaptureInterceptor` records reader and scalar command text in a thread-safe queue. It is a test
instrument, not a mock database. Queries still execute through SQLite and relational constraints remain
active.

Production interceptors can add telemetry, enforce policies, or modify commands, but they sit on a hot
path. Keep them fast, thread-safe, low-allocation, cancellation-aware, and free from recursive database
calls. Never log parameter values that may contain secrets or personal data without an explicit policy.

## Indexes Follow Query Shapes

The model has unique indexes for category name and course slug. The keyset query benefits from the slug
index. Filtering by title prefix may need a title index depending on provider collation, selectivity,
and workload.

Do not add an index for every predicate. Indexes consume storage and make inserts/updates more
expensive. Use representative data and execution plans. Composite index column order follows equality,
range, and ordering patterns—not property declaration order.

## Compiled Queries and Query Cache

EF Core already caches query compilation by expression-tree shape and parameterizes values. Avoid
dynamically constructing a different constant-shaped tree per request. Explicit compiled queries can
reduce overhead in extremely hot, stable paths but add complexity and model constraints. Benchmark
after fixing round trips, result size, indexes, and tracking; compilation is rarely the first bottleneck.

## Testing Real Project Scenarios

The query tests prove:

- explicit loading in a loop creates N+1 commands;
- eager loading of one known collection uses one command;
- projection uses a count plus a narrow data command and no tracking;
- keyset continuation returns the next slug without `OFFSET`;
- query tags appear in executed SQL;
- an unsupported client method fails translation before loading data.

Provider-specific SQL still needs production-provider tests. SQLite passing does not certify collation,
functions, indexes, plans, or pagination syntax elsewhere.

## Review Checklist

- Where does this `IQueryable` execute, and where is it materialized?
- Which columns, rows, navigations, and aggregates does the consumer actually need?
- How many commands and round trips does one use case generate?
- Can any loop trigger lazy or explicit database access?
- Would projection, eager loading, batching, or split queries fit the graph better?
- Is pagination deterministic, indexed, and appropriate for deep navigation?
- Is an exact total count worth its separate query?
- Does generated SQL contain expected predicates, joins, tags, and parameters?
- Have representative execution plans been checked on the production provider?
- Are interceptors thread-safe and free of sensitive parameter logging?

## Further Reading

- [Efficient querying](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)
- [Loading related data](https://learn.microsoft.com/en-us/ef/core/querying/related-data/)
- [Single versus split queries](https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries)
- [Pagination](https://learn.microsoft.com/en-us/ef/core/querying/pagination)
- [Client versus server evaluation](https://learn.microsoft.com/en-us/ef/core/querying/client-eval)
- [Interceptors](https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/interceptors)
