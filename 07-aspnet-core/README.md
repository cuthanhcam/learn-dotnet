---
title: "Phase 07 — ASP.NET Core"
description: "A production-oriented path through hosting, middleware, dependency injection, configuration, routing, validation, HTTP semantics, logging, and API testing."
phase: 7
status: in-progress
target-framework: net10.0
prerequisites: [phase-06-async-concurrency]
previous-phase: ../06-async-concurrency/README.md
next-phase: ../08-ef-core/README.md
---

# ASP.NET Core

> Turn the language, runtime, memory, algorithm, and concurrency foundations from Phases 01–06 into observable HTTP application behavior.

## Learning Outcomes

After this phase, you should be able to explain the generic host, service container, configuration
providers, options validation, structured logging, request pipeline, routing, parameter binding,
validation, HTTP result semantics, error contracts, API documentation, cancellation, health checks,
and integration testing.

## Study Path

| Order | Article | Executable focus |
|---:|---|---|
| 0 | [Roadmap](docs/00-roadmap.md) | Phase boundaries and study workflow |
| 1 | [Hosting and request pipeline](docs/01-hosting-request-pipeline.md) | `WebApplication`, middleware, correlation |
| 2 | [DI, configuration, and options](docs/02-dependency-injection-configuration.md) | lifetimes and startup validation |
| 3 | [Routing, validation, and Problem Details](docs/03-routing-validation-problem-details.md) | product endpoints and HTTP contracts |
| 4 | [HTTP semantics, pagination, and concurrency](docs/04-http-semantics-pagination-concurrency.md) | bounded reads, ETags, and conditional writes |
| 5 | [Controllers, validation, and filters](docs/05-controllers-model-validation-filters.md) | ApiController conventions and scoped filters |
| 6 | [OpenAPI contracts with ASP.NET Core 10](docs/06-openapi-contracts-dotnet-10.md) | OpenAPI 3.1 JSON/YAML and contract verification |
| 7 | [Errors, observability, and health checks](docs/07-errors-observability-health-checks.md) | safe failures, liveness, readiness, and correlation |

Additional slices will cover controllers, filters, OpenAPI, logging, caching, rate limiting, CORS,
health checks, resilience boundaries, background services, security integration points, and advanced
testing. Authentication and authorization receive their dedicated deep phase later in the roadmap.

## Structure

```text
07-aspnet-core/
├── docs/
├── src/Learning.Api/
│   ├── Configuration/
│   ├── Features/Products/
│   └── Middleware/
├── tests/Learning.Api.Tests/
└── 07-aspnet-core.slnx
```

The sample uses feature folders so related HTTP contract, endpoint mapping, and application boundary
remain discoverable. Persistence stays behind `IProductRepository`; Phase 08 can replace the in-memory
implementation without rewriting the endpoint contract.

## Run and Test

```powershell
dotnet restore 07-aspnet-core.slnx
dotnet build 07-aspnet-core.slnx --no-restore
dotnet test 07-aspnet-core.slnx --no-build
dotnet run --project src/Learning.Api
```

Example requests:

```http
GET /health

POST /api/products
Content-Type: application/json

{
  "name": "Mechanical Keyboard",
  "price": 129.99
}
```

## Initial Code Map

| Concern | Implementation |
|---|---|
| Host and pipeline composition | `Program.cs` |
| Typed, startup-validated configuration | `Configuration/LearningOptions.cs` |
| Structured request correlation | `Middleware/CorrelationIdMiddleware.cs` |
| Minimal API route group and typed results | `Features/Products/ProductEndpoints.cs` |
| Application orchestration | `Features/Products/ProductCatalog.cs` |
| Request, resource, and pagination contracts | `Features/Products/ProductContracts.cs` |
| Persistence boundary | `IProductRepository.cs` |
| Concurrent learning implementation | `InMemoryProductRepository.cs` |
| In-process HTTP specifications | `tests/Learning.Api.Tests/ProductApiTests.cs` |
| Conditional-request specifications | `tests/Learning.Api.Tests/ProductLifecycleTests.cs` |
| Controller and filter example | `Features/OrderQuotes/` |
| Generated contract specifications | `tests/Learning.Api.Tests/OpenApiTests.cs` |
| Operational boundaries and probes | `Operations/` and `tests/Learning.Api.Tests/OperationalEndpointsTests.cs` |

## Design Rules

- Keep composition in `Program.cs`; keep domain/application behavior in focused types.
- Treat middleware ordering as application behavior, not formatting preference.
- Validate configuration during startup when invalid values make the service unusable.
- Respect DI lifetimes; never inject a scoped dependency into a singleton directly.
- Accept request cancellation and pass it through the complete operation graph.
- Return intentional status codes and standard Problem Details error bodies.
- Validate untrusted input before reflecting it into headers, logs, paths, or queries.
- Test through HTTP for routing, binding, serialization, middleware, and response contracts.

## Completion Criteria

- [ ] Explain request and response flow through ordered middleware.
- [ ] Select transient, scoped, and singleton lifetimes with captive-dependency awareness.
- [ ] Describe configuration-provider precedence and validate required options at startup.
- [ ] Implement route groups, constraints, named routes, and typed results.
- [ ] Return stable validation and exception contracts through Problem Details.
- [ ] Add OpenAPI descriptions without exposing internal implementation details.
- [ ] Apply logging scopes and safe correlation identifiers.
- [ ] Configure CORS, rate limiting, caching, and health checks by explicit policy.
- [ ] Test success, validation, missing resources, failure, cancellation, and middleware behavior.
- [ ] Pass the complete Phase 07 test suite.

## Previous Phase

Revisit [Phase 06 — Async and Concurrency](../06-async-concurrency/README.md) when reasoning about
request cancellation, shared singleton state, bounded downstream calls, and background work.
