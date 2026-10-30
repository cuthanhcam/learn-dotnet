---
title: "HTTP Integration Tests and Fixture Ownership"
description: "Exercise real ASP.NET Core routing, dependency injection, serialization, failure handling, and request cancellation while keeping mutable test state isolated."
slug: testing-http-integration-fixtures
phase: 12
order: 3
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 18
topics: [testing, aspnet-core, integration, fixtures, isolation]
prerequisites: [testing-time-doubles-async]
status: maintained
last-reviewed: 2026-10-25
---

# HTTP Integration Tests and Fixture Ownership

## What this boundary proves

Unit tests call the loan policy or service directly. These tests start the actual
[API entry point](../src/Learning.Testing.Api/Program.cs) through `WebApplicationFactory<Program>` and
send requests through an in-process `TestServer`. They exercise routing, dependency injection, the
workflow, JSON serialization, exception handling, and request cancellation together. A route typo or
wrong status code can fail here while every unit test still passes.

They do not exercise a TCP listener, reverse proxy, TLS, browser, production database, or real sender.
Name that boundary accurately rather than calling the suite a production end-to-end test.

## Keep the framework real and the external effects controlled

[ReminderApiFactory](../tests/Learning.Testing.IntegrationTests/ReminderApiFactory.cs) starts the
application in `Testing`, replaces the clock and sender through `ConfigureTestServices`, and normally
keeps the application's offline reader. `RemoveAll` makes the replaced registration explicit.
The configured clock is asserted through the captured reminder, not merely checked in the DI container.

Reader overrides are used only for fault and cancellation scenarios. The test still traverses the real
endpoint and service. Replacing the service itself with a stub would bypass important wiring evidence.
The sender's concurrent queue records attempts safely; a recorded attempt is not proof of delivery.

## Route and response contracts

Read [ReminderHttpTests](../tests/Learning.Testing.IntegrationTests/ReminderHttpTests.cs).

| Request | Expected evidence |
|---|---|
| Due loan POST | 200, JSON `loanId`/`outcome`, one attempt using the configured time |
| Future or returned loan POST | 200 `NotRequired`, no attempt |
| Unknown valid GUID POST | 404 Problem Details, no attempt |
| Empty GUID POST | 400 Problem Details, no attempt |
| Malformed GUID in path | 404 because the `:guid` route constraint does not match |
| GET on the POST-only route | 405, no attempt |
| Reader or sender faults | 500 Problem Details without internal exception text |
| Repeat the due-loan POST | Two attempts; this API has no deduplication protocol |

Status alone is insufficient: the success test checks JSON field names and values, and Problem
Details tests check content type and body status. The generic 500 test avoids coupling itself to
framework exception prose or dynamic trace IDs. The application has an exception handler; the tests
do not rely on a development exception page.

The malformed-route case is deliberately distinct from a parsed-but-invalid empty GUID. If a later API
chooses to model-bind an unconstrained path parameter and return 400 instead, that is a public contract
change to discuss and update, not a reason to make the test accept every 4xx status.

## Own fixtures and mutable state

[ReadOnlyHostTests](../tests/Learning.Testing.IntegrationTests/ReadOnlyHostTests.cs) uses xUnit
`IClassFixture<ReminderApiFactory>` to share one host for health and unknown-route requests. Neither
case mutates sender state. Each test disposes its own client; xUnit disposes the shared factory after
the class finishes. Do not dispose a borrowed shared fixture inside a test.

Write-path cases create and dispose a fresh factory and client in each test. No clearing operation or
test ordering is required. One case explicitly starts two hosts, sends through the first, and verifies
that the second has no attempt history. Sharing a mutable sender and calling `Clear()` in setup would
not make parallel writes safe.

Keep static fixture data immutable. For expensive database fixtures, distinguish the container/server
lifetime from per-test schema/data ownership. A shared server does not imply a shared mutable test
database. That provider-specific setup belongs in the later provider slice.

## Observe cancellation after the request reaches the application

The cancellation test injects a reader that signals when the request enters pending I/O. Only then
does it cancel the HTTP caller, await the caller's cancellation, and await confirmation that the
reader observed request cancellation. The sender must remain untouched. An infinite cancellable wait
models pending I/O; ten-second waits bound test failure rather than measure performance.

A pre-canceled HTTP request alone would not prove that the service received a token: the client may
reject it before dispatch. The server's request token is not required to be the same token instance
as the client's token. The test asserts the effect of cancellation at the reader boundary instead.

## Run and diagnose

From the repository root:

```powershell
dotnet test 12-testing/12-testing.slnx --configuration Release --filter "Category=Integration"
dotnet test 12-testing/12-testing.slnx --configuration Release --filter "Category=Unit"
dotnet test 12-testing/12-testing.slnx --configuration Release
```

For a failing HTTP case, inspect status, content type, body, and application logs before changing the
assertion. A 500 can mean a missing DI registration, not a policy defect. Run the phase suite after
the focused fix, then the repository gate. TestServer does not need a free network port.

## Exercises

1. Temporarily change the success response to 202. Acceptance: the HTTP test fails while policy tests
   remain green; restore 200 before committing.
2. Move sender history into a static queue locally. Acceptance: the two-host isolation test catches
   the leak; explain why disabling parallelization would hide rather than fix shared state.
3. Add a public correlation header. Acceptance: send a value, assert the response contract, reject or
   normalize invalid input deliberately, and avoid asserting generated trace IDs exactly.
4. Design a real-provider fixture. Acceptance: specify ownership, startup readiness, cleanup,
   migration behavior, concurrency evidence, and a CI job that actually starts that provider.

## Scope, references, and navigation

The runnable API is unauthenticated and offline. Use the loopback command in the
[phase README](../README.md); the sender only logs. Authentication, durable delivery, and deployment
are separate lessons, not hidden capabilities of this testing example.

- [ASP.NET Core integration testing](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0)
- [xUnit fixture ownership](https://xunit.net/docs/shared-context)
- Previous: [Time and async boundaries](02-time-doubles-async.md)
- Next: coverage and test quality in the [delivery ledger](00-roadmap.md)
