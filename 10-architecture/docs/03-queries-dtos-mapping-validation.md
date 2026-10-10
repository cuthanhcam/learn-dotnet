---
title: "Queries, DTOs, Mapping, and Validation Boundaries"
description: "Trace a catalog query through bounded input, application contracts, SQL projection, and explicit response mapping without leaking domain or ORM types."
slug: architecture-queries-dtos-mapping-validation
phase: 10
order: 3
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 25
topics: [cqrs, dto, mapping, validation, pagination]
prerequisites: [architecture-aggregates-use-cases-concurrent-enrollment]
status: maintained
last-reviewed: 2026-10-10
---

# Queries, DTOs, Mapping, and Validation Boundaries

## The Problem Before the Pattern

The enrollment command must load learner membership to decide whether a request consumes a seat.
The catalog only needs a title, capacity, count, and version. Loading every aggregate for a catalog
response would deserialize learner identities, allocate sets, and expose information the caller does
not need. Returning an EF entity instead would couple clients to persistence details and mutable state.

Start with the question each operation answers. A command asks the model to decide and change state.
A query describes an existing view. Their code paths can differ while their data remains in one database.
This phase uses that small form of CQRS, not separate read/write services or event sourcing.

## Code Reading Map

| File | Responsibility |
|---|---|
| `Application/Offerings/ListOfferings.cs` | query input, application validation, and read port |
| `Application/Offerings/OfferingSummary.cs` | materialized catalog DTO and page envelope |
| `Infrastructure/Persistence/EfCourseOfferingStore.cs` | database ordering, paging, and projection |
| `Api/Features/EnrollmentEndpoints.cs` | HTTP binding and field-oriented validation responses |
| `tests/Learning.Architecture.Tests/OfferingQueryTests.cs` | invalid ranges and bounded page behavior |
| `tests/Learning.Architecture.Tests/Persistence/RelationalEnrollmentTests.cs` | SQL-backed projection |

Paths in the first four rows are relative to their `src/Learning.Architecture.*` project directories.

## Follow One Read

```text
GET /api/v1/offerings?offset=0&limit=20
  -> bind nullable integer parameters
  -> choose documented defaults and reject invalid ranges
  -> ListOfferingsHandler
  -> IOfferingQueries
  -> SQL ORDER BY, OFFSET/LIMIT, SELECT only catalog fields
  -> OfferingPage / OfferingSummary
  -> JSON response
```

`IOfferingQueries` returns a completed page, not `IQueryable`. The adapter owns translation and execution.
Callers cannot append provider-specific expressions after the application method returns or accidentally
enumerate a query after its DbContext has been disposed. Exceptions and cancellation occur within the
operation's lifetime rather than being deferred to an unrelated serializer.

`AsNoTracking` makes read intent explicit. The projection does not materialize tracked domain entities
in the first place. The relational test also checks the change tracker is empty after querying.

## What a DTO Protects

`OfferingSummary` contains scalar public catalog values. It excludes the learner set and JSON storage
representation. `AvailableSeats` is derived from capacity and count rather than maintained as a third
independent mutable field. `OfferingPage` includes the requested offset and limit so a client can
understand the page without inferring server defaults.

A DTO is not inherently safe merely because it is named DTO. Review every field for necessity,
confidentiality, compatibility, and authority. A mutable collection of internal entities inside a DTO
still leaks internal state. This sample materializes an array of immutable record values at the boundary.
The returned `IReadOnlyList` describes the contract; it does not claim deep immutability of arbitrary
implementations supplied by another adapter.

## Manual Mapping Is a Deliberate Choice

The SQL projection names every selected field. There is no reflection mapping library, hidden property
discovery, or convention that can suddenly expose a newly added persistence field.

For a small contract this explicitness is useful. A mapping library may become worthwhile when many
repetitive mappings have stable conventions and validation tests. Even then, confirm whether mapping
executes as translated SQL or after materialization. A convenient mapping expression that causes all
rows to load into memory is not an equivalent implementation.

Do not map client input directly onto `OfferingRow` or `CourseOffering.Restore`. Restoration accepts
trusted persistence snapshots subject to invariant checks; it is not an authorization or request model.

## Validation Has More Than One Owner

| Boundary | Example | Failure representation |
|---|---|---|
| HTTP binding | `limit=not-a-number` | framework 400; no application call |
| HTTP request shape | zero/oversized page or malformed request key | validation problem response |
| Application contract | invalid page from console/job caller | argument exception |
| Domain rule | full offering or existing learner | typed business decision |
| Database invariant | version/capacity/count constraint | persistence failure; investigate cause |
| Authorization | missing scope or learner identity | 401/403 before mutation |

Some range checks appear in both host and application intentionally: the host supplies a useful client
error, while the application protects non-HTTP callers. Repetition becomes problematic when the values
or semantics diverge. For a growing contract, centralize reusable pure validators without making the
domain reference ASP.NET validation types.

Do not catch every `ArgumentException` across the pipeline and convert it to 400. An internal mapping
bug can throw the same exception type. Validate at a known boundary and translate only expected failures.
Likewise, “course full” is not an exception that should fill error logs during ordinary business use.

## Pagination Guarantees and Limits

The handler allows `offset` from 0 through 10000 and `limit` from 1 through 100. These are lab workload
budgets, not universal defaults. They prevent an unbounded catalog request from becoming an accidental
full export. Both adapters order by offering ID before paging; dictionary enumeration order is not a
contract.

Offset pagination is easy to understand but can scan skipped rows and shift under concurrent inserts
or deletes. A stable ordering does not create a multi-request snapshot. For a large live catalog,
consider keyset pagination using a documented stable key and indexed ordering. If sorting by a nonunique
field, include a unique tie-breaker and define collation/null behavior.

This response intentionally omits a total count. Computing one would be a separate query whose cost
and consistency need justification. An empty page is valid, including when the offset exceeds the
current catalog size.

## Tests to Read and Extend

Run:

```powershell
dotnet test 10-architecture/tests/Learning.Architecture.Tests --filter "FullyQualifiedName~OfferingQueryTests|FullyQualifiedName~Query_Projects"
```

The existing tests reject invalid ranges, enforce a one-item page in the in-memory adapter, verify
available seats, and execute a relational projection without tracked state. HTTP integration tests
exercise invalid numeric binding and ensure learner information is absent from the catalog JSON.

Add a new sort mode as an exercise. Specify the public enum/string contract, reject unknown values,
add a unique tie-breaker, compare SQL translation, and test page boundaries. Do not accept raw SQL column
names from a query string.

## Review Questions

1. Why does the read port return a page instead of an aggregate collection?
2. Which checks would remain if the HTTP host disappeared tomorrow?
3. Is a page returned before or after a concurrent enrollment guaranteed to contain a particular count?
4. When would a separate read database justify its synchronization and operational cost?
5. Which response changes are compatible additions, and which break an existing client?

## Official References

- [CQRS pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs) explains the separation and its trade-offs; this lab selects the shared-database variant.
- [Efficient querying](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying) covers projection and bounded result sets.
- [EF Core pagination](https://learn.microsoft.com/en-us/ef/core/querying/pagination) discusses ordering and keyset alternatives.

## Navigation

- Previous: [Aggregates and concurrent enrollment](02-aggregates-use-cases-concurrent-enrollment.md)
- Next: [Persistence, transactions, and durable idempotency](04-persistence-transactions-idempotency.md)
