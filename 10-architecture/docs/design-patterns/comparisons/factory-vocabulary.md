---
title: "Factory Vocabulary: Static Factories, Simple Factories, Factory Method, and Injection"
description: "Distinguish similarly named construction techniques by collaboration, ownership, and extension mechanism rather than by the name of a Create method."
slug: patterns-comparison-factory-vocabulary
phase: 10
order: 160
difficulty: advanced
article-type: reference
estimated-reading-minutes: 12
topics: [factory-method, abstract-factory, dependency-injection, comparisons]
prerequisites: [patterns-creational-factory-method]
status: maintained
last-reviewed: 2026-10-10
---

# Factory Vocabulary: Static Factories, Simple Factories, Factory Method, and Injection

## Why the Name Is Not Enough

Teams often call every object-returning method a factory. That informal vocabulary can be useful,
but it does not identify which extension mechanism exists or who owns a created instance. Describe the
collaboration before relying on a pattern label in a review.

| Technique | Defining mechanism | Example in this repository | Important distinction |
|---|---|---|---|
| Constructor | initialize a concrete instance | `new ReportDocument(...)` | direct construction can be the clearest choice |
| Static named factory | named static method constructs valid state | architecture `CourseOffering.Create` | not an overridable creator hook |
| Simple factory | central selection creates one of several implementations | `SwitchReportExporter` selection | a switch is allowed; not automatically GoF Factory Method |
| GoF Factory Method | creator workflow delegates product creation to a subclass hook | `ReportExporter.CreateFormatter` | creation extension through inheritance |
| Constructor injection | caller supplies a collaborator | `InjectedReportExporter` | composition, not product creation by the receiving context |
| Abstract Factory | contract creates related compatible product families | planned dedicated lesson | not just several unrelated `Create` methods |
| DI registration factory | container delegate creates a service under a lifetime | `AddScoped`/`AddSingleton` overloads | ownership/lifetime supplied by container rules, not by GoF terminology |

The Abstract Factory row is conceptual: its implementation is not delivered yet. Do not infer complete
catalog coverage from this comparison table.

## Ask Four Questions

1. **Who selects the implementation?** A caller, config mapping, subclass, or container can own selection.
2. **Who invokes construction?** Selection and construction need not occur in the same object.
3. **Who owns lifetime?** A product may be fresh, shared, borrowed, pooled, or disposable.
4. **What extension is required?** New products, related families, or merely clearer construction names
   motivate different techniques.

For the report example, the caller selects a creator. The creator's hook produces a formatter for each
export, and the shared workflow uses it. In the injected variant, the caller selects and supplies an
already constructed formatter. Both can produce identical output while making different extension and
ownership decisions.

## Avoid Accidental Service Location

A factory that receives `IServiceProvider` and resolves arbitrary dependencies everywhere may become
a service locator, hiding what consumers actually need. An explicit purpose-specific factory can be
appropriate for scoped creation, but it should state its product and ownership contract. Do not rename
an unbounded container lookup method to “Abstract Factory” to make hidden dependencies sound intentional.

Factory Method does not justify resolving request-scoped services from a singleton creator. A product
graph must still obey DI lifetimes, resource ownership, and thread-safety rules.

## Review Exercise

Given `static IReportFormatter Create(string format)`, ask whether the format set is closed, how unknown
input behaves, and who disposes the returned object. Then identify the difference from a protected
virtual `CreateFormatter` called by a shared export workflow. Finally explain why accepting an
`IReportFormatter` constructor argument can eliminate the creation hook entirely.

The best choice depends on the application. Recognizing the canonical pattern is useful; forcing it
where constructor injection is simpler is not a learning success.

## References and Navigation

- [Factory Method implementation](../creational/factory-method.md).
- [GoF taxonomy](https://www.informit.com/store/design-patterns-elements-of-reusable-object-oriented-9780321770462).
- [.NET DI guidelines](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/guidelines).
- Return: [Catalog roadmap](../00-roadmap.md).
