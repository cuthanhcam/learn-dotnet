---
title: "Async, Await, and Task"
description: "Task states, async state machines, composition, exception flow, and sequential versus concurrent awaiting."
slug: dotnet-async-await-task
phase: 6
order: 1
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 38
topics: [async, await, task]
prerequisites: [dotnet-async-concurrency-roadmap]
status: maintained
last-reviewed: 2026-08-15
---

# Async, Await, and Task

## Learning Objectives

- Explain synchronous execution before the first incomplete `await`.
- Distinguish a task from a thread and asynchronous concurrency from CPU parallelism.
- Compose success, failure, and cancellation without abandoning tasks.
- Choose between `Task<T>`, `ValueTask<T>`, and `IAsyncEnumerable<T>`.
- Design streaming operators with bounded memory and enumeration cancellation.

An async method runs synchronously until it reaches an incomplete await. It records the continuation state and returns a task to its caller. When the awaited operation completes, the continuation becomes eligible to run; this does not imply a dedicated thread waited for it.

Await tasks sequentially when operation B depends on A. Start independent operations before awaiting `Task.WhenAll` when overlap is safe. Preserve the association between inputs and tasks because completion order may differ from result-array order.

Exceptions thrown before or after suspension fault the returned task and are rethrown by `await`. Always observe tasks. `async void` is reserved for event-handler contracts because callers cannot await or compose it.

`ValueTask<T>` can avoid an allocation in specialized frequently-synchronous APIs, but it has usage restrictions and greater complexity. Default to `Task<T>` until measurement and API semantics justify otherwise.

## What the Compiler Builds

For an async method that may suspend, the compiler creates a state machine containing parameters,
locals needed after suspension, the current state, and a method builder. Calling the method executes
synchronously until it returns, throws before producing a task, or encounters an await whose awaiter
is incomplete. At suspension, the method arranges a continuation and returns its task-like result.

When the awaited operation completes, the continuation is scheduled according to the awaiter and
captured context. It may run on the same thread, another pool thread, or a context-owned thread.
Thread identity is not a reliable request or operation identity; use explicit state and tracing
correlation.

An await whose operation is already complete may continue synchronously. Therefore code before and
after `await` must be correct under both synchronous and asynchronous completion. This is one reason
inline continuation behavior matters in low-level coordination code.

## Task Lifecycle and Ownership

A `Task` is a promise of one terminal state:

- successful completion, optionally with a result;
- fault, with one or more exceptions; or
- cancellation, represented through an appropriate cancellation exception and token.

Starting work creates an ownership obligation. Some component must await it, supervise it, record
failure, participate in shutdown, and decide how long it may outlive the initiating call. Assigning a
task to `_` does not remove this obligation. Application frameworks provide explicit background
service lifetimes; short-lived request code should not start detached work that uses disposed scopes.

Tasks are generally “hot”: an async method starts executing when called, not when first awaited.
Calling the method twice normally starts two operations. Store and share one task only when shared
lifetime, cancellation, failure caching, and retry behavior are intentional.

## Sequential and Concurrent Composition

Start operation B after awaiting A when B depends on A's result or side effect. When operations are
independent, create both tasks before the first await:

```csharp
Task<Customer> customerTask = LoadCustomerAsync(id, token);
Task<Order[]> ordersTask = LoadOrdersAsync(id, token);

await Task.WhenAll(customerTask, ordersTask);
return new CustomerView(await customerTask, await ordersTask);
```

Concurrency does not guarantee lower resource use. `WhenAll` over a very large input eagerly creates
all operations and retains their tasks/results until completion. Use a semaphore, `Parallel.ForEachAsync`,
or a bounded channel when fan-out must respect memory or downstream capacity.

## Composition Patterns Beyond `WhenAll`

`Task.WhenAny` supports completion-order processing. Remove and await the returned task before the next iteration; otherwise the same completed task can be selected repeatedly. `TaskCompositionExample.InCompletionOrder` exposes this pattern as `IAsyncEnumerable<T>` and accepts enumeration cancellation.

Awaiting a faulted `WhenAll` rethrows a failure, while the completed tasks retain all faults. If diagnostics require every exception, inspect the faulted tasks after the aggregate operation has completed. Do not abandon sibling tasks after observing the first failure.

`Task.WaitAsync(token)` cancels the caller's wait; it does not forcibly cancel the underlying operation. Prefer passing a token into the operation itself when the API supports cooperative cancellation.

`WhenAny` also does not observe a completed task's result or exception by itself. Await the returned
task, remove it from the pending set, and retain ownership of every sibling. Racing operations require
an explicit loser policy: cancel, await/drain, or deliberately supervise them.

## Exception Semantics

An exception in an async method is stored in the returned task and rethrown by `await` with its useful
type and stack information. `Task.WhenAll` completes only after all supplied tasks complete. Awaiting
it throws a failure, while individual task exception properties retain all failures for diagnostics.

Catch only where code can recover, translate at a meaningful boundary, add context without destroying
the original exception, or perform cleanup. A broad catch that logs and returns a default value turns
failure into misleading success.

`async void` sends exceptions to the current synchronization context and cannot be awaited by its
caller. Its legitimate use is an event-handler signature. Move real work into a `Task`-returning method
that the handler invokes inside an intentional error boundary.

## Choosing Task or ValueTask

Use `Task`/`Task<T>` by default. Completed task instances can already be cached by APIs, and `Task` is
simple to store, await repeatedly, combine, and pass between components.

Consider `ValueTask<T>` only for a measured, allocation-sensitive API that often completes
synchronously. A `ValueTask` may wrap a reusable source and generally must be awaited once. Do not
casually await it multiple times, call `AsTask()` repeatedly, or store it for later. Public API
complexity and misuse risk can cost more than the avoided allocation.

## TaskCompletionSource

`TaskCompletionSource<T>` adapts callbacks, signals, or externally completed operations into TAP. Use `TaskCreationOptions.RunContinuationsAsynchronously` for shared coordination primitives so completing the source does not unexpectedly execute arbitrary continuations inline on the signaling thread.

Complete a source exactly once through `TrySetResult`, `TrySetException`, or `TrySetCanceled`. Decide who owns completion and how concurrent completion attempts are resolved.

When adapting an event or callback:

1. subscribe before starting the operation;
2. handle success, failure, and cancellation;
3. use `TrySet...` because callbacks can race;
4. unsubscribe on every terminal path; and
5. register cancellation without letting the registration outlive the operation.

`TaskCompletionSource` does not make a callback-based operation inherently cancelable. It can cancel
the caller-facing task, but the underlying operation needs its own stop mechanism if resource usage
must end.

## Async Streams

`IAsyncEnumerable<T>` represents values that arrive over time. Consume with `await foreach`, propagate enumeration cancellation, and dispose the enumerator. An async stream can fail or cancel between any two elements, so partial consumption is part of its contract.

Async iterators are lazy: their body begins as enumeration advances. `[EnumeratorCancellation]`
allows the token supplied by `WithCancellation` or `GetAsyncEnumerator(token)` to participate in the
iterator. Pass that token to every cancellable wait inside the iterator.

Streaming does not automatically mean bounded memory. Calling `ToListAsync` on an infinite or
unexpectedly large stream still grows without limit. `AsyncStreamOperators.ToBoundedListAsync`
requires a maximum count and fails when the contract is exceeded.

`AsyncStreamOperators.Buffer` emits fixed-size arrays plus a possible partial final batch. It copies
each emitted batch because yielding a mutable buffer and then clearing it would change data already
observed by the consumer. Cancellation before normal completion does not manufacture a final partial
batch; enumeration ends through cancellation.

`SelectAwait` intentionally processes one item at a time and preserves streaming order. Use the
bounded channel work pool when independent transformations should overlap. Naming this distinction
prevents an innocent-looking operator from creating accidental unbounded concurrency.

## Testing Async Code Deterministically

- Await the returned task; never let a test finish while owned work is still running.
- Use `TaskCompletionSource` with `RunContinuationsAsynchronously` as a controllable gate.
- Assert active and peak counts instead of assuming operations overlapped after a delay.
- Test synchronous completion as well as suspension when writing low-level adapters.
- Test fault before suspension, fault after suspension, and cancellation where the contract differs.
- For async streams, test empty input, partial consumption, failure between elements, cancellation,
  disposal, and bounded materialization.

## Implementation Map

| Concern | Source | Tests |
|---|---|---|
| Simulated I/O and ordered `WhenAll` | `AsyncBasicsExample.cs` | `AsyncExamplesTests.cs` |
| Completion-order iteration and full failure observation | `TaskCompositionExample.cs` | `AdvancedCoordinationTests.cs` |
| Buffering, sequential transform, bounded materialization | `AsyncStreamOperators.cs` | `AsyncStreamOperatorsTests.cs` |
| Shared completion signal | `AsyncManualResetEvent.cs` | `AdvancedCoordinationTests.cs` |

## Review Questions

1. Does calling an async method automatically queue it to another thread?
2. What happens when an awaited operation is already complete?
3. Who owns the tasks that lose a `WhenAny` race?
4. Why is `ValueTask<T>` not a universal faster replacement for `Task<T>`?
5. How can an async stream still cause unbounded memory growth?
6. Why should a reusable `TaskCompletionSource` signal run continuations asynchronously?

## References

- [Task asynchronous programming model](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model)
- [Asynchronous programming scenarios](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios)
- [Async streams](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/generate-consume-asynchronous-stream)
- [Understanding the whys, whats, and whens of ValueTask](https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/)

## Navigation

- Previous: [Phase roadmap](00-roadmap.md)
- Next: [Cancellation and timeouts](02-cancellation-timeouts.md)
