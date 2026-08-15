---
title: "Phase 06 — Async and Concurrency"
description: "A detailed guide to tasks, async/await, cancellation, threads, synchronization, parallelism, channels, concurrent collections, deadlocks, and asynchronous design."
phase: 6
status: complete
target-framework: net8.0
prerequisites: [phase-05-dsa]
previous-phase: ../05-dsa/README.md
next-phase: ../07-aspnet-core/README.md
---

# Async and Concurrency

> Build responsive, bounded, cancellable, and race-free workflows without confusing asynchronous waiting with parallel execution.

## Learning Outcomes

After this phase, you should be able to explain task lifecycles, compose independent operations, propagate cancellation, distinguish I/O-bound work from CPU-bound work, protect shared state, limit concurrency, use channels for backpressure, select concurrent collections, diagnose deadlocks, and test asynchronous behavior without timing-dependent assertions.

## Study Path

| Order | Lesson | Practice |
|---:|---|---|
| 0 | [Roadmap](docs/00-roadmap.md) | Establish terminology and workflow |
| 1 | [Async, await, and Task](docs/01-async-await-task.md) | Sequential versus concurrent composition |
| 2 | [Cancellation and timeouts](docs/02-cancellation-timeouts.md) | Token propagation and timeout ownership |
| 3 | [Threads and ThreadPool](docs/03-threading-threadpool.md) | I/O-bound versus CPU-bound work |
| 4 | [Synchronization](docs/04-synchronization-shared-state.md) | `lock`, `Interlocked`, `SemaphoreSlim` |
| 5 | [Parallelism](docs/05-parallelism.md) | Bounded CPU and async concurrency |
| 6 | [Channels and pipelines](docs/06-channels-pipelines.md) | Producer/consumer backpressure |
| 7 | [Concurrent collections](docs/07-concurrent-collections.md) | Atomic collection operations |
| 8 | [Deadlocks and context](docs/08-deadlocks-context.md) | Blocking cycles and `ConfigureAwait` |
| 9 | [Coordination primitives](docs/09-coordination-primitives-deep-dive.md) | Choose by invariant and ownership |
| 10 | [Production patterns](docs/10-production-concurrency-patterns.md) | Coalescing, retry, pipelines, shutdown |
| 11 | [Common pitfalls](docs/common-pitfalls.md) | Review failure patterns |

## Structure

```text
06-async-concurrency/
├── docs/
├── src/
│   ├── AsyncConcurrency.Examples/
│   └── AsyncConcurrency.ConsoleApp/
├── exercises/AsyncConcurrency.Exercises/
├── tests/AsyncConcurrency.Tests/
└── 06-async-concurrency.slnx
```

## Run the Phase

```powershell
dotnet restore 06-async-concurrency.slnx
dotnet build 06-async-concurrency.slnx --no-restore
dotnet test 06-async-concurrency.slnx --no-build
dotnet run --project src/AsyncConcurrency.ConsoleApp
dotnet run --project exercises/AsyncConcurrency.Exercises
```

## Core Mental Models

- `async` does not create a thread. It enables a method to suspend while an awaited operation is incomplete.
- A `Task` represents eventual completion: success, fault, or cancellation.
- I/O concurrency overlaps waiting; CPU parallelism executes computation on multiple workers.
- Cancellation is cooperative and flows through an operation graph via `CancellationToken`.
- A timeout is a policy owned by a boundary; distinguish it from caller cancellation.
- Shared mutable state requires an atomic transition, synchronization, confinement, or elimination.
- Bounded concurrency protects dependencies and memory; unbounded fan-out is not free throughput.
- Channels combine asynchronous coordination with explicit capacity and completion.
- Thread-safe collections make individual operations safe, not arbitrary multi-step workflows atomic.
- Sync-over-async can starve a thread pool or form a synchronization-context deadlock.

## Code Map

| Type | Concept |
|---|---|
| `AsyncBasicsExample` | Non-blocking waits and `Task.WhenAll` ordering |
| `CancellationExample` | Cooperative cancellation and owned timeout translation |
| `ThreadSafeCounter` | Atomic read-modify-write with `Interlocked` |
| `BoundedExecutor` | `SemaphoreSlim`, ordered results, and release in `finally` |
| `ChannelPipelineExample` | Bounded producer/consumer flow and writer completion |
| `AsyncMap` | Exercise in bounded asynchronous transformation |
| `AsyncRetry` | Selective retry with cancellation-aware delay |

## Design Checklist

- Does every public async method return `Task`, `Task<T>`, `ValueTask`, or `ValueTask<T>` rather than `async void`?
- Is every accepted token passed to cancellable dependencies?
- Is concurrency bounded according to downstream capacity?
- Is each semaphore acquisition paired with release in `finally`?
- Does the channel writer complete on success and failure?
- Can an event, task, or callback retain an object beyond its intended lifetime?
- Are exceptions observed and allowed to reach an intentional boundary?
- Does retry apply only to transient, safe-to-repeat operations?
- Are tests driven by signals and contracts rather than arbitrary sleeps?

## Completion Criteria

- [ ] Explain what happens before and after an incomplete `await`.
- [ ] Compose independent work with `Task.WhenAll` and preserve result mapping.
- [ ] Propagate caller cancellation and distinguish a locally owned timeout.
- [ ] Choose `Interlocked`, `lock`, or `SemaphoreSlim` for three concrete invariants.
- [ ] Bound asynchronous fan-out and verify the maximum concurrency in a test.
- [ ] Build a bounded channel pipeline that completes under success, failure, and cancellation.
- [ ] Explain why `ConcurrentDictionary` does not make a check-then-act sequence atomic.
- [ ] Describe one sync-over-async deadlock and one thread-pool starvation scenario.
- [ ] Pass `dotnet test 06-async-concurrency.slnx`.

## Next Phase

Continue with [Phase 07 — ASP.NET Core](../07-aspnet-core/README.md), where cancellation, dependency injection, logging, configuration, and concurrent request handling become application-level concerns.
