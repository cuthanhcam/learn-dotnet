---
title: "Output Caching and Mutation Invalidation"
description: "Cache reusable ASP.NET Core responses safely with bounded policies, tag-based invalidation, request variation, and correlation-aware middleware ordering."
slug: aspnet-core-output-caching-invalidation
phase: 7
order: 9
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 42
topics: [aspnet-core, output-caching, cache-invalidation, middleware, http]
prerequisites: [aspnet-core-cors-compression-rate-limiting]
status: maintained
last-reviewed: 2026-08-16
---

# Output Caching and Mutation Invalidation

Caching trades freshness and memory for lower latency and reduced downstream work. Adding a cache is
easy; defining when two requests may share a response and when a mutation makes that response invalid
is the engineering work. The product collection uses ASP.NET Core output caching only after those
rules are executable and tested.

## Output Cache Versus HTTP Response Caching

ASP.NET Core output caching stores a generated response on the server and can enforce server-owned
policies. HTTP response caching follows standard cache headers and may involve browsers or shared
proxies. A server output cache can protect an expensive handler even when clients do not cache, while
`Cache-Control` communicates freshness rules across HTTP participants.

Neither is the same as caching domain objects or database query results. Choose the layer whose work
you need to avoid and account for serialization, authorization, tenant isolation, and invalidation at
that layer.

## Cache Only Safe Representations

The sample caches the paginated `GET /api/products` response for 30 seconds. By default, output cache
behavior is conservative around methods, status codes, authentication, and cookies. The named policy
makes duration and tag discoverable rather than scattering magic values across route mappings.

Before caching an endpoint, identify every input affecting its representation:

- route and query values;
- selected request headers and content negotiation;
- authenticated identity, roles, permissions, and tenant;
- culture, currency, feature flags, and API version;
- the underlying mutable state.

Missing a variation dimension can serve one caller's data to another. High-cardinality variation can
make the cache ineffective or exhaust memory. Personalized or authorization-sensitive responses are
often better left uncached unless isolation is rigorously designed.

## Tag-Based Mutation Invalidation

Every cached product collection entry receives the `products` tag. Pagination creates many distinct
keys, but one successful create, update, or delete can affect totals, ordering, or membership on any
page. Evicting the tag invalidates all those related variants without enumerating keys.

Invalidation runs only after a successful state change:

1. validate the request and conditional headers;
2. perform the atomic repository mutation;
3. evict the product tag;
4. return success.

Validation failures, conflicts, stale ETags, and missing resources do not change state and therefore
do not need eviction. Evicting before a mutation creates needless misses when the operation fails.

There is still a small process-local ordering boundary between persistence commit and cache eviction.
For distributed systems, consider transaction/outbox-driven invalidation, short TTLs, versioned cache
keys, or accepting explicitly documented eventual consistency. A distributed cache does not make
invalidation atomic by itself.

## Correlation Headers Must Not Be Cached

A cached response includes headers as well as its body. If correlation middleware writes
`X-Correlation-ID` before the cache captures the response, later callers may receive the first
request's identifier. Logs and support traces then point at the wrong request.

The middleware resolves the ID and registers `Response.OnStarting` to add it immediately before
headers are sent. The reusable representation is cached without request-specific correlation state,
while both misses and hits receive a fresh response header. An integration test executes the same
cached request twice, proves the repository ran once, and proves the IDs differ.

Audit other per-request headers before enabling output caching: trace identifiers, nonces, user data,
rate-limit state, request-specific cookies, and dynamic security headers may also be unsafe to replay.

## Expiration Is Not Invalidation

Expiration bounds how long an entry can live. Invalidation removes an entry because known state
changed. A 30-second TTL alone would permit stale collection responses for up to 30 seconds after a
write. Tag eviction improves read-after-write behavior inside this application instance.

Use a finite TTL even with invalidation. It limits the damage from missed events, bugs, topology
changes, and abandoned tags. Adding random jitter can prevent many entries from expiring at the same
instant and causing a cache stampede.

## Stampedes and Locking

When a popular entry expires, many concurrent misses can invoke the expensive producer. ASP.NET Core
output caching provides resource locking behavior to reduce duplicate population work. Regardless of
framework defaults, downstream systems still need capacity limits and timeouts because cold starts,
many different keys, or disabled locking can create bursts.

Do not hold application locks across slow network calls merely to build a cache. Prefer cache features
designed for request coalescing and measure tail latency during cold-cache scenarios.

## Compression and Cache Ordering

Compression varies by `Accept-Encoding`; caches must not confuse Brotli, gzip, and uncompressed
representations. `Vary: Accept-Encoding` communicates this to HTTP caches. Middleware ordering also
determines whether a server output cache stores compressed or uncompressed bytes and where CPU is
spent on a hit. Choose deliberately, then verify actual headers and payloads rather than relying on a
diagram alone.

## Testing Cache Behavior

The tests replace the repository with a counting wrapper:

- two identical collection requests cause one repository page call;
- a successful POST evicts the tag;
- the next collection request causes a second page call;
- cached callers receive distinct correlation identifiers.

Avoid tests based only on elapsed time; timing is noisy and cannot prove which layer performed work.
A deterministic spy or counter demonstrates cache hits and invalidation directly. Each test creates a
fresh host so cache state cannot leak across parallel tests.

## Review Checklist

- Is the cached method safe and the response free of caller-private data?
- Does the key vary by every representation input and no unbounded unnecessary inputs?
- Are finite expiration and explicit mutation invalidation both defined?
- Do all successful mutations evict every affected collection and item variant?
- Can persistence commit while invalidation fails, and what consistency is promised then?
- Are correlation IDs, nonces, cookies, and trace-specific headers excluded from reuse?
- Are compression and content-negotiation variants correct?
- Do tests count producer calls and prove invalidation rather than infer it from timing?

## Further Reading

- [Output caching middleware in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output)
- [Response caching in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/response)
- [HTTP caching semantics](https://www.rfc-editor.org/rfc/rfc9111)

