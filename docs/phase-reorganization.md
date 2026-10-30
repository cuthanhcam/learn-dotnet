---
title: "Phase Reorganization: Patterns, Architecture, and Testing"
description: "Map old paths and numbers to the independent Design Patterns, Architecture, and Testing phases and explain command and navigation changes."
slug: phase-reorganization-patterns-architecture-testing
phase: 0
order: 4
difficulty: reference
article-type: reference
estimated-reading-minutes: 6
topics: [curriculum, migration, navigation, maintenance]
prerequisites: []
status: maintained
last-reviewed: 2026-10-30
---

# Phase Reorganization

Design Patterns studies class/object collaboration. Software Architecture studies application/module
boundaries and system-level trade-offs. The catalog now has its own phase before architecture;
the earlier combined 10A/10B placement is historical context rather than the current structure.

## Path and Number Map

| Previous location | Current location |
|---|---|
| `10-architecture/design-patterns/` | [10-design-patterns/](../10-design-patterns/README.md) |
| `10-architecture/docs/design-patterns/` | [10-design-patterns/docs/](../10-design-patterns/docs/00-roadmap.md) |
| Remaining `10-architecture/` | [11-architecture/](../11-architecture/README.md) |
| `11-testing/` | [12-testing/](../12-testing/README.md) |
| Planned 12 Projects | Planned 13 Projects |
| Planned 13 DevOps | Planned 14 DevOps |
| Planned 14 System Design | Planned 15 System Design |
| Planned 15 Observability | Planned 16 Observability |

Phases 01–09 keep their paths and numbers. Planned phases do not gain empty folders. Existing C#
namespaces, assembly names, behavior, and dependency versions stay unchanged by this move.

## Solutions and Commands

```powershell
dotnet test 10-design-patterns/10-design-patterns.slnx --configuration Release
dotnet test 11-architecture/11-architecture.slnx --configuration Release
dotnet test 12-testing/12-testing.slnx --configuration Release --filter "Category!=Postgres"
pwsh -NoProfile -File scripts/Measure-PhaseCoverage.ps1 -Phase 10
pwsh -NoProfile -File scripts/Measure-PhaseCoverage.ps1 -Phase 11
pwsh -NoProfile -File scripts/Measure-PhaseCoverage.ps1 -Phase 12
pwsh -NoProfile -File scripts/Test-PostgresProvider.ps1
```

The root solution stays `learn-dotnet.slnx`; reopen it or the renamed phase solution if an IDE retains
an old project path. Coverage defaults to Phase 12 and uses the new phase numbers. The PostgreSQL
runner still provisions an isolated fixture and exercises the architecture adapter through Phase 12.

## Documents and Saved Links

Phase README navigation, article phase/order metadata, project references, test source-path checks,
validation roots, coverage settings, and PR path labels follow the new layout. The pattern articles
now use a compact 0–6 reading sequence matching the guided path. No pattern article requires the later
architecture audit as a prerequisite.

Published article `slug` identifiers remain stable. In particular,
`architecture-phase-10-completion-audit` is a historical identifier for the architecture evidence
audit now in Phase 11. Its metadata and displayed title use the current phase. Old filesystem paths
and old solution filenames are not maintained as duplicate source trees; use the mapping above.
Historical Git branch names and PR numbers remain delivery evidence, not current directory names.

The root README is a compact course index and quick start. Detailed topic lists belong in phase
READMEs and roadmaps, avoiding multiple copies of the same curriculum. `NuGet.config` remains the
minimal package-source configuration.

## Navigation

- [Course index](../README.md#course-index)
- [Start Here](../START-HERE.md)
- [Curriculum blueprint](design-patterns-curriculum-plan.md)
