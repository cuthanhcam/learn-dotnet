---
title: "Phase 10 Completion Audit and Testing Handoff"
description: "Map each architecture learning objective to source, executable evidence, articles, and clearly stated limits before continuing to the testing phase."
slug: architecture-phase-10-completion-audit
phase: 10
order: 9
difficulty: reference
article-type: reference
estimated-reading-minutes: 18
topics: [architecture, completion-audit, testing, production-readiness]
prerequisites: [architecture-common-pitfalls]
status: maintained
last-reviewed: 2026-10-10
---

# Phase 10 Completion Audit and Testing Handoff

## Completion Means the Learning Scope Is Executable

The Phase 10 architecture learning scope is implemented through a bounded enrollment scenario, a
relational adapter, two separately owned modules, and authenticated HTTP entry points. Completion does
not mean this repository is a deployable training SaaS or that every production concern has been solved.
The articles explicitly distinguish tested guarantees from provider/operational extensions.

No Phase 11 project is created by this audit. Its handoff is a list of testing capabilities to build next.

## Objective-to-Evidence Matrix

| Roadmap objective | Implementation | Executable evidence | Article |
|---|---|---|---|
| Layered/clean dependency direction | Domain, Application, Infrastructure, hosts | assembly and declared project graph checks | 01, 07 |
| Purpose-specific repository/store | `ICourseOfferingStore`, dictionary and EF adapters | atomic dictionary race and stale SQL snapshots | 02, 04 |
| Application service | `EnrollLearnerHandler` | duplicate, full, missing, conflict, cancellation | 02 |
| DTOs and mapping | `OfferingSummary`, SQL projection | bounded ordered page; empty tracker; no identity leak | 03 |
| Validation strategies | HTTP shape, application range, domain rules, DB checks | page/key/claim/capacity/restoration tests | 03, 06 |
| CQRS basics | separate write handler/read port, shared database | query and command specifications | 03 |
| DDD introduction | immutable capacity/membership aggregate | snapshot safety and corrupt restoration rejection | 02, 04 |
| Transaction and request recovery | durable workflow, receipt and outbox | replay, changed input, rollback after SQL mutation | 04 |
| Modular monolith and service trade-offs | independent Notifications and Contracts | module reference checks; two database delivery | 05, 07 |
| Integration reliability | versioned outbox and deduplicated welcome work item | lost acknowledgement, schema rejection, cancellation, batch budget | 05 |
| HTTP security and versioning | v1 endpoints, JWT bearer, self-enrollment policy | real signed-token and HTTP contract tests | 06 |
| Feature-based organization and decisions | named feature folders and two ADRs | source graph fitness checks | 07, ADRs |

Article numbers refer to the study path in the [phase README](../README.md). Tests deliberately exercise
failure paths, not just happy-path output. Read the tests alongside the relevant class comments.

## Verification Commands

From the repository root:

```powershell
dotnet restore 10-architecture/10-architecture.slnx --locked-mode
dotnet build 10-architecture/10-architecture.slnx --configuration Release --no-restore
dotnet test 10-architecture/10-architecture.slnx --configuration Release --no-build
dotnet run --project 10-architecture/src/Learning.Architecture.ConsoleApp --configuration Release --no-build
dotnet run --project 10-architecture/src/Learning.Architecture.ConsoleApp --configuration Release --no-build -- --relational
dotnet format whitespace learn-dotnet.slnx --no-restore --verify-no-changes
./scripts/Test-ArticleMetadata.ps1
./scripts/Test-MarkdownQuality.ps1
./scripts/Test-MarkdownLinks.ps1
./scripts/Test-SolutionInventory.ps1
```

The phase contains nine projects: seven source projects and two test projects. Its current automated
suite has 67 specifications: 45 core/relational/module fitness cases and 22 HTTP/JWT integration cases.
These are discovered test cases, including theory rows, not a code-coverage percentage.

The basic console prints `Enrolled`, `AlreadyEnrolled`, and `Full`. The relational console prints one
enrollment, its replay, one dispatched message, no second dispatch, no available seats, and one welcome
work item. It requires no external identity provider or message broker.

## The Most Important Guarantees

1. Aggregate snapshots cannot be mutated by another reader.
2. A duplicate learner consumes no additional seat, including on a full offering.
3. A stale relational snapshot cannot overwrite the winning version.
4. Seat mutation, receipt, and source delivery intent roll back together on a later local failure.
5. A repeated request key cannot change learner/offering input or create another outbox message.
6. A receiver commit followed by lost acknowledgement can be retried without another local work item.
7. Unknown message schema and interrupted delivery are not silently acknowledged.
8. HTTP self-enrollment derives learner identity from trusted claims and checks write scope.
9. Real bearer validation rejects bad signature, issuer, audience, and expiry.
10. Core/project boundaries are guarded by both compiled and source-level checks.

## What Is Intentionally Not Claimed

- There is no broker, distributed transaction, exactly-once transport, or real email sender.
- The dispatcher is single-worker; there is no lease, poison queue, retry schedule, or ordering guarantee.
- SQLite tests do not substitute for provider-specific production concurrency, deadlock, or migration tests.
- HTTP tests do not contact an actual identity provider or test discovery/key-rollover outages.
- Local `EnsureCreated` is not a production schema migration/deployment pipeline.
- The catalog uses bounded offset pagination, not a multi-request snapshot or large-export API.
- Receipt replay is semantic, not byte-for-byte storage of all HTTP response headers and payloads.
- Multitenancy, admin-on-behalf enrollment, waitlists, withdrawal, billing, and payment compensation are not implemented.
- Operational hardening such as rate limiting, readiness, distributed tracing, retention, and backup drills remains
  a deployment design task; earlier/later phases provide the foundations.

These boundaries should remain visible when examples are reused. Do not remove the limitation comments
while copying an adapter into another project.

## Suggested Phase 11 Work

Start from the existing evidence rather than replacing it with a second unrelated demo:

1. Separate test taxonomy: pure unit, port contract, relational integration, HTTP, environment/system.
2. Add reusable test-data builders without hiding important business inputs.
3. Run persistence contract tests against the intended production database using containers.
4. Inject failures near commit and transport acknowledgement, and assert durable state afterward.
5. Add focused mocks only where observable behavior matters; avoid verifying implementation call order everywhere.
6. Measure coverage while reviewing missing branches, not as proof of architectural correctness.
7. Exercise host shutdown, signing-key rollover, provider outages, and schema upgrade compatibility.
8. Test public contract compatibility and stable machine-readable problem types.

## Self-Assessment

Before moving on, explain without reading the implementation:

- Why the aggregate cannot prevent a stale database write by itself.
- Why `ExecuteUpdate` needs its own version predicate.
- Why two request keys for the same learner are not the same as replaying one key.
- Why a pending outbox row may correspond to an already committed receiver effect.
- Why separate module databases do not imply separately deployed microservices.
- Why an unused project reference still matters to architectural boundaries.
- What changes when enrollment is initiated by a trusted job rather than HTTP.

If any answer is unclear, revisit the linked article and run its focused tests before adding more patterns.

## Navigation

- Previous: [Common architecture pitfalls](08-common-pitfalls.md)
- Return: [Phase 10 study path](../README.md)
- Next: Phase 11 testing (not yet implemented)
