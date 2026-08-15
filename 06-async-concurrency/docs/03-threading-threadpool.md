---
title: "Threads and the ThreadPool"
description: "OS threads, ThreadPool workers, scheduling, starvation, and I/O-bound versus CPU-bound work."
slug: dotnet-threading-threadpool
phase: 6
order: 3
difficulty: advanced
article-type: concept
estimated-reading-minutes: 24
topics: [threads, threadpool, scheduling]
prerequisites: [dotnet-cancellation-timeouts]
status: maintained
last-reviewed: 2026-08-15
---

# Threads and the ThreadPool

An OS thread is an execution resource with scheduling and stack costs. The .NET ThreadPool reuses workers for short work items and continuations. Blocking pool threads during I/O reduces capacity and can cause starvation under load.

Await naturally asynchronous APIs for I/O-bound work. Do not wrap asynchronous I/O in `Task.Run`. Use `Task.Run` at a deliberate application boundary to move CPU-bound work off a responsiveness-sensitive thread; it does not make the computation cheaper.

Long-running dedicated threads are rare in application code. Prefer high-level tasks, channels, timers, and hosted-service abstractions unless thread affinity or a specialized scheduler is a proven requirement.
