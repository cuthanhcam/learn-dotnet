---
title: "Parallelism and Bounded Concurrency"
description: "CPU parallelism, async fan-out, Parallel APIs, ordering, degree limits, and workload suitability."
phase: 6
order: 5
topics: [parallelism, concurrency, semaphore]
---

# Parallelism and Bounded Concurrency

Parallelism is useful for sufficiently large independent CPU work. Coordination, partitioning, cache contention, and context switching can make small workloads slower. Measure representative input.

`Parallel.ForEach` targets synchronous CPU work. `Parallel.ForEachAsync` supports asynchronous delegates with bounded parallelism. For custom async fan-out, `SemaphoreSlim` can cap in-flight operations while an indexed result array preserves input order.

Unbounded `Select(async ...)` plus `WhenAll` eagerly creates all operations. That may overwhelm sockets, databases, rate limits, or memory even though the code is concise.
