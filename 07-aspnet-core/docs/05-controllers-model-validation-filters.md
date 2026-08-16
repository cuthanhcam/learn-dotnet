---
title: "Controllers, Model Validation, and Filters"
description: "Understand controller activation, ApiController conventions, action return types, DataAnnotations validation, DI-created filters, and request-scoped context."
slug: aspnet-core-controllers-model-validation-filters
phase: 7
order: 5
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 38
topics: [aspnet-core, controllers, model-binding, validation, filters, dependency-injection]
prerequisites: [aspnet-core-http-semantics-pagination-concurrency]
status: maintained
last-reviewed: 2026-08-15
---

# Controllers, Model Validation, and Filters

ASP.NET Core supports Minimal APIs and controller-based APIs on the same host. Neither programming
model is universally superior. The repository uses products to demonstrate a compact Minimal API
vertical slice and order quotes to demonstrate controller conventions, filters, and request-scoped
state. Comparing executable examples is more useful than treating style preference as architecture.

## Register and Map Both Halves

Controller support has two deliberate composition steps:

```csharp
builder.Services.AddControllers();
// ...
app.MapControllers();
```

`AddControllers` registers MVC services such as controller activation, model binding, validation,
formatters, and filters. `MapControllers` creates endpoints from attribute routes. Omitting either
step produces an incomplete application: registration alone exposes no routes, while mapping without
the required services cannot activate the framework.

## What `[ApiController]` Adds

`OrderQuotesController` derives from `ControllerBase`, not `Controller`, because it returns HTTP data
and needs no view support. `[ApiController]` adds API-focused conventions:

- attribute routing becomes required;
- binding-source inference reduces repetitive attributes;
- invalid model state automatically produces an HTTP 400 validation response;
- error responses use Problem Details conventions;
- multipart form binding receives useful inference.

Automatic validation means the action normally receives a structurally valid request. It does not
replace business validation. DataAnnotations can express local rules such as required values, string
length, patterns, and numeric ranges; rules involving current state, permissions, or other entities
belong in application/domain behavior and authoritative persistence constraints.

## Input DTOs and DataAnnotations

`CreateOrderQuoteRequest` uses properties rather than accepting a domain entity. Its annotations
describe the public input contract:

- SKU is required, length-bounded, and restricted to a safe character set;
- quantity is between 1 and 1,000;
- unit price is a positive decimal under a stated upper bound.

The non-nullability of a C# property and JSON runtime validation are related but different concerns.
A nullable `string?` plus `[Required]` honestly represents the object before validation. The service
uses the value only after the controller pipeline has established validity.

For complex rules, prefer a dedicated validator or application rule over increasingly elaborate
attributes. Keep validation errors stable enough for clients, avoid exposing internal exception text,
and test the serialized response rather than only calling `Validate` in memory.

## Action Return Types

`ActionResult<OrderQuote>` communicates that success carries an `OrderQuote` while the action may
also produce an HTTP result. `IActionResult` offers maximum flexibility but loses the declared success
type; returning the model directly is concise but less expressive when multiple statuses are normal.

`ProducesResponseType` documents important response alternatives for API-description tooling. It is
contract metadata, not runtime validation. Tests still need to prove that actual status codes and
bodies match the declaration.

## Filter Pipeline

MVC filters surround selected portions of controller execution:

1. authorization filters run first;
2. resource filters surround model binding and most of the pipeline;
3. action filters surround action invocation;
4. exception filters handle eligible unhandled action exceptions;
5. result filters surround result execution.

Middleware surrounds the entire ASP.NET Core endpoint pipeline and is usually correct for universal
HTTP concerns such as correlation and exception handling. Filters have MVC context: action arguments,
model state, controller metadata, and results. Endpoint filters are the corresponding local mechanism
for Minimal APIs. Choose the narrowest mechanism whose context matches the concern.

Filters are not a substitute for authentication middleware or authorization policies. The tenant
header example deliberately teaches boundary validation and scoped context; a real tenant identity
must be derived from trusted authentication claims and checked by authorization.

## Dependency-Activated Filters

Attributes are metadata and their constructor arguments must be representable as attribute values.
They are not a natural place to manually construct services. `[ServiceFilter<T>]` asks DI to resolve
the filter, allowing `RequireTenantFilter` to receive `TenantContext` and a typed logger.

The filter validates exactly one `X-Tenant-Id` value against a length-bounded allowlist. If invalid,
it assigns a Problem Details result and returns without invoking `next`; this is short-circuiting. If
valid, it populates the scoped context, opens a structured logging scope, awaits the remainder of the
pipeline, and disposes the scope afterward.

Never put mutable request data in a singleton. `TenantContext` is scoped, so concurrent requests get
different instances. Its setter rejects a second assignment, turning unexpected pipeline composition
into an immediate diagnostic rather than silently changing identity midway through a request.

## Monetary Calculation Boundary

`OrderQuoteService` demonstrates a small application service rather than placing business arithmetic
in a controller. It uses `decimal`, applies a bulk-discount rule, and rounds explicitly at the business
boundary. In a real commerce domain, currency, tax jurisdiction, rounding per line versus total, and
discount precedence must all be explicit; `decimal` alone does not define monetary policy.

The controller translates HTTP into a validated request and translates the service result back into
HTTP. The service owns calculation. The filter owns reusable controller-boundary behavior. This keeps
each component independently testable without creating layers that merely forward every call.

## Controller and Minimal API Comparison

| Concern | Minimal API | Controller API |
|---|---|---|
| Route declaration | Mapping calls and route groups | Attributes on controller/actions |
| Handler shape | Delegates or methods | Action methods |
| Validation | Explicit, endpoint filters, or libraries | ApiController model-state convention |
| Cross-cutting local behavior | Endpoint filters | MVC filters |
| Result typing | `TypedResults` and result unions | `ActionResult<T>` and result types |
| Best fit | Focused APIs, compact vertical slices | Convention-heavy APIs and MVC extensibility |

Both models use the same host, middleware, DI container, configuration, logging, authorization, and
endpoint routing. Teams should optimize for understandable consistency within a bounded context,
not force every endpoint in a large system into one style.

## Test Matrix

The order-quote integration tests prove three distinct pipeline outcomes:

- a valid tenant and model reach the service and produce the expected calculation;
- a missing tenant is short-circuited by the action filter;
- an invalid annotated model produces automatic validation errors without action execution.

Add tests whenever filter ordering matters. Test concurrent requests when scoped state is involved,
and ensure logs do not include secrets or uncontrolled high-cardinality values merely because a value
was available in an action argument.

## Review Checklist

- Are `AddControllers` and `MapControllers` both intentionally present?
- Do input DTOs exclude server-owned and sensitive properties?
- Are local shape rules separated from stateful business rules?
- Is middleware versus filter placement justified by required context and scope?
- Are DI-dependent filters activated by the container?
- Does every short-circuit path return immediately without invoking the next delegate?
- Is request-specific mutable state scoped and assigned predictably?
- Do integration tests exercise binding, automatic validation, filter behavior, and serialization?

## Further Reading

- [Create web APIs with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [Filters in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters)
- [Model validation in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)

