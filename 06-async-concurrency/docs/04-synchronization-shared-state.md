---
title: "Synchronization and Shared State"
description: "Race conditions, atomicity, memory visibility, lock, Interlocked, SemaphoreSlim, and invariant protection."
phase: 6
order: 4
topics: [concurrency, synchronization, locks]
---

# Synchronization and Shared State

A race exists when correctness depends on uncontrolled operation ordering. `counter++` is a read-modify-write sequence, not one atomic operation. Use `Interlocked` for supported single-value transitions, `lock` for short synchronous critical sections protecting a compound invariant, and `SemaphoreSlim` for asynchronous admission control.

Never `await` inside `lock`. Keep critical sections small, avoid calling unknown code while holding a lock, and establish one global lock order when multiple locks are unavoidable.

Synchronization also provides memory-ordering guarantees. `volatile` affects visibility and ordering for a field but does not make compound operations atomic.
