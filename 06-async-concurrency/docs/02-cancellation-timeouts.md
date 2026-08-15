---
title: "Cancellation and Timeouts"
description: "Cooperative cancellation, token ownership, linked sources, timeout policy, and cancellation contracts."
slug: dotnet-cancellation-timeouts
phase: 6
order: 2
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 34
topics: [async, cancellation, timeouts]
prerequisites: [dotnet-async-await-task]
status: maintained
last-reviewed: 2026-08-15
---

# Cancellation and Timeouts

## Learning Objectives

- Define cancellation as a cooperative operation contract rather than forced termination.
- Assign ownership for caller tokens, linked sources, and timeout sources.
- distinguish caller cancellation, locally owned timeout, and dependency failure;
- place cancellation checkpoints without corrupting committed state;
- test cancellation without relying on slow wall-clock delays.

Cancellation is a request, not forced thread termination. Accept a `CancellationToken`, pass it to every cancellable dependency, check it in CPU loops, and allow `OperationCanceledException` to communicate the canceled completion state.

The creator of a `CancellationTokenSource` owns and disposes it. Link caller cancellation with a locally owned timeout when both must stop the operation. Translate to `TimeoutException` only when your timeout fired and caller cancellation did not; otherwise preserve the caller's cancellation contract.

After an irreversible side effect, define whether cancellation can still be honored safely. Cancellation must not leave partial state that violates an invariant.

## Token and Source Ownership

A `CancellationToken` is a lightweight value that may be copied and passed downstream. A
`CancellationTokenSource` owns registrations, timers, and the authority to request cancellation.
The component that creates a source normally disposes it after all users have stopped registering
or waiting on it.

Public APIs usually accept a token with a default value:

```csharp
public Task<Order> LoadOrderAsync(
    Guid orderId,
    CancellationToken cancellationToken = default);
```

Pass the same token to each dependency that belongs to that operation. In CPU loops, call
`ThrowIfCancellationRequested` at a frequency that balances responsiveness with overhead. Do not
invent a new source merely to pass a token; that disconnects the operation from its caller.

## Cancellation Is a Terminal State

Let `OperationCanceledException` propagate when cancellation is the public contract. Catching it and
returning a default value reports success with misleading data. Catching it and wrapping it in an
unrelated exception reports failure instead of cancellation.

When throwing explicitly, prefer `cancellationToken.ThrowIfCancellationRequested()` so the exception
carries the relevant token. Consumers and tests can then distinguish which cancellation authority
ended the operation.

Cancellation can race with successful completion. Once an operation has produced and committed its
result, a token becoming canceled immediately afterward does not retroactively cancel that success.
Document the commit point instead of promising impossible instantaneous cancellation.

## Owned Timeouts

A timeout is a boundary policy: this caller is willing to wait or spend resources for a limited
duration. It is not proof that the dependency stopped, rolled back, or never performed a side effect.

`CancellationExample.WithTimeoutAsync` creates a timeout source and links it with the caller token:

```csharp
using var timeoutSource = new CancellationTokenSource(timeout);
using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(
    callerToken,
    timeoutSource.Token);
```

Its exception filter translates cancellation to `TimeoutException` only when the owned timeout fired
and the caller token did not. The observable outcomes remain distinct:

| Outcome | Public result |
|---|---|
| Operation completes | return result |
| Dependency faults | preserve dependency exception |
| Caller requests cancellation | preserve `OperationCanceledException` and caller token |
| Locally owned deadline expires first | translate to `TimeoutException` |

If both sources race, define a precedence rule. The sample gives caller cancellation precedence when
the caller token is observed as canceled by the filter.

`Task.WaitAsync(timeout)` can bound how long a caller waits, but it does not automatically cancel the
underlying operation. Pass a cooperative token to the dependency when it supports one, and retain
ownership of the underlying task even if the local wait ends.

## Cancellation Around Side Effects

Divide a workflow into phases:

1. validation and preparation, usually freely cancellable;
2. reversible work, cancellable with cleanup;
3. commit or irreversible side effect;
4. post-commit response and notification.

Immediately before commit, check the token if abandoning is still safe. After commit, either finish
the operation without cancellation, return a result that reports the committed state, or use a
compensating action designed by the domain. Blindly throwing after a successful charge or durable
write can make the caller retry an operation that already happened.

Cancellation is not transaction rollback. Database transactions, idempotency keys, outbox patterns,
and compensating operations address different consistency requirements.

## Linked Tokens and Registration Lifetime

Linked sources are useful when shutdown, caller cancellation, and a deadline should all request the
same operation to stop. Linking many tokens for every tiny operation adds allocation and registration
cost, so create the scope at a meaningful boundary.

Cancellation callbacks can execute concurrently with normal code. Keep callbacks small, non-blocking,
and exception-safe. Dispose registrations when the callback is no longer needed, especially when the
registered state would otherwise be retained for a long-lived token.

## Testing Cancellation

- Test a token canceled before the method starts.
- Use a completion source or signal to cancel while work is queued or active.
- Assert the correct exception category and token.
- Verify cleanup, permit release, temp-file removal, and channel completion.
- Test success near the boundary without depending on a precise scheduler race.
- Inject a controllable clock when timeout logic depends heavily on wall time.
- Keep one small real-timer integration test if necessary; use generous margins rather than millisecond
  assumptions on a loaded CI machine.

The repository tests successful completion, caller cancellation, owned timeout translation,
dependency failure preservation, and invalid timeout configuration.

## Common Mistakes

- Accepting a token but not forwarding it.
- Catching all exceptions and converting cancellation into generic failure.
- Calling `Cancel` from a component that does not own the source.
- Disposing a source while other work still registers with or uses it.
- Treating timeout as proof that remote work did not complete.
- Checking cancellation after an irreversible commit and reporting a false cancellation result.

## Implementation Map

| Concern | Source | Tests |
|---|---|---|
| Cooperative loop and owned timeout | `CancellationExample.cs` | `AsyncExamplesTests.cs` |
| Caller-only cancellation of shared work | `AsyncMemoizer.cs` | `AsyncMemoizerTests.cs` |
| Pipeline sibling cancellation | `ChannelWorkPool.cs` | `ChannelWorkPoolTests.cs` |

## Review Questions

1. Who is allowed to call `Cancel` on a source?
2. Why must caller cancellation and timeout remain distinguishable?
3. Does canceling `WaitAsync` cancel the underlying task?
4. Where is the cancellation commit point in a durable write or payment workflow?
5. Why can cancellation callbacks require synchronization?

## References

- [Cancellation in managed threads](https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads)
- [How to listen for cancellation requests](https://learn.microsoft.com/en-us/dotnet/standard/threading/how-to-listen-for-cancellation-requests-by-polling)
- [Task cancellation](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-cancellation)

## Navigation

- Previous: [Async, await, and Task](01-async-await-task.md)
- Next: [Threads and the thread pool](03-threading-threadpool.md)
