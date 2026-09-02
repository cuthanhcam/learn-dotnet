---
title: "Abuse Resistance and Account Lifecycle"
description: "Layer account lockout, partitioned rate limiting, generic failures, recovery tokens, proxy trust, audit events, and operational controls around authentication endpoints."
slug: auth-abuse-resistance-account-lifecycle
phase: 9
order: 8
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 44
topics: [rate-limiting, lockout, credential-stuffing, account-recovery, audit, forwarded-headers]
prerequisites: [auth-browser-security-cookies-csrf-token-storage]
status: maintained
last-reviewed: 2026-09-02
---

# Abuse Resistance and Account Lifecycle

## Correct Authentication Is Not Sufficient

A password verifier may be cryptographically correct and still expose the system to credential
stuffing, distributed guessing, account discovery, recovery abuse, notification flooding, or resource
exhaustion. Authentication endpoints are intentionally public and perform expensive work, so they need
layers that fail safely under hostile volume.

The sample combines two controls with different partition keys:

| Control | Partition | Mitigates | Important limitation |
|---|---|---|---|
| Fixed-window API limiter | policy + observed network address | bursts and repeated expensive calls | shared networks and distributed attackers reduce accuracy |
| Timed account lockout | normalized account identity | focused guessing across many addresses | attackers can deliberately lock a known victim |

Neither control is enough alone. Rate limiting by account reveals and targets identities; limiting
only by IP punishes NAT users and is bypassed by botnets. Production defenses often add edge limits,
device and risk signals, breached-password detection, MFA, and alerting.

## Enumeration-Resistant Failure Contracts

Unknown account, malformed identity, wrong password, disabled account, and active lockout all produce
the same public sign-in message and status in this API. Unknown identities run a dummy password verify
so the most obvious timing difference is reduced. Internally, the service preserves distinct results
for policy and telemetry without exposing them to the caller.

Perfectly equal timing is difficult: data access, caching, hashing upgrades, network jitter, and
downstream work differ. Test distributions rather than expecting identical stopwatch values. Do not
“fix” enumeration by adding a predictable sleep; it consumes capacity and remains measurable.

Registration, password reset, and email verification also need generic public responses. For example,
“If an eligible account exists, instructions were sent” avoids confirming an address. Apply the same
rule to response body, status, redirect, latency, email behavior, support workflow, logs, and metrics.

## Account Lockout Invariant

`UserAccount.RecordFailedSignIn` owns the consecutive-failure count and lockout transition. It caps the
counter, synchronizes concurrent mutations in the learning adapter, and does not extend an active
lockout on every new attempt. A correct password cannot clear an active lockout; after expiration, a
successful verification resets the state.

The password is still verified before returning the unavailable-account result. That keeps disabled
and locked accounts closer to ordinary failures and ensures a caller cannot cheaply probe status.

In a relational store, do not load a counter, increment it in memory, and overwrite it without a
concurrency strategy. Use an atomic conditional update or optimistic concurrency token, retry bounded
conflicts, and persist the account state and security audit consistently. Multiple application nodes
must observe the same lockout state.

Lockout creates a denial-of-service lever against known accounts. Keep it bounded, combine it with
network/device throttling, notify the owner appropriately, and consider progressive delay or risk-based
challenges for systems where availability matters more than a hard lock. Administrative unlock must
be authenticated, authorized, and audited.

## ASP.NET Core Rate Limiting

The API registers endpoint-specific policies and calls `UseRateLimiter` after routing. Credential
operations allow ten requests per one-minute process-local window; refresh and revoke operations have
a separate twenty-request budget. There is no queue: authentication callers receive `429 Too Many
Requests` rather than holding connections and consuming memory. When the limiter supplies retry
metadata, the response includes `Retry-After` and safe Problem Details.

Fixed windows are intentionally easy to study but permit bursts at boundaries. Sliding-window and
token-bucket limiters smooth traffic differently; a concurrency limiter protects a scarce concurrent
resource. Select and load-test the algorithm from measured workload and attacker behavior.

The built-in in-memory limiter is per process. Adding replicas multiplies the effective allowance.
Production systems commonly enforce a coarse shared rule at the gateway/CDN and a semantic rule in
the application, or use a distributed limiter whose failure mode is explicitly chosen. Decide whether
limiter-store failure fails open for availability or closed for protection.

## Partition Keys and Trusted Proxies

The sample partitions by `HttpContext.Connection.RemoteIpAddress`, never by a raw
`X-Forwarded-For` header. Behind a reverse proxy, configure Forwarded Headers Middleware with exact
`KnownProxies` or `KnownNetworks` before rate limiting. Otherwise:

- trusting arbitrary forwarded headers lets an attacker choose a fresh partition on every request;
- ignoring the trusted proxy makes every Internet user appear to share the proxy's address;
- parsing a multi-hop chain incorrectly attributes the wrong address;
- storing full addresses indefinitely can create privacy and retention concerns.

IP address is a weak, changing network signal, not an identity. Do not use it as the sole authorization
factor or emit it as an unbounded metrics label.

## Password Reset and Verification Tokens

Recovery can bypass the password and deserves at least the same protection as sign-in. A safe reset or
email-verification credential should be:

- generated with a cryptographically secure random source and enough entropy;
- purpose-bound to one operation, account, and security state;
- short-lived and single-use through an atomic consume operation;
- stored as a digest when a database read would otherwise reveal a usable token;
- invalidated after password, email, account-status, or relevant security-stamp changes;
- transported only over HTTPS and kept out of logs, analytics, and referrer-bearing URLs.

After a password reset, decide explicitly whether to revoke every refresh-token family, preserve the
current recovery session, and notify the owner. Do not automatically sign a user into a high-privilege
session merely because a reset link was redeemed. Recovery email changes need protection against both
the old and new address becoming attacker-controlled.

This slice documents recovery invariants but deliberately does not add an email sender or pretend that
an in-memory reset flow represents production delivery. Those belong behind application ports with
one-time persistence and integration tests.

## Audit Without Secret Leakage

Record security events such as sign-in success/failure category, lockout start/end, recovery requested
and completed, session rotation/replay/revocation, privilege change, and administrative unlock. Include
a server-generated event ID, UTC timestamp, stable internal subject where known, outcome, coarse source
context, and trace correlation.

Never log passwords, access tokens, refresh tokens, reset links, cookie values, client secrets, or raw
authorization headers. Sanitize user-provided values before structured logging to prevent log forging.
Keep high-cardinality identities out of metric labels; use controlled outcome dimensions and place
detailed events in access-controlled audit storage with retention and tamper protections.

## Testing Strategy

The executable tests use a manual `TimeProvider` instead of waiting fifteen minutes. They verify
threshold transition, correct-password rejection during lockout, recovery after expiry, state reset,
and twenty concurrent failures. An integration test exhausts the endpoint budget and asserts `429`,
safe Problem Details, and `Retry-After`.

Production validation should also cover multiple nodes, atomic database conflicts, proxy chains,
window boundaries, limiter-store outage, IPv4/IPv6 representation, NAT fairness, cancellation, and
load at the configured password-hash cost. Security limits are capacity assumptions and require
monitoring and tuning after deployment.

## Implementation Map

| Concern | Code |
|---|---|
| Atomic failure and lockout state | `Domain/Users/UserAccount.cs` |
| Validated policy settings | `Application/Identity/SignInSecurityOptions.cs` |
| Enumeration-resistant orchestration | `Application/Identity/CredentialSignInService.cs` |
| Partitioned HTTP policies and `429` response | `Api/Security/AuthRateLimitPolicies.cs` |
| Endpoint policy attachment and middleware order | `Api/Program.cs` |
| Controllable time and concurrent transition tests | `UnitTests/IdentityServicesTests.cs` |
| Real middleware rejection contract | `IntegrationTests/AbuseProtectionTests.cs` |

## Common Failure Modes

- Returning “account locked” only for known accounts while unknown accounts return “invalid user.”
- Skipping expensive verification for disabled accounts and creating a timing oracle.
- Updating a failure counter with a lost-update race across concurrent requests.
- Extending lockout on every hostile request so the victim can never recover.
- Applying one global limiter so one noisy client denies service to everyone.
- Queuing large numbers of expensive password requests instead of rejecting excess traffic.
- Trusting arbitrary forwarded headers or treating an IP address as a durable user identity.
- Assuming an in-process limiter is shared by every replica.
- Logging credentials or using account/email/IP as unrestricted metric labels.
- Making reset tokens reusable, long-lived, stored in plaintext, or valid after security changes.

## Exercises

1. Replace the fixed window with a token bucket and explain the new burst behavior.
2. Implement an EF Core atomic lockout update with a concurrency token and conflict tests.
3. Add a purpose-bound, digest-stored, one-time password reset token and revoke all token families.
4. Configure a known reverse proxy and prove that spoofed forwarded headers cannot choose a partition.
5. Define alerts for distributed guessing without placing email addresses in metric labels.

## References

- [Rate limiting middleware in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-10.0)
- [Configure ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-10.0)
- [Configure ASP.NET Core to work with proxy servers and load balancers](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-10.0)
- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- [OWASP Forgot Password Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Forgot_Password_Cheat_Sheet.html)

## Navigation

- Previous: [Browser security: cookies, CSRF, CORS, and token storage](07-browser-security-cookies-csrf-token-storage.md)
- Next: [Security testing, operations, and incident response](09-security-testing-operations-incident-response.md)
