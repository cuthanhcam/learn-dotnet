---
title: "Refresh-Token Rotation, Replay Detection, and Revocation"
description: "Design renewable sessions with opaque credentials, digest-only storage, atomic one-time rotation, token families, replay response, and explicit revocation."
slug: auth-refresh-token-rotation-replay
phase: 9
order: 4
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 34
topics: [refresh-tokens, token-rotation, replay-detection, revocation, session-security]
prerequisites: [auth-schemes-jwt-bearer-validation]
status: maintained
last-reviewed: 2026-08-31
---

# Refresh-Token Rotation, Replay Detection, and Revocation

## Why Refresh Tokens Exist

Short-lived access tokens reduce the useful lifetime of a stolen bearer credential, but users should
not need to enter a password every few minutes. A refresh token represents a longer-lived server-side
session that can mint a new access token after the old one expires.

That convenience increases risk. A refresh token is a high-value bearer credential: possession can
extend a session repeatedly. It needs stronger storage, rotation, revocation, and monitoring than a
short-lived access token.

## The Session Model

This implementation separates four related identifiers:

| Value | Purpose | Client-visible | Stored by server |
|---|---|---:|---:|
| Raw refresh token | Opaque bearer credential | Yes, once | Never |
| SHA-256 digest | Stable lookup value | No | Yes |
| Session ID | Identifies one rotation generation | No | Yes |
| Family ID | Connects all generations from one sign-in | No | Yes |

The token contains 256 random bits encoded with base64url. It carries no claims and has no meaning to
the client. Hashing an already random high-entropy token is not a substitute for password hashing; it
is a safe digest lookup that prevents a database read from immediately revealing usable credentials.

## One-Time Rotation

When generation A is presented successfully:

1. compute its digest;
2. load and lock the authoritative session row;
3. confirm it is active and unexpired;
4. mark A as used;
5. insert generation B in the same family;
6. commit both changes atomically;
7. return B and a new short-lived access token.

A is never valid again. The in-memory adapter uses one lock to make this state transition observable.
A relational adapter needs a transaction plus a concurrency predicate or token. A read followed by
independent updates can allow two concurrent requests to create two valid replacements.

## Replay Detection

Consider an attacker and legitimate client that both possess A. One rotates first and receives B.
When A appears again, the server knows a credential was copied because a correctly behaving client
would have discarded it. The sample revokes every session in A's family, including B.

This response sacrifices the legitimate session to contain compromise. It should also emit a
security audit event without recording either raw token, notify the user where appropriate, and
support investigation. The public endpoint returns only a generic invalid-credential response so it
does not reveal whether a token existed, expired, was revoked, or triggered replay handling.

## Expiration and Revocation

Refresh expiration is checked against server time. The sample uses a 14-day lifetime with a validated
configuration range of 1–30 days. A production policy should decide whether rotation uses:

- sliding expiration, where every generation receives a new relative lifetime;
- absolute family expiration, where no generation can outlive the original sign-in boundary;
- both, commonly a short inactivity window plus a hard maximum lifetime.

The learning code demonstrates sliding expiration. Adding absolute expiration requires storing a
family-level deadline and carrying it unchanged across rotation.

`POST /auth/revoke` revokes the entire family and always returns `204 No Content`, including for an
unknown token. Idempotent, indistinguishable responses avoid making logout a token-validity oracle.
Password changes, account disablement, risk events, and administrative logout should revoke all
applicable families through an authenticated management use case.

## Client Storage

Native and confidential clients should use platform-protected credential storage. Browser applications
usually benefit from a secure backend-for-frontend or an `HttpOnly`, `Secure`, appropriately
`SameSite` cookie so JavaScript cannot read the refresh credential. Cookie authentication introduces
CSRF considerations; sending tokens to JavaScript introduces XSS theft considerations. There is no
storage choice without a threat model.

Never place refresh tokens in URLs, query strings, logs, telemetry properties, analytics events,
exception messages, local storage by default, or source control.

## Failure and Concurrency Matrix

| Presented state | Internal result | Public result | Mutation |
|---|---|---|---|
| Active | Rotated | New token pair | consume current, insert replacement |
| Expired | Expired | `401` generic | mark current revoked |
| Revoked | Revoked | `401` generic | none |
| Already used | Replay detected | `401` generic | revoke entire family |
| Unknown/malformed | Not found | `401` generic | none |
| Account disabled after rotation | Revoked | `401` generic | revoke replacement family |

## Implementation Map

| Concern | Code |
|---|---|
| Session state transitions | `Domain/Sessions/RefreshSession.cs` |
| Atomic storage contract | `Application/Abstractions/IRefreshSessionStore.cs` |
| Opaque-token contract | `Application/Abstractions/IRefreshTokenService.cs` |
| Sign-in session creation | `Application/Identity/SessionSignInService.cs` |
| Rotation and revocation use cases | `Application/Identity/RefreshSessionService.cs` |
| Random generation and digest | `Infrastructure/Tokens/CryptographicRefreshTokenService.cs` |
| Atomic learning adapter | `Infrastructure/Sessions/InMemoryRefreshSessionStore.cs` |
| Race and replay specifications | `UnitTests/RefreshSessionTests.cs` |
| End-to-end HTTP lifecycle | `IntegrationTests/AuthenticationFlowTests.cs` |

## Production Persistence Contract

A database implementation should uniquely index token digest, index family and user identifiers,
update the presented generation with an optimistic concurrency predicate, and insert the replacement
inside the same transaction. Retain used generations long enough to detect replay; deleting A as soon
as it rotates turns a meaningful replay into an indistinguishable unknown token.

Cleanup must preserve the security retention window while bounding storage. Encrypt sensitive session
metadata if required, minimize device data, and document privacy retention.

## Exercises

1. Add absolute family expiration and test rotation immediately before the deadline.
2. Persist sessions with EF Core and prove two database connections cannot both rotate A.
3. Add “revoke all other sessions” without exposing raw token values.
4. Emit a structured replay audit event containing family ID, user ID, time, and request correlation
   data—but no token or digest.
5. Design signing-key rotation independently from refresh-session rotation.

## References

- [OAuth 2.0 Security Best Current Practice — Refresh token protection](https://www.rfc-editor.org/rfc/rfc9700.html#name-refresh-token-protection)
- [ASP.NET Core authentication overview](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-10.0)

## Navigation

- Previous: [Authentication schemes and JWT bearer validation](03-authentication-schemes-jwt.md)
- Next: Claims, roles, and policy-based authorization (planned)
