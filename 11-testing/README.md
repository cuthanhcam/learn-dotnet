---
title: "Phase 11 — Testing"
description: "Learn observable contracts, deterministic unit tests, test doubles, and integration boundaries through a loan reminder workflow."
phase: 11
status: in-progress
target-framework: net10.0
prerequisites: [phase-06-async-concurrency, phase-10-architecture]
previous-phase: ../10-architecture/README.md
---

# Testing

Earlier phases already use tests to specify behavior. This phase explains how to design those tests,
choose the boundary they prove, and detect an implementation that is almost correct.
Start with the [roadmap and delivery ledger](docs/00-roadmap.md).

## Delivered lessons

| Lesson | Read | Run or inspect |
|---|---|---|
| Unit contracts and boundary cases | [Unit test design](docs/01-unit-test-design.md) | `ReminderPolicyTests` |
| Deterministic time, doubles, failure, cancellation | [Time and asynchronous boundaries](docs/02-time-doubles-async.md) | `LoanReminderServiceTests` |

The example sends an offline reminder when a loan is unreturned and the decision instant is at or
after its deadline. The policy is pure; the workflow reads a loan, samples an injected clock, and
awaits an injected sender. No database, real email, HTTP service, or Docker is needed for these lessons.

## Run from the repository root

```powershell
dotnet restore 11-testing/11-testing.slnx --locked-mode
dotnet build 11-testing/11-testing.slnx --configuration Release --no-restore
dotnet run --project 11-testing/src/Learning.Testing.ConsoleApp --configuration Release --no-build
dotnet test 11-testing/11-testing.slnx --configuration Release --no-build --no-restore
dotnet test 11-testing/11-testing.slnx --configuration Release --no-build --filter "Category=Unit"
```

All projects target .NET 10 and inherit the root SDK and central package versions. This phase uses
the repository's xUnit v2/VSTest setup; it does not switch test platforms or add a mocking framework.
The fixed-clock console prints one recorded reminder, `Sent`, two `NotRequired` outcomes, and
`Missing loan: NotFound`. It never sends a real message.

## Source map

```text
11-testing/
  docs/                         # Lessons, exercises, delivery ledger
  src/
    Learning.Testing.Examples/   # Loan policy and asynchronous reminder workflow
    Learning.Testing.ConsoleApp/ # Offline, fixed-clock walkthrough
  tests/
    Learning.Testing.UnitTests/  # Isolated tests; fresh doubles per test
  11-testing.slnx
```

The root solution also includes these projects. To validate a contribution, run
`pwsh -NoProfile -File scripts/Validate-Repository.ps1`.

## Production boundaries and remaining work

The sender interface is a teaching boundary, not durable delivery. Calling the workflow twice can
send twice. Cancellation after an external service accepts a message cannot retract it. There is no
transaction, delivery receipt, deduplication, account authorization, scheduler, or email adapter here.
Phase 10 explains durable outbox/inbox protocols; this phase explains how to test such contracts.

HTTP integration tests, fixture ownership, coverage interpretation, production-provider tests,
and a completion audit are tracked in the roadmap. Phase 11 is **in progress** until those scoped
lessons and their exit gates are delivered. Phase 12 remains planned.

## Navigation

- Previous: [Architecture](../10-architecture/README.md)
- Start: [Testing roadmap](docs/00-roadmap.md)
- Next lesson: [Unit test design](docs/01-unit-test-design.md)
