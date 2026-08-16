---
title: "CORS, Response Compression, and Rate Limiting"
description: "Apply explicit browser-origin policy, negotiated Brotli or gzip compression, and partitioned request limits without confusing these controls with authentication."
slug: aspnet-core-cors-compression-rate-limiting
phase: 7
order: 8
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 45
topics: [aspnet-core, cors, compression, rate-limiting, security, resilience]
prerequisites: [aspnet-core-errors-observability-health-checks]
status: maintained
last-reviewed: 2026-08-15
---

# CORS, Response Compression, and Rate Limiting

Traffic policies change which browser calls are readable, how response bytes travel, and how much
work a client may request. They are easy to enable globally and difficult to reason about globally.
The sample isolates them in a traffic-policy route group so their behavior, ordering, and tests remain
visible before applying them to production endpoints.

## CORS Is a Browser Read Policy

Cross-Origin Resource Sharing tells a conforming browser whether JavaScript from one origin may read
a response from another origin. An origin consists of scheme, host, and port. These are different:

```text
https://learn-dotnet.example
http://learn-dotnet.example
https://learn-dotnet.example:8443
```

CORS is not authentication, authorization, a firewall, or protection from non-browser clients. A
script, mobile app, server, or command-line client can call the API without enforcing browser CORS.
The server must still authenticate identities and authorize every protected operation.

The sample loads an exact allowlist from validated configuration and attaches the named policy only
to the demonstration group. Avoid `AllowAnyOrigin` for credentialed or private APIs. ASP.NET Core
correctly prevents the invalid combination of wildcard origins and credentials because reflecting
credentials to every site would destroy the browser boundary.

## Simple Requests and Preflight

A browser can send some cross-origin requests directly and validate response headers afterward.
Other requests first send an `OPTIONS` preflight containing the intended method and headers:

```http
OPTIONS /api/traffic-policy-demo/compressed
Origin: https://learn-dotnet.example
Access-Control-Request-Method: GET
```

An allowed preflight receives `Access-Control-Allow-Origin` and related policy headers. A denied
origin does not receive an allow header. The HTTP status alone is not the complete CORS decision; the
browser examines response headers. Integration tests therefore assert both allowed and absent headers.

Preflight caching can reduce traffic, but long cache durations delay policy changes. Ensure reverse
proxies vary cached responses by relevant origin/request headers and never replace an exact allowlist
with substring or suffix checks vulnerable to look-alike domains.

## Response Compression Is Negotiated

Clients advertise supported content codings:

```http
Accept-Encoding: br, gzip
```

The server chooses an enabled provider, compresses a suitable content type, returns
`Content-Encoding`, and adds `Vary: Accept-Encoding`. `Vary` prevents a shared cache from serving
compressed bytes to a client that did not negotiate them.

ASP.NET Core supports Brotli and gzip providers. Brotli often compresses text more effectively; gzip
has broad compatibility. The sample uses `CompressionLevel.Fastest` because online response latency
and CPU usually matter more than achieving the smallest possible archive. Benchmark representative
payloads and deployment hardware instead of assuming maximum compression is cheaper overall.

Do not compress everything:

- JPEG, PNG, video, ZIP, and similar formats are already compressed;
- tiny bodies may grow after headers and framing;
- compression consumes CPU and can amplify overload;
- secrets compressed beside attacker-controlled input can enable side-channel attacks;
- a reverse proxy may already own compression.

`EnableForHttps` is explicit in the sample. For responses mixing secrets and reflected input, disable
compression or redesign the response so sensitive and attacker-controlled values are not compressed
together.

## Rate Limiting Protects Capacity

Rate limiting rejects or queues work before expensive application behavior. It improves fairness and
reduces accidental overload, but it is not a complete denial-of-service defense. Network-edge controls,
autoscaling, bounded concurrency, downstream timeouts, and resource quotas remain necessary.

The demonstration policy uses a fixed window:

- two permits;
- one-minute window;
- no queue;
- partition by remote client address.

The third request receives `429 Too Many Requests`, a Problem Details body, and `Retry-After` when the
limiter supplies that metadata. Queue length is zero because waiting HTTP requests consume memory,
connections, and request-abort bookkeeping. Queuing can smooth small bursts, but it must be tightly
bounded and its processing order chosen intentionally.

## Choose the Right Algorithm

| Limiter | Behavior | Typical trade-off |
|---|---|---|
| Fixed window | Counter resets at a fixed boundary | Simple, but bursts can straddle boundaries |
| Sliding window | Segments approximate a moving interval | Smoother distribution with more state |
| Token bucket | Tokens replenish; saved tokens permit bursts | Good sustained rate plus controlled burst |
| Concurrency | Limits simultaneous in-flight requests | Protects scarce parallel capacity, not request frequency |

Many services combine a token/frequency policy with a concurrency limiter. Select limits from measured
capacity and service objectives. A copied number such as “100 requests/minute” has no inherent safety.

## Partition Keys and Trust

A global limiter lets one noisy caller consume everybody's permits. Partitioning improves fairness,
but the key must reflect a trustworthy boundary:

- authenticated account or API-client ID after authentication;
- tenant ID derived from verified claims;
- remote address when proxy forwarding is configured safely;
- route or workload class for endpoints with different cost.

The learning endpoint uses remote address because authentication is a later phase. Behind a reverse
proxy, configure forwarded headers and trusted proxy networks before treating that address as the
client. Never partition solely on an arbitrary caller-supplied header: attackers can rotate values and
create unbounded limiter partitions.

Partition keys also create memory cardinality. Normalize and bound them, expire inactive state where
the implementation supports it, and monitor partition growth.

## Middleware and Endpoint Policy

The middleware is registered once, while named policies attach only to intended endpoints. This
separates infrastructure capability from public policy:

```text
UseResponseCompression
UseCors
UseRateLimiter
  -> endpoint metadata selects named CORS/rate policies
```

Health endpoints are intentionally not rate-limited by the strict demonstration policy. OpenAPI is
development-only. A production API usually assigns different policies to anonymous reads,
authenticated writes, expensive exports, callbacks, and operational endpoints.

Ordering matters when authentication supplies the partition key or authorization determines CORS
behavior. Follow the framework's routing/authentication/authorization guidance and exercise preflight,
rejection, and authenticated cases through the hosted pipeline.

## Caching Is Deliberately Not Added Yet

Compression changes representation bytes without changing resource freshness. Output caching reuses
a previous response and therefore needs a correctness policy for variation and invalidation. Caching
the mutable product collection before defining invalidation for POST, PUT, and DELETE would teach a
stale-data bug. The next caching slice introduces tags, mutation eviction, cache-control semantics,
and tests that distinguish server output caching from client/proxy response caching.

## Tests as Policy Specifications

The suite verifies:

- Brotli is selected when the client advertises it;
- `Vary: Accept-Encoding` is emitted;
- an allowed preflight receives the configured origin;
- a denied preflight receives no allow-origin header;
- two fixed-window requests succeed and the third returns `429`;
- rejection uses Problem Details media type and advertises retry timing.

Each limiter test creates a fresh application factory so permit state cannot leak between tests.
Stateful middleware tests must isolate or explicitly reset state to remain deterministic under
parallel execution.

## Review Checklist

- Is every allowed origin exact, configuration-driven, and environment-appropriate?
- Are authentication and authorization enforced independently of CORS?
- Do preflight tests inspect headers, not only status codes?
- Does compression emit `Vary` and exclude unsuitable or sensitive responses?
- Has compression CPU/latency been benchmarked with real payloads?
- Is rate limiting partitioned by a bounded, trustworthy key?
- Are queues disabled or tightly bounded?
- Does `429` provide a stable error contract and retry guidance?
- Are operational endpoints governed by deliberate separate policies?
- Are proxy forwarding and client-address trust configured together?

## Further Reading

- [Enable CORS in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/cors)
- [Response compression in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/response-compression)
- [Rate limiting middleware in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)

