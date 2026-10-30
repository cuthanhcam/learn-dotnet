# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]

### Added

- Observer lesson with disposable bounded subscriptions, explicit callback/terminal policies,
  event-based alternative, deterministic demo, and lifecycle/concurrency behavior tests.
- Phase 10 Decorator lesson with independent bounded-retry/required-audit wrappers, ordering and
  cancellation tests, correct baseline, and offline demo. Added a source-based design-principle guide
  and clarified the remaining whole-phase curriculum instead of implying Phase 11 completion.
- Phase 12 PostgreSQL/Docker provider baseline with shared Phase 11 store contracts, independent
  connection writes, atomic rollback/replay checks, disposable fixtures, and dedicated CI evidence.
- Phase 12 coverage and test-quality lesson with fresh per-test-project/module reports for Phases
  11/11, focused diagnosis, mutation exercises, and explicit limits on percentage interpretation.
- Phase 12 HTTP integration lesson with an offline API, real pipeline tests, Problem Details,
  request-abort propagation, isolated write fixtures, and shared read-only host ownership.
- Phase 12 unit-testing foundations: offline loan-reminder workflow, deterministic time, boundary
  theories, recording doubles, awaited completion, failure/cancellation contracts, and guided exercises.
- Beginner start guide with first-run commands, SDK/runtime troubleshooting, and a prioritized
  repository audit covering runtime migration, Phase 12–16, and the remaining GoF lessons.
- A shared fail-fast PowerShell validation entry point for local, VS Code, and Windows/Linux CI.
- Phase 10 GoF track with focused category solutions, Strategy pricing variants, Factory Method
  report exporters, Adapter shipping integration, 70 specifications, topic-sized branches, and
  diagram-backed articles with modern C# alternatives and explicit production boundaries.
- Phase 11 architecture curriculum with immutable enrollment aggregates, bounded read projections,
  conditional relational writes, durable request receipts, outbox/inbox recovery, independently owned
  notification state, authenticated v1 HTTP endpoints, two ADRs, and 67 executable specifications.
- Repository-wide `.slnx` workspace for all maintained projects.
- Central package management, dependency vulnerability audit, CodeQL, Dependabot, and documentation gates.

### Changed

- Standardize tracked API launch profiles and loopback ports across Phases 07/09/11/12, with
  optional HTTPS, explicit configuration requirements, and a shared local-development guide.
- Split Design Patterns into Phase 10, move Architecture to Phase 11 and Testing to Phase 12,
  update navigation/solutions/validation, and replace the long root roadmap with a compact index.
- Align .NET 10 platform/auth packages and the EF tool at 10.0.14, update VSTest tooling to
  18.11.0 and its xUnit adapter to 4.0.1, and regenerate the complete maintained lock graph.
- Enrollment schema constraints quote identifiers for SQLite/PostgreSQL compatibility. Portable
  validation excludes PostgreSQL cases, which run explicitly in their own Docker-backed gate.
- Factory Method export captures formatter metadata once and returns the validated snapshot,
  including when an extension product has a stateful metadata property.
- Phase 11 HTTP mapping recognizes EF-wrapped SQLite busy/locked failures and rejects invalid
  success outcomes, with pipeline tests for retry hints and private diagnostic suppression.
- Phase 11 durable replay rejects malformed receipt results without mutating enrollment state;
  the completion audit now links delivered Phase 12 lessons and distinguishes the unfinished GoF catalog.
- Phase 11 in-memory/SQLite stores now share definition/version/cancellation contract tests and reject
  incompatible immutable metadata; the handler observes cancellation before and after lookup.
- Migrated Phases 01–06 and their exercises, tests, and benchmarks to .NET 10 through separate
  reviewed PRs. All maintained projects inherit the root SDK; CI and setup now require only .NET 10.
- The inventory gate rejects inconsistent target frameworks and nested maintained SDK selectors.
- Markdown structure checks now include phase entry points and fail on missing configured roots;
  article metadata validation resolves paths relative to the repository.
- Clarified .NET 8 support status and string comparison comments for beginners.
- CI now restores, builds, and tests the maintained master solution once and excludes historical code.
- Safe configuration, migration history, and dependency lock files are treated as source-controlled contracts.

## [0.8.0] - 2026-08-30

### Added

- Completed Phase 08 with EF Core 10 modeling, migrations, tracking, queries, relationships,
  concurrency, transactions, outbox, bulk operations, diagnostics, and 48 executable specifications.

## [0.7.0] - 2026-08-16

### Added

- Completed Phase 07 with ASP.NET Core 10 hosting, HTTP contracts, operational policies, background
  services, caching, OpenAPI, and integration testing.

## [0.6.0] - 2026-08-15

### Added

- Completed and audited the theory-focused learning foundation across Phases 01–06.
