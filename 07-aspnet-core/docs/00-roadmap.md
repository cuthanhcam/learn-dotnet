---
title: "ASP.NET Core Learning Roadmap"
description: "The ordered path from hosting fundamentals to production-oriented HTTP APIs and integration testing."
slug: aspnet-core-roadmap
phase: 7
order: 0
difficulty: intermediate
article-type: roadmap
estimated-reading-minutes: 14
topics: [aspnet-core, web-api, roadmap]
prerequisites: [dotnet-async-concurrency-pitfalls]
status: maintained
last-reviewed: 2026-08-15
---

# ASP.NET Core Learning Roadmap

## Goal

Build HTTP services whose behavior remains understandable under success, invalid input, dependency
failure, cancellation, concurrency, deployment configuration, and shutdown.

## Progressive Layers

1. Hosting and application lifetime.
2. Middleware and request/response flow.
3. Dependency injection and configuration.
4. Routing, binding, validation, and results.
5. Controllers, filters, and API conventions.
6. OpenAPI and versioned contracts.
7. Logging, diagnostics, health, and metrics.
8. CORS, caching, compression, and rate limiting.
9. Background services and graceful shutdown.
10. Integration and architecture-level testing.

Each topic must include an article, executable implementation, normal and failure tests, operational
trade-offs, and links to official Microsoft Learn documentation.

## Study Loop

For each slice:

1. Predict where the behavior belongs: middleware, endpoint, service, or infrastructure.
2. Read the article and state the lifecycle or HTTP contract.
3. Run the API and inspect status, headers, and body.
4. Read integration tests as executable specifications.
5. Introduce one invalid or failure case.
6. Confirm cancellation and cleanup ownership.
7. Review logging for sensitive or high-cardinality data.

## Navigation

- Previous: [Phase 06 — Async and Concurrency](../../06-async-concurrency/README.md)
- Next: [Hosting and request pipeline](01-hosting-request-pipeline.md)
