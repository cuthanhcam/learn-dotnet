---
title: "Roles, Claims, Policies, and Resource-Based Authorization"
description: "Build least-privilege ASP.NET Core authorization with stable claim vocabularies, named policies, imperative resource checks, owner rules, and correct challenge-versus-forbid behavior."
slug: auth-roles-claims-policies-resource-authorization
phase: 9
order: 5
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 36
topics: [authorization, roles, claims, policies, resource-based-authorization, least-privilege]
prerequisites: [auth-refresh-token-rotation-replay]
status: maintained
last-reviewed: 2026-09-02
---

# Roles, Claims, Policies, and Resource-Based Authorization

## Authentication Is Not Permission

Authentication answers “which principal presented acceptable evidence?” Authorization answers “may
this principal perform this operation on this resource now?” Treating every authenticated user as
authorized collapses two different trust decisions and is a common source of data exposure.

ASP.NET Core supports increasingly contextual authorization mechanisms:

| Mechanism | Best fit | Example |
|---|---|---|
| Authenticated user | any signed-in principal | create a personal draft |
| Role | broad organizational responsibility | administrator operations |
| Claim/permission policy | stable capability | `profile.read` |
| Custom requirement | multi-input rule | verified account and supported region |
| Resource-based handler | decision needs loaded state | owner edits draft; anyone reads published |

Choose the least powerful mechanism that expresses the rule clearly. Do not force object ownership
into a global role, and do not load a database object merely to evaluate a static permission.

## Roles and Permissions

A role groups responsibilities such as `administrator`; a permission names a capability such as
`profile.read`. Large applications generally authorize against permissions and map roles to those
permissions administratively. This reduces endpoint coupling to organizational names.

The learning model uses a closed vocabulary for both. It rejects a role string supplied directly by
a request, preventing callers from inventing authority through mass assignment. Production role and
permission changes require an authenticated administrative use case, audit event, persistence
constraint, and session invalidation decision.

## Claims Are Assertions and Snapshots

A claim is an assertion from the trusted issuer, not automatically a permission. The API trusts a
role or scope claim only after token signature, issuer, audience, algorithm, and lifetime validation.
Claims copied into a stateless access token are snapshots: changing an account in the database does
not rewrite already-issued tokens.

Keep access tokens short-lived and revoke refresh sessions after privilege changes. High-risk systems
may additionally check a security stamp, authorization version, or online policy store. That improves
revocation speed but adds availability and latency coupling to every request.

The sample disables inbound claim mapping and explicitly uses `role` and `scope`. A consistent claim
vocabulary prevents silent mapping differences between issuer, middleware, policies, and tests.

## Named Policies

`AuthorizationPolicies.ProfileRead` requires the `profile.read` scope claim. The `/auth/me` endpoint
uses that named policy rather than inspecting claims inside the handler. `Administrator` requires the
administrator role.

Named policies provide one review location and make endpoint intent readable. Avoid magic policy
strings distributed across controllers. Multiple requirements inside one policy are AND conditions;
multiple handlers for one requirement can provide OR behavior when any handler succeeds it.

## Challenge Versus Forbid

- `401 Unauthorized` means authentication did not produce an acceptable principal. Despite the HTTP
  name, it is an authentication challenge.
- `403 Forbidden` means the principal authenticated but did not satisfy authorization.

Do not redirect API clients to a login HTML page. Bearer-protected APIs should return the appropriate
status without leaking sensitive policy internals. Integration tests explicitly lock the `401`,
`403`, and `200` distinction.

## Resource-Based Authorization

An endpoint cannot decide ownership from `{ownerId}` in a route or request body because the caller
controls those values. Load the authoritative resource first, then call `IAuthorizationService` with
the principal, resource, and operation.

The document handler implements these rules:

| Operation | Owner | Other member | Administrator |
|---|---:|---:|---:|
| Read draft | Allow | Deny | Allow |
| Read published | Allow | Allow | Allow |
| Update | Allow | Deny | Allow |
| Publish | Allow | Deny | Allow |

The handler succeeds a requirement only when a rule allows access. It does not call `Fail` for an
ordinary non-match because another registered handler might legitimately satisfy the same requirement.
Explicit `Fail` is reserved for conditions that must override every alternative, such as a known
account ban in a carefully designed policy.

## Existence Disclosure

This sample returns `404` when a document is absent and `403` when it exists but is inaccessible, so
learners can observe the two paths. In systems where identifiers or existence are sensitive, return
`404` for both and ensure timing, caches, and logs do not reintroduce the distinction. This is a threat
model and product decision, not a universal status-code rule.

## Handler and Data Boundaries

Authorization handlers should be deterministic and side-effect free where possible. Load only the
state required for the decision, honor cancellation in upstream resource loading, and avoid writing
audit state from multiple handlers that may execute in unspecified order. Emit the final decision at
an appropriate boundary without logging sensitive claims or tokens.

Do not rely only on UI visibility. Every server endpoint and background command must enforce the same
authority. Database row-level security can add defense in depth but does not replace application
policy clarity.

## Implementation Map

| Concern | Code |
|---|---|
| Closed role and permission vocabulary | `Domain/Users/UserAccount.cs` |
| Authoritative resource state | `Domain/Documents/LearningDocument.cs` |
| Resource persistence port | `Application/Abstractions/ILearningDocumentRepository.cs` |
| Claim issuance | `Infrastructure/Tokens/JwtAccessTokenIssuer.cs` |
| Named policies | `Api/Authorization/AuthorizationPolicies.cs` |
| Owner/admin operation rules | `Api/Authorization/DocumentAuthorizationHandler.cs` |
| Protected HTTP boundaries | `Api/Program.cs` |
| Challenge/forbid/resource tests | `IntegrationTests/AuthorizationTests.cs` |

## Common Pitfalls

- Checking `User.Identity.IsAuthenticated` and assuming the operation is allowed.
- Trusting role, owner, or permission fields from a request body.
- Using mutable email as the ownership identifier instead of stable `sub`.
- Encoding every business rule as a role and creating role explosion.
- Scattering claim-string comparisons through endpoints.
- Forgetting that access-token claims remain valid until token expiration.
- Returning `401` when an authenticated user lacks permission.
- Authorizing before loading authoritative resource state.
- Caching a response without including authorization dimensions in the cache key.

## Exercises

1. Add a `document.review` permission and a reviewer handler without granting update ownership.
2. Introduce an authorization-version claim and reject tokens older than the stored account version.
3. Change inaccessible-document behavior to conceal existence and test both response and timing budget.
4. Add a second handler that lets a delegated editor update one document until an expiry time.
5. Test authorization inside a background command where no `HttpContext` exists.

## References

- [Policy-based authorization in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0)
- [Resource-based authorization in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resource-based?view=aspnetcore-10.0)
- [Claims-based authorization in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims?view=aspnetcore-10.0)

## Navigation

- Previous: [Refresh-token rotation, replay detection, and revocation](04-refresh-token-rotation-replay.md)
- Next: [OAuth 2.0, OpenID Connect, and provider boundaries](06-oauth2-openid-connect-provider-boundaries.md)
