---
title: "Hosted Services, Bounded Queues, and Graceful Shutdown"
description: "Move durable work beyond HTTP requests using bounded channels, scoped processors, observable job state, overload rejection, and deadline-aware shutdown draining."
slug: aspnet-core-hosted-services-graceful-shutdown
phase: 7
order: 10
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 48
topics: [aspnet-core, hosted-services, channels, background-jobs, graceful-shutdown, backpressure]
prerequisites: [aspnet-core-output-caching-invalidation]
status: maintained
last-reviewed: 2026-08-16
---

# Hosted Services, Bounded Queues, and Graceful Shutdown

An HTTP request should not start an untracked task and return while that task still depends on scoped
request services. The scope is disposed when the request ends, exceptions become difficult to observe,
deployments can terminate the process, and enough fire-and-forget work can exhaust memory. The sample
uses an explicit bounded queue and hosted worker so acceptance, ownership, overload, status, and
shutdown behavior are visible.

## Decide Whether Work Belongs In Process

An in-process queue is appropriate for learning and for non-critical, recoverable work where losing
accepted items during a crash is an explicit trade-off. It is not durable. A process crash, node
replacement, or abrupt shutdown loses memory-only jobs and their status.

Use a durable broker or database-backed queue when an accepted job must survive process failure,
multiple instances must share work, delivery needs retries/dead-lettering, or auditing matters. The
HTTP `202 Accepted` response says processing has not completed; production semantics should also state
the durability guarantee behind acceptance.

## Bounded `Channel<T>` and Backpressure

`BackgroundJobQueue` wraps a bounded `Channel<BackgroundJob>`:

- capacity is two waiting jobs;
- multiple request writers are allowed;
- one worker reader preserves simple processing order;
- asynchronous continuations avoid running consumer work inline on a request thread;
- `TryWrite` rejects immediately when full.

Bounded capacity turns overload into a controlled outcome. An unbounded queue can make every request
appear successful while memory and wait time grow until the process fails. The endpoint returns safe
`503 Service Unavailable` Problem Details when it cannot accept ownership.

`BoundedChannelFullMode.Wait` describes asynchronous writer behavior, but this HTTP boundary uses
`TryWrite` intentionally. Waiting inside requests merely transforms queue pressure into many pending
connections. Other workloads may use `WriteAsync` with a short deadline when waiting is part of the
contract.

## Acceptance and Observable State

The endpoint validates the request, creates a queued state, attempts enqueue, and returns:

```http
HTTP/1.1 202 Accepted
Location: /api/background-jobs/{id}
```

Clients poll the named status route and observe `Queued`, `Running`, `Completed`, or `Failed`. If
enqueue fails, the provisional status entry is removed so the service never advertises ownership it
did not accept.

The in-memory state store is concurrent because requests read status while the worker updates it.
Terminal failures expose a stable error code, not raw exception text. A production store needs
retention/cleanup, authorization, tenant isolation, and limits preventing status records from growing
forever.

## Hosted Service Lifetime and DI Scopes

Hosted services registered with `AddHostedService` are singletons. Injecting a scoped DbContext or
application service directly creates a captive dependency. The worker instead injects
`IServiceScopeFactory`, creates an asynchronous scope per job, resolves `IBackgroundJobProcessor`, and
disposes the scope after that unit of work.

This gives every job independent scoped services and cleanup. A scope is not a transaction; the
processor must still define persistence atomicity and external side-effect boundaries.

## Failure Isolation and Idempotency

The worker catches an individual processing exception, records a stable failure state, logs the full
exception internally, and continues to the next item. Without that boundary, one poison message could
terminate the only consumer and strand the queue.

Retries can duplicate effects. Every real processor should be idempotent or use an idempotency key and
authoritative state transition. Examples include unique operation IDs, compare-and-set status,
transactional outbox messages, and provider-specific deduplication. Retrying an email, payment, or
webhook blindly can be worse than failing once.

Distinguish transient failures from permanent invalid work. Apply bounded retry counts, exponential
backoff with jitter, and dead-letter handling outside the immediate hot loop. Infinite retry blocks
later work and hides poison data.

## Graceful Shutdown as a State Transition

Shutdown does not mean immediately cancelling all work. The worker's sequence is:

1. complete the channel writer so no more jobs are accepted;
2. allow the reader to drain accepted jobs;
3. wait for the worker within the host shutdown deadline;
4. cancel processing only when that deadline expires.

This implementation uses a custom `IHostedService` rather than relying on a loop that observes the
normal stopping token immediately. A private forced-stop token is cancelled by the host deadline. The
design makes “drain first, cancel at deadline” explicit.

During real deployment, traffic must stop before or alongside application shutdown. Otherwise new
requests can race with queue completion and receive rejection. Readiness can transition to unhealthy
before termination so load balancers stop sending new work.

## Cancellation Ownership

Request cancellation should not cancel a job after the service has returned `202` and accepted
ownership. The queued job is no longer owned by that client connection. It receives the worker's
shutdown/deadline token instead.

Conversely, a synchronous request operation should normally propagate `RequestAborted`. Cancellation
token choice documents ownership: caller-owned work follows the caller; accepted background work
follows the service lifecycle.

## Scaling Beyond One Consumer

Increasing consumers can improve throughput only when downstream systems support the concurrency.
It also changes ordering and exposes races. Bound concurrency explicitly, partition jobs when ordering
matters, and measure queue wait time separately from execution time.

With multiple application replicas, each in-memory queue is independent and a client cannot reliably
poll any node for status without sticky routing or shared storage. This is another signal that durable
distributed background work belongs in shared infrastructure.

## Testing Lifecycle Behavior

The hosted integration tests cover more than a happy-path processor call:

- an accepted job eventually reaches `Completed` with lifecycle timestamps;
- a blocked processor allows the bounded queue to fill and the next submission is rejected;
- application disposal waits for accepted running work;
- releasing the processor allows graceful shutdown to finish within the test deadline.

The fake processor uses `TaskCompletionSource` with asynchronous continuations. Tests wait on explicit
signals rather than arbitrary sleeps for job start. A short delay is used only to assert that disposal
has not already completed, then every wait has a timeout so a defect fails rather than hangs CI.

## Review Checklist

- Is accepted background work durable enough for its business promise?
- Is queue capacity finite and derived from measured resource limits?
- Does overload reject predictably instead of accumulating requests or memory?
- Can clients observe status without seeing internal exception details?
- Does the singleton worker create and dispose a DI scope per job?
- Can one poison job terminate the consumer?
- Are processors idempotent under retry or duplicate delivery?
- Does shutdown stop acceptance, drain, and finally cancel at a deadline?
- Is request cancellation separated from service-owned accepted work?
- Do lifecycle tests use deterministic gates and bounded waits?

## Further Reading

- [Background tasks with hosted services in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)
- [Channels library](https://learn.microsoft.com/en-us/dotnet/core/extensions/channels)
- [Generic Host shutdown](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)

