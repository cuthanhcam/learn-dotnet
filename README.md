# Learn .NET

[![CI](https://github.com/cuthanhcam/learn-dotnet/actions/workflows/ci.yml/badge.svg?branch=develop)](https://github.com/cuthanhcam/learn-dotnet/actions/workflows/ci.yml)
[![CodeQL](https://github.com/cuthanhcam/learn-dotnet/actions/workflows/codeql.yml/badge.svg?branch=develop)](https://github.com/cuthanhcam/learn-dotnet/actions/workflows/codeql.yml)

A progressive C# and .NET 10 learning workspace with runnable examples, focused tests, and guided articles.

**New here? Read [Start Here](START-HERE.md)** for setup and your first lesson.
Use [Contributing](CONTRIBUTING.md) for validation and [the development plan](docs/repository-audit-and-development-plan.md) for unfinished work.

## Course Index

| Phase | Topic | Status |
|---:|---|---|
| 01 | [C# Fundamentals](01-csharp-basics/README.md) | Delivered |
| 02 | [Object-Oriented Programming](02-oop/README.md) | Delivered |
| 03 | [Core .NET](03-core-dotnet/README.md) | Delivered |
| 04 | [Memory and Performance](04-memory-performance/README.md) | Delivered |
| 05 | [Data Structures and Algorithms](05-dsa/README.md) | Delivered |
| 06 | [Async and Concurrency](06-async-concurrency/README.md) | Delivered |
| 07 | [ASP.NET Core](07-aspnet-core/README.md) | Delivered |
| 08 | [Entity Framework Core](08-ef-core/README.md) | Delivered |
| 09 | [Authentication and Authorization](09-auth/README.md) | Delivered |
| 10 | [Design Patterns](10-design-patterns/README.md) | In progress: 4 of 23 GoF patterns |
| 11 | [Software Architecture](11-architecture/README.md) | Enrollment baseline delivered |
| 12 | [Testing](12-testing/README.md) | Foundations, coverage, and PostgreSQL baseline delivered; schema evolution/audit remain |
| 13–16 | Projects, DevOps, system design, observability | Planned |

Each phase README owns its detailed study path, prerequisites, commands, and limits.
Design Patterns covers object collaboration; Architecture covers application/module boundaries.
Delivered lessons cover their stated scope and do not certify production readiness.
See the [phase reorganization note](docs/phase-reorganization.md) if you use an older checkout or saved path.

## Open and Validate

Open `learn-dotnet.slnx` for the maintained workspace, or a phase solution for a focused view.
All maintained projects use .NET 10 and inherit the root SDK selection.

```powershell
dotnet restore learn-dotnet.slnx --locked-mode
dotnet build learn-dotnet.slnx --configuration Release --no-restore
pwsh -NoProfile -File scripts/Validate-Repository.ps1
```

The validation script runs portable tests and repository checks. PostgreSQL tests have a separate
Docker-backed command: `pwsh -NoProfile -File scripts/Test-PostgresProvider.ps1`.
CI runs portable Windows/Linux jobs and the PostgreSQL job; inspect all checks before merging.

Phase folders keep lessons beside their `docs/`, `src/`, and `tests/` where applicable.
`legacy/` is historical material outside the maintained build. Generated `bin/`, `obj/`, and `artifacts/`
are local output and must not be committed.

Licensed under [MIT](LICENSE).
