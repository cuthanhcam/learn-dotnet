---
title: "Authentication Schemes and JWT Bearer Validation"
description: "Issue compact learning access tokens and configure ASP.NET Core to validate signature, issuer, audience, algorithm, lifetime, and claims explicitly."
slug: auth-schemes-jwt-bearer-validation
phase: 9
order: 3
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 32
topics: [authentication, jwt, bearer-tokens, claims, aspnet-core]
prerequisites: [auth-identity-credential-storage]
status: maintained
last-reviewed: 2026-08-31
---

# Authentication Schemes and JWT Bearer Validation

## Authentication Before Authorization

Authentication converts evidence carried by a request into a `ClaimsPrincipal`. Authorization then
uses that principal and, when necessary, authoritative resource state to decide whether an operation
is allowed. A valid token is not blanket permission; it is only accepted identity evidence for its
intended issuer, audience, time window, and scheme.

ASP.NET Core authentication schemes name a handler and its options. The bearer handler extracts an
`Authorization: Bearer` value, validates it, and builds the principal. `UseAuthentication` must run
before `UseAuthorization`, and endpoints should opt into a policy with `RequireAuthorization`.

## JWT Is a Format, Not a Login Protocol

A JSON Web Token is a signed set of claims. Its payload is base64url encoded, not encrypted. Anyone
holding the token can normally read it, so never put passwords, refresh tokens, secrets, or
unnecessary personal data in claims.

OAuth 2.0 defines authorization flows and access-token use; OpenID Connect adds an identity layer.
JWT alone does not define login, consent, client authentication, discovery, key rotation, logout, or
federation. This phase issues a small first-party JWT to expose validation mechanics. A production
authorization server should normally be a maintained standards-based identity platform.

## Validation Contract

An API must validate all of these together:

- cryptographic signature against a trusted key;
- expected issuer (`iss`);
- intended API audience (`aud`);
- expiration and not-before times;
- an explicitly permitted signing algorithm;
- required claims and their application semantics.

Failure returns `401 Unauthorized`. An authenticated principal that lacks permission receives
`403 Forbidden`. Do not use a token's unverified payload to choose a key, tenant, or authorization
path without a securely constrained lookup policy.

The sample sets zero clock skew so expiry tests and the learning contract are exact. Distributed
production systems may need a small documented skew together with synchronized clocks. Large default
skew can extend effective token lifetime beyond what operators expect.

## Issuance Contract

The sample access token contains `sub`, `email`, `jti`, `iat`, and role claims. `sub` is the stable
account identifier; email can change and should not be a database key for authorization. `jti` makes
each token identifiable for audit or an optional deny-list. Access tokens last ten minutes by
default and are not persisted.

The HMAC key is read from configuration, must contain at least 256 bits, and is deliberately absent
from committed appsettings. Supply `Jwt__SigningKey` through user secrets, an environment variable,
or a deployment secret manager. Production systems often prefer asymmetric keys so APIs validate
with public material while only the issuer holds the private signing key.

## Claims Mapping

The sample disables inbound claim mapping so token claim names remain predictable. It explicitly
sets name and role claim types and uses the same `role` name during issuance and consumption. Silent
claim remapping is a frequent source of authorization bugs and confusing tests.

## Executable Flow

1. `POST /auth/register` creates an account through the credential boundary.
2. `POST /auth/sign-in` verifies credentials and issues a short-lived access token.
3. The client sends the token in the `Authorization` header—not a query string.
4. `GET /auth/me` runs through the real bearer handler and requires authentication.
5. Integration tests prove missing and tampered tokens receive a challenge.

## Operational Rules

- Use HTTPS end to end and configure trusted proxies deliberately.
- Never log authorization headers or raw tokens.
- Keep access tokens short-lived and design key rotation before deployment.
- Validate the audience at every receiving API; a token for one service is not valid everywhere.
- Keep authorization-critical mutable state out of long-lived claims or revalidate it online.
- Do not store bearer tokens in browser locations exposed unnecessarily to injected JavaScript.

## Implementation Map

| Concern | Code |
|---|---|
| Technology-neutral issuance port | `Application/Abstractions/IAccessTokenIssuer.cs` |
| Credential-to-session orchestration | `Application/Identity/SessionSignInService.cs` |
| JWT options and fail-fast validation | `Infrastructure/Tokens/JwtOptions.cs` |
| Minimal claim issuance | `Infrastructure/Tokens/JwtAccessTokenIssuer.cs` |
| Bearer validation and HTTP endpoints | `Api/Program.cs` |
| Real pipeline verification | `IntegrationTests/AuthenticationFlowTests.cs` |

## References

- [ASP.NET Core authentication overview](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-10.0)
- [Configure JWT bearer authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-10.0)

## Navigation

- Previous: [Identity and credential storage](02-identity-credential-storage.md)
- Next: Refresh-token lifecycle (planned)
