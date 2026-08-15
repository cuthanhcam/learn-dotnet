---
title: "Garbage Collection"
description: "Reachability, generations, promotion, the LOH, finalization, disposal, and GC trade-offs."
phase: 4
order: 2
topics: [dotnet, garbage-collection, disposal]
---

# Garbage Collection

## What GC Solves

The .NET garbage collector automatically reclaims managed heap memory that is no longer reachable. This removes most manual memory-freeing work, but it does not make allocation free and it does not clean every kind of resource deterministically.

GC answers one question: "Can this managed object still be reached?"

It does not answer:

- "Should this file handle be closed now?"
- "Should this database connection return to its pool now?"
- "Should this rented array be returned now?"
- "Is this allocation cheap enough for this hot path?"

## Reachability

An object is collectible only when no GC root can reach it. Roots include active stack references, statics, finalizer queues, handles, and runtime-managed references.

```csharp
var customer = new Customer("Mina", 10);
customer = null;
```

After the reference is removed, the object may become eligible for collection. The runtime chooses when to collect based on memory pressure and GC heuristics.

## Generations

.NET uses generational GC because most objects die young.

| Generation | Meaning | Typical contents |
| --- | --- | --- |
| Gen 0 | Newly allocated objects | short-lived temporary objects |
| Gen 1 | Buffer between young and old | objects that survived one collection |
| Gen 2 | Long-lived objects | caches, app-wide state, long-lived graphs |

Generations describe object age, not object type. A `byte[]`, a `Customer`, and a closure can all move through generations if they survive.

## Promotion

If an object survives a collection, it may be promoted to an older generation. Promotion is useful for long-lived objects because the GC can avoid scanning them constantly. Promotion becomes a problem when many objects accidentally live too long.

Common causes of accidental long lifetime:

- static collections that grow forever
- event handlers not unsubscribed
- cached delegates or closures capturing large objects
- long-lived tasks holding references
- global service singletons holding request data

## Large Object Heap

Large objects, commonly arrays around 85,000 bytes or larger, are allocated on the Large Object Heap (LOH). LOH behavior matters because large allocations are expensive and can fragment memory.

Examples:

```csharp
byte[] buffer = new byte[100_000];
int[] numbers = new int[30_000];
```

Large buffers should be reused when the workload repeatedly needs them. This is where pooling can help, but pooling adds ownership rules.

## IDisposable Is Not GC

`IDisposable` is about deterministic cleanup. It is commonly used for:

- file streams
- sockets
- timers
- database connections
- native handles
- pooled ownership

```csharp
using var stream = File.OpenRead("data.txt");
```

The GC might eventually collect the `FileStream`, but you should not wait for that to close the file handle.

## Finalizers

Finalizers are a safety net for unmanaged resources, not a normal cleanup strategy. They make objects more expensive because finalizable objects need special GC handling and usually survive at least one collection.

Prefer:

- `SafeHandle` for native handles
- `IDisposable` for deterministic cleanup
- `using` or `await using` at call sites

## Forcing GC

`GC.Collect()` is usually a diagnostic tool, not an application optimization. Forcing a collection can hurt throughput because it interrupts the runtime's heuristics.

Acceptable cases include:

- controlled experiments
- benchmark setup/cleanup
- memory investigations
- rare application modes after unloading a large isolated workload

Do not use it as a routine fix for allocation-heavy code.

## Reading GC Signals

Useful APIs for learning:

```csharp
GC.CollectionCount(0);
GC.GetAllocatedBytesForCurrentThread();
GC.GetTotalMemory(false);
```

Interpret them as signals, not absolute truth:

- allocation deltas show pressure created by a code path
- collection counts show whether pressure triggered GC work
- total memory changes depend on runtime timing

The example `GarbageCollectionExample.AllocateShortLivedObjects()` uses allocation deltas for a controlled demonstration.

## Practice

1. Run `GarbageCollectionExample.Run()`.
2. Increase payload size and iteration count.
3. Observe allocated bytes and collection counts.
4. Add a large buffer and reason about LOH behavior.
5. Replace deterministic disposal with no disposal and explain why this is wrong for external resources.
