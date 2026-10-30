---
title: "Coverage and Test Quality"
description: "Collect fresh coverage reports, interpret line and branch evidence, and use deliberate defects to evaluate assertions."
slug: testing-coverage-test-quality
phase: 11
order: 4
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 18
topics: [testing, coverage, mutation-testing, test-design]
prerequisites: [testing-http-integration-fixtures]
status: maintained
last-reviewed: 2026-10-30
---

# Coverage and Test Quality

## Start with a Question

A green test says its assertions held for the inputs it supplied. Coverage helps locate code the run
visited. Neither establishes that an untested input, a different provider, or a future change is correct.
Use a missed branch to ask what behavior needs evidence, then choose an assertion that could fail.

The [reminder policy](../src/Learning.Testing.Examples/Loans/ReminderPolicy.cs) is deliberately small:
an unreturned loan is due at or after its deadline. Visiting the comparison does not establish whether
equality is correct. The exact-deadline test supplies that evidence.

## Collect a Fresh Run

From the repository root, with PowerShell 7 and the root .NET SDK installed:

```powershell
pwsh -NoProfile -File scripts/Measure-PhaseCoverage.ps1 -Phase 11
pwsh -NoProfile -File scripts/Measure-PhaseCoverage.ps1 -Phase 10
```

The [helper](../../scripts/Measure-PhaseCoverage.ps1) restores locked packages, builds the selected
phase in Release, and runs each declared test project separately. It uses the existing xUnit v2/VSTest
setup and [collector settings](../coverage.runsettings), not another test platform or package.
Phase 10 means the architecture solution; its separately maintained GoF solutions are outside this run.

Each invocation creates `artifacts/coverage/phase-<number>/<timestamp>-<unique-id>/`. Expect TRX and
Cobertura XML under each test project's directory and a `summary.md` at the run root. The console prints
that path and a table with test project, module, line percentage, and branch percentage. Exact values
can change with source, SDK, and instrumentation; they are not a checked-in promise.

The helper fails on restore/build/test failure, missing reports, or reports without modules. It writes
the summary only after all selected test projects succeed. Failed runs can retain partial diagnostics;
they are not complete reports. It neither deletes previous runs nor scans them for convenient results.
Two invocations use distinct directories; do not run them concurrently against the same build outputs.

## Read the Scope Before the Percentage

| Observation | Reasonable interpretation | Follow-up |
|---|---|---|
| A line was visited | Some test executed instrumented code there | Inspect inputs and assertions |
| A branch was missed | An instrumented outcome was not observed | Find the business condition or defensive path |
| A module is absent | It was not reported in this run | Check references, loading, and collector filters |
| Branch value is `n/a` | The reported module has no instrumented branch lines | Do not describe this as proven branch behavior |
| Tests pass at high coverage | Checked assertions held across exercised code | Try a plausible incorrect implementation |

The helper reports each module from each test project separately. Unit and HTTP suites may both
execute `Learning.Testing.Examples`; adding their percentages double-counts shared source. Averaging
also loses the numerator and denominator. This helper deliberately produces no combined percentage.
For a union report, use a tool that merges compatible coverage records and review its source identity
and denominator rules before publishing the result.

For one raw report, inspect `lines-covered`/`lines-valid` and `branches-covered`/`branches-valid` on the
Cobertura root. Its package rates describe each module. A zero denominator means no measured items,
not evidence of perfect behavior. Generated methods, expression lowering, and defensive branches can
make a compiler-level branch differ from the decision a reader sees in C#.

The include filter limits this lesson to `Learning.Testing.*` and `Learning.Architecture.*`; test
assemblies are excluded. The console entry point is not covered by merely running library tests.
Do not remove hard-to-test application code from the denominator to improve a number. Explain its
boundary and add the appropriate execution or smoke evidence instead.

## Diagnose One Gap at a Time

1. Select a source file and identify the missed condition in the current XML.
2. Write the expected behavior in plain language before adding a test.
3. Choose the narrowest boundary that can prove it: pure policy, service port, SQLite, or HTTP.
4. Make the assertion inspect an outcome or durable state, not only that a method was called.
5. Run the focused project, then recollect coverage and explain what new evidence exists.

For example, the [shared Phase 10 store contracts](../../10-architecture/tests/Learning.Architecture.Tests/Contracts/CourseOfferingStoreContractTests.cs)
run the same behavioral promises against dictionary and SQLite adapters. Their mismatched-metadata
cases establish agreement between implementations. Their coverage does not establish behavior under
physical lock contention, another provider, or a database collation not used by the fixture.

Use project paths for focused tests rather than filtering the entire solution and producing messages
from unrelated assemblies:

```powershell
dotnet test 11-testing/tests/Learning.Testing.UnitTests --configuration Release --filter FullyQualifiedName~ReminderPolicyTests
dotnet test 10-architecture/tests/Learning.Architecture.Tests --configuration Release --filter FullyQualifiedName~CourseOfferingStoreContractTests
```

## Mutation Exercises

Use a temporary branch or worktree, keep one deliberate defect at a time, and restore the original
implementation after each experiment. Never commit or promote the broken implementation.

| Deliberate change | Behavior a useful test should reject |
|---|---|
| Change deadline `>=` to `>` | A loan exactly at its deadline becomes incorrectly not due |
| Ignore `IsReturned` | A returned loan incorrectly triggers a reminder |
| Remove `await` from the sender operation | The workflow can report completion before delivery completes |
| Remove the store's expected-version condition | A stale snapshot can overwrite an accepted enrollment |
| Accept any persisted receipt status | Corrupt stored data can become a fabricated replay success |

Acceptance criteria: record the changed rule, the failing test, and the observable mismatch. After
restoring the implementation, that test must pass again. If a mutation survives, inspect the assertion,
input, and execution boundary before adding another test. A mutation can also be equivalent for the
public contract; explain that possibility rather than forcing a meaningless assertion.

The existing [policy tests](../tests/Learning.Testing.UnitTests/ReminderPolicyTests.cs),
[async workflow tests](../tests/Learning.Testing.UnitTests/LoanReminderServiceTests.cs), and
[receipt tests](../../10-architecture/tests/Learning.Architecture.Tests/Persistence/RelationalEnrollmentTests.cs)
are starting points. Avoid copying their setup into every test: share stable construction helpers,
but keep the important deadline, learner identity, or version visible in the scenario.

## CI and Remaining Evidence

The shared [repository gate](../../scripts/Validate-Repository.ps1) already collects coverage for all
maintained test projects. [CI](../../.github/workflows/ci.yml) uploads TRX and coverage artifacts on both
Windows and Linux. This focused helper is for interpreting a selected phase without re-running the
entire course. It does not add a second redundant CI test pass or an arbitrary percentage threshold.

Production-provider fixtures, real concurrent connections, schema upgrades, and the phase completion
audit remain separate work in the [roadmap](00-roadmap.md). Coverage of a fake or SQLite adapter does
not substitute for those capabilities.

## References

- [Coverlet VSTest integration and collector settings](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md)

## Navigation

- Previous: [HTTP integration and fixture ownership](03-http-integration-fixtures.md)
- Return: [Testing study path](../README.md)
