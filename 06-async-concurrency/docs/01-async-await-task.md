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
