---
title: "Change Tracking, Identity Resolution, and Disconnected Updates"
description: "Use EF Core tracking intentionally across read models, aggregate updates, identity-map conflicts, no-tracking queries, change detection, and disconnected project workflows."
slug: ef-core-change-tracking-disconnected-updates
phase: 8
order: 3
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 52
topics: [ef-core, change-tracking, identity-map, no-tracking, disconnected-updates]
prerequisites: [ef-core-migrations-schema-evolution]
status: maintained
last-reviewed: 2026-08-30
---

# Change Tracking, Identity Resolution, and Disconnected Updates

EF Core's change tracker is an identity map plus state manager for one unit of work. It lets application
code load entities, execute domain behavior, detect property changes, order database commands, and
accept persisted values. It also consumes memory and creates conflicts when disconnected graphs are
attached without understanding which instance and state should win.

## Entity States

Each tracked entry has one primary state:

| State | Meaning at `SaveChanges` |
|---|---|
| `Detached` | Context does not manage the instance; no command is generated. |
| `Unchanged` | Row identity and original values are known; no changes detected. |
| `Added` | Generate an insert. |
| `Modified` | Generate an update for marked/detected properties. |
| `Deleted` | Generate a delete. |

After a successful save, added and modified entries generally become unchanged, deleted entries become
detached, store-generated values are propagated, and current values become the new original values.
`SaveChanges(false)` defers acceptance and is an advanced coordination tool—not a routine optimization.

## Identity Map

One context tracks at most one logical entity instance for a given entity type/key. Repeating a tracked
query for the same course returns the same CLR object. This provides relationship fixup and prevents
two conflicting in-memory versions from being persisted unknowingly.

Trying to attach a second `Course` instance with the same key throws `InvalidOperationException`.
Critically, the failed operation may already leave both instances visible in tracker entries. EF Core
documents some `InvalidOperationException` failures as unrecoverable programming errors. Dispose that
unit of work instead of catching the exception and continuing to issue database commands.

This happens frequently in real projects when code:

- queries an entity and later maps a DTO into a new entity with the same key;
- combines graphs deserialized from multiple requests;
- shares entity instances between contexts;
- uses a long-lived context that has forgotten what it already tracks;
- calls `Update` on a graph while related entities are already loaded.

## Tracking Queries for Commands

The normal update workflow is:

1. query the authoritative entity in the current context;
2. apply allowed domain methods or explicitly map allowed fields;
3. save once at the consistency boundary.

The snapshot tracker compares original and current values. `CourseEditor` implements this workflow for
a disconnected command. The caller supplies scalar input, not an entity graph. The editor reloads the
course, applies `UpdateDetails`, increments its version, and saves tracked differences.

This costs a read before the update but provides current state for business rules, correct original
values for optimistic concurrency, and a narrow over-posting boundary. Performance exceptions require
an equally explicit correctness design.

## Why Blind `Update` Is Dangerous

`DbContext.Update(entity)` recursively marks reachable unknown entities as modified or added according
to key state. With a DTO-shaped graph, this can:

- overwrite columns the caller never had permission to edit;
- replace newer values with stale serialized values;
- insert or update related rows unintentionally;
- mark every column modified and enlarge SQL/audit noise;
- collide with an instance already in the identity map;
- bypass meaningful aggregate behavior.

Do not expose EF entities as API request contracts. Map an allowlist of fields onto a tracked aggregate,
or use a carefully designed set-based/attach-stub command where every modified property and concurrency
condition is explicit.

## Read Models and `AsNoTracking`

Read-only queries should normally avoid tracking entities they will not modify. `CourseQueries` starts
with `AsNoTracking` and projects in SQL to `CourseListItem` or `CourseDetails`. Projection selects only
required columns, aggregates module count in the database, and avoids materializing a complete entity
graph.

After the queries, the context tracker remains empty. This is stronger evidence than simply calling
`AsNoTracking`: a test guards the intended read boundary.

No-tracking is not a universal performance switch. If a unit of work will modify an entity, tracking
it once is often simpler and more efficient than detached mutation plus attach/merge logic. Also note
that projections containing entity instances can still track those entities unless the query is
explicitly no-tracking.

## `AsNoTrackingWithIdentityResolution`

A normal no-tracking query does not use the context identity map. If multiple result rows reference the
same category, separate category objects can be materialized. `AsNoTrackingWithIdentityResolution`
uses a temporary stand-alone tracker during enumeration so repeated keys share one CLR instance, then
discards that tracker. The returned graph is not attached to the context.

Use it when a read-only graph has repeated entities and reference identity/memory reduction matters.
It adds tracking work during materialization, so compare it with direct DTO projection rather than
applying it automatically.

## Change Detection

Snapshot change tracking normally runs automatically before APIs that require current state, including
`SaveChanges`. Disabling `AutoDetectChangesEnabled` can improve specific large batch workflows, but it
creates a correctness obligation.

The test demonstrates the failure mode:

1. load a tracked course;
2. disable automatic detection;
3. mutate the course;
4. save produces zero updates;
5. call `DetectChanges` explicitly;
6. save persists one update;
7. restore the original setting in `finally`.

Never disable change detection globally because a benchmark showed overhead in an artificial loop.
Measure a representative batch, minimize tracked graph size first, and restore configuration even when
work throws.

Notification entities and change-tracking proxies offer alternative detection strategies but add
interface/proxy constraints and do not remove identity-map or lifecycle concerns.

## Original, Current, and Database Values

For a tracked property EF can expose:

- original value captured when tracking began;
- current in-memory value;
- database value queried separately.

Concurrency resolution later uses all three to decide whether to reject, overwrite, or merge. Calling
`Reload` discards local changes; setting original values affects the concurrency predicate. These are
business decisions, not generic exception-retry mechanics.

## `ChangeTracker.Clear`

`Clear` efficiently detaches every entry without firing individual detach operations. It can be useful
in intentional batch loops using one context, but routine web units of work should dispose the context.
Clearing discards tracked changes and original values and can mask an overlong context lifetime.

Never use `Clear` as recovery from a programming error merely to keep a damaged context alive. Create
a new context and restart the unit of work from authoritative persisted state.

## Query Boundaries in a Real Project

`CourseQueries` owns read expressions and returns read models. `CourseEditor` owns one command workflow.
This is not a requirement to wrap every `DbSet` method in a generic repository. A generic repository
often hides EF capabilities while reproducing an incomplete query API.

Useful boundaries are use-case or aggregate oriented:

- keep provider-translated LINQ in persistence/application query components;
- prevent controllers from constructing arbitrary tracked graphs;
- make transaction ownership visible;
- allow focused test doubles above EF only where the project needs them;
- still test important query expressions against the actual provider.

## Testing Scenarios

The executable suite covers:

- two tracked queries return the same instance;
- detected scalar changes generate one update and become unchanged after save;
- mutating a no-tracking entity produces no database update;
- attaching a duplicate-key instance fails and contaminates tracker state;
- identity-resolution no-tracking shares navigation references without context tracking;
- disabling detection silently loses an update until explicit detection;
- projected read workflows leave the tracker empty;
- disconnected updates reload and modify only allowed fields;
- missing update targets return an expected outcome instead of throwing.

These tests model bugs seen in APIs, message consumers, import jobs, and long-lived desktop contexts—not
only isolated ORM syntax.

## Review Checklist

- Is this query a read model or part of a command unit of work?
- Does the context already track an instance with this key?
- Are API/message DTOs kept separate from persistence entities?
- Does an update map only fields the caller may change?
- Are original values available for concurrency checks?
- Does a no-tracking query project only required columns and leave the tracker empty?
- Is identity resolution valuable for this graph, or would projection be clearer?
- If automatic detection is disabled, is explicit detection and `finally` restoration guaranteed?
- Will an `InvalidOperationException` cause context disposal rather than continued use?
- Is the context lifetime short enough that `Clear` is unnecessary?

## Further Reading

- [Change tracking in EF Core](https://learn.microsoft.com/en-us/ef/core/change-tracking/)
- [Tracking versus no-tracking queries](https://learn.microsoft.com/en-us/ef/core/querying/tracking)
- [Disconnected entities](https://learn.microsoft.com/en-us/ef/core/saving/disconnected-entities)
- [Explicitly tracking entities](https://learn.microsoft.com/en-us/ef/core/change-tracking/explicit-tracking)
