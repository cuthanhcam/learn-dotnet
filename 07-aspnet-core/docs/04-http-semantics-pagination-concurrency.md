---
title: "HTTP Semantics, Pagination, and Optimistic Concurrency"
description: "Design bounded collection APIs and safe conditional mutations with ETag, If-Match, Problem Details, and atomic repository invariants."
slug: aspnet-core-http-semantics-pagination-concurrency
phase: 7
order: 4
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 42
topics: [aspnet-core, http, pagination, etag, optimistic-concurrency, problem-details]
prerequisites: [aspnet-core-routing-validation-problem-details]
status: maintained
last-reviewed: 2026-08-15
---

# HTTP Semantics, Pagination, and Optimistic Concurrency

An HTTP API is a concurrent distributed system boundary. A handler can be correct in isolation and
still lose data when two clients read the same representation and later write different changes.
This article extends the product API from basic CRUD into an explicit protocol for bounded reads,
uniqueness conflicts, and conditional writes.

## Model the Contract Before the Handler

The sample keeps four representations distinct:

- `CreateProductRequest` contains only client-writable creation fields.
- `UpdateProductRequest` contains only client-writable replacement fields.
- `Product` is the resource representation and includes server-owned identity, timestamps, and version.
- `PagedResponse<Product>` carries items plus navigation metadata.

Separate input models prevent over-posting. A client cannot choose an identifier, creation time, or
version merely by adding JSON properties. The same separation makes future authorization rules and
backward-compatible API evolution easier to reason about.

## Bounded Collection Reads

An unbounded `GET /api/products` eventually becomes a reliability defect: response allocation,
serialization, bandwidth, and database work grow with the entire data set. The sample accepts
one-based `page` and `pageSize`, supplies documented defaults, and rejects values outside the
configured maximum.

```http
GET /api/products?page=2&pageSize=20
```

The response envelope includes `items`, `page`, `pageSize`, `totalCount`, and the calculated
`totalPages`. This page-number design is approachable and supports jumping to an arbitrary page.
For a large or rapidly changing data set, prefer cursor/keyset pagination: order by a stable unique
key and ask for records after the last key observed. Offset pagination becomes slower at deep pages
and can skip or duplicate records when concurrent inserts change earlier offsets.

Pagination rules are part of the public contract:

- define a deterministic order with a unique tie-breaker;
- impose a server-side maximum regardless of client input;
- document whether indexes are zero- or one-based;
- avoid claiming a perfectly stable total if the data can change between requests;
- use checked arithmetic when converting page numbers into offsets.

## Status Codes Describe Different Failures

The API deliberately distinguishes these cases:

| Status | Meaning in this sample |
|---:|---|
| `400 Bad Request` | Field validation failed or `If-Match` has invalid syntax. |
| `404 Not Found` | The identified product does not exist. |
| `409 Conflict` | A different product already owns the case-insensitive name. |
| `412 Precondition Failed` | The supplied ETag no longer represents the current product. |
| `428 Precondition Required` | A mutation omitted the required `If-Match` precondition. |

All errors use Problem Details rather than unrelated anonymous JSON shapes. Clients can branch on
the status and stable problem contract while people still receive a useful title and detail.

## The Lost-Update Timeline

Suppose a product is at version 7:

1. Client A reads version 7.
2. Client B also reads version 7.
3. Client A updates the price, producing version 8.
4. Client B submits a name change based on version 7.
5. An unconditional update overwrites some or all of Client A's change.

The API exposes the version as a strong entity tag:

```http
ETag: "7"
```

The client sends that validator with a write:

```http
PUT /api/products/8d266ff8-2cc5-4fa0-a349-ccb18fe81f26
If-Match: "7"
Content-Type: application/json

{ "name": "Standing Desk", "price": 499.00 }
```

The server performs the write only if the stored version is still 7. A successful update increments
the version and returns the new ETag. A stale write receives `412`; the client must fetch the latest
representation, reconcile the change, and retry deliberately. Blind automatic retries would merely
hide the conflict and can restore the lost-update bug.

## Header Parsing Is Input Validation

Headers are untrusted input. This learning contract requires exactly one strong, quoted, positive
integer ETag. It rejects weak tags, comma-separated alternatives, wildcard matching, and malformed
values. A production API may implement the complete conditional-request grammar, but it must do so
intentionally rather than accepting ambiguous strings with ad-hoc trimming.

Validation order matters. Body syntax and field validity are checked before application behavior;
the conditional header is checked before mutation; the version comparison and state change occur in
one atomic repository operation.

## Atomicity Belongs at the Persistence Boundary

Checking a version in the application service and updating later is a time-of-check/time-of-use race.
Another request can modify the resource between those operations. Therefore
`TryUpdateAsync(product, expectedVersion)` and `TryDeleteAsync(id, expectedVersion)` compare and mutate
inside the repository's critical section.

The in-memory repository also owns a case-insensitive name index. One lock protects both dictionaries
because together they form one invariant: every stored product has exactly one indexed name and no
two products share that normalized name. Concurrent collections alone would not make a multi-step
check-and-update sequence atomic.

Phase 08 can replace the lock with database mechanisms:

- a unique database index is the authoritative name constraint;
- a concurrency token participates in the `UPDATE` or `DELETE` predicate;
- zero affected rows indicates a missing resource or version conflict;
- database exceptions are translated at the application boundary, not leaked to HTTP clients.

## Application Service Boundary

`ProductCatalog` coordinates time, identity, normalization, version increments, and repository calls.
Endpoints remain responsible for HTTP concerns: binding, headers, status codes, and response shapes.
The repository remains responsible for atomic state transitions. This division is modest rather than
ceremonial, and each layer has a reason to change independently.

`TimeProvider` is injected instead of reading `DateTimeOffset.UtcNow` throughout the code. Tests can
replace time deterministically without changing production behavior. Cancellation tokens flow from
the request into the service and repository so a real asynchronous persistence implementation can
stop unnecessary work when the client disconnects.

## Testing the Protocol

The integration suite verifies behavior through an in-process server:

- duplicate names conflict even when casing differs;
- page metadata is returned and the configured maximum is enforced;
- successful updates increment the version and issue a new ETag;
- stale writes fail with `412`;
- missing preconditions fail with `428`;
- weak or malformed tags fail with `400`;
- conditional deletion removes the resource.

Repository tests separately prove that a failed stale update changes no state, renaming updates both
indexes atomically, and pre-cancelled operations do not mutate data. Integration tests protect the
wire contract; focused tests protect invariants and make failures easier to diagnose.

## Review Checklist

- Is every collection endpoint bounded and deterministically ordered?
- Can clients distinguish invalid input, missing resources, uniqueness conflicts, and stale writes?
- Are ETags treated as opaque HTTP validators by clients?
- Does the version comparison occur atomically with the mutation?
- Is uniqueness enforced in the authoritative persistence store?
- Does every successful mutation return the current representation or a clear retrieval location?
- Are cancellation and concurrency paths tested, not only happy paths?

## Further Reading

- [MDN: Conditional requests](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Conditional_requests)
- [RFC 9110: HTTP Semantics](https://www.rfc-editor.org/rfc/rfc9110)
- [ASP.NET Core Minimal API responses](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses)

