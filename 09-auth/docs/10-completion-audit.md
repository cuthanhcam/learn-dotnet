---
title: "Phase 09 Completion Audit"
description: "Map authentication and authorization outcomes to articles, executable security specifications, honest production boundaries, repository gates, and the architecture handoff."
slug: auth-phase-09-completion-audit
phase: 9
order: 10
difficulty: advanced
article-type: reference
estimated-reading-minutes: 28
topics: [authentication, authorization, audit, security-testing, learning-path]
prerequisites: [auth-security-testing-operations-incident-response]
status: maintained
last-reviewed: 2026-09-02
---

# Phase 09 Completion Audit

Phase 09 is complete as a security-first authentication and authorization foundation. Completion means
the trust boundaries, credential and session invariants, authorization decisions, hostile cases,
browser/protocol choices, and operational responsibilities are documented and backed by executable
evidence where a local sample can be honest.

It does not claim that a compact repository is an identity platform. Production federation, MFA,
passkeys, account recovery delivery, client registration, consent, managed signing keys, persistent
session storage, and global abuse intelligence should normally come from maintained providers and
deployment-specific infrastructure.

## Coverage Matrix

| Learning outcome | Primary article | Executable evidence |
|---|---|---|
| Threat model and dependency ownership | [Security architecture](01-security-architecture-project-structure.md) | architecture reference tests and project references |
| Stable identity and password hashing | [Identity and credential storage](02-identity-credential-storage.md) | `EmailAddressTests`, `PasswordHashServiceTests`, registration tests |
| Strict JWT bearer validation | [Authentication schemes and JWT](03-authentication-schemes-jwt.md) | real API middleware, tampered-token and challenge tests |
| Refresh rotation and replay response | [Refresh-token lifecycle](04-refresh-token-rotation-replay.md) | concurrent rotation and family-revocation tests |
| Roles, permissions, named policies | [Claims, roles, and policies](05-roles-claims-policies-resource-authorization.md) | vocabulary tests and `/auth/me`/administrator tests |
| Object ownership and contextual access | [Resource authorization](05-roles-claims-policies-resource-authorization.md) | owner, other member, published, administrator, `401`/`403` tests |
| OAuth/OIDC and provider selection | [Protocol boundaries](06-oauth2-openid-connect-provider-boundaries.md) | decision rules; provider implementation deliberately external |
| Browser session threat model | [Browser security](07-browser-security-cookies-csrf-token-storage.md) | BFF/cookie/CSRF/CORS operational guidance |
| Lockout and request throttling | [Abuse resistance](08-abuse-resistance-account-lifecycle.md) | manual-time, concurrent lockout, real `429`/`Retry-After` tests |
| Recovery and proxy invariants | [Abuse resistance](08-abuse-resistance-account-lifecycle.md) | explicit implementation boundary and deployment checklist |
| Safe security telemetry | [Security operations](09-security-testing-operations-incident-response.md) | closed event contract, source-generated logging, event assertion |
| Key/secret lifecycle and incidents | [Security operations](09-security-testing-operations-incident-response.md) | rotation sequence, alerts, deployment and response checklists |

## Executable Inventory

```text
Domain/
├── Users/EmailAddress, UserAccount       identity, authority, and atomic lockout invariants
├── Sessions/RefreshSession               digest-only, one-time refresh state
└── Documents/LearningDocument            authoritative resource ownership and publication

Application/
├── Registration and credential sign-in   normalization, generic failures, rehash, lockout
├── Session sign-in and refresh            access issuance, rotation, replay, revocation
├── repository/crypto/token ports          technology-independent use-case boundaries
└── ISecurityEventSink                     closed non-secret operational vocabulary

Infrastructure/
├── ASP.NET Core PasswordHasher adapter    supported adaptive password storage
├── JWT and cryptographic refresh adapters strict claims and opaque 256-bit credentials
└── concurrency-safe in-memory stores      visible learning semantics, not durable production state

API/
├── strict bearer handler and named policies
├── resource authorization handler
├── partitioned credential/session rate limits
└── source-generated structured security-event adapter
```

The phase has six maintained projects: four production layers and two test projects. Its 29 executable
tests comprise 19 unit/domain/application specifications and 10 full HTTP integration tests using the
real ASP.NET Core authentication, authorization, and rate-limiting pipeline.

## Security Invariant Audit

- Normalized identity uniqueness belongs to the authoritative store, not a read-before-write check.
- Passwords are adaptively hashed through a supported framework API and rehashed when required.
- Unknown identities perform dummy verification and share public sign-in failure behavior.
- JWT validation pins issuer, audience, signing key, algorithm, signature, expiry, and zero clock skew.
- Refresh values are random bearer credentials; only SHA-256 digests enter server storage.
- Refresh rotation is atomic; replay revokes the replacement family rather than issuing twice.
- Role and permission values come from a closed server-owned vocabulary.
- Resource authorization loads authoritative state and distinguishes authentication from permission.
- Account failure state is atomic and time-driven; an active lockout is bounded and not attacker-extended.
- Authentication endpoints reject excess work without queuing and return safe retry information.
- Operational events use controlled types and contain no password, token, email, header, or raw message.

## Honest Production Boundaries

| Learning implementation | Production requirement |
|---|---|
| In-memory accounts, documents, and sessions | durable store with constraints, concurrency, encryption/access policy, backup, and cleanup |
| Shared object lockout transition | atomic database update or optimistic concurrency across every replica |
| Process-local rate limiter | coordinated edge/distributed policy with measured fail-open/fail-closed behavior |
| Local symmetric JWT issuer | standards-based provider and managed asymmetric key lifecycle in most systems |
| Configuration-injected signing key | secret manager/workload identity, access audit, overlap rotation, emergency revocation |
| Documented reset/verification contract | digest-stored, purpose-bound, single-use persistent tokens plus protected delivery adapter |
| Documented browser/BFF model | application-specific cookie, antiforgery, CORS, Data Protection, and proxy integration tests |
| Structured application logger | protected centralized audit pipeline, retention, alerts, integrity, and incident ownership |

These boundaries are deliberate. Adding fake email delivery, an in-memory OAuth authorization endpoint,
or a pretend distributed limiter would increase code volume while teaching unsafe production equivalence.

## Verification Evidence

The completion run on 2026-09-02 produced:

- locked restore and Release build for all 43 maintained projects with zero warnings and zero errors;
- 451 passing tests across Phases 01–09, including all 29 Phase 09 tests, with zero failures/skips;
- 105 valid article metadata files and all local Markdown links valid after this audit article;
- master solution inventory validation for all 43 projects;
- NuGet audit with no known vulnerable packages from the configured source;
- Actionlint success for repository workflows;
- Gitleaks history scan across 300 commits with no unsuppressed leaks.

Re-run the main gates from the repository root:

```powershell
dotnet restore learn-dotnet.slnx --locked-mode
dotnet build learn-dotnet.slnx --configuration Release --no-restore
dotnet test learn-dotnet.slnx --configuration Release --no-build --no-restore
./scripts/Test-PackageVulnerabilities.ps1
./scripts/Test-ArticleMetadata.ps1
./scripts/Test-MarkdownQuality.ps1
./scripts/Test-MarkdownLinks.ps1
./scripts/Test-SolutionInventory.ps1
```

GitHub Actions additionally runs CodeQL, dependency review, secret scanning, external-link validation,
scorecard analysis, and repository hygiene controls. External-network checks can transiently fail and
must be investigated rather than bypassed.

## Exit Checklist

- [x] Every Phase 09 study-path entry has article metadata, prerequisites, references, and navigation.
- [x] Authentication, authorization, challenge, and forbid are distinct and exercised through real middleware.
- [x] Credential storage, token validation, refresh rotation, replay, and revocation have negative tests.
- [x] Role, permission, policy, ownership, publication, and administrator decisions are executable.
- [x] OAuth/OIDC, PKCE, provider ownership, browser storage, cookies, CSRF, and CORS are accurately bounded.
- [x] Lockout and throttling address concurrency, timing, proxy trust, fairness, and distributed limitations.
- [x] Recovery token requirements are explicit without presenting an in-memory email flow as production-ready.
- [x] Security events use a closed safe contract and operations include rotation, alert, and incident guidance.
- [x] Phase and repository build, test, documentation, package, workflow, and secret gates pass.

## Handoff to Phase 10

Phase 10 can now study architecture using meaningful security boundaries rather than generic layers. It
should preserve domain-owned invariants, application ports, infrastructure adapters, composition-root
configuration, transactional persistence, explicit authorization at every entry point, and non-secret
operational events.

Architecture patterns should be selected to solve observed coupling, consistency, deployment, or team
problems. They must not hide authentication state in global helpers, move authorization exclusively to
controllers, wrap EF Core in an anemic generic repository, or turn every operation into ceremony.

Continue to Phase 10 — Architecture when that phase is implemented.

## Navigation

- Previous: [Security testing, operations, and incident response](09-security-testing-operations-incident-response.md)
- Next: Phase 10 — Architecture (planned)
