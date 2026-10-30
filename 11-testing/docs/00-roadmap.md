---
title: "Testing Learning Roadmap"
description: "A progressive path from isolated behavior specifications to HTTP integration, provider testing, and trustworthy coverage."
slug: testing-learning-roadmap
phase: 11
order: 0
difficulty: intermediate
article-type: roadmap
estimated-reading-minutes: 8
topics: [testing, roadmap, xunit, test-design]
prerequisites: [architecture-learning-roadmap]
status: maintained
last-reviewed: 2026-10-30
---

# Testing Learning Roadmap

## Outcomes

By the end of this phase, explain what each test proves, select a suitable test boundary, construct
reproducible inputs, assert observable contracts, and diagnose a failure without relying on test order,
machine time, or an external service that the test does not control.

## Delivery ledger

| Slice | Status | Scope and exit criteria |
|---|---|---|
| Unit foundations | Delivered | Pure deadline policy, awaited workflow, fixed clock, explicit doubles, failure/cancellation tests, console, docs and exercises |
| HTTP integration | Delivered | Real routing/DI/serialization/error handling, status/body assertions, isolated state, cancellation, and fixture ownership |
| Coverage and test quality | Delivered | Fresh per-project/module reports for Phases 10/11, branch versus line interpretation, mutation exercises, focused suites, and failure diagnosis |
| PostgreSQL provider baseline | Delivered | Docker fixture, shared store contracts, competing connections, rollback, replay, server constraints, cleanup, and a dedicated CI job |
| Provider schema evolution | Planned | Reviewed PostgreSQL migrations, populated-schema upgrade tests, deployment compatibility, and recovery limits |
| Completion audit | Planned | Trace outcomes to code/tests, verify all commands, and document remaining operational boundaries |

Each slice uses its own branch and PR into `develop`, following the existing runtime delivery model.
Do not mark the whole phase complete merely because the first unit suite passes.

Unit foundations were merged in [PR #90](https://github.com/cuthanhcam/learn-dotnet/pull/90) after all
hosted checks passed. HTTP integration was merged in [PR #91](https://github.com/cuthanhcam/learn-dotnet/pull/91).
Coverage and test quality continues in its own branch with full local validation and hosted checks.

## Study sequence

1. Run the [phase console and tests](../README.md).
2. Read [unit test design](01-unit-test-design.md). Predict the result one tick before, at, and after a deadline.
3. Read [time, doubles, and async](02-time-doubles-async.md). Trace the same workflow through success,
   missing data, cancellation, and dependency failure.
4. Attempt the exercises before consulting the reference implementation.
5. Study [HTTP integration and fixtures](03-http-integration-fixtures.md). It adds evidence about
   framework wiring and state ownership rather than replacing unit-level boundary tests.
6. Collect [coverage and evaluate test quality](04-coverage-and-test-quality.md); explain one missed
   branch and reproduce one deliberate defect before restoring the correct implementation.
7. Run [PostgreSQL provider tests](05-postgresql-provider-tests.md) and distinguish server evidence
   from SQLite and injected failures.

## Reuse the earlier phases deliberately

| Existing material | What to examine |
|---|---|
| [Phase 03 time tests](../../03-core-dotnet/tests/CoreDotNet.Tests/DateTimeAndTimeZone/TimeProviderExampleTests.cs) | The exact expiration boundary and injected time |
| [Phase 06 async](../../06-async-concurrency/README.md) | Token ownership, awaiting tasks, synchronization, bounded work |
| [Phase 07 API tests](../../07-aspnet-core/README.md) | Framework integration versus direct method calls |
| [Phase 08 persistence tests](../../08-ef-core/README.md) | SQLite evidence versus production-provider behavior |
| [Phase 10 architecture](../../10-architecture/README.md) | Concurrent writes, replay, and durable delivery boundaries |

## Exit gates

A delivered lesson needs a runnable sample, a clear contract, meaningful boundary/failure cases,
exercises with acceptance criteria, primary references, and honest limits. Run the phase suite first,
then all repository gates; inspect hosted Windows/Linux/security checks before merging its PR.

## Navigation

- Previous: [Phase 10](../../10-architecture/README.md)
- Next: [Unit test design](01-unit-test-design.md)
