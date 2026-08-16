---
title: "Minimal API Routing, Validation, and Problem Details"
description: "Route groups, constraints, parameter binding, typed results, validation boundaries, status codes, resource creation, and standard errors."
slug: aspnet-core-routing-validation-problem-details
phase: 7
order: 3
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 34
topics: [aspnet-core, minimal-apis, routing, validation, problem-details]
prerequisites: [aspnet-core-di-configuration-options]
status: maintained
last-reviewed: 2026-08-15
---

# Minimal API Routing, Validation, and Problem Details

## Route Design

Route groups apply a shared prefix, tags, authorization, filters, or metadata to related endpoints.
Constraints such as `{id:guid}` participate in route matching; they are not complete business
validation. Use stable nouns and HTTP methods rather than embedding implementation actions in URLs.

Named routes allow creation responses to generate a `Location` header without hard-coding a URL.
`CreatedAtRoute` returns `201 Created`, the representation, and a link to the newly created resource.

## Binding and Validation

Minimal API handler parameters can come from route values, query strings, headers, body JSON,
services, and special framework types. Keep request DTOs separate from persistence/domain types so
clients cannot bind internal fields accidentally.

The product request validates required name, maximum length, and positive price. Validation produces
`HttpValidationProblemDetails`, a standardized object containing field errors. Syntax validity does
not prove business validity; uniqueness, permissions, inventory, and concurrency checks belong at
appropriate application/domain boundaries.

## Typed Results

`Results<T1,T2>` declares every response variant in the method signature. The compiler checks returned
variants, and endpoint metadata/OpenAPI tooling can infer the contract more accurately than from a
generic `IResult`.

Initial endpoint contracts:

| Operation | Success | Other expected result |
|---|---|---|
| List products | `200 OK` | — |
| Get product | `200 OK` | `404 Not Found` |
| Create product | `201 Created` | `400 Validation Problem` |

Malformed JSON, unsupported media types, authentication, authorization, concurrency conflicts, and
dependency failures require additional explicit contracts as the phase grows.

## Problem Details

Problem Details provides a consistent error shape with fields such as type, title, status, detail,
and instance. Do not expose stack traces, SQL, secrets, internal hostnames, or exception messages to
untrusted clients. Log diagnostic detail server-side with correlation while returning safe client
information.

`AddProblemDetails` registers the service and `UseExceptionHandler` handles downstream exceptions.
More advanced slices will map known domain/application exceptions deliberately rather than treating
every failure as an undifferentiated status.

## Integration Tests

`WebApplicationFactory<Program>` hosts the real pipeline in-process. Tests send HTTP requests and
therefore cover route selection, JSON serialization, binding, validation, middleware, status codes,
headers, DI, and repository behavior together.

Keep fast unit tests for complex pure policies, but do not replace HTTP contract tests with direct
calls to private handler methods.

## References

- [Minimal APIs overview](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview)
- [Parameter binding](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding)
- [Create Minimal API responses](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses)
- [Handle errors in ASP.NET Core APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/handle-errors)

## Navigation

- Previous: [Dependency injection and configuration](02-dependency-injection-configuration.md)
- Next: Phase 07 controller, filter, and OpenAPI article
