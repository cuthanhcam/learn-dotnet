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
| HTTP integration and fixture ownership | [Real pipeline and isolated state](docs/03-http-integration-fixtures.md) | `ReminderHttpTests`, `ReadOnlyHostTests` |
| Coverage and test quality | [Interpret reports and challenge assertions](docs/04-coverage-and-test-quality.md) | `scripts/Measure-PhaseCoverage.ps1 -Phase 11` |

The example sends an offline reminder when a loan is unreturned and the decision instant is at or
after its deadline. The policy is pure; the workflow reads a loan, samples an injected clock, and
awaits an injected sender. Unit lessons need no infrastructure; HTTP integration uses an in-process
test server. Neither suite needs a database, real email service, or Docker.

## Run from the repository root

```powershell
dotnet restore 11-testing/11-testing.slnx --locked-mode
dotnet build 11-testing/11-testing.slnx --configuration Release --no-restore
dotnet run --project 11-testing/src/Learning.Testing.ConsoleApp --configuration Release --no-build
dotnet test 11-testing/11-testing.slnx --configuration Release --no-build --no-restore
dotnet test 11-testing/11-testing.slnx --configuration Release --no-build --filter "Category=Unit"
dotnet test 11-testing/11-testing.slnx --configuration Release --no-build --filter "Category=Integration"
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
    Learning.Testing.Api/        # Offline HTTP host; logs instead of sending
  tests/
    Learning.Testing.UnitTests/  # Isolated tests; fresh doubles per test
    Learning.Testing.IntegrationTests/ # Actual pipeline and fixture ownership
  11-testing.slnx
```

The root solution also includes these projects. To validate a contribution, run
`pwsh -NoProfile -File scripts/Validate-Repository.ps1`.

## Run the offline API

In one terminal at the repository root, after building the phase:

```powershell
dotnet run --project 11-testing/src/Learning.Testing.Api --configuration Release --no-build --no-launch-profile -- --urls http://127.0.0.1:5111
```

In another terminal:

```powershell
Invoke-RestMethod http://127.0.0.1:5111/health
Invoke-RestMethod -Method Post http://127.0.0.1:5111/loans/20000000-0000-0000-0000-000000000001/reminders
```

Expect `status: ok` and then the loan ID with `outcome: Sent`; the host records a log, not an email.
The optional [HTTP request collection](src/Learning.Testing.Api/requests.http) includes future,
returned, missing, and invalid-ID cases for IDEs with HTTP support. The catalog's due case uses the
Unix epoch and its future case uses `DateTimeOffset.MaxValue`; test factories inject a fixed clock.
This unauthenticated host is for loopback learning, not deployment. Stop it with Ctrl+C.

## Production boundaries and remaining work

The sender interface is a teaching boundary, not durable delivery. Calling the workflow twice can
send twice. Cancellation after an external service accepts a message cannot retract it. There is no
transaction, delivery receipt, deduplication, account authorization, scheduler, or email adapter here.
Phase 10 explains durable outbox/inbox protocols; this phase explains how to test such contracts.

Production-provider tests and a completion audit are tracked in the
roadmap. Phase 11 is **in progress** until those scoped
lessons and their exit gates are delivered. Phase 12 remains planned.

## Navigation

- Previous: [Architecture](../10-architecture/README.md)
- Start: [Testing roadmap](docs/00-roadmap.md)
- Next lesson: [Unit test design](docs/01-unit-test-design.md)
