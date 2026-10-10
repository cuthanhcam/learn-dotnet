---
title: "HTTP Security, Error Contracts, and API Version Evolution"
description: "Compose the enrollment module behind authenticated HTTP endpoints with claim-bound learner identity, stable errors, replay semantics, and an explicit v1 evolution policy."
slug: architecture-http-security-errors-versioning
phase: 10
order: 6
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 30
topics: [aspnet-core, authorization, jwt, problem-details, api-versioning]
prerequisites: [architecture-modules-events-outbox-inbox]
status: maintained
last-reviewed: 2026-10-10
---

# HTTP Security, Error Contracts, and API Version Evolution

## The Host Adapts; It Does Not Own Enrollment Rules

`Learning.Architecture.Api` is a composition root and HTTP adapter. It selects EF implementations,
configures authentication, registers scoped handlers, maps input and output, and starts the single
outbox worker. Capacity and duplicate-learner decisions remain in the aggregate. Durable replay and
transaction coordination remain behind `IEnrollmentRequests`.

The host references infrastructure to construct the application, not to let every endpoint mutate
database entities directly. Endpoints receive application handlers/ports through DI. The worker receives
infrastructure dispatch coordination because delivery is itself an infrastructure operation.

## Public Routes and Authority

| Route | Access | Behavior |
|---|---|---|
| `GET /api/v1/offerings?offset=0&limit=20` | anonymous | bounded public catalog DTO |
| `POST /api/v1/offerings/{id}/enrollments` | authenticated learner with `enrollment.write` | enroll the caller using one GUID request key |

There is no endpoint to submit an arbitrary learner ID, dump all memberships, mint access tokens, or
trigger administrative enrollment. Those are different authority boundaries and need separate design.
The public catalog intentionally omits membership identities.

An authenticated subject must carry a nonempty GUID `learner_id` claim and a space-separated `scope`
claim containing `enrollment.write`. That claim mapping is this lab's identity integration contract;
OIDC does not require all providers to represent users as GUIDs. A real deployment needs a trusted
issuer-side mapping or a local subject-to-learner lookup. Do not reinterpret any arbitrary `sub` string
as a domain identifier without an explicit identity mapping.

The learner ID comes only from validated claims. A body or user-controlled header cannot choose another
learner. The policy runs before the workflow, including before a stored receipt can be replayed.

## Token Validation Configuration

The API uses the standard JWT bearer handler with an HTTPS authority and expected audience. It leaves
signature, issuer, audience, and lifetime validation enabled. Claim name mapping is disabled so the
configured `learner_id` and `scope` contract remains explicit.

The base configuration contains empty authority/audience values, not a secret or a permissive fallback.
Startup options validation requires an absolute HTTPS authority and nonblank audience. The API does not
collect a username/password or issue its own production access token. Acquire an access token from a
properly configured OAuth/OIDC identity provider.

For a local learning host, set configuration for your identity provider in the current PowerShell session:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:Identity__Authority = "https://your-identity-provider.example"
$env:Identity__Audience = "your-enrollment-api-audience"
dotnet run --project 10-architecture/src/Learning.Architecture.Api -- --urls http://localhost:5180
```

Replace the example values; they are not a hosted identity service. Loopback HTTP here is a local lab
transport only. Deploy behind a correctly configured HTTPS endpoint/proxy, restrict allowed hosts,
and apply the forwarded-header, HSTS, and operational lessons from Phase 07.

Development startup initializes two file-backed SQLite databases and one sample offering:
`10000000-0000-0000-0000-000000000001`, with capacity two. Files are resolved from the process working
directory, so use explicit absolute database paths for repeatable local environments. Automatic
initialization is disabled by default and refused outside Development/Testing.

For a fully offline walkthrough without an identity provider, use the relational console lab instead.
Do not copy the test authentication handler into the API to make manual testing easier.

## Send an Enrollment Request

With a valid access token carrying the required mapped claims:

```powershell
$enrollmentHeaders = @{
    Authorization = "Bearer $env:ENROLLMENT_ACCESS_TOKEN"
    "Idempotency-Key" = [guid]::NewGuid().ToString()
}
Invoke-RestMethod -Method Post `
    -Uri "http://localhost:5180/api/v1/offerings/10000000-0000-0000-0000-000000000001/enrollments" `
    -Headers $enrollmentHeaders
```

Keep the same key when retrying an uncertain request. Avoid printing or committing the token. The
request has no learner body because the authenticated identity selects the learner.

Success returns a transport DTO such as:

```json
{
  "status": "Enrolled",
  "version": 1,
  "replayed": false
}
```

The same key and input replay the original result with `replayed: true`. A new key for an existing
membership returns `AlreadyEnrolled`. Both use 200 in this API because the operation reports an
enrollment decision rather than exposing a separately addressable enrollment resource with a `Location`.
If you later introduce such a resource, document how 201/200 and replay semantics evolve.

## Error Mapping Is a Contract

| Condition | HTTP status | Caller action |
|---|---:|---|
| No valid access token | 401 with bearer challenge | obtain/refresh a valid token |
| Valid identity without required learner mapping/scope | 403 | request appropriate authority; do not retry blindly |
| Invalid key or page range | 400 validation problem | correct request input |
| Unknown offering | 404 problem | correct resource ID |
| Full offering | 409 problem | show a business outcome; retry is not guaranteed to help |
| Same key with different semantic input | 409 problem | preserve original request or use a genuinely new key |
| Optimistic write conflict | 409 problem | retry under the same key in a new request scope |
| SQLite busy/locked | 503 with `Retry-After: 1` | bounded backoff; retain request key |
| Unexpected server failure | 500 problem | retain key; investigate server telemetry |

Client-visible failures avoid stack traces, SQL, and raw exception messages. `AddProblemDetails`, exception
handling, and status-code pages establish the host-level response policy. Framework binding failures
are not application business results. Tests require problem JSON for explicit key validation but do not
promise every middleware-generated 400 has an identical field dictionary.

Do not overload 409 to mean the client should retry forever. A full course, key misuse, and a stale
write have distinct meanings even when sharing a status code. For a production public API, add stable
machine-readable problem type identifiers/error codes, contract tests, localization policy, and an
API specification. The current titles are readable lab explanations, not a frozen error-code registry.

## V1 Is an Actual Contract, Not a Versioning Demo Label

Routes explicitly use `/api/v1`. No v2 endpoint or version-negotiation library is installed merely to
suggest a feature exists. URL segment versioning keeps the first contract visible and avoids ambiguous
default-version negotiation for this small API.

Compatibility decisions still require work:

- Adding a response field is usually compatible for tolerant clients; verify the clients you support.
- Renaming/removing a field, changing status semantics, or changing identity mapping can break callers.
- Adding new enum-like status strings can break exhaustive client code even without a schema removal.
- A request-key fingerprint must include new fields that affect behavior.
- Retained v1 receipts may need to replay after server upgrades; preserve their result interpretation.
- Public contract version and integration event schema version are independent lifecycles.

Introduce v2 only for a justified breaking contract, run v1 and v2 side by side during migration, and
publish a deprecation timeline. At larger scale, a versioning library can help route/report versions,
but it cannot decide business compatibility or retention for you.

## What the Integration Tests Actually Exercise

The first fixture uses an authentication handler compiled only into the test assembly. It gives
deterministic identities for HTTP authorization and request semantics. The second fixture retains the
real JWT bearer handler and supplies a local test OIDC configuration plus an RSA signing key. No
network discovery is required, but cryptographic and claim validation still run.

The JWT tests accept a correctly signed token, reject bad signature/issuer/audience/expiry, verify a
bearer challenge, and distinguish a valid token without write scope as 403. HTTP tests cover public
projection, invalid/missing identity, replay, cross-learner key reuse, full capacity, missing offerings,
invalid request keys, and page binding/ranges.

Each factory owns two isolated named in-memory SQLite databases. The outbox worker is disabled there
to keep HTTP tests deterministic; delivery behavior has its own separate-database specifications.
These tests do not validate a real provider's discovery outage, signing-key rollover, proxy headers,
TLS deployment, or production database contention. Include those in an environment-level test plan.

## Exercises

1. Add stable problem type URIs and test each expected error contract.
2. Define a v2 response change and a migration policy without duplicating domain rules.
3. Add an administrator-only enroll-on-behalf operation with explicit audit and resource authorization.
4. Introduce tenant identity and show every place its isolation must be enforced.
5. Add an OpenAPI contract and validate unauthorized responses and idempotency headers in it.

## Official References

- [Configure JWT bearer authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-10.0).
- [Policy-based authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0).
- [Handle errors in ASP.NET Core APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling-api?view=aspnetcore-10.0).
- [ASP.NET Core integration tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0).

## Navigation

- Previous: [Module events, outbox, and inbox](05-modules-events-outbox-inbox.md)
- Next: [Architecture decisions and fitness checks](07-decisions-fitness-maintainability.md)
