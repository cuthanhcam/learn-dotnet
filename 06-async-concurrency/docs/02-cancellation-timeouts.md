---
title: "Cancellation and Timeouts"
description: "Cooperative cancellation, token ownership, linked sources, timeout policy, and cancellation contracts."
phase: 6
order: 2
topics: [async, cancellation, timeouts]
---

# Cancellation and Timeouts

Cancellation is a request, not forced thread termination. Accept a `CancellationToken`, pass it to every cancellable dependency, check it in CPU loops, and allow `OperationCanceledException` to communicate the canceled completion state.

The creator of a `CancellationTokenSource` owns and disposes it. Link caller cancellation with a locally owned timeout when both must stop the operation. Translate to `TimeoutException` only when your timeout fired and caller cancellation did not; otherwise preserve the caller's cancellation contract.

After an irreversible side effect, define whether cancellation can still be honored safely. Cancellation must not leave partial state that violates an invariant.
