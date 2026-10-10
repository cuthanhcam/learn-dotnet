---
title: "ADR 0001: Feature-Oriented Layers and a Modular Monolith"
description: "Record why the enrollment example combines inward layers with feature folders and keeps notifications as a separately owned module within one deployment."
slug: architecture-adr-feature-oriented-modular-monolith
phase: 10
order: 10
difficulty: advanced
article-type: reference
estimated-reading-minutes: 8
topics: [adr, modular-monolith, layering, feature-folders]
prerequisites: [architecture-decisions-fitness-maintainability]
status: maintained
last-reviewed: 2026-10-10
---

# ADR 0001: Feature-Oriented Layers and a Modular Monolith

## Decision Status

Accepted for the Phase 10 learning workload on 2026-10-10. This is a scoped decision, not a repository-wide
requirement that every phase use the same number of projects.

## Context

The example must teach reusable business rules, explicit application orchestration, replaceable
persistence, and the difference between compile-time references and runtime calls. It must also show
what changes when a second capability owns its own state and failures. The repository prioritizes
readable executable examples over infrastructure ceremony or independent service operations.

## Decision

Use Domain, Application, and Infrastructure projects for enrollment, with feature folders inside each.
Use ConsoleApp and Api as composition roots. Keep Notifications independently implemented behind a
small Contracts project, with its own database. Deploy the example as a modular monolith.

Notifications may depend on the publisher's integration contract but not enrollment domain/application/
infrastructure. The host connects implementations. Fitness tests enforce direct project references
and inspect compiled dependencies.

## Alternatives Considered

| Alternative | Benefit | Reason not selected here |
|---|---|---|
| One project with feature folders | least physical ceremony | does not make dependency boundaries executable at project level |
| Technology-only layers without feature folders | familiar grouping | scatters one use case across generic service/repository buckets |
| Vertical slice per project for every endpoint | very strong physical separation | unnecessary project overhead for a small learning workload |
| Independently deployed services immediately | deployment isolation | introduces transport/operations before the domain boundary is understood |

These alternatives remain legitimate in different constraints. A small production application can
use fewer projects and still preserve disciplined boundaries.

## Consequences

Readers can trace one feature and test the core without constructing an HTTP host. Notifications owns
its persistence and can later gain a transport adapter. The host has implementation references by design.
More project and mapping files exist than in a single-project CRUD app. Shared contracts need ownership
discipline to avoid turning into a shared implementation model.

The notification module is intentionally simple and not split into its own three-layer hierarchy.
Apply that split only if its business model and change pressures justify it.

## Reconsideration Triggers

- Independent deployment or team ownership has measurable value.
- Cross-module change frequency shows the proposed boundary is incorrect.
- The contracts project accumulates unrelated concepts or implementation dependencies.
- A much smaller workload makes project navigation cost exceed its teaching benefit.

## Related Material

- [Architecture decisions and fitness checks](../07-decisions-fitness-maintainability.md).
- [Common web application architectures](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures).
- [ADR guidance](https://learn.microsoft.com/en-us/azure/well-architected/architect-role/architecture-decision-record).
