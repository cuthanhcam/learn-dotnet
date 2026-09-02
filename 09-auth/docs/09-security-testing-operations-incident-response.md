---
title: "Security Testing, Operations, and Incident Response"
description: "Operate authentication safely with negative testing, controlled audit events, key and secret rotation, telemetry, deployment gates, and rehearsed incident response."
slug: auth-security-testing-operations-incident-response
phase: 9
order: 9
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 46
topics: [security-testing, audit-logging, key-rotation, secrets, telemetry, incident-response]
prerequisites: [auth-abuse-resistance-account-lifecycle]
status: maintained
last-reviewed: 2026-09-02
---

# Security Testing, Operations, and Incident Response

## Security Continues After the Feature Ships

Authentication is a living production control. Issuer keys rotate, dependencies change, accounts are
compromised, clocks drift, proxies are reconfigured, response behavior leaks information, and limits
that looked safe in development fail under real traffic. Operations must preserve the same invariants
that code and tests establish.

Create an owner for identity-provider configuration, application authorization policy, secrets,
security telemetry, incident response, and recovery. “The platform team handles security” is not an
ownership model unless responsibilities and escalation paths are explicit.

## Test Trust Boundaries, Not Only Happy Paths

| Boundary | Required negative cases |
|---|---|
| Password | unknown/malformed identity, wrong value, rehash, disabled/locked account, concurrent failures |
| JWT | bad signature, wrong issuer/audience/algorithm, missing expiry, expired/not-yet-valid, malformed claims |
| Refresh session | unknown, expired, revoked, duplicated, concurrent rotation, replayed ancestor |
| Authorization | unauthenticated, wrong role/scope, non-owner, stale claims, missing resource, concealed existence |
| Browser | missing/mismatched antiforgery token, hostile origin, unsafe redirect, cookie policy, key persistence |
| Abuse controls | exact boundary, retry metadata, many partitions, proxy chain, replica behavior, store outage |

Tests should assert the external contract and the protected state transition. A `401` alone does not
prove a replayed refresh-token family was revoked. A `403` alone does not prove that the resource was
left unchanged. Exercise cancellation and concurrency, and replace wall-clock waits with `TimeProvider`.

Run dependency vulnerability audit, secret scanning, static analysis, formatting, build, tests, and
documentation validation in CI. Treat scanners as fallible controls: investigate findings, record
narrow suppressions with rationale, and remove obsolete suppressions. Never weaken a gate globally to
silence one understood exception.

## A Safe Security-Event Contract

The application emits a closed `SecurityEventType`, server timestamp, and optional stable internal
subject identifier through `ISecurityEventSink`. The API adapter uses source-generated `LoggerMessage`
templates. It never accepts an arbitrary message, email, credential, header, token, or attacker-provided
label. This design makes the safe path easier than ad hoc logging.

Events currently distinguish sign-in success/rejection, lockout start, refresh rotation/rejection/replay,
and session revocation. Public HTTP responses remain generic. Operators can investigate patterns without
turning the response into an account or token oracle.

Security logs need access controls, integrity protection, retention and deletion rules, UTC timestamps,
clock synchronization, trace correlation, and alert ownership. They are sensitive data, not a debugging
dump. Keep account IDs and network data out of low-cardinality metric dimensions; use controlled event
type/outcome counters and query protected audit records during an investigation.

## Signing-Key Lifecycle

The sample uses one symmetric HMAC key injected from configuration to teach validation mechanics. A
production authorization server normally uses an asymmetric key protected by a managed key service or
HSM, publishes public verification keys, and identifies them with `kid`. Resource servers should obtain
keys only from a configured trusted issuer and cache metadata within bounded rules.

A safe rollover sequence is:

1. Generate new key material in the protected key system; never in source control or a build log.
2. Publish the new public key while the old signing key remains verifiable.
3. Begin signing new tokens with the new key and unique key identifier.
4. Observe validation success across every resource server and refresh caches safely.
5. Retain the old verification key until every token it signed has expired plus bounded clock/cache skew.
6. Remove or disable the old key, preserve required audit evidence, and test emergency rollback.

Emergency compromise rotation is different: stop issuance, remove trust as quickly as the risk demands,
revoke server-side sessions, force reauthentication, investigate access, notify stakeholders, and accept
that offline self-contained access tokens cannot be individually recalled without an online check or
very short lifetime.

## Secrets and Configuration

Signing private keys, symmetric keys, client credentials, database passwords, SMTP/API credentials,
and Data Protection keys require deliberate storage and access policy. Prefer workload identity and
short-lived credentials over copied static secrets. Separate environments and tenants; a development
credential must never authenticate to production.

Validate security configuration at startup, as this sample does for JWT and sign-in settings. Fail
closed on missing issuer, audience, key strength, or impossible limits. Avoid printing configuration
objects because their future fields may become secret. Rotation must be rehearsed before expiry, not
invented during an outage.

## Telemetry and Alerts

Useful controlled metrics include:

- sign-in attempts and outcome categories;
- lockouts started and recovery rate;
- refresh rotations, replay detections, and revocations;
- authentication challenge and authorization forbid counts by endpoint group;
- rate-limit accepted/rejected counts and limiter latency;
- token-validation failures by controlled reason;
- identity-provider latency, availability, metadata refresh, and key age.

Alert on changes in rate and ratio, not just a fixed global count. A surge of failures across many
accounts suggests credential stuffing; many sources targeting one account suggests focused takeover;
refresh replay may indicate credential theft or a client concurrency bug. Every alert needs a runbook,
severity, owner, evidence links, and a tested route to containment.

## Incident Response

Prepare playbooks for leaked signing keys, stolen refresh tokens, compromised user accounts, malicious
administrators, identity-provider outage, recovery-channel takeover, and audit-pipeline failure. A useful
playbook separates:

1. **Detect and validate:** establish scope without spreading secrets through tickets or chat.
2. **Contain:** revoke token families, disable accounts/clients, restrict routes, or rotate keys.
3. **Eradicate:** remove persistence, repair configuration/code, and invalidate affected credentials.
4. **Recover:** restore service gradually, monitor for recurrence, and support legitimate users.
5. **Learn:** preserve evidence, document timeline and impact, improve tests/controls, and assign actions.

Do not destroy evidence during cleanup. Record who initiated high-impact actions and require dual control
where the threat model warrants it. Practice key rotation and global session revocation in a non-production
environment with realistic topology.

## Deployment Checklist

- HTTPS and trusted proxy/network configuration are explicit.
- Issuer, audience, algorithms, redirect URIs, origins, and hosts use environment-specific allowlists.
- Secrets are injected from an approved store and absent from repository, artifacts, and logs.
- Data Protection and session state survive planned restarts and scale-out.
- Database uniqueness, optimistic concurrency, and atomic refresh rotation are enforced persistently.
- Rate limits are load-tested at the password-hash cost and coordinated across replicas/edge controls.
- Security events reach protected storage; dashboards, alerts, retention, and redaction are verified.
- Key rotation, provider outage, rollback, and incident playbooks have named owners.
- Negative integration tests run against production-equivalent authentication configuration.

## Implementation Map

| Concern | Code |
|---|---|
| Closed event vocabulary and port | `Application/Abstractions/ISecurityEventSink.cs` |
| Sign-in event production | `Application/Identity/CredentialSignInService.cs` |
| refresh/replay/revocation event production | `Application/Identity/RefreshSessionService.cs` |
| Source-generated structured logging adapter | `Api/Security/StructuredSecurityEventSink.cs` |
| Event registration and composition | `Api/Program.cs` |
| Event transition assertion | `UnitTests/IdentityServicesTests.cs` |

## Common Failure Modes

- Logging a complete request, authorization header, token, cookie, password, or reset URL.
- Building log messages from user input or allowing unbounded labels in metrics.
- Rotating a signing key before resource servers can obtain the new public key.
- Removing an old verification key while valid tokens still reference it.
- Assuming that deleting a browser cookie revokes copied access or refresh tokens.
- Alerting on every failed login without baselines, ownership, or an actionable runbook.
- Testing authentication only with mocked handlers and never exercising real middleware validation.
- Keeping a scanner suppression after the code or dependency that required it has disappeared.
- Discovering during an incident that global revocation and key rollover were never rehearsed.

## Exercises

1. Add a recording event sink test for refresh replay without storing or asserting the raw token.
2. Model an asymmetric signing-key overlap and test both keys before and after retirement.
3. Write an incident runbook for refresh-token database disclosure, including user impact and evidence.
4. Create low-cardinality metrics and alerts that distinguish stuffing from a focused account attack.
5. Run a game day where issuer metadata is unavailable but cached signing keys remain valid.

## References

- [ASP.NET Core security and Identity](https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-10.0)
- [Logging in .NET and ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-10.0)
- [Compile-time logging source generation](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/source-generation)
- [Safe storage of app secrets in development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-10.0)
- [OWASP Logging Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Logging_Cheat_Sheet.html)
- [NIST SP 800-61 Revision 3](https://csrc.nist.gov/pubs/sp/800/61/r3/final)

## Navigation

- Previous: [Abuse resistance and account lifecycle](08-abuse-resistance-account-lifecycle.md)
- Next: Phase 09 completion audit (planned)
