---
title: "ASP.NET Core Hosting and the Request Pipeline"
description: "WebApplication startup, service registration, middleware ordering, short-circuiting, correlation, exception handling, and application lifetime."
slug: aspnet-core-hosting-request-pipeline
phase: 7
order: 1
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 32
topics: [aspnet-core, hosting, middleware, logging]
prerequisites: [aspnet-core-roadmap]
status: maintained
last-reviewed: 2026-08-15
---

# ASP.NET Core Hosting and the Request Pipeline

## Learning Objectives

- Separate service registration, app construction, pipeline composition, and server execution.
- Explain forward request flow and reverse response flow through middleware.
- Place exception handling and security-sensitive middleware intentionally.
- Implement correlation without trusting arbitrary request-header content.
- Avoid starting request-owned fire-and-forget work.

## Host Construction

`WebApplication.CreateBuilder(args)` assembles defaults for configuration, logging, dependency
injection, hosting, and command-line processing. Registration configures recipes in `IServiceCollection`;
services are normally created later when resolved.

`builder.Build()` creates the application and its root service provider. Middleware and endpoints are
then mapped before `Run()` starts the server and participates in host shutdown.

## Middleware Flow

Middleware executes in registration order on the request path and unwinds in reverse order for the
response path:

```text
request  -> exception -> correlation -> endpoint
response <- exception <- correlation <- endpoint
```

A component can call `next`, perform work before and after it, or short-circuit by producing a response
without calling downstream middleware. Ordering changes security, functionality, observability, and
performance. Exception handling must precede components whose failures it should translate.

## Correlation Middleware

The sample accepts one bounded correlation header containing only letters, numbers, `-`, or `_`.
Invalid/multiple values are replaced with a generated identifier. Reflecting unbounded raw headers
can create log injection, oversized telemetry, or malformed response headers.

A logging scope attaches correlation to structured logs for the remaining pipeline. Correlation is
diagnostic context, not authentication and not a globally unique business transaction identifier.

## Request Lifetime

`HttpContext.RequestAborted` is bound automatically to a route-handler `CancellationToken`. Pass it to
downstream asynchronous dependencies. Client disconnect does not roll back a committed database or
remote side effect.

Do not start detached work that captures `HttpContext`, scoped services, or request buffers. The
request scope is disposed after the response. Durable/background work requires an owned queue and
hosted-service lifecycle.

## Implementation Map

- `Program.cs`: registration, build, middleware order, endpoints, run.
- `CorrelationIdMiddleware.cs`: validated correlation and logging scope.
- `ProductApiTests.cs`: generated and caller-provided header behavior.

## References

- [ASP.NET Core fundamentals](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/)
- [ASP.NET Core middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)
- [Write custom middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write)

## Navigation

- Previous: [Roadmap](00-roadmap.md)
- Next: [Dependency injection and configuration](02-dependency-injection-configuration.md)
