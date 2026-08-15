---
title: "Async and Concurrency Roadmap"
description: "The ordered learning path and practice loop for Phase 06."
slug: dotnet-async-concurrency-roadmap
phase: 6
order: 0
difficulty: intermediate
article-type: roadmap
estimated-reading-minutes: 12
topics: [async, concurrency, roadmap]
prerequisites: [dsa-common-pitfalls]
status: maintained
last-reviewed: 2026-08-15
---

# Async and Concurrency Roadmap

Study task composition first, then cancellation, execution resources, shared-state safety, bounded parallelism, coordination primitives, and failure diagnosis. For every example, identify ownership, completion states, cancellation path, exception path, concurrency limit, and observable invariant.

Use deterministic signals in tests. Avoid proving concurrency with a long `Task.Delay`; count active operations or coordinate them with a task completion source, semaphore, or channel.

The phase is complete when you can design a workflow that terminates under success, failure, and cancellation without leaking permits, tasks, subscriptions, or channel readers.
