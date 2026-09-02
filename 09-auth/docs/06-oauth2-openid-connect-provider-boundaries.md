---
title: "OAuth 2.0, OpenID Connect, and Provider Boundaries"
description: "Choose standards-based delegated authorization and authentication flows without confusing access tokens, ID tokens, clients, resource servers, or identity providers."
slug: auth-oauth2-openid-connect-provider-boundaries
phase: 9
order: 6
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 42
topics: [oauth2, openid-connect, pkce, identity-provider, token-validation]
prerequisites: [auth-roles-claims-policies-resource-authorization]
status: maintained
last-reviewed: 2026-09-02
---

# OAuth 2.0, OpenID Connect, and Provider Boundaries

## Start with the Problem Each Protocol Solves

OAuth 2.0 is an authorization framework: a client obtains limited authority to call a resource
server on behalf of a resource owner or as itself. OAuth alone does not define an application login
session or prove who the human is. OpenID Connect (OIDC) adds an identity layer, an ID token, a UserInfo
endpoint, discovery metadata, and rules that let a client authenticate an end user.

This distinction prevents several dangerous substitutions:

- An **access token** is presented to the API named by its audience. It is not application profile data.
- An **ID token** tells its client about an authentication event. It must not be accepted as an API
  access token.
- A **refresh token** is presented only to the authorization server. It must not be sent to resource
  APIs.
- A local application cookie represents the web application's session. It is not an OAuth token.

## Actors and Trust Boundaries

| Actor | Responsibility | Must not assume |
|---|---|---|
| Resource owner | grants or withholds delegated access | that a consent screen makes a malicious client safe |
| Client | initiates the flow and uses granted tokens | that token contents are valid before protocol validation |
| Authorization server / OIDC provider | authenticates, obtains consent, and issues tokens | that every registered redirect URI is harmless |
| Resource server | validates access tokens and enforces permissions | that an authenticated caller may access every resource |
| User agent | carries front-channel redirects | that URL data remains secret or unmodified |

The roles are logical. One product may deploy more than one role, but each role still needs a clear
issuer, audience, secret, redirect, and policy boundary.

## Authorization Code with PKCE

Authorization Code with Proof Key for Code Exchange (PKCE) is the normal interactive flow for native,
browser-based, and server-rendered clients. The client creates a high-entropy `code_verifier`, sends
its derived `code_challenge` in the authorization request, and later proves possession of the verifier
when redeeming the short-lived code. A stolen authorization code is therefore insufficient by itself.

The client must also create and verify:

- `state`, bound to the initiating browser session, to correlate the response and resist request
  forgery;
- OIDC `nonce`, bound to the authentication request and checked in the ID token, to resist replay;
- an exact pre-registered redirect URI rather than an open redirect or wildcard;
- issuer and discovery metadata from an explicitly trusted authority.

PKCE does not replace redirect URI validation, TLS, `state`, `nonce`, client authentication for
confidential clients, or token validation. It solves the authorization-code interception problem.

## Client Types and Secret Reality

A confidential client can protect credentials in a controlled server environment. A public client,
including installed applications and code shipped to a browser, cannot keep a static secret: every
user can inspect it. Giving a SPA a `client_secret` only publishes that secret.

| Application | Typical pattern | Important boundary |
|---|---|---|
| Server-rendered web app | OIDC code flow + PKCE, local secure cookie | tokens remain on the trusted server |
| Browser SPA | code flow + PKCE or a backend-for-frontend (BFF) | browser storage and XSS materially change risk |
| Native/mobile app | system browser + code flow + PKCE | use claimed HTTPS or private-use redirect schemes safely |
| Service/daemon | client credentials or workload identity | no human user is implied by the grant |
| API | validate access token | never run an interactive login flow for an API request |

The Resource Owner Password Credentials grant and implicit flow are legacy designs and should not be
introduced into new systems. Device Authorization Grant is appropriate for input-constrained devices,
not a general substitute for a normal browser flow.

## Token Validation Is a Protocol Operation

An API should validate an access token using the issuer's supported middleware and metadata. For a
JWT, validation includes signature, trusted issuer, intended audience, allowed algorithm, lifetime,
and required claims. For an opaque token, the API may use introspection through a protected channel.
Parsing a JWT or Base64-decoding its payload proves nothing.

Key identifiers help select a candidate verification key but are not trust anchors. Keys come from
trusted issuer metadata, metadata retrieval needs TLS and caching, and key rollover must tolerate a
brief overlap without accepting keys from an arbitrary issuer. Validation failure is fail-closed.

## Scope, Consent, and Authorization

Scopes describe delegated authority requested by a client and granted by the authorization server.
They should be narrowly named for stable API capabilities. The resource server still applies local
authorization using current resource state. A scope such as `documents.write` does not prove that the
caller owns document 42.

Consent is not a replacement for organizational policy, and a token should not contain every piece
of profile or permission data. Minimize claims, avoid sensitive personal data, and remember that a
self-contained token is a snapshot until it expires.

## Provider Selection

Implementing an authorization server means owning protocol conformance, signing-key protection and
rotation, client registration, redirect validation, consent, MFA, federation, recovery, revocation,
abuse prevention, auditability, and vulnerability response. Most teams should use a maintained
identity platform or a standards-compliant provider rather than construct these capabilities from
individual JWT APIs.

The token issuer in this phase is deliberately local so the lifecycle is visible in tests. Its
boundary is an educational first-party session issuer, not a reusable OAuth/OIDC authorization server.

## Operational Checklist

- Register exact redirect and post-logout redirect URIs per environment.
- Require HTTPS outside isolated local development.
- Use discovery only from configured trusted issuers; validate the returned issuer exactly.
- Request the minimum scopes and validate the API audience.
- Keep authorization codes short-lived and single-use; require PKCE.
- Keep client credentials and signing keys outside source control and rotate them deliberately.
- Do not log codes, tokens, cookie values, client secrets, or sensitive claims.
- Model logout separately: ending the local session, revoking grants, and provider sign-out are
  different operations.

## Common Failure Modes

- Treating OAuth as authentication without OIDC and accepting an access token as identity evidence.
- Sending an ID token to an API or accepting a token minted for a different audience.
- Storing a client secret in JavaScript, a desktop binary, or a mobile package.
- Omitting PKCE, `state`, or OIDC `nonce`, or failing to compare them with session-bound values.
- Allowing wildcard redirects, open redirects, or user-controlled return URLs.
- Selecting verification keys from an untrusted `jku`, `x5u`, or issuer.
- Translating a broad scope directly into ownership of every domain resource.
- Building a provider because issuing a signed JWT appears simple.

## Exercises

1. Draw the front-channel and back-channel messages for a web client using code flow and PKCE.
2. Explain which checks reject an ID token replayed against the Learning API.
3. Design scopes for document read, authoring, and publishing without encoding document IDs in scopes.
4. Compare a SPA that stores tokens with a BFF that exposes only a same-site session cookie.
5. Write a key-rollover test where old and new signing keys overlap, then remove the old key.

## References

- [OAuth 2.0 and OpenID Connect in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-oidc-web-authentication?view=aspnetcore-10.0)
- [Microsoft identity platform and OAuth 2.0 authorization code flow](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-auth-code-flow)
- [OAuth 2.0 Security Best Current Practice (RFC 9700)](https://www.rfc-editor.org/rfc/rfc9700)
- [Proof Key for Code Exchange (RFC 7636)](https://www.rfc-editor.org/rfc/rfc7636)
- [OpenID Connect Core 1.0](https://openid.net/specs/openid-connect-core-1_0.html)

## Navigation

- Previous: [Roles, claims, policies, and resource-based authorization](05-roles-claims-policies-resource-authorization.md)
- Next: [Browser security: cookies, CSRF, CORS, and token storage](07-browser-security-cookies-csrf-token-storage.md)
