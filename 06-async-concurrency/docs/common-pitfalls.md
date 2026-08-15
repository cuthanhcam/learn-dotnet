---
title: "Common Async and Concurrency Pitfalls"
description: "Unobserved tasks, async void, sync-over-async, lost cancellation, races, unbounded fan-out, and retry hazards."
slug: dotnet-async-concurrency-pitfalls
phase: 6
order: 99
difficulty: reference
article-type: pitfalls
estimated-reading-minutes: 22
topics: [async, concurrency, pitfalls]
prerequisites: [dotnet-production-concurrency-patterns]
status: maintained
last-reviewed: 2026-08-15
---

# Common Async and Concurrency Pitfalls

- Starting fire-and-forget work without ownership, exception observation, shutdown, or lifetime control.
- Using `async void` outside an event handler.
- Calling `.Result` or `.Wait()` on asynchronous operations.
- Accepting a token but failing to pass it downstream.
- Converting caller cancellation into a timeout or generic failure.
- Releasing a semaphore only on the success path.
- Creating one task per unbounded input item.
- Assuming a thread-safe collection makes a multi-step invariant atomic.
- Retrying permanent failures, non-idempotent side effects, or cancellation.
- Using arbitrary delays to test scheduling behavior.
- Completing a channel only on success, leaving consumers waiting after failure.
- Holding a lock while invoking callbacks or unknown code.
- Releasing a semaphore when `WaitAsync` failed or was canceled before acquisition.
- Treating `SemaphoreSlim` as ownership-aware: any code with access can release it.
- Double-releasing a permit through copied or repeatedly disposed releasers.
- Disposing a coordination primitive while workers still use it.
- Running `TaskCompletionSource` continuations inline on a sensitive signaling thread.
- Assuming canceling `WaitAsync` cancels an underlying non-cancelable operation.

During review, trace success, failure, cancellation, and shutdown separately. Every path must terminate and release what it owns.
