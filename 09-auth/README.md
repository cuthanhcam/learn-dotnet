---
title: "Phase 09 — Authentication and Authorization"
description: "A security-first learning path through identity, credentials, tokens, sessions, authorization policies, and operational defenses in ASP.NET Core 10."
phase: 9
status: in-progress
target-framework: net10.0
prerequisites: [phase-08-entity-framework-core]
previous-phase: ../08-ef-core/README.md
next-phase: ../10-architecture/README.md
---

# Authentication and Authorization

> Build identity boundaries that make trust, token ownership, authorization decisions, revocation,
> and failure behavior explicit—and know when a dedicated identity provider should own them instead.

## Learning Outcomes

After this phase, you should be able to distinguish authentication from authorization, choose an
appropriate browser or API authentication scheme, hash and verify passwords with supported framework
APIs, validate JWT bearer tokens defensively, rotate refresh tokens, detect replay, express role,
claim, policy, and resource-based authorization, and test both successful and adversarial flows.

## Study Path

| Order | Article | Executable focus |
|---:|---|---|
| 0 | [Roadmap](docs/00-roadmap.md) | threat model, trust boundaries, and study sequence |
| 1 | [Security architecture and project structure](docs/01-security-architecture-project-structure.md) | dependency direction and security ownership |
| 2 | [Identity and credential storage](docs/02-identity-credential-storage.md) | normalized identity, password hashing, and enumeration resistance |
| 3 | [Authentication schemes and JWT validation](docs/03-authentication-schemes-jwt.md) | bearer handler, issuer, audience, lifetime, and signing keys |
| 4 | [Refresh-token lifecycle](docs/04-refresh-token-rotation-replay.md) | hashed storage, rotation, replay detection, and family revocation |
| 5 | [Claims, roles, policies, and resource authorization](docs/05-roles-claims-policies-resource-authorization.md) | least privilege, explicit requirements, owner/admin decisions |
| 6 | [OAuth 2.0, OpenID Connect, and provider boundaries](docs/06-oauth2-openid-connect-provider-boundaries.md) | protocol roles, flows, PKCE, and provider selection |
| 7 | [Browser security: cookies, CSRF, CORS, and token storage](docs/07-browser-security-cookies-csrf-token-storage.md) | BFF, hardened cookies, antiforgery, origins, and browser storage |
| 8 | [Abuse resistance and account lifecycle](docs/08-abuse-resistance-account-lifecycle.md) | partitioned throttling, atomic lockout, recovery, proxy trust, and audit events |
| 9 | [Security testing, operations, and incident response](docs/09-security-testing-operations-incident-response.md) | negative tests, safe audit events, key rotation, telemetry, and response playbooks |
| 10 | Completion audit | coverage matrix and Phase 10 handoff |

## Structure

```text
09-auth/
├── docs/                                  # Article-ready security curriculum
├── src/
│   ├── Learning.Auth.Domain/              # Identity and session invariants
│   ├── Learning.Auth.Application/         # Use cases and technology-neutral ports
│   ├── Learning.Auth.Infrastructure/      # Hashing, token, clock, and persistence adapters
│   └── Learning.Auth.Api/                 # HTTP contracts and composition root
├── tests/
│   ├── Learning.Auth.UnitTests/           # Domain and application specifications
│   └── Learning.Auth.IntegrationTests/    # Real authentication pipeline and HTTP boundaries
└── 09-auth.slnx
```

## Architecture Rules

- Domain code owns identity and session invariants and references no outer project.
- Application code orchestrates use cases through interfaces; it does not know HTTP or token libraries.
- Infrastructure implements cryptography, persistence, and token formats behind application ports.
- API owns dependency injection, authentication handlers, authorization policies, and HTTP contracts.
- Tokens never contain secrets; claims are minimized and treated as a snapshot, not live authority.
- Refresh tokens are high-value credentials: store only digests, rotate on use, and detect replay.
- Examples explain local token issuance for learning, while production guidance prefers a standards-based
  identity provider when the system is acting as an OAuth/OIDC authorization server.

## Run and Test

```powershell
dotnet restore 09-auth.slnx
dotnet build 09-auth.slnx --configuration Release --no-restore
dotnet test 09-auth.slnx --configuration Release --no-build
```

## Status

The project boundary, credential model, adaptive password hashing, atomic registration contract,
enumeration-resistant sign-in, short-lived JWT issuance, strict bearer validation, and refresh-token
rotation with replay-driven family revocation, named permission and role policies, and resource-based
owner/administrator decisions, atomic account lockout, and endpoint-specific rate limiting are
implemented. OAuth/OIDC provider boundaries, browser security, recovery invariants, proxy trust, safe
audit telemetry, key rotation, and incident response are documented; the completion audit follows.
