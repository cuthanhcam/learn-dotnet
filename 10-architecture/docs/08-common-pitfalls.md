---
title: "Common Architecture Pitfalls and Production Review Questions"
description: "Diagnose abstraction, transaction, idempotency, event delivery, security, and maintenance mistakes using the enrollment implementation as a concrete reference."
slug: architecture-common-pitfalls
phase: 10
order: 8
difficulty: advanced
article-type: pitfalls
estimated-reading-minutes: 25
topics: [architecture, anti-patterns, reliability, security, code-review]
prerequisites: [architecture-decisions-fitness-maintainability]
status: maintained
last-reviewed: 2026-10-10
---

# Common Architecture Pitfalls and Production Review Questions

## 1. Treating a Template as a Universal Best Practice

**Symptom:** every service starts with identical layers, base entities, generic repositories, and
mediator pipelines before any requirement is understood.

**Why it hurts:** readers must learn ceremony unrelated to the problem, and abstractions encode
assumptions that may not fit the workload. The template can become harder to change than the feature.

**In this phase:** layers protect enrollment invariants; a second module protects ownership and failure
isolation. No generic repository or mediator is required. Reconsider project boundaries when requirements
justify it, and record the reason rather than calling the new structure perfect.

## 2. Inverting an Interface but Not the Semantics

**Symptom:** an application interface returns `DbSet`, `IQueryable`, or provider exceptions and is
declared independent of EF merely because it is in a different assembly.

**Why it hurts:** the caller still depends on deferred execution, provider translation, and context
lifetime. Swapping an adapter is not simply implementing the interface.

**In this phase:** the write port promises atomic version comparison; the query port returns materialized
DTOs. The stronger durable workflow promises a transaction boundary, not just a renamed CRUD method.

## 3. Checking Version Before an Unconditional Update

**Symptom:** code loads a row, compares a version in memory, then overwrites without a database predicate.

**Why it hurts:** another writer can commit between comparison and mutation. Both callers can believe
they acquired the last seat.

**In this phase:** the SQL update includes both offering ID and expected version. A test starts from
two stale snapshots and verifies only the first save succeeds.

## 4. Assuming Concurrency Tokens Apply to Every EF Operation

**Symptom:** `IsConcurrencyToken` is configured, so a later `ExecuteUpdate` omits the version condition.

**Why it hurts:** bulk-style immediate updates do not use tracked original values automatically.

**In this phase:** the adapter writes the predicate explicitly and interprets the affected row count.
Tracked and immediate update paths are not mixed for the same row in one operation.

## 5. Owning a Transaction in Too Many Places

**Symptom:** each repository commits independently, while an application service assumes several calls
are one unit of work.

**Why it hurts:** the seat can commit while receipt/outbox insertion fails, defeating replay and reliable
delivery. Nested method names do not imply transactional nesting.

**In this phase:** the durable coordinator opens one transaction covering the immediate update and
subsequent inserts. Fault injection after the update proves rollback of the entire local operation.

## 6. Confusing Membership Idempotency with Request Replay

**Symptom:** a duplicate learner check is presented as a complete idempotency-key implementation.

**Why it hurts:** it cannot reconstruct the original result, detect changed input under a reused key,
or atomically coordinate receipts with side effects.

**In this phase:** membership uniqueness belongs to the aggregate; durable request receipts belong to
the workflow adapter. A new key for an enrolled learner differs from replaying the original key.

## 7. Retrying Every Failure Inside the Same Scope

**Symptom:** a loop catches `Exception` and repeats the handler with the same failed DbContext.

**Why it hurts:** pending inserts, failed transactions, and nonidempotent external effects can survive.
Cancellation and permanent validation errors may become endless retries.

**In this phase:** conflicts are explicit and provider failures are not disguised. Caller retries must
preserve the key, create a new scope, classify failures, and use a bounded policy. No automatic blanket
retry is hidden inside the handler.

## 8. Claiming Exactly-Once Delivery from an Outbox

**Symptom:** a source row marked dispatched is treated as proof an external effect occurred exactly once.

**Why it hurts:** a crash after receiver commit but before source acknowledgement causes redelivery.

**In this phase:** a welcome work item is deduplicated under the message ID in a separate database.
The failure-injection test proves one local work item after duplicate delivery. External email delivery
and broker transport are explicitly outside the sample's guarantee.

## 9. Swallowing All Database Errors as Duplicate Messages

**Symptom:** every `DbUpdateException` in a consumer returns success.

**Why it hurts:** connection failures, invalid schema, unrelated constraints, and disk errors are
acknowledged as if work committed. The source may permanently discard an undelivered message.

**In this phase:** an existing matching inbox work item is a known duplicate. Unexpected failures
propagate and leave source delivery pending. Changed payload under the same ID is rejected.

## 10. Accepting Identity from the Request Body

**Symptom:** an authenticated caller posts another learner's GUID and the handler enrolls it.

**Why it hurts:** authentication proves the caller's identity, not authority over every supplied ID.

**In this phase:** self-enrollment derives identity from validated claims and requires a write scope.
An administrator-on-behalf workflow would need a separate policy and audit model, not an optional field.

## 11. Shipping Test Authentication as a Convenience

**Symptom:** a header-based identity handler remains registered in the runnable API.

**Why it hurts:** clients can forge identity without token validation, often through an overlooked
environment switch or DI registration.

**In this phase:** the fake handler exists only in the integration test assembly. Separate tests use
the real bearer handler and local RSA-signed tokens. The production source has no header-auth fallback.

## 12. Calling EnsureCreated a Migration Strategy

**Symptom:** application startup calls `EnsureCreated` in all environments and assumes upgrades work.

**Why it hurts:** existing schemas are not evolved by that call. Concurrent startup and deployment
permissions also become operational problems.

**In this phase:** initialization is restricted to disposable local/test labs. Real deployment must
use reviewed migration/provisioning artifacts and provider-level validation.

## 13. Adding Version Numbers Without Compatibility Rules

**Symptom:** `/v1` exists, but persisted status values are renumbered or response meanings change silently.

**Why it hurts:** old clients and retained receipts can break despite an unchanged route.

**In this phase:** receipt statuses have explicit numeric values. The HTTP article defines evolution
questions for routes, status strings, key comparison, and integration event schemas. A versioning package
does not replace those decisions.

## 14. Relying Only on Assembly Dependency Tests

**Symptom:** a forbidden reference is added but unused, so compiled reference inspection passes.

**Why it hurts:** future code can use the dependency without any project change or review signal.

**In this phase:** fitness tests also inspect declared project references and pure-core package rules.
Behavioral tests remain necessary because a legal graph can still contain misplaced business logic.

## Production Adoption Checklist

- Select and test the actual database provider, schema upgrades, timeout, and isolation behavior.
- Normalize membership if query needs or aggregate size justify it.
- Define tenant isolation and issuer-to-learner identity mapping.
- Add API specification, stable problem types, rate limits, and resource budgets.
- Design receipt/outbox/inbox retention and privacy handling together.
- Add retry schedules, poison-message handling, and multi-worker claims before scaling delivery.
- Configure HTTPS, proxy trust, secrets, file permissions, backups, and disaster recovery.
- Establish metrics, tracing, alerts, health/readiness semantics, and operator runbooks.
- Test signing-key rollover, provider outages, uncertain commits, and deployment compatibility.
- Preserve small focused examples; do not hide these gaps behind a “production-ready” label.

## Official References

- [Architectural principles](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles).
- [EF Core transactions](https://learn.microsoft.com/en-us/ef/core/saving/transactions).
- [Configure JWT bearer authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-10.0).

## Navigation

- Previous: [Architecture decisions and fitness checks](07-decisions-fitness-maintainability.md)
- Next: [Completion audit](09-completion-audit.md)
