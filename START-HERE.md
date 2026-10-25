# Start Here

Start with Phase 01 even if the architecture examples look more interesting. This repository is a
course made of independent samples, not a single application to deploy.

## Prepare your machine

Install Git, the .NET 10 SDK (including its runtime), and PowerShell 7. Use the .NET CLI,
VS Code with C# Dev Kit, Rider, or Visual Studio 2026 or later for the root `.slnx` solution.
Windows PowerShell 5.1 (`powershell`) is different from PowerShell 7 (`pwsh`).

Run these commands from the repository root in PowerShell 7:

```powershell
git switch develop
dotnet --list-sdks
dotnet --list-runtimes
dotnet --version
```

The root `global.json` selects .NET SDK 10. All maintained phases inherit that selection and target
.NET 10, including when your working directory is inside a phase. A solution path does not change
the current working directory or SDK selection. Legacy projects have their own historical policies.

Keep the installed .NET 10 SDK/runtime patched. The completed runtime migration is recorded in the
[development plan](docs/repository-audit-and-development-plan.md). See the
[official support policy](https://dotnet.microsoft.com/en-us/platform/support/policy).

## Complete your first lesson

Stay at the repository root. You do not need Docker, a database server, cloud credentials, or the
complete workspace build for your first lesson.

```powershell
dotnet restore 01-csharp-basics/01-csharp-basics.slnx --locked-mode
dotnet build 01-csharp-basics/01-csharp-basics.slnx --configuration Release --no-restore
dotnet run --project 01-csharp-basics/src/CSharpBasics.ConsoleApp --configuration Release --no-build
dotnet test 01-csharp-basics/tests/CSharpBasics.Tests --configuration Release --no-build
```

The console prints `C# FUNDAMENTALS DEMO RUNNER` and sections for variables, control flow, methods,
collections, strings, nullability, and memory. The test command should report no failures.
Timings and allocation numbers can vary by machine.

Read the [Phase 01 roadmap](01-csharp-basics/docs/00-roadmap.md), then its first topic. Predict an
example's output, run it, change one input, and explain the result. Exercise projects already contain
reference implementations: attempt the problem in your own branch or playground before comparing.
Passing existing tests alone does not mean you have mastered the topic.

## Choose the next step

| Your goal | Next action |
|---|---|
| New to C# | Follow Phase 01 → 02 → 03 in the [course index](README.md#repository-status) |
| Understand performance and algorithms | Continue through Phase 04 → 05 → 06; benchmarks are optional |
| Build APIs | Complete the foundations, then Phase 07 → 08 → 09 → 10 |
| Study design patterns | Finish OOP and architecture, then use the [10B lesson catalog](10-architecture/design-patterns/README.md) |
| Learn to design tests | Follow the [Phase 11 testing roadmap](11-testing/docs/00-roadmap.md) after the earlier phases |
| Contribute a change | Read [Contributing](CONTRIBUTING.md), then run `pwsh -NoProfile -File scripts/Validate-Repository.ps1` |
| Find unfinished material | Read the [audit and delivery plan](docs/repository-audit-and-development-plan.md) |

`Completed` in the course index means the scoped lesson set is delivered, not that the subject is
exhausted or that the sample is ready for an internet-facing production deployment. Phase 10B has
3 of 23 GoF patterns. Phase 11 is in progress with runnable unit foundations; Phases 12–15 are planned
and their folders do not yet exist.

## Find the right files

- `README.md` in each phase: outcomes, prerequisites, runnable commands, and navigation.
- `docs/`: explanations, trade-offs, and links to the matching code.
- `src/`: runnable applications and supporting libraries; select an application for `dotnet run`.
- `tests/`: executable behavior contracts, including boundary and failure cases.
- `exercises/`: practice and reference solutions where present; `benchmarks/`: optional measurements.
- `legacy/`: archived material, excluded from the maintained solution and validation.
- `artifacts/`, `bin/`, and `obj/`: generated local output; never edit or commit them.

The root solution is the complete workspace. Individual phase solutions are easier to navigate while
learning. Phase 01 also retains a historical `.sln`; prefer `.slnx` to follow the maintained workflow.

## Resolve common setup problems

| Symptom | Check |
|---|---|
| SDK requested by `global.json` cannot be found | Run `dotnet --list-sdks` and check your current directory; install the requested major SDK |
| A maintained test requests `Microsoft.NETCore.App 8.0` | Check out the current `develop` branch and rebuild without `--no-build`; an old build or archived project may still target .NET 8 |
| `pwsh` is not recognized | Install PowerShell 7 and reopen the terminal |
| Locked restore reports a dependency mismatch | On an unchanged checkout, fetch the complete branch; for intentional dependency edits, follow the lock update procedure in Contributing |
| `dotnet run` says the project is a library | Select the phase's console or API project, rather than its examples library |
| API requires configuration or authentication | Follow that phase's README and safe local setup; never put secrets into tracked settings |

Before moving on, explain the lesson without reading it, make a small change, and add or adapt one
boundary assertion. Use the next-phase link in that phase's README when ready.
