---
title: "Phase 10 — Architecture and Best Practices"
description: "Build an enrollment module with explicit business invariants, application boundaries, concurrency contracts, and architecture decisions."
phase: 10
status: in-progress
target-framework: net10.0
prerequisites: [phase-09-auth]
previous-phase: ../09-auth/README.md
next-phase: ../11-testing/README.md
---

# Architecture and Best Practices

Architecture is the set of decisions that makes a system understandable and changeable under real
constraints. This phase develops a course-enrollment module, using limited seats and concurrent requests
to expose decisions that cannot be answered by folder names alone.

## Learning Outcomes

By the end of the phase, explain dependency direction, distinguish application services from domain
behavior, organize features within layers, select purpose-specific persistence ports, separate commands
and queries, and justify modular monolith versus service boundaries with evidence. Trace validation,
authorization, transactions, idempotency, and concurrency across the complete request path.

## Study Path

| Order | Article | Current scope |
|---:|---|---|
| 0 | [Roadmap](docs/00-roadmap.md) | scenario, delivery sequence, and explicit remaining slices |
| 1 | [Dependency direction and project boundaries](docs/01-dependency-direction-project-boundaries.md) | implemented project graph and composition root |
| 2 | [Aggregates, use cases, and concurrent enrollment](docs/02-aggregates-use-cases-concurrent-enrollment.md) | immutable aggregate, command handler, compare-and-save adapter |
| 3 | Queries, DTOs, and validation | planned: read projection and transport contracts |
| 4 | Persistence, transactions, and idempotency | planned: EF Core adapter and durable write semantics |
| 5 | Modules and domain events | planned: bounded contexts and explicit integration contracts |
| 6 | HTTP boundaries and API versioning | planned: authenticated host, error mapping, version evolution |
| 7 | Architecture decisions and completion audit | planned: enforceable boundaries and coverage matrix |

## Project Structure

```text
10-architecture/
├── docs/
├── src/
│   ├── Learning.Architecture.Domain/           # Business rules and immutable aggregate states
│   ├── Learning.Architecture.Application/      # Feature handlers and persistence ports
│   ├── Learning.Architecture.Infrastructure/   # Concrete adapters
│   └── Learning.Architecture.ConsoleApp/       # Executable composition root
├── tests/
│   └── Learning.Architecture.Tests/            # Domain, handler, and adapter specifications
└── 10-architecture.slnx
```

Features remain visible inside each project. Project boundaries express compile-time dependencies;
feature folders express the use cases a reader is looking for. Introduce another project only when it
protects a meaningful boundary, not merely to mirror a diagram.

## Run and Verify

From the repository root:

```powershell
dotnet restore 10-architecture/10-architecture.slnx --locked-mode
dotnet run --project 10-architecture/src/Learning.Architecture.ConsoleApp
dotnet test 10-architecture/10-architecture.slnx --configuration Release
```

The console demonstrates successful enrollment, a repeated request for the same learner, and a full
offering. Tests additionally exercise immutable snapshots, racing saves, explicit conflicts, missing
resources, invalid capacities, and cancellation.

## Current Boundary

The first slice implements a complete application use case through an in-memory adapter. It does not
yet expose HTTP, persist across restarts, implement distributed transactions, or integrate identity.
Those are tracked delivery slices rather than implied production capabilities. Phase 08 supplies the
persistence foundation and Phase 09 supplies the authorization foundation for the next adapters.
