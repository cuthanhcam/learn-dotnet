# Memory Model

## What This Chapter Covers

This chapter explains how .NET stores data, how copies behave, and why the same type can behave differently depending on where it is used.

You will see:

- stack frames and local values
- heap objects and reference variables
- value type copying
- reference aliasing
- why lifetime is about reachability, not just scope

## Stack vs Heap

The stack stores method frames and short-lived local state. It is fast and predictable, and the runtime removes the frame automatically when the method returns.

The managed heap stores objects whose lifetime can outlast the current method. Those objects are reclaimed later by the garbage collector.

```csharp
int count = 5;
var customer = new MemoryPerformanceExample.Customer("Ava", 3);
```

`count` is a value. `customer` is a reference to a heap object.

## Value Types

Value types include `int`, `bool`, `double`, `struct`, and `record struct`. Copying a value type creates an independent value.

```csharp
var first = new MemoryPerformanceExample.Point(2, 4);
var second = first;
```

After the assignment, `first` and `second` are separate values. Updating one does not update the other.

This is what `MemoryModelExample.ValueTypeCopyExample()` demonstrates in code: the copied value changes, but the original stays intact.

## Reference Types

Reference types include classes, arrays, delegates, and strings. Copying the variable copies the reference, not the object.

```csharp
var firstCustomer = new MemoryPerformanceExample.Customer("Mina", 1);
var secondCustomer = firstCustomer;
secondCustomer.Name = "Updated";
```

Both variables refer to the same object, so the mutation is shared.

This is the behavior behind `MemoryModelExample.ReferenceAliasExample()`.

## Object Lifetime

An object remains alive while something still references it. Once the object becomes unreachable, it becomes eligible for collection.

Important detail: eligible does not mean immediately destroyed. The garbage collector decides when to reclaim memory.

## Copying Semantics In Practice

The important question is not only “is this a value type or reference type?” The more useful question is “what happens when I assign, pass, or return this value?”

Examples to reason about:

- passing a `Point` by value creates a copy
- assigning a `Customer` variable creates another reference to the same object
- returning a struct from a method copies the result

## Common Misconceptions

The statement “value types always live on the stack” is too simple. Value types can also be embedded inside heap objects, arrays, closures, or other containers.

The more accurate rule is:

- value types are copied by value
- reference types are copied by reference
- the storage location depends on where the containing object lives

## Hands-On Checks

Use the example methods to test your understanding:

- `MemoryPerformanceExample.StackAllocationExample()` returns a simple local value result
- `MemoryPerformanceExample.HeapAllocationExample()` returns a heap object
- `MemoryModelExample.ValueTypeCopyExample()` shows independent values
- `MemoryModelExample.ReferenceAliasExample()` shows shared mutation

## Why It Matters

This chapter is the foundation for the rest of the module. If you do not understand value and reference semantics, it becomes much harder to reason about GC pressure, boxing, pooling, and why certain optimizations work.
