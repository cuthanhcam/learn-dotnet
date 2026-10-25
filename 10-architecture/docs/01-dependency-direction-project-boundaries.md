---
title: "Dependency Direction and Project Boundaries"
description: "Separate business rules, use-case coordination, adapters, and hosts while keeping features readable and avoiding unnecessary layers."
slug: architecture-dependency-direction-project-boundaries
phase: 10
order: 1
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 22
topics: [architecture, dependency-inversion, domain-modeling, concurrency]
prerequisites: [architecture-learning-roadmap]
status: maintained
last-reviewed: 2026-10-10
---

# Dependency Direction and Project Boundaries

## Begin with Change Pressure

An architecture decision should identify a kind of change the system needs to tolerate. In this sample,
course capacity rules should survive replacing storage, application use cases should survive replacing
the transport, and a console learner should be able to exercise the domain without starting a database.

A conventional layered architecture can satisfy many applications. Clean architecture adds a specific
dependency rule: business policy should not require an implementation of a database or web framework.
The practical mechanism is dependency inversion through ports owned by the code that needs them.

## The Implemented Dependency Graph

```text
ConsoleApp → Infrastructure → Application → Domain
```

The console is the composition root. It creates an in-memory store and passes it to a handler. The
handler accepts an application-owned interface; at runtime it calls the concrete store through that
interface. Domain types never resolve services, access configuration, inspect HTTP requests, or call EF.

| Project | Owns | Avoid |
|---|---|---|
| Domain | valid offering state and enrollment decisions | HTTP status codes and database APIs |
| Application | command orchestration and required external contracts | selecting concrete persistence |
| Infrastructure | atomic storage mechanics | deciding business capacity policy |
| ConsoleApp | construction and demonstration | duplicating business rules |

The host can reference infrastructure to perform composition. That exception is localized and visible.
The API follows the same rule: enrollment endpoints use application contracts; concrete adapter selection
remains in startup registration. This diagram isolates the introductory path. The complete graph adds
Contracts and Notifications, explained in [module boundaries](05-modules-events-outbox-inbox.md).

## Why the Port Is Specific

`ICourseOfferingStore.TrySaveAsync` accepts an expected version and reports whether the comparison
succeeded. This is a behavioral persistence contract. A generic `IRepository<T>.Update(T)` would hide
the race that matters most to enrollment.

EF Core already implements identity tracking and unit-of-work behavior. The relational adapter uses EF
where appropriate, not wrap every method in an interface with identical signatures. The port is useful
here because the application requires an explicit compare-and-save outcome across multiple adapters.

## Feature Organization Within Layers

`Application/Enrollments/EnrollLearner.cs` keeps the command, result, status, and handler together. They
change for the same use case. Small related contract types can share a file; large independent handlers
should have their own files as the feature grows.

A vertical slice describes how a feature is organized through its request path. It does not prohibit
layers or demand a mediator. Layers answer who may depend on whom; feature organization answers where
to find behavior. Both can be useful without generating separate assemblies for every request.

## Composition and Lifetime

The console constructs one store and one handler. This makes dependencies explicit without a container.
An ASP.NET Core host will register corresponding services with lifetimes appropriate to their state:
a relational DbContext is scoped; an immutable configuration object may be singleton; a scoped handler
must not be captured by a singleton background service.

Avoid a service locator or static container. Those approaches conceal requirements, complicate tests,
and make lifetime errors harder to inspect.

## Avoiding Ceremony

Not every class needs an interface. Domain behavior can be tested directly. Introduce ports for actual
external dependencies or interchangeable behavior. Do not create a service that only forwards to a
repository unless it establishes a meaningful application operation.

A modular monolith will become relevant when another capability, such as billing, needs ownership and
integration contracts. A microservice boundary additionally introduces independent deployment, network
failure, distributed tracing, version skew, and consistency tradeoffs. Those costs require evidence.

## Review Checklist

- Can domain behavior run without a host or database?
- Does each external port describe the guarantee the use case needs?
- Is adapter selection confined to the composition root?
- Can a reader locate one feature's request, result, and orchestration?
- Does each project protect a real change or ownership boundary?
- Are dependency rules checked in code review and eventually automated?

## Exercises

1. Sketch a relational implementation of the store without changing the handler constructor.
2. Explain why returning an IQueryable from the port would leak provider concerns.
3. Move one use case into a separate project and identify whether the new boundary provides value.
4. Compare direct handler invocation with a mediator and list the additional behavior actually needed.
5. Draw compile-time references and runtime calls separately for one enrollment.

## References

- [Common web application architectures](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Architectural principles](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles)

## Navigation

- Previous: [Architecture roadmap](00-roadmap.md)
- Next: [Aggregates, use cases, and concurrent enrollment](02-aggregates-use-cases-concurrent-enrollment.md)
