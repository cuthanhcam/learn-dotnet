---
title: "Deadlocks, SynchronizationContext, and ConfigureAwait"
description: "Circular waits, sync-over-async, context capture, thread-pool starvation, and deadlock prevention."
phase: 6
order: 8
topics: [deadlocks, synchronization-context, configureawait]
---

# Deadlocks, SynchronizationContext, and ConfigureAwait

A deadlock requires a cycle of waits in which no participant can progress. Blocking on `.Result`, `.Wait()`, or `GetAwaiter().GetResult()` can deadlock when an awaited continuation needs the blocked synchronization context. In server code it can instead contribute to thread-pool starvation.

Prefer async all the way. `ConfigureAwait(false)` tells an await not to resume on the captured context and is useful in reusable libraries that do not require it. It is not a substitute for removing blocking waits, and ASP.NET Core normally has no classic request `SynchronizationContext`.

For lock-based code, minimize held resources, never wait asynchronously while holding a monitor, impose a consistent acquisition order, and use timeouts only as failure detection—not as proof the design is safe.
