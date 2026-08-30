# Contributing

Thank you for helping improve this learning-oriented .NET knowledge base. Contributions should make a
concept more accurate, executable, approachable, or operationally honest.

## Prerequisites

- Git;
- the .NET 8 and .NET 10 SDKs/runtimes;
- PowerShell 7 (`pwsh`) for repository validation scripts;
- Visual Studio 2026+, Rider, VS Code, or the .NET CLI.

The root `global.json` selects the .NET 10 SDK. Earlier phases continue to target .NET 8 and therefore
also require the .NET 8 runtime to execute their tests.

## Workflow

1. Fork or fetch the repository and branch from `develop`.
2. Use `feature/`, `bugfix/`, `docs/`, or `chore/` plus a short kebab-case description.
3. Keep one conceptual change per commit and follow `COMMIT_MESSAGE_GUIDELINES.md`.
4. Add executable specifications when behavior changes.
5. Update the relevant article and metadata when a public concept or workflow changes.
6. Run the complete local validation before opening a pull request.

## Build and Test

```powershell
dotnet tool restore
dotnet restore learn-dotnet.slnx --locked-mode
dotnet format whitespace learn-dotnet.slnx --verify-no-changes --no-restore
dotnet build learn-dotnet.slnx --configuration Release --no-restore
dotnet test learn-dotnet.slnx --configuration Release --no-build
```

Run repository quality gates:

```powershell
./scripts/Test-ArticleMetadata.ps1
./scripts/Test-MarkdownLinks.ps1
./scripts/Test-MarkdownQuality.ps1
./scripts/Test-SolutionInventory.ps1
./scripts/Test-PackageVulnerabilities.ps1
```

## Dependency Changes

Package versions belong in `Directory.Packages.props`; individual project files declare package usage
without versions. Keep related Microsoft platform packages on compatible versions. Include restore,
build, test, and vulnerability-audit output in the pull request.

When an intentional dependency change updates the graph, run `dotnet restore learn-dotnet.slnx
--force-evaluate` and commit the resulting `packages.lock.json` changes. Normal validation uses
`--locked-mode` and fails when project declarations and lock files disagree.

Do not upgrade transitive packages blindly. Prefer updating the owning top-level dependency and review
release notes for breaking behavior.

## Documentation Contract

Publishable articles live under a phase `docs/` directory and use the front matter defined in
`docs/article-metadata-schema.md`. Preserve stable slugs. Add primary official references for
version-sensitive claims, and keep examples synchronized with executable code.

Use English for repository content. Explain why a pattern is safe, where it fails, and which behavior
depends on a database, operating system, runtime, or external service.

## EF Core Migrations

Migrations are reviewed source-controlled schema history. Generate them with the pinned local
`dotnet-ef` tool, inspect both operations and model snapshot, run migration tests, and never rewrite an
already-deployed migration merely to make history look cleaner.

## Pull Request Expectations

- Clear problem statement and scope.
- Explanation of important design decisions and deferred boundaries.
- Linked issue when one exists.
- Exact validation commands and results.
- Documentation, migration, security, and compatibility impact.
- No generated output, secrets, local databases, or unrelated formatting changes.
