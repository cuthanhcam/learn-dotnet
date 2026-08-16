---
title: "Advanced ASP.NET Core Integration Testing"
description: "Design isolated WebApplicationFactory hosts, deterministic time, dependency failures, cancellation tests, protocol boundary cases, lifecycle gates, and metric assertions."
slug: aspnet-core-advanced-integration-testing
phase: 7
order: 11
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 50
topics: [aspnet-core, integration-testing, webapplicationfactory, cancellation, test-isolation, metrics]
prerequisites: [aspnet-core-hosted-services-graceful-shutdown]
status: maintained
last-reviewed: 2026-08-16
---

# Advanced ASP.NET Core Integration Testing

Unit tests can prove a calculation while missing routing, model binding, middleware ordering,
serialization, DI lifetimes, environment gates, cancellation, and host shutdown. ASP.NET Core's
in-process test server runs the real application pipeline without binding a public network port. The
Phase 07 suite treats these tests as executable HTTP and lifecycle specifications.

## Test Through the Public Boundary

`WebApplicationFactory<Program>` discovers the application entry point, creates a test host, and
provides an `HttpClient`. Requests exercise endpoint routing, filters, middleware, formatters, Problem
Details, compression, output caching, and hosted services. Test assertions should focus on observable
contracts: status, headers, body, state transition, and dependency interaction.

Do not replace every application service with a mock. A host containing only mocks proves wiring to
mocks, not useful vertical behavior. Replace the smallest external or nondeterministic boundary needed
for the scenario while retaining real HTTP and application code.

## Reusable Isolated Factory

`LearningApiFactory` demonstrates three controlled overrides:

- environment is `Testing`, proving development-only OpenAPI is not exposed;
- configuration changes the catalog name without editing production files;
- repository and clock are fresh deterministic instances owned by that factory.

Each factory instance gets independent mutable infrastructure. Sharing one singleton repository
across unrelated test classes makes results depend on execution order and parallelism. A class fixture
is appropriate when shared state is intentional and tests use unique data; otherwise create an
isolated factory per test or collection.

## Deterministic Time

Production code injects `TimeProvider`, and tests replace it with `ManualTimeProvider`. A created
product receives an exact timestamp without freezing the process clock or asserting an imprecise time
window. Tests can advance the manual clock explicitly for expiration or lifecycle scenarios.

Avoid `Task.Delay` as a clock substitute. It makes tests slow and flaky and cannot reliably establish
ordering on loaded CI agents. Some lifecycle tests still need a very short delay to prove that a task
has not completed, but meaningful state transitions use `TaskCompletionSource` gates and bounded waits.

## Dependency Replacement and Failure Injection

`ConfigureTestServices` plus `RemoveAll<T>` replaces the production registration after normal startup
composition. Examples in this phase include:

- an unavailable repository for `503` and readiness behavior;
- a counting repository for cache hit/invalidation proof;
- a cancellation-aware repository for request-abort propagation;
- a blocking job processor for queue overload and shutdown draining.

Fakes should be deterministic and purpose-specific. A large programmable mock with dozens of setups
can become harder to understand than the production interface. Include simulated secrets in internal
exception messages when verifying that public error and health contracts do not leak them.

## Protocol Boundary Matrix

Happy-path JSON is only one input class. Advanced integration tests cover:

- malformed JSON (`400`);
- unsupported media type (`415`);
- route-constraint mismatch (`404`);
- known route with unsupported method (`405` and `Allow`);
- validation Problem Details;
- duplicate/conflict and stale ETag behavior;
- missing and malformed conditional headers;
- CORS preflight headers;
- compressed content negotiation;
- rate-limit rejection and retry guidance.

Status-code pages normalize otherwise empty framework-generated errors as Problem Details. Testing the
wire revealed this gap; calling endpoint methods directly would not.

## Cancellation Propagation

The cancellation test starts an HTTP collection request against a repository that waits indefinitely
using the received token. After the fake signals that it has started, the client cancels. The test
asserts both sides:

1. the client observes cancellation;
2. the repository observes cancellation through the propagated token.

Only checking the client is insufficient: the client can stop waiting while the server continues
expensive work. Every wait has a timeout so broken cancellation fails CI rather than hanging forever.

Request cancellation is expected control flow and should not be logged as an unexpected 500. Code
that catches `OperationCanceledException` must distinguish the token it owns from unrelated timeout
or dependency cancellation.

## Stateful Middleware Tests

Output cache and rate limiter state lives in the host. Tests create fresh factories to avoid permits
or entries leaking between cases. Cache tests count producer calls rather than infer hits from elapsed
time. Rate tests consume a known fixed capacity in one isolated partition.

Parallel execution can still affect process-global observers. `MeterListener` callbacks may receive
measurements from concurrently running hosts, so the metrics test uses a thread-safe collection and
filters by stable instrument name and bounded tag value.

## Hosted-Service Lifecycle Tests

Background tests use explicit gates:

- `Started` proves the worker owns a job;
- `Release` controls when processing completes;
- queued submissions fill exact bounded capacity;
- disposal begins while the processor is blocked;
- the test proves disposal remains incomplete, releases work, and observes timely shutdown.

Always release gates in robust cleanup paths in larger suites. A failed assertion before release can
otherwise make factory disposal wait for the test framework's global timeout.

## Environment and Security Assertions

The Testing environment intentionally receives `404` from the OpenAPI route because document mapping
is Development-only. Similar tests should cover developer exception pages, diagnostic routes, test
authentication handlers, and feature flags. Never assume an environment `if` branch is correct without
hosting at least one non-development configuration.

Test configuration must not weaken production defaults accidentally. A permissive test CORS policy or
authentication bypass can hide missing production registration. Keep test overrides visible and narrow.

## Metrics Assertions

`LearningMetrics` creates `System.Diagnostics.Metrics` counters with stable instrument names. The test
attaches a `MeterListener`, enables only the application's meter, submits a job through HTTP, and
observes the accepted outcome. This verifies instrumentation without requiring a particular exporter.

Metric tags must come from bounded sets. Job IDs, descriptions, product IDs, correlation IDs, raw URLs,
and user identities are unsuitable labels. They create unbounded time series and belong in correlated
logs or traces instead.

## Test Taxonomy

| Level | Best at proving | Common blind spot |
|---|---|---|
| Unit | Pure rule and edge-case combinations | Framework and composition behavior |
| Component/integration | Hosted vertical slice and controlled dependencies | Real network/infrastructure differences |
| Contract | Compatibility with a schema or consumer | Runtime infrastructure behavior |
| End-to-end | Deployed system and real integrations | Slow feedback and difficult failure isolation |

A mature service uses all levels selectively. Do not push every combinatorial rule through HTTP, and
do not declare an API production-ready because pure unit tests pass.

## Review Checklist

- Does each test state the observable contract it protects?
- Is mutable infrastructure isolated or intentionally shared?
- Are time, failures, cancellation, and lifecycle controlled deterministically?
- Are malformed syntax, unsupported media, routing, and method failures covered?
- Do tests prove secrets stay out of public error and health bodies?
- Are cache and limiter tests based on state/counters rather than timing?
- Does cancellation reach the deepest controlled dependency?
- Are every asynchronous wait and poll bounded by a timeout?
- Are environment-only surfaces tested outside Development?
- Are metric listeners thread-safe and tags bounded?

## Further Reading

- [Integration tests in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [Test ASP.NET Core middleware](https://learn.microsoft.com/en-us/aspnet/core/test/middleware)
- [Metrics instrumentation in .NET](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-instrumentation)

