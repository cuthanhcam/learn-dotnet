---
title: "Concurrent Collections"
description: "ConcurrentDictionary, ConcurrentQueue, ConcurrentStack, partitioning, and compound-operation semantics."
phase: 6
order: 7
topics: [concurrency, concurrent-collections]
---

# Concurrent Collections

Concurrent collections support thread-safe individual operations. Use `ConcurrentDictionary.GetOrAdd` or `AddOrUpdate` rather than composing `ContainsKey` with an update. Even atomic APIs may invoke a value factory more than once; factories must tolerate duplicate computation and should not rely on exactly-once side effects.

`ConcurrentQueue<T>` and `ConcurrentStack<T>` provide non-blocking-style FIFO/LIFO operations. A channel is usually a better fit when consumers must wait asynchronously for new items and require completion or capacity semantics.

Thread safety does not define a business transaction. Protect multi-object or multi-step invariants with a higher-level design, synchronization boundary, actor/queue ownership, or transactional store.
