# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]

### Added

- Phase 11 HTTP integration lesson with an offline API, real pipeline tests, Problem Details,
  request-abort propagation, isolated write fixtures, and shared read-only host ownership.
- Phase 11 unit-testing foundations: offline loan-reminder workflow, deterministic time, boundary
  theories, recording doubles, awaited completion, failure/cancellation contracts, and guided exercises.
- Beginner start guide with first-run commands, SDK/runtime troubleshooting, and a prioritized
  repository audit covering runtime migration, Phase 11–15, and the remaining GoF lessons.
- A shared fail-fast PowerShell validation entry point for local, VS Code, and Windows/Linux CI.
- Phase 10B GoF track with focused category solutions, Strategy pricing variants, Factory Method
  report exporters, Adapter shipping integration, 70 specifications, topic-sized branches, and
  diagram-backed articles with modern C# alternatives and explicit production boundaries.
- Phase 10 architecture curriculum with immutable enrollment aggregates, bounded read projections,
  conditional relational writes, durable request receipts, outbox/inbox recovery, independently owned
  notification state, authenticated v1 HTTP endpoints, two ADRs, and 67 executable specifications.
- Repository-wide `.slnx` workspace for all maintained projects.
- Central package management, dependency vulnerability audit, CodeQL, Dependabot, and documentation gates.

### Changed

- Phase 10 in-memory/SQLite stores now share definition/version/cancellation contract tests and reject
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
