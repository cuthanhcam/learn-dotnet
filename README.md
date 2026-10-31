# Learn .NET

![Learning](https://img.shields.io/badge/Learning-Journey-blueviolet?logo=dotnet&logoColor=white)
![.NET 10](https://img.shields.io/badge/.NET-10-blueviolet?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET-Core-blueviolet?logo=dotnet&logoColor=white)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)
[![CI](https://github.com/cuthanhcam/learn-dotnet/actions/workflows/ci.yml/badge.svg?branch=develop)](https://github.com/cuthanhcam/learn-dotnet/actions/workflows/ci.yml)
[![CodeQL](https://github.com/cuthanhcam/learn-dotnet/actions/workflows/codeql.yml/badge.svg?branch=develop)](https://github.com/cuthanhcam/learn-dotnet/actions/workflows/codeql.yml)

A progressive C# and .NET 10 learning workspace with runnable examples, focused tests, and guided articles.
Learn fundamentals, web development, design patterns, software architecture, and testing through independent samples.

## Get Started

Install Git, the .NET 10 SDK selected by [global.json](global.json), and PowerShell 7.
Clone this repository, open its root directory, and run your first example:

```powershell
dotnet run --project 01-csharp-basics/src/CSharpBasics.ConsoleApp
```

**Follow [Start Here](START-HERE.md)** for setup, the learning order, and your first tests.
Docker is only required for the PostgreSQL integration tests.

## Explore

- [Learning guide](START-HERE.md) — prerequisites, study order, and commands; each phase README provides its detailed lessons.
- [Development plan](docs/repository-audit-and-development-plan.md) — delivered scope, unfinished work, and next steps.
- [Local API profiles](docs/local-api-profiles.md) — launch commands, ports, and configuration.
- [Contributing](CONTRIBUTING.md) — coding conventions, validation, and the pull request workflow.
- [Phase reorganization](docs/phase-reorganization.md) — path changes for older checkouts.

Open `learn-dotnet.slnx` for the maintained workspace or a phase solution for focused study.
Lessons live alongside `docs/`, `src/`, and `tests/` where applicable. `legacy/` contains historical material outside the maintained build.
The curriculum is still in progress; each lesson documents its scope and limitations.

## Build and Validate

Run from the repository root in PowerShell 7:

```powershell
pwsh -NoProfile -File scripts/Validate-Repository.ps1
```

This gate restores locked dependencies, builds in Release, runs portable tests, and checks repository quality.
For Docker-backed PostgreSQL tests, run `pwsh -NoProfile -File scripts/Test-PostgresProvider.ps1`.
CI covers Windows, Linux, and PostgreSQL; review all pull request checks before merging.

Licensed under [MIT](LICENSE).
