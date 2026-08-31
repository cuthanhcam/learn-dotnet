---
title: "Security Architecture and Project Structure"
description: "Design authentication code around trust boundaries, dependency direction, feature ownership, and testable security invariants."
slug: auth-security-architecture-project-structure
phase: 9
order: 1
difficulty: intermediate
article-type: concept
estimated-reading-minutes: 22
topics: [security-architecture, project-structure, dependency-inversion, threat-modeling]
prerequisites: [auth-authorization-roadmap]
status: maintained
last-reviewed: 2026-08-31
---

# Security Architecture and Project Structure

## Why Structure Matters More in Security Code

Authentication code converts untrusted evidence into a trusted `ClaimsPrincipal`. A misplaced
dependency can let HTTP details leak into domain rules, let storage shape dictate security behavior,
or allow an endpoint to mint claims without the application use case enforcing account state.
Structure does not create security by itself, but it makes authority and review boundaries visible.

## Dependency Direction

```text
HTTP request
    │
    ▼
Learning.Auth.Api ───────────────┐
    │ composition and policies   │
    ▼                            ▼
Learning.Auth.Application ◄── Learning.Auth.Infrastructure
    │ use cases + ports          │ adapters
    ▼                            │
Learning.Auth.Domain ◄───────────┘
    identity and session invariants
```

The API and Infrastructure projects are peers at the outside edge. API selects implementations;
Infrastructure does not call endpoints. Application depends only on Domain. Domain has no project
dependency and remains testable without ASP.NET Core, a database, or a token library.

## Responsibilities

| Project | Owns | Must not own |
|---|---|---|
| Domain | account state, identity invariants, refresh-session state transitions | HTTP, EF Core, JWT serialization |
| Application | registration, sign-in, refresh, revoke use cases and ports | framework handlers, SQL, signing implementation |
| Infrastructure | password hasher adapter, token issuer, persistence, time/random adapters | endpoint decisions, business authorization |
| API | contracts, validation, DI, authentication schemes, authorization policies | credential algorithms, persistence rules |

## Organize by Feature Inside Each Boundary

Layers answer dependency questions; feature folders answer navigation questions. For example,
`Application/SignIn` keeps its command, result, validator, and handler together, while shared ports
such as `IUserRepository` remain under a small `Abstractions` area. Avoid generic `Services` folders:
their names do not reveal ownership or behavior.

## Security Invariants to Make Executable

- A normalized email maps to at most one account.
- Password verification uses a supported adaptive password hasher and can request rehashing.
- Authentication failure does not reveal whether an account exists.
- Disabled or locked accounts cannot receive new sessions.
- Access tokens use a short lifetime and strict validation parameters.
- A refresh token is accepted at most once; reuse revokes its token family.
- Authorization reads stable identifiers and authoritative resource state.
- Secrets and raw credentials never enter logs, metrics labels, URLs, or exception messages.

## Why Earlier Phases Keep Their Current Shape

Phase 07 isolates ASP.NET Core behaviors in one API so middleware, hosting, routing, caching, and
testing remain easy to trace. Phase 08 isolates relational behavior in one persistence library so SQL
and `DbContext` semantics remain visible. Splitting either retroactively into four layers would add
navigation cost without teaching its primary topic. Phase 09 introduces stronger boundaries because
multiple security responsibilities genuinely evolve independently and Phase 10 will compare this
approach with layered, clean, vertical-slice, and modular-monolith alternatives.

## Review Checklist

- Can a reviewer locate every credential and token sink quickly?
- Does each project reference point inward?
- Can domain and application behavior run without HTTP or infrastructure?
- Are authentication and authorization failures tested through the real middleware pipeline?
- Are concurrency and replay rules enforced by an atomic persistence operation, not only by code order?
- Does documentation distinguish a teaching implementation from a production identity provider?

## References

- [ASP.NET Core authentication overview](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-10.0)
- [Policy-based authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0)
- [Resource-based authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resource-based?view=aspnetcore-10.0)

## Navigation

- Previous: [Authentication and authorization roadmap](00-roadmap.md)
- Next: Identity and credential storage (planned)
