---
title: "Exception Handling, Observability, and Health Checks"
description: "Build safe Problem Details mappings, correlated structured logs, and distinct liveness and readiness probes for ASP.NET Core services."
slug: aspnet-core-errors-observability-health-checks
phase: 7
order: 7
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 44
topics: [aspnet-core, exception-handling, problem-details, logging, health-checks, observability]
prerequisites: [aspnet-core-openapi-contracts-dotnet-10]
status: maintained
last-reviewed: 2026-08-15
---

# Exception Handling, Observability, and Health Checks

Production behavior includes dependency failure, invalid state, client cancellation, and partial
outages. A service is operable when it converts failures into stable client contracts, leaves useful
diagnostic evidence, and tells infrastructure whether it should receive traffic. These are related
concerns, but each audience needs a deliberately different amount of information.

## Three Audiences for Failure Information

| Audience | Needs | Must not receive |
|---|---|---|
| API client | Stable status, problem type/title, trace identifier, remediation hint | Stack traces, SQL, credentials, internal topology |
| Operator | Structured event, exception, request correlation, dependency and latency context | Unbounded cardinality and unnecessary personal data |
| Orchestrator/load balancer | Cheap live/ready signal and HTTP status | Detailed diagnostics or expensive recovery work |

One serialized exception cannot safely serve all three audiences. The API therefore logs the full
exception internally, returns an allowlisted Problem Details object, and exposes minimal health probe
responses.

## Central Exception Mapping

`IExceptionHandler` provides a focused boundary for exceptions that escape endpoint behavior. The
sample maps `CatalogUnavailableException` to `503 Service Unavailable` and unknown exceptions to
`500 Internal Server Error`. The public response never copies `Exception.Message` because messages
often contain connection strings, paths, queries, identifiers, or other implementation details.

Known exceptions should express meaningful application or infrastructure categories. Do not create
a different exception type merely to choose every HTTP status. Expected validation, not-found,
conflict, and concurrency outcomes are usually clearer as normal result values. Exceptions remain
appropriate for an operation that cannot continue normally.

The handler returns `true` only after it writes the response. A handler that declines an exception
allows the next registered handler to try. Registration order therefore becomes policy and should be
covered by tests when multiple handlers exist.

## Problem Details Customization

`AddProblemDetails` centralizes a safe extension:

```json
{
  "status": 503,
  "title": "The product catalog is temporarily unavailable.",
  "traceId": "..."
}
```

The trace identifier lets support correlate a client report with server telemetry without exposing
the exception. A public `type` URI is valuable for a mature API because it gives each stable problem
category a durable identifier and documentation page. Titles and detail text can evolve for people;
clients should branch on status and stable type/code rather than parse prose.

Production responses must not enable developer exception pages. Detailed pages are local development
tools and should be guarded by environment. Even in Development, avoid screenshots or copied traces
that contain real secrets.

## Middleware Ordering and Logging Scopes

Correlation middleware runs before exception handling in this sample. It validates or generates the
correlation ID, adds the response header, and opens a logging scope. The exception handler executes
inside that scope, so its structured error log carries the same correlation property.

```text
Correlation middleware
  -> Exception-handler middleware
     -> Routing/endpoint
```

If the exception handler surrounded the correlation middleware, the correlation scope would unwind
before the handler logged the escaped exception. The response header might still exist, but the most
important error event could lose its application correlation property.

Use structured templates such as `"Request failed with {StatusCode}"`, not interpolated strings.
Providers can index named properties while retaining the original template. Log an exception through
the exception parameter so its type and stack remain structured.

## Correlation Is Not Distributed Tracing

A caller-supplied correlation header can help join related logs, but it is not a complete tracing
system. .NET uses `Activity` and W3C Trace Context for distributed trace and span propagation.
OpenTelemetry can export traces, metrics, and logs to a backend. Retain a human-facing request ID when
useful, but do not invent a parallel distributed tracing protocol.

Validate reflected header values, bound their length, and avoid treating correlation IDs as secrets
or authenticated identity. Attacker-controlled high-cardinality values can increase logging and
metrics costs.

## Liveness Versus Readiness

The sample exposes two probes:

- `/health/live` checks whether the process and HTTP pipeline respond. It executes no dependency checks.
- `/health/ready` checks whether the product catalog can perform useful bounded work.

An orchestrator can restart a process that is not live and temporarily remove an unready process from
traffic. If liveness checks a database, a shared database outage can cause every instance to restart,
adding load and destroying useful diagnostic state. Dependency reachability normally belongs in
readiness, not liveness.

ASP.NET Core health registrations use tags to select checks for each endpoint. Liveness uses a
predicate that selects none: the health framework itself returns Healthy when the process can execute
the request. Readiness selects checks tagged `ready`.

## Designing Dependency Probes

A health probe must be cheap, bounded, cancellation-aware, and representative. The in-memory catalog
probe performs a one-item read. A database implementation should use a provider-supported lightweight
operation with a short timeout. Avoid table scans, remote fan-out, writes, migrations, or repair logic.

Probe failures return the registration's configured failure status. Cancellation requested by the
health framework is rethrown rather than mislabeled as dependency failure. Other exceptions are
captured in the internal `HealthReport`; the public writer deliberately omits exception details.

Health checks can amplify traffic because every orchestrator node may poll every application
instance. Choose intervals, timeout, and thresholds at deployment level. A single transient failure
should not necessarily evict an otherwise healthy instance.

## Health Response Contract

Serializing `HealthReport` directly is unsafe. It can contain exception messages and arbitrary data
added by checks. `HealthResponseWriter` produces an allowlisted contract containing overall status,
durations, check names, and individual statuses.

Even check names can reveal architecture, so public internet exposure is rarely appropriate. Protect
diagnostic endpoints at the network or management-plane level while ensuring orchestrators can still
reach them. Do not apply a normal user authentication flow that depends on the same failing service
being probed.

## Testing Failure Paths

The integration tests replace `IProductRepository` through `ConfigureTestServices` with a deterministic
failing implementation. This proves the complete hosted behavior without needing an unreliable real
dependency:

- liveness remains `200`;
- readiness becomes `503`;
- a normal catalog request becomes safe `503` Problem Details;
- the internal exception message containing a simulated secret never appears in either body;
- the client receives a trace identifier.

The test also exposed a .NET 10 binding detail: non-nullable Minimal API query parameters are required.
Optional pagination inputs are therefore `int?` and converted to explicit defaults inside the handler.
This is why migration work must execute behavioral tests rather than stopping at a successful compile.

## Metrics and Telemetry Boundaries

Useful service signals commonly include request rate, error rate, duration distributions, saturation,
dependency latency, queue depth, and rejected work. Never use unbounded product IDs, user IDs, URLs
with raw route values, or correlation IDs as metric labels. High-cardinality dimensions can exhaust
memory and make observability platforms prohibitively expensive.

Logs answer detailed event questions, metrics show aggregate trends and alerts, and traces explain a
single distributed request. Instrumentation should connect these signals using trace/span context
without duplicating every payload into all three.

## Review Checklist

- Is exception-to-status mapping centralized and deliberately small?
- Are expected application outcomes modeled without exceptions where practical?
- Can any exception message, stack trace, path, query, or credential reach the response?
- Does every error response contain a safe correlation handle?
- Does middleware ordering preserve the logging scope around exception handling?
- Is liveness independent of shared dependencies?
- Are readiness checks bounded, cheap, cancellation-aware, and representative?
- Does the health writer expose only allowlisted fields?
- Do tests inject deterministic failures and assert that secrets stay internal?
- Are log properties and metric dimensions bounded in cardinality?

## Further Reading

- [Handle errors in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [Health checks in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Logging in .NET and ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/)
- [Distributed tracing concepts](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts)

