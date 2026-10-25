---
title: "Repository Audit and Development Plan"
description: "A prioritized delivery plan for onboarding, runtime maintenance, testing, patterns, and operational learning."
slug: repository-audit-development-plan
phase: 0
order: 3
difficulty: reference
article-type: roadmap
estimated-reading-minutes: 12
topics: [roadmap, maintenance, onboarding, testing, devops]
prerequisites: []
status: maintained
last-reviewed: 2026-10-25
---

# Repository Audit and Development Plan

## Scope and evidence

This audit reviews the maintained solution, phase entry points and completion audits, shared build
policy, validation scripts, workflow definitions, and selected source comments. It is not a claim
that every source line has received a fresh security or pedagogical review. Historical `legacy/`
projects remain outside the supported build.

On 2026-10-25 the integrated workspace restores in locked mode and builds in Release with zero
warnings/errors on Windows using SDK 10.0.402. All **588 tests across 15 test projects** pass without
skips, including 70 pattern cases. These are executable test-case counts, not a coverage percentage.
The solution contains 59 maintained projects. The full validation entry point also passes whitespace,
125 article metadata files, 318 local links, 142 Markdown structure checks, solution inventory, and
the NuGet vulnerability audit. `actionlint` accepts all workflow definitions. The Phase 01 console
smoke run matches the onboarding guide. Counts describe this audit snapshot and will change.

Remote settings inspected during the audit show an active main-branch ruleset; `develop` has no
branch protection. This change does not modify repository settings. Hosted Windows/Linux results
must still be verified after push before accepting the promotion PR.

## Findings and immediate corrections

| Finding | Why it matters | Resolution in this audit |
|---|---|---|
| Setup and study instructions spread across root/phase READMEs | A beginner cannot easily choose the first command or interpret the folders | Added [Start Here](../START-HERE.md) with first run, expected output, SDK/runtime distinction, exercises, and troubleshooting |
| Root SDK 10 and nested SDK 8 policies | Running the same command from a different directory can select a different SDK | Documented working-directory behavior and both installed runtimes |
| Phase 01 calls .NET 8 the current LTS baseline | This hides the imminent support deadline | Corrected the claim and prioritized runtime migration |
| README embeds project/article/test totals | Counts drift whenever a lesson changes | Root overview now points to executable inventory and test output |
| Markdown structure gate only traverses phase `docs/` | Phase READMEs and the nested patterns entry point can escape checks | Expanded traversal to maintained phase roots and the new onboarding file |
| Metadata gate depends on shell working directory and silently ignores missing roots | Running outside the repo can appear successful without validating articles | Resolve configured paths against the repository and fail for missing roots |
| Local validation requires many manual commands | Contributors may omit a gate that CI runs | Added [Validate-Repository.ps1](../scripts/Validate-Repository.ps1) and connected CI/VS Code to it |
| CI tests only Linux | This Windows-oriented learning workflow lacks hosted Windows regression evidence | Added Linux/Windows jobs with separate result artifacts |
| String comments encourage casing before comparison and describe ordinal comparison as bytes | Learners may allocate unnecessarily or confuse UTF-16 code units with bytes | Corrected the explanation without changing behavior |
| Feature work is ahead of remote `develop` | Reviewing only `develop` misses delivered architecture and pattern lessons | Integrate the existing feature history and this audit after local validation |

## Delivery backlog

### Runtime migration delivery sequence

Runtime maintenance is split into small branches, each based on the preceding verified `develop`.
Each branch gets its own PR, full local validation, hosted Windows/Linux/security checks, and a merge
commit into `develop` after those checks pass. The existing promotion PR into `main` stays open.

| Sequence | Branch | Scope | Delivery |
|---:|---|---|---|
| 1 | `chore/net10-phases-01-02` | Retarget basics/OOP, regenerate locks, inherit root SDK, update local docs | [PR #86](https://github.com/cuthanhcam/learn-dotnet/pull/86), merged after all checks passed |
| 2 | `chore/net10-phases-03-04` | Retarget core/performance, regenerate locks, inherit root SDK, update local docs | [PR #87](https://github.com/cuthanhcam/learn-dotnet/pull/87), merged after all checks passed |
| 3 | `chore/net10-phases-05-06` | Retarget DSA/async, regenerate locks, inherit root SDK, update local docs | [PR #88](https://github.com/cuthanhcam/learn-dotnet/pull/88), merged after all checks passed |
| 4 | `chore/net10-repository-baseline` | Remove mixed-runtime setup guidance and CI installation, verify one SDK baseline | Baseline finalization in this change |

Dependency versions and teaching implementations remain scoped separately from the runtime migration.
Historical statements such as “.NET 8 introduced TimeProvider” remain valid and are preserved.

At runtime migration completion, all 59 maintained projects targeted `net10.0`; the six phase SDK selectors were removed and
the root selector explicitly disallows prerelease SDKs. CI and CodeQL install SDK 10 only. Setup docs
require a single SDK, and the inventory gate rejects framework drift or nested SDK selectors.
The existing 588 behavior contracts pass after retargeting. SDK selection and console smoke runs were
checked from inside the migrated phases; benchmark catalogs were checked where their runners support
listing. This validates behavior contracts, not a claim that runtime changes have identical performance.
Re-run measurements before comparing numbers across runtimes.

Use one bounded feature branch/PR per deliverable. Each row has observable exit criteria; creating
empty folders or unchecked lists does not complete a phase.

| Priority / sequence | Deliverable | Exit criteria |
|---|---|---|
| P0 — delivered in runtime slices above | Move Phases 01–06 to .NET 10 before 2026-11-10 | Target frameworks, root SDK inheritance, locks, local docs, 588 tests, and Windows/Linux/security checks validated; repository baseline finalization is the last slice |
| P1 — in progress | [Phase 11 testing foundations](../11-testing/README.md) | Unit contracts, deterministic time, explicit doubles, failure/cancellation, and exercises delivered; HTTP integration and fixture lifetime are the next slice |
| P1 — second testing slice | Production database tests | Pick and document one production provider; add isolated Testcontainers fixtures, migration-from-empty and upgrade tests, concurrency/query assertions, cleanup, and a clearly named CI job requiring Docker; SQLite tests remain a distinct fast suite |
| P1 — parallel curriculum track in later PRs | Continue Phase 10B | Deliver the remaining 20 GoF patterns in the sequence of the [approved curriculum](design-patterns-curriculum-plan.md); each gets a baseline, refactor, modern alternative, runner command, focused tests, article, trade-offs, and exercises |
| P2 — after testing foundations | Phase 12 first capstone | One bounded API combining identity, persistence, pagination, idempotency, and authorization; include acceptance criteria and negative HTTP tests rather than copying every architecture pattern |
| P2 — after a runnable capstone | Phase 13 packaging and deployment | Multi-stage image, non-root runtime, `.dockerignore`, health checks, graceful shutdown, Compose, safe example configuration, smoke tests, and documented rollback; CI builds/tests the image; deployment uses a concrete target and scoped credentials |
| P2 — before calling the capstone operational | Phase 15 observability | Structured/redacted logs, trace propagation, metrics, health signals, a reproducible local dashboard, and one tested incident exercise; explain cardinality and sensitive-data boundaries |
| P3 — after measuring the capstone | Phase 14 system design | Document workload and limits; compare cache, queue, retries, backpressure, and scaling choices using measurable experiments and failure scenarios |
| Ongoing | Dependency and automation maintenance | Review the existing bot PRs individually, reconcile the central lock graph, review action release notes, and wait for required hosted checks; never merge a bot branch solely because it is newer |

.NET 8 support ends on 2026-11-10; .NET 10 is supported through 2028-11-14 according to the
[official .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy), checked on
2026-10-25. Retargeting is a separate behavior-affecting change, not an incidental documentation edit.

## Folder and supporting-file decisions

Keep the numbered phase roots. They communicate progression and already have stable links, article
slugs, focused solutions, and tests. A global move into `src/` and `tests/` would scatter each lesson
and create link churn without improving the learning path.

- Keep shared operational references in root `docs/`; phase-specific explanations stay beside their phase.
- Keep GoF implementation under `10-architecture/design-patterns/` and articles under
  `10-architecture/docs/design-patterns/`; the catalog links these deliberately separate views.
- Add `11-testing/`, `12-projects/`, `13-devops/`, `14-system-design/`, and `15-observability/` only
  with their first coherent lesson. Register each real project in the master solution and update the
  root lists in inventory, metadata, Markdown gates, and external-link configuration as appropriate.
- Prefer feature folders within application projects, following Phase 10; introduce layers only when
  the lesson explains ownership and dependency direction.
- Retain the Phase 01 `.sln` as a compatibility artifact for now. Document `.slnx` as the maintained
  entry point; remove the duplicate in a dedicated change after checking IDE consumers.
- Existing license, security policy, contribution guide, changelog, EditorConfig, Git attributes,
  lock files, templates, and automation are present. No extra policy file is needed merely for appearance.
- Add `.dockerignore`, Dockerfiles, Compose, provider fixtures, dashboards, and deployment files with
  the runnable phase that owns them. The current `.gitignore` excludes `.dockerignore`; adjust that
  rule when introducing the packaging lesson so the build-context contract is actually tracked.

## Code, comments, and article standard

Use small examples with observable behavior. Explain a non-obvious invariant, ownership decision,
failure, cancellation rule, or intentional baseline trade-off near the code. Avoid comments that only
repeat the syntax, unsupported performance claims, and phrases such as “always best practice.”
Keep comparison examples when their older or simpler form is the teaching point.

For each new lesson, link the article to a runnable source path and a test path, include prerequisites,
show one normal and one boundary/failure case, provide expected observable output, and explain when
the approach is unsuitable. Exercises should state acceptance criteria and distinguish a prompt from
its reference solution. Use the [article contract](article-metadata-schema.md) for publishable docs.

Do not add abstractions or XML comments to every member solely to make a sample look professional.
The correctness and clarity of the contract matter more than class count or comment volume.

## Release and pipeline gates

Run `pwsh -NoProfile -File scripts/Validate-Repository.ps1` before push. It restores tools and locked
packages, checks whitespace, builds Release, runs tests with coverage output, validates metadata/local
links/Markdown/inventory, and audits NuGet packages. It stops at the first failure.

CI runs the same script on Linux and Windows and uploads each platform's TRX and coverage artifacts.
CodeQL, dependency review, and secret scanning remain separate security checks. External links and
Scorecard remain scheduled checks with their [documented policy](repository-automation.md).
There is no deployment target yet, so a green CI run certifies the learning workspace, not deployment.

Before merging the `develop` → `main` PR, inspect hosted checks and any required branch rules, review
the full branch diff, and confirm no generated output or credentials entered the commit. Do not bypass
protection or auto-merge this promotion. Production provider, Docker, deployment, and monitoring gates
become required when those runnable capabilities are introduced, not as placeholder green jobs.

## Navigation

- [Start Here](../START-HERE.md)
- [Course index](../README.md)
- [Contributing and validation](../CONTRIBUTING.md)
- [Design patterns delivery plan](design-patterns-curriculum-plan.md)
