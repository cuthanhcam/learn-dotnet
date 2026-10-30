---
title: "Phase 11 — Architecture and Best Practices"
description: "Study application and module boundaries through an enrollment system with explicit contracts, dependency rules, and reliability trade-offs."
phase: 11
status: in-progress
target-framework: net10.0
prerequisites: [phase-09-auth]
previous-phase: ../10-design-patterns/README.md
next-phase: ../12-testing/README.md
---

# Architecture and Best Practices

Architecture is the set of decisions that makes a system understandable and changeable under real
constraints. This phase develops a course-enrollment module, using limited seats and concurrent requests
to expose decisions that cannot be answered by folder names alone.

## Scope and Status

The enrollment baseline delivers application layers, module ownership, aggregate consistency,
transactions, request recovery, authenticated entry points, ADRs, and 110 specifications. Read the
[evidence audit](docs/09-completion-audit.md) for tested guarantees and operational extensions.

Class/object collaboration has its own [Design Patterns phase](../10-design-patterns/README.md).
The delivered pattern foundations are useful background; completing the entire 23-pattern reference
catalog is not a gate for running this architecture example.

## Learning Outcomes

By the end of the phase, explain dependency direction, distinguish application services from domain
behavior, organize features within layers, select purpose-specific persistence ports, separate commands
and queries, and justify modular monolith versus service boundaries with evidence. Trace validation,
authorization, transactions, idempotency, and concurrency across the complete request path.

## Study Path

For a file-by-file reading route, start with the [guided source tour](docs/12-guided-source-tour.md).
Then use [design principles in source](docs/13-design-principles-in-source.md) to connect SOLID,
DRY, KISS, and YAGNI to real decisions rather than memorizing names.

| Order | Article | Current scope |
|---:|---|---|
| 0 | [Roadmap](docs/00-roadmap.md) | scenario, delivery sequence, and explicit remaining slices |
| 1 | [Dependency direction and project boundaries](docs/01-dependency-direction-project-boundaries.md) | implemented project graph and composition root |
| 2 | [Aggregates, use cases, and concurrent enrollment](docs/02-aggregates-use-cases-concurrent-enrollment.md) | immutable aggregate, command handler, compare-and-save adapter |
| 3 | [Queries, DTOs, mapping, and validation](docs/03-queries-dtos-mapping-validation.md) | bounded reads, SQL projection, and validation ownership |
| 4 | [Persistence, transactions, and idempotency](docs/04-persistence-transactions-idempotency.md) | conditional updates, receipts, rollback, and recovery |
| 5 | [Modules, events, outbox, and inbox](docs/05-modules-events-outbox-inbox.md) | separate ownership, duplicate delivery, and failure windows |
| 6 | [HTTP security, errors, and version evolution](docs/06-http-security-errors-versioning.md) | real JWT validation, self-enrollment policy, and v1 contracts |
| 7 | [Architecture decisions and fitness checks](docs/07-decisions-fitness-maintainability.md) | project graph enforcement, trade-offs, and two ADRs |
| 8 | [Common pitfalls](docs/08-common-pitfalls.md) | diagnosis and production adoption checklist |
| 9 | [11 evidence audit](docs/09-completion-audit.md) | enrollment objective-to-evidence matrix and Phase 12 handoff |

## Project Structure

```text
11-architecture/
├── docs/
├── src/
│   ├── Learning.Architecture.Domain/           # Business rules and immutable aggregate states
│   ├── Learning.Architecture.Application/      # Feature handlers and persistence ports
│   ├── Learning.Architecture.Infrastructure/   # Dictionary/EF adapters, receipts, outbox
│   ├── Learning.Architecture.Contracts/        # Versioned integration contract; no implementation dependency
│   ├── Learning.Architecture.Notifications/    # Separate schema and deduplicated welcome work
│   ├── Learning.Architecture.ConsoleApp/       # Offline composition root and relational walkthrough
│   └── Learning.Architecture.Api/              # Authenticated HTTP composition root and delivery worker
├── tests/
│   ├── Learning.Architecture.Tests/            # Domain, relational, module, and graph specifications
│   └── Learning.Architecture.IntegrationTests/ # HTTP contracts and real signed-token validation
└── 11-architecture.slnx
```

Features remain visible inside each project. Project boundaries express compile-time dependencies;
feature folders express the use cases a reader is looking for. Introduce another project only when it
protects a meaningful boundary, not merely to mirror a diagram.

This solution contains only the architecture scenario. The separate
[Design Patterns solution](../10-design-patterns/README.md) is not a dependency of enrollment projects.

## Run and Verify

From the repository root:

```powershell
dotnet restore 11-architecture/11-architecture.slnx --locked-mode
dotnet run --project 11-architecture/src/Learning.Architecture.ConsoleApp
dotnet run --project 11-architecture/src/Learning.Architecture.ConsoleApp -- --relational
dotnet test 11-architecture/11-architecture.slnx --configuration Release
```

The basic console demonstrates successful enrollment, a repeated request for the same learner, and a
full offering. The relational walkthrough uses two real SQLite databases to show durable request replay,
outbox dispatch, and one deduplicated welcome work item without requiring an external identity provider.

The architecture track has **110 executable specifications**: 78 domain/application/relational/module/fitness cases
and 32 HTTP/JWT cases. Tests cover immutable snapshots, invalid restoration, racing saves, stale SQL
writes, explicit conflicts, missing resources, validation, cancellation, rollback after mutation,
request-key misuse, delivery acknowledgement loss, bounded batches, and real token validation failures.

## Trace the Complete Scenario

```text
Authenticated learner + Idempotency-Key
  -> v1 endpoint: bind and authorize the caller
  -> durable workflow: inspect receipt, begin local transaction
  -> application handler -> aggregate: decide capacity/membership
  -> EF adapter: update only if expected version still matches
  -> receipt + outbox: commit with the seat mutation
  -> outbox worker: deliver a versioned contract after commit
  -> notifications: deduplicate and persist a local welcome work item
  -> acknowledge source delivery (safe to repeat after interruption)
```

Catalog queries take a separate path: endpoint -> application read handler -> SQL projection -> DTO.
They do not load aggregate membership or expose learner identities.

## Source Organization

Within Application, `Enrollments` contains command/replay contracts and `Offerings` contains catalog
queries/DTOs. `Abstractions` holds the introductory aggregate store port. Infrastructure groups
`Courses`, `Persistence`, and `Messaging` rather than mixing adapters into a generic services folder.
API `Features`, `Security`, and `Hosting` separate transport mapping, identity configuration, and worker
lifecycle. Notifications owns its own DbContext and consumer and references only Contracts among
implementation projects.

Mapping is explicit. The domain never binds HTTP input or EF entities. Persistence restores validated
snapshots; reads project only needed columns; the HTTP response exposes a dedicated enrollment DTO.
Comments explain transaction timing, retry scope, and provider boundaries near the relevant code.

## HTTP Lab

Follow the [HTTP tutorial](docs/06-http-security-errors-versioning.md) to configure an HTTPS OAuth/OIDC
authority and audience. There is no checked-in secret, token issuer, or permissive development-auth
handler. Without valid identity configuration the API intentionally fails startup validation.
The tracked `http` profile uses `http://127.0.0.1:5180`; `https` adds `https://localhost:7180`.
See [local API profiles](../docs/local-api-profiles.md) for the shared conventions.

The anonymous catalog route is `GET /api/v1/offerings`. Self-enrollment uses
`POST /api/v1/offerings/{id}/enrollments`, a GUID `Idempotency-Key`, and a validated token carrying the
lab's `learner_id` mapping and `enrollment.write` scope. Database initialization is local/test-only.

## Current Boundary

The enrollment architecture baseline is implemented, with evidence and limits in the
[architecture audit](docs/09-completion-audit.md). The API uses file-backed SQLite; tests and the offline
console use isolated in-memory SQLite databases. Notification work is prepared, not actually emailed.
Delivery is an in-process, single-worker example with at-least-once recovery, not a broker or distributed
transaction. Multi-worker leases, poison handling, retention, production migrations, multitenancy,
and deployment hardening are explicit extensions—not hidden capabilities.

Phase 08 supplies provider/migration fundamentals, Phase 09 supplies security fundamentals, and Phase 12
now adds coverage interpretation and PostgreSQL contract/transaction tests. Read the two
[architecture decision records](docs/07-decisions-fitness-maintainability.md) before adapting the structure
to a different workload.
