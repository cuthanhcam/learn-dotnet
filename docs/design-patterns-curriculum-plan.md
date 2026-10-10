---
title: "Advanced Design Patterns Curriculum and Solution Blueprint"
description: "Plan a dedicated GoF learning track with explicit phase ownership, all 23 patterns, focused solutions, progressive examples, tests, and implementation gates."
slug: advanced-design-patterns-curriculum-plan
phase: 0
order: 2
difficulty: advanced
article-type: roadmap
estimated-reading-minutes: 25
topics: [curriculum, design-patterns, gof, architecture, solution-structure]
prerequisites: [oop-composition-and-patterns, architecture-phase-10-completion-audit]
status: reviewed
last-reviewed: 2026-10-10
---

# Advanced Design Patterns Curriculum and Solution Blueprint

## Status and Scope

This is a proposed implementation blueprint, not a claim that the pattern catalog is already built.
The existing Phase 10 architecture scope remains complete. The proposal extends learning with a
separately navigable Track 10B, without renumbering planned Phases 11 through 15.

No projects, solution files, or runnable commands shown as proposed below exist merely because this
plan describes them. Update this document and the active roadmap as each complete slice is delivered.

## Placement Decision

Keep Phase 02 focused on encapsulation, substitution, composition, equality, object lifetime, SOLID,
and introductory examples. Place the full GoF catalog in **Phase 10B: Object Design and GoF Patterns**,
physically under `10-architecture/design-patterns/`, with its own solution and reading entry point.

Treat the current enrollment architecture curriculum as Track 10A. Its existing project paths and
`10-architecture.slnx` remain stable; do not move working code solely to make the directory tree symmetric.
The root workspace will include new pattern projects as they are delivered. A dedicated patterns
solution and category solutions provide smaller entry points for readers.

This choice groups architecture and object design at the advanced design stage, while keeping their
scope distinct. It avoids expanding a beginner phase into a reference encyclopedia or shifting every
later numbered folder/link. A standalone numbered design-pattern phase is an alternative only if a
future curriculum-wide renumbering is explicitly chosen.

## Which Phases Own Which Knowledge?

| Phase | Owns | Changes or connections needed |
|---|---|---|
| 02 OOP | language/object-model foundation and composition intuition | retain short introductions; clarify factory vocabulary and unsafe Singleton examples; link forward |
| 03 Core .NET | DI, configuration, lifetime, runtime mechanisms | link Singleton lifetime versus global Singleton; no full catalog duplication |
| 06 Async/concurrency | cancellation, synchronization, safe concurrent state | provide prerequisites for async decorators, observers, and singleton state |
| 07 ASP.NET Core | concrete HTTP/middleware/framework applications | link Adapter/Decorator/Chain-like examples without calling every pipeline an exact GoF implementation |
| 08 EF Core | actual persistence behavior | distinguish Repository/Unit of Work from GoF; test adapters against real providers |
| 09 Auth | security boundaries and claims | constrain Proxy/Decorator examples; never treat a wrapper as authorization by itself |
| 10A Architecture | dependencies, modules, aggregates, application/persistence patterns | preserve the current enrollment walkthrough; add navigation to 10B |
| 10B GoF | object collaboration, construction, composition, and all 23 canonical patterns | implement the new catalog, comparison articles, exercises, and focused case study |
| 11 Testing | broader test engineering and infrastructure | deepen pattern contract tests, property tests, lifecycle tests, and test isolation |
| 12 Projects | selective application under real requirements | refactor a realistic system using only justified patterns; do not cram in all 23 |
| 14 System design | distributed architecture and system-level patterns | separate Saga, CQRS, Outbox, circuit breaking, and deployment choices from GoF taxonomy |

Phase 02 audit found a `Singleton.Instance => _instance ??= new Singleton()` snippet without a local
thread-safety explanation and a factory that silently maps every non-dog input to a cat. The first
implementation slice should label/repair those teaching boundaries, not silently present them as
recommended production examples. Keep the fundamental lesson concise and cross-link the advanced track.

## Taxonomy Boundaries

The canonical GoF catalog has 23 patterns: five creational, seven structural, and eleven behavioral.
Repository, Unit of Work, CQRS, Outbox/Inbox, dependency injection, and Clean Architecture are important
design concepts, but they are not additional GoF entries. Explain their relationships in a separate
comparison article rather than mixing taxonomies in one list.

Microsoft Learn is the primary reference for .NET implementation/lifetime/API behavior. The GoF book
and its publisher's official catalog are the primary references for canonical names and intent. Do not
claim Microsoft defines the GoF taxonomy or copy copyrighted chapter text into repository articles.

## Proposed Physical Structure

```text
10-architecture/
  README.md                              # Track selector: architecture versus object patterns
  10-architecture.slnx                    # Existing architecture application remains focused
  src/                                   # Existing enrollment and notification projects unchanged
  tests/                                 # Existing architecture specifications unchanged
  docs/
    00-roadmap.md ... 09-completion-audit.md
    decisions/
    design-patterns/
      00-roadmap.md
      01-pattern-thinking-and-selection.md
      creational/                        # One full article per pattern, plus group comparison
      structural/
      behavioral/
      comparisons/                       # Similar structure, different intent
      case-studies/                      # Refactoring narrative and decisions
      common-pitfalls.md
      completion-audit.md
  design-patterns/
    README.md                            # Beginner-friendly entry point for this advanced track
    design-patterns.slnx                  # Complete delivered catalog, not the enrollment host
    solutions/
      creational.slnx                    # Only category library and its tests
      structural.slnx
      behavioral.slnx
    src/
      Learning.Patterns.Creational/
        FactoryMethod/
        AbstractFactory/
        Builder/
        Prototype/
        Singleton/
      Learning.Patterns.Structural/
        Adapter/ Bridge/ Composite/ Decorator/ Facade/ Flyweight/ Proxy/
      Learning.Patterns.Behavioral/
        ChainOfResponsibility/ Command/ Interpreter/ Iterator/ Mediator/
        Memento/ Observer/ State/ Strategy/ TemplateMethod/ Visitor/
      Learning.Patterns.ConsoleApp/       # Explicit demo selection; never run the whole catalog at once
    tests/
      Learning.Patterns.Creational.Tests/
      Learning.Patterns.Structural.Tests/
      Learning.Patterns.Behavioral.Tests/
    exercises/                           # Task specifications first; projects only when justified
    case-studies/
      OrderProcessing/                   # Added after catalog foundations, not before
```

Publishable articles remain under `10-architecture/docs/`, so the existing metadata/quality gates
discover them recursively. Do not place unvalidated article copies beside code. Category folders in
the sketch are proposed names, not a requirement to create empty directories in the first commit.

Start with seven projects: three independent pattern libraries, one runner, and three test projects.
Do not create 23 projects for 23 patterns. A pattern is normally a folder/namespace boundary; category
projects provide focused navigation without excessive solution overhead.

Add an exercises project/test pair only when runnable assessed exercises need them. A later case study
may justify a separate implementation library, console host, and test project. These are staged
decisions, not a requirement to scaffold twelve mostly empty projects immediately.

All new projects target .NET 10 and inherit repository SDK/package/formatting rules. Do not add a nested
`global.json`, duplicate central package versions, or introduce mapping/mediator libraries just to
demonstrate a pattern that can be expressed clearly with the BCL.

## Dependency Rules

```text
ConsoleApp -> Creational
           -> Structural
           -> Behavioral

Each category's tests -> that category library
Category libraries do not depend on each other or on the runner.
Existing enrollment application does not depend on catalog examples.
Case study is independently designed; example types are not a reusable production framework.
```

Avoid a shared `Common` project unless repeated types have a genuinely identical contract and ownership.
Small local scenario types are preferable to hidden coupling across unrelated lessons. Register demos
explicitly in the runner, using simple delegates if sufficient; reflection discovery and a generic
pattern execution framework would distract from the object-design lessons.

The root master solution must include every maintained project. Category solutions are convenient
subsets, not separate dependency/versioning authorities. Existing inventory validation already scans
nested project paths under `10-architecture`.

## Anatomy of One Pattern Folder

For a proposed `Behavioral/Strategy` lesson:

```text
Strategy/
  Baseline/                              # Small working design that exposes change pressure
  Refactored/                            # Named collaborators with clear responsibilities
  ModernCSharp/                          # Delegate alternative only when it adds a useful comparison
  StrategyDemo.cs                        # Deterministic observable walkthrough

tests/.../Strategy/
  PricingPolicyContractTests.cs
  StrategySelectionTests.cs

docs/design-patterns/behavioral/
  strategy.md                            # Intent, collaboration, code map, decisions, tests, exercises
```

Baseline does not mean knowingly insecure code. It should be correct under its original requirements
and show why a later change makes it awkward. Unsafe snippets belong in clearly labeled nonexecuting
discussion, not runnable examples that introduce CodeQL findings.

Split meaningful roles into separate classes/files when that aids tracing. Do not insist on one class
per file for every tiny record, nor hide an entire collaboration in one giant `Example.cs`. Prefer
domain names such as `RegionalPricingPolicy` over `ConcreteStrategyA`.

Comments should explain invariants, ownership, dispatch order, lifecycle, failure semantics, and why a
simpler alternative was rejected. Avoid repeating the syntax of every assignment.

## Complete Pattern Coverage Matrix

These are proposed learning scenarios. Their names and exact class boundaries can change after the
baseline requirements are specified; the canonical intent and acceptance evidence must remain clear.

| Category | Pattern | Proposed scenario | Essential evidence or contrast |
|---|---|---|---|
| Creational | Factory Method | extensible document exporters | creator extension; simple factory versus virtual creation |
| Creational | Abstract Factory | compatible document rendering families | family consistency; not just one switch returning one product |
| Creational | Builder | validated immutable report specification | mandatory fields, reuse, and partial construction boundaries |
| Creational | Prototype | copying a configured campaign/template graph | shallow/deep copy; mutable child isolation; no resource cloning |
| Creational | Singleton | immutable process metadata | safe initialization; global access versus DI singleton lifetime |
| Structural | Adapter | legacy shipping-provider response mapping | units/errors translated; client contract unchanged |
| Structural | Bridge | notification abstraction across delivery implementations | two independent variation dimensions; contrast Adapter |
| Structural | Composite | hierarchical document/layout components | recursive behavior; leaf/container contracts; cycle policy |
| Structural | Decorator | instrumented report generation | composition order, exceptions, cancellation, disposal ownership |
| Structural | Facade | report publication workflow | simplify subsystem use without becoming a god service |
| Structural | Flyweight | repeated immutable document formatting | intrinsic/extrinsic state; sharing and cache/lifetime trade-offs |
| Structural | Proxy | lazy/remote document access simulation | access forwarding, lazy load count, failure semantics; contrast Decorator |
| Behavioral | Chain of Responsibility | routing support requests to handlers | handled/unhandled outcome and stop/continue policy |
| Behavioral | Command | editable document operations | encapsulated request, undo eligibility; contrast CQRS command DTO |
| Behavioral | Interpreter | small permission/filter expression language | grammar, invalid syntax, precedence, and bounded input |
| Behavioral | Iterator | traversal of an owned collection/tree | ordering, empty cases, mutation policy; compare `IEnumerable`/`yield` |
| Behavioral | Mediator | coordinating reservation participants | colleague decoupling; avoid a god mediator; not a library tutorial |
| Behavioral | Memento | editor undo snapshots | state restoration, snapshot isolation, memory/retention cost |
| Behavioral | Observer | in-process progress subscriptions | unsubscribe, duplicate handlers, failure policy, lifetime; not outbox delivery |
| Behavioral | State | order fulfillment transitions | legal/illegal transitions; compare enums and transition tables |
| Behavioral | Strategy | interchangeable pricing policies | behavior contract and selection; compare delegates |
| Behavioral | Template Method | invariant document import steps | step order, extension hooks, inheritance cost; contrast Strategy |
| Behavioral | Visitor | operations across document element types | double dispatch; adding operations versus adding types; compare pattern matching |

Do not force asynchronous APIs onto pure computations. Where operations are naturally asynchronous,
propagate cancellation and document partial effects. Thread-safe initialization is not the same as
thread-safe mutable state, and a DI container's thread safety does not make its resolved objects safe.

## Reading Order Is Not Alphabetical Order

Use three entry points:

1. **Guided path:** recommended sequence with prerequisites and completion checkpoints.
2. **Catalog:** all 23 patterns grouped by canonical category for reference lookup.
3. **Problem index:** start from change pressure, such as incompatible API, varying algorithm, or object graph.

The initial guided path should be Strategy -> Factory Method -> Adapter -> Decorator -> Observer ->
Command -> State. These build useful intuition before Abstract Factory/Bridge/Visitor and less common
patterns. Then complete each category and the comparison articles. Singleton should be taught with
its limitations, not as the default first solution to dependency management.

The runner should eventually support listing demos and selecting a single named pattern/scenario.
Document the actual implemented command syntax when it exists; do not publish commands for an empty runner.
Its default behavior should show help or a short guided starting point, not hundreds of lines of output.

## Required Article Contract

Each pattern article must include:

1. A concrete original requirement and the change that exposes a design problem.
2. A correct baseline and why adding this pattern is justified—or not.
3. Canonical intent and named participant responsibilities.
4. A small collaboration diagram only when it improves understanding.
5. A source map with specific files and a trace of one execution.
6. Invariants, authority, lifecycle, cancellation, and failure behavior where relevant.
7. Focused tests, expected sample output, and runnable commands.
8. Modern C#/.NET alternatives and distinctions from superficially similar patterns.
9. Costs, misuse, and conditions under which the pattern should be removed.
10. Exercises at recognition, refactoring, and extension levels with explicit acceptance criteria.
11. Primary references and previous/next navigation.

Use the existing [metadata schema](article-metadata-schema.md). Assign globally unique article slugs,
reserve Phase 10 orders 100+ for this new track, and use explicit prerequisite slugs. The filesystem
category is not the sole source of reading order. Do not renumber existing published architecture articles.

## Comparisons Needed Beyond the Catalog

- Simple Factory versus static factory method versus GoF Factory Method versus Abstract Factory.
- Builder versus constructors, named factories, records, and object initializers.
- Adapter versus Bridge versus Facade versus Proxy versus Decorator.
- Strategy versus State versus Template Method.
- Command pattern versus application/CQRS command versus background job/message.
- Observer versus .NET events versus reactive streams versus durable integration events.
- Mediator pattern versus mediator library versus event bus.
- Iterator versus Composite traversal versus Visitor.
- Singleton pattern versus DI singleton versus static utility versus service locator.
- GoF versus enterprise/application/distributed patterns and architectural styles.

## Delivery Slices and Completion Gates

| Slice | Deliverable | Gate before the next slice |
|---|---|---|
| 0 | approved scope, track navigation, Phase 02 teaching corrections | boundaries clear; no premature project scaffold |
| 1 | first complete Strategy lesson and minimal runner/library/test path | baseline/refactor/alternative, docs, tests, deterministic output |
| 2 | Factory Method and Adapter, establishing other category paths | category solutions and master inventory valid |
| 3 | Decorator, Observer, Command, State | lifecycle/failure semantics and comparison tests |
| 4 | remaining creational patterns and group comparison | all five implemented; Singleton caveats explicit |
| 5 | remaining structural patterns and group comparison | all seven implemented; ownership/cycle/sharing cases covered |
| 6 | remaining behavioral patterns and group comparison | all eleven implemented; language alternatives explained |
| 7 | comparative articles and graded exercises | no hidden skipped tests or unimplemented “finished” exercises |
| 8 | focused order-processing refactoring case study | only justified patterns; application behavior preserved by tests |
| 9 | completion audit, catalog/problem indexes, handoff to Phase 11 | 23-pattern evidence matrix and all repository gates pass |

The seven-project layout is the destination of the initial category slices, not a demand to create
all seven in slice 1. Add runnable projects only when they have delivered content. Do not use
`NotImplementedException` placeholders or skipped tests to make a scaffold look complete.

Use small commits: runnable pattern+tests, explanatory article/comparison, and workspace integration
when needed. Keep each slice buildable. New package usage requires central versions, complete lock
graph updates, and the existing security audit. Quantity of classes/lines is not an acceptance metric.

## Testing Strategy

Test observable behavior and pattern-specific guarantees rather than every internal method call.
Examples include copy isolation for Prototype/Memento, family consistency for Abstract Factory,
subscription lifetime for Observer, and ordering/cancellation propagation for Decorator.

Use deterministic synchronization for concurrency tests instead of sleeps. Do not assert a particular
worker wins a race. Keep network/providers simulated in the object-pattern catalog; use real adapter
integration tests in later application examples when external behavior is the subject of the lesson.

Bound parser input, traversal depth, sharing caches, and history buffers where relevant. Explain these
budgets as scenario decisions. Benchmark only an actual performance question, with reproducible setup;
do not manufacture benchmark projects for all patterns.

## Case Study Policy

After core lessons, implement a small order-processing refactoring story: interchangeable pricing,
provider adaptation, instrumentation, and legal fulfillment transitions. Begin with a correct baseline,
then introduce one requirement at a time and record the smallest justified design change.

The case study is not an excuse to insert every GoF pattern. Include a decision log showing patterns
explicitly rejected and why. Preserve behavior tests through refactoring and explain differences from
the existing enrollment architecture case. Catalog code is teaching code, not a shared framework that
the case study must import wholesale.

## Acceptance Checklist

- [ ] Placement and navigation accepted; existing phase numbering and working paths preserved.
- [ ] Phase 02 stays introductory and unsafe/misleading snippets have explicit teaching boundaries.
- [ ] All 23 patterns have implemented examples, tests, articles, and documented alternatives.
- [ ] Guided path, catalog index, and problem index agree with delivered content.
- [ ] Focused category solutions and root inventory remain correct.
- [ ] Ownership, failure, cancellation, and lifecycle behavior are explained where applicable.
- [ ] Comparison articles prevent common taxonomy/name confusion.
- [ ] Exercises and case study have real acceptance evidence, not placeholders.
- [ ] Formatting, locked restore, build/test, documentation, links, and security gates pass.
- [ ] Completion audit distinguishes learning coverage from production guarantees.

## Primary References

- [GoF book and canonical table of contents](https://www.informit.com/store/design-patterns-elements-of-reusable-object-oriented-9780321770462): taxonomy and pattern intent, not a .NET implementation guide.
- [.NET dependency injection guidelines](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines): lifetime, ownership, and singleton pitfalls.
- [Existing architecture track](../10-architecture/README.md).
- [Existing OOP introduction](../02-oop/docs/07-oop-patterns.md).

## Navigation

- Start: [Repository roadmap](../README.md).
- Foundation: [OOP phase](../02-oop/README.md).
- Advanced architecture: [Phase 10](../10-architecture/README.md).
