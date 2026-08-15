---
title: "Channels and Pipelines"
description: "Producer-consumer design, bounded capacity, backpressure, completion, failure propagation, and pipelines."
phase: 6
order: 6
topics: [channels, pipelines, backpressure]
---

# Channels and Pipelines

`Channel<T>` coordinates asynchronous producers and consumers. A bounded channel makes capacity explicit: when full, a writer can wait, drop data according to policy, or reject input. Waiting provides backpressure so production cannot grow memory without limit.

Choose single-reader/single-writer options only when the topology guarantees them. The writer must complete, including on failure, so readers terminate. Consumers should drain with `ReadAllAsync` and observe propagated completion errors.

`System.IO.Pipelines` provides advanced buffer management for high-performance byte streams. Learn channels first; pipelines add parsing and buffer-lifetime contracts that should be justified by throughput needs.
