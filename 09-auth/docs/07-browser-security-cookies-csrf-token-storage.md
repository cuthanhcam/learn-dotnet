---
title: "Browser Security: Cookies, CSRF, CORS, and Token Storage"
description: "Design browser authentication around XSS, CSRF, origin, cookie, storage, HTTPS, and backend-for-frontend trust boundaries."
slug: auth-browser-security-cookies-csrf-token-storage
phase: 9
order: 7
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 40
topics: [cookies, csrf, cors, xss, same-site, token-storage, bff]
prerequisites: [auth-oauth2-openid-connect-provider-boundaries]
status: maintained
last-reviewed: 2026-09-02
---

# Browser Security: Cookies, CSRF, CORS, and Token Storage

## The Browser Changes the Threat Model

Browsers automatically attach eligible cookies, execute downloaded JavaScript, enforce origin rules,
retain data in several storage systems, and leak URL data through history, referrers, screenshots, and
intermediaries. A safe API token design does not automatically become a safe browser session design.

Start with two independent attacker capabilities:

- Cross-site scripting (XSS) executes code in the application's origin and can read browser-accessible
  tokens or issue same-origin requests as the user.
- Cross-site request forgery (CSRF) causes a browser to send ambient credentials, usually cookies,
  from an attacker-controlled origin.

An `HttpOnly` cookie makes its value unavailable to JavaScript and reduces token theft through XSS,
but injected script can still perform actions. A bearer token in memory avoids automatic CSRF-style
attachment, but injected script can read or use it. No storage choice compensates for unsafe script.

## Prefer a Server-Side Browser Session

For sensitive browser applications, a backend-for-frontend (BFF) can act as the confidential OIDC
client, retain upstream access and refresh tokens on the server, and issue the browser only a hardened
session cookie. The BFF calls downstream APIs on the user's behalf.

This removes long-lived tokens from JavaScript and centralizes refresh, revocation, and client-secret
handling. It adds server state, a CSRF boundary, proxy responsibility, and horizontal-scaling needs.
It is an architectural tradeoff, not a magic security product.

Avoid persistent bearer tokens in `localStorage`: any script running in the origin can read them and
they commonly outlive the page that created them. In-memory storage limits persistence but is still
available to compromised JavaScript. Never place tokens in query strings or URLs.

## Harden Authentication Cookies

A production authentication cookie normally needs:

- `Secure`, so the browser sends it only over HTTPS;
- `HttpOnly`, so ordinary JavaScript cannot read the value;
- an explicit `SameSite` policy compatible with the selected sign-in flow;
- a narrow `Path` and no broad `Domain` unless cross-subdomain sharing is intentional;
- an opaque, integrity-protected value with bounded lifetime and server-side revocation strategy.

`SameSite=Strict` provides the strongest cross-site restriction but can break legitimate navigation.
`Lax` permits selected top-level navigation and is often a practical session default. `None` enables
cross-site use and requires `Secure`; it expands the CSRF surface. OIDC correlation and nonce cookies
often need settings different from the main application cookie. Test the actual redirects rather than
globally weakening every cookie.

Cookie encryption keys are operational secrets. In a multi-instance deployment, persist and share
ASP.NET Core Data Protection keys securely, restrict access, set an application discriminator, and
plan backup and rotation. Ephemeral keys invalidate sessions after restart.

## CSRF Defenses

State-changing cookie-authenticated requests require an antiforgery control. ASP.NET Core uses a token
pair so a cross-site attacker who can cause a request cannot supply the matching request token. Send
the request token in a form field or custom header and validate it on unsafe methods.

Defense in depth includes:

- use `GET`, `HEAD`, and `OPTIONS` only for safe operations;
- apply an appropriate `SameSite` cookie policy;
- validate `Origin` or `Referer` where the deployment can do so reliably;
- reject simple content types when an API contract requires JSON;
- require recent reauthentication for destructive or high-value actions.

Do not put an antiforgery token in an `HttpOnly` cookie if JavaScript must copy it into a header. The
authentication cookie remains `HttpOnly`; the independently scoped request token may be readable.
XSS can generally defeat CSRF protections, so output encoding, safe DOM APIs, Content Security Policy,
dependency hygiene, and removal of inline script remain essential.

## CORS Is Not Authentication or CSRF Protection

Cross-Origin Resource Sharing tells a browser which origins may read a cross-origin response. It does
not stop non-browser clients, prove caller identity, or replace authorization. Some cross-origin
requests can be sent even when their responses cannot be read.

For credentialed requests:

- list exact trusted origins; do not combine credentials with a wildcard origin;
- allow only required methods and headers;
- understand and cache preflight responses deliberately;
- vary caches by `Origin` when responses differ by origin;
- keep development origins out of production configuration.

Origins include scheme, host, and port. `https://app.example.com` and
`https://admin.example.com` are different origins even though they share a registrable domain.

## HTTPS, Proxies, and Host Trust

Terminate TLS only at trusted infrastructure and preserve the original scheme and client information
through explicitly configured forwarded headers. Trust only known proxy networks. Blindly accepting
`X-Forwarded-For`, `X-Forwarded-Proto`, or `Host` lets a direct caller influence security decisions,
redirect generation, rate-limit partitions, and audit data.

Enable HSTS for established HTTPS production sites, but stage its rollout because cached policy can
make a misconfigured domain unavailable. Validate hosts and construct security-sensitive callback
URLs from trusted configuration where practical.

## Logout and Session Expiration

Logout must match the application's promise. Deleting the browser cookie ends that local browser
session, but it may not revoke a server session, refresh token family, provider session, or access
token already copied elsewhere. Define idle timeout, absolute timeout, sliding renewal, concurrent
session behavior, privilege-change invalidation, and global sign-out separately.

Return cache directives that prevent authenticated HTML and sensitive API responses from being stored
in shared caches. Clear relevant site data carefully on logout without erasing unrelated applications
on a shared parent domain.

## Decision Guide

| Requirement | Strong starting point |
|---|---|
| Server-rendered app | OIDC client plus secure, HttpOnly, same-site application cookie |
| Sensitive SPA with first-party backend | BFF plus hardened cookie and antiforgery validation |
| Public API used by non-browser clients | bearer access token validated by the resource server |
| Cross-origin cookie calls | exact CORS allowlist, credentials, CSRF defense, and proxy review |
| Offline native access | platform-protected token storage and authorization code with PKCE |

## Common Failure Modes

- Persisting access or refresh tokens in `localStorage` without documenting the XSS consequence.
- Assuming `HttpOnly` prevents injected JavaScript from acting as the user.
- Disabling antiforgery validation because the endpoint consumes JSON.
- Treating CORS as a server-to-server access control or using `AllowAnyOrigin` with credentials.
- Setting every cookie to `SameSite=None` to repair one external login callback.
- Sharing cookies across all subdomains and allowing an untrusted subdomain to set parent cookies.
- Losing Data Protection keys on deployment and unexpectedly signing out every user.
- Trusting forwarded headers from any network or deriving callback URLs from an unvalidated host.
- Implementing logout as a UI redirect while leaving server sessions and refresh credentials active.

## Exercises

1. Threat-model the same document editor as a token-storing SPA and as a BFF application.
2. Add antiforgery validation to a cookie-authenticated mutation and test missing, mismatched, and
   valid token pairs.
3. Configure exact CORS origins for development and production, then test a hostile origin preflight.
4. Persist Data Protection keys for two application instances and verify that either can read a cookie.
5. Specify local logout, provider logout, one-session revocation, and global revocation as separate use cases.

## References

- [Prevent Cross-Site Request Forgery attacks in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-10.0)
- [Enable Cross-Origin Requests in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-10.0)
- [SameSite cookies in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-10.0)
- [Host ASP.NET Core behind a proxy or load balancer](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-10.0)
- [ASP.NET Core Data Protection overview](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-10.0)

## Navigation

- Previous: [OAuth 2.0, OpenID Connect, and provider boundaries](06-oauth2-openid-connect-provider-boundaries.md)
- Next: Abuse resistance and account lifecycle (planned)
