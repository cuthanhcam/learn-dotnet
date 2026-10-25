---
title: "Deterministic Time, Test Doubles, and Async Contracts"
description: "Control a workflow's clock and ports to test payloads, failure, cancellation, and awaiting without sleeps or external services."
slug: testing-time-doubles-async
phase: 11
order: 2
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 16
topics: [testing, timeprovider, async, cancellation, test-doubles]
prerequisites: [testing-unit-contracts, dotnet-async-concurrency-roadmap]
status: maintained
last-reviewed: 2026-10-25
---

# Deterministic Time, Test Doubles, and Async Contracts

## Choose what the test controls

The [reminder service](../src/Learning.Testing.Examples/Reminders/LoanReminderService.cs) reads a loan,
checks cancellation, samples `TimeProvider.GetUtcNow()` once, evaluates the policy, and awaits the
sender. Success means the sender's task completed successfully; it does not mean an email arrived.

```text
loan ID + caller token
  -> reader -> cancellation checkpoint
  -> absent? NotFound
  -> one clock snapshot -> policy
  -> not due or returned? NotRequired
  -> awaited sender -> Sent
```

The clock is sampled after the lookup, so the policy uses the time of decision rather than the time
an earlier I/O operation started. The same instant is carried into the reminder payload.

## Stub, fake, recording double, mock

Inspect [TestDoubles.cs](../tests/Learning.Testing.UnitTests/TestDoubles.cs):

- `StubLoanReader` returns a configured value or fault. It also records the requested ID/token.
- `RecordingSender` records attempted calls and allows completion, failure, or cancellation to be controlled.
- `FixedClock` returns a chosen instant. Its read counter makes the one-snapshot contract observable.

A fake usually provides a working simplified implementation, such as an in-memory catalog. A mock
usually verifies configured interaction expectations; a framework can generate one, but the framework
is not the testing strategy. Names overlap in practice: state the double's behavior and limits.
The recording sender counts attempts, not confirmed deliveries.

Every test creates its own doubles. There is no static mutable list to clear, no shared clock to reset,
and no dependency on the order xUnit chooses. Fixed-clock data is immutable; mutable call histories
belong to one test. These small doubles are not designed for concurrent use.

## A fixed clock is not a virtual scheduler

`FixedClock` only overrides `GetUtcNow`. It does not advance `Task.Delay`, implement timers, or speed up
a retry loop. For code that uses provider-based timers, consider Microsoft's `FakeTimeProvider` and
its testing package; supply the provider to the timer API too. This lesson needs no additional package
because its workflow has no timer.

## Async evidence without sleeping

Read [LoanReminderServiceTests](../tests/Learning.Testing.UnitTests/LoanReminderServiceTests.cs).
`SendAsync_PendingSender_DoesNotReportSentBeforeCompletion` supplies a `TaskCompletionSource` and
holds completion until the test releases it. Removing the production `await` would make that test
fail. `RunContinuationsAsynchronously` avoids inline continuations becoming an accidental dependency.
The five-second `WaitAsync` is a failure bound, not a delay or a scheduling assertion.

The remaining cases specify missing data, skipped reminders, correct payloads, token forwarding,
pre-cancellation, cancellation during a reader that ignores the token, sender cancellation, and
reader/sender failure. Failures propagate; they are not converted into `NotFound` or `Sent`.
Await the assertions as well as the operation under test; an unawaited test assertion can escape
the test's lifetime.

Cancellation is cooperative. A checkpoint prevents deliberately starting another step after observed
cancellation, but cannot guarantee that a token never changes between a check and an external effect.
If a sender accepts a request and the response is lost, a retry may duplicate the effect. This sample
does not claim durable delivery, atomic rollback, or idempotency. See [Phase 10](../../10-architecture/README.md)
for the corresponding protocols.

## Exercises and acceptance criteria

1. Add a test where the reader faults asynchronously. Acceptance: the same fault escapes and the
   sender is never invoked; do not use `Thread.Sleep` or an elapsed-time threshold.
2. Hold the reader with a `TaskCompletionSource`, cancel the caller, then complete the reader normally.
   Acceptance: no sender attempt occurs and the caller's cancellation token is preserved.
3. Call the workflow twice for the same due loan. Acceptance: demonstrate two attempts and explain
   what additional persistent protocol would be needed to deduplicate them.
4. Replace the explicit `TimeProvider` with `DateTimeOffset.UtcNow` locally. Acceptance: explain why
   a deadline test would depend on the calendar, and restore the injected clock before committing.

## References and navigation

- [TimeProvider overview](https://learn.microsoft.com/en-us/dotnet/standard/datetime/timeprovider-overview)
- [FakeTimeProvider testing](https://learn.microsoft.com/en-us/dotnet/core/extensions/timeprovider-testing)
- [xUnit shared context](https://xunit.net/docs/shared-context)
- Previous: [Unit test design](01-unit-test-design.md)
- Next: HTTP integration slice in the [delivery ledger](00-roadmap.md)
