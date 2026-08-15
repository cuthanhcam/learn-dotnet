---
title: "Async, Await, and Task"
description: "Task states, async state machines, composition, exception flow, and sequential versus concurrent awaiting."
phase: 6
order: 1
topics: [async, await, task]
---

# Async, Await, and Task

An async method runs synchronously until it reaches an incomplete await. It records the continuation state and returns a task to its caller. When the awaited operation completes, the continuation becomes eligible to run; this does not imply a dedicated thread waited for it.

Await tasks sequentially when operation B depends on A. Start independent operations before awaiting `Task.WhenAll` when overlap is safe. Preserve the association between inputs and tasks because completion order may differ from result-array order.

Exceptions thrown before or after suspension fault the returned task and are rethrown by `await`. Always observe tasks. `async void` is reserved for event-handler contracts because callers cannot await or compose it.

`ValueTask<T>` can avoid an allocation in specialized frequently-synchronous APIs, but it has usage restrictions and greater complexity. Default to `Task<T>` until measurement and API semantics justify otherwise.

## Composition Patterns Beyond `WhenAll`

`Task.WhenAny` supports completion-order processing. Remove and await the returned task before the next iteration; otherwise the same completed task can be selected repeatedly. `TaskCompositionExample.InCompletionOrder` exposes this pattern as `IAsyncEnumerable<T>` and accepts enumeration cancellation.

Awaiting a faulted `WhenAll` rethrows a failure, while the completed tasks retain all faults. If diagnostics require every exception, inspect the faulted tasks after the aggregate operation has completed. Do not abandon sibling tasks after observing the first failure.

`Task.WaitAsync(token)` cancels the caller's wait; it does not forcibly cancel the underlying operation. Prefer passing a token into the operation itself when the API supports cooperative cancellation.

## TaskCompletionSource

`TaskCompletionSource<T>` adapts callbacks, signals, or externally completed operations into TAP. Use `TaskCreationOptions.RunContinuationsAsynchronously` for shared coordination primitives so completing the source does not unexpectedly execute arbitrary continuations inline on the signaling thread.

Complete a source exactly once through `TrySetResult`, `TrySetException`, or `TrySetCanceled`. Decide who owns completion and how concurrent completion attempts are resolved.

## Async Streams

`IAsyncEnumerable<T>` represents values that arrive over time. Consume with `await foreach`, propagate enumeration cancellation, and dispose the enumerator. An async stream can fail or cancel between any two elements, so partial consumption is part of its contract.
