---
title: "Advanced Relationships and Graph Loading"
description: "Model explicit many-to-many joins, composite keys, aggregate collections, cascade ownership, filtered includes, cartesian explosion, and split-query consistency."
slug: ef-core-relationships-graph-loading
phase: 8
order: 5
difficulty: advanced
article-type: deep-dive
estimated-reading-minutes: 50
topics: [ef-core, relationships, many-to-many, includes, split-queries, cascade-delete]
prerequisites: [ef-core-querying-loading-performance]
status: maintained
last-reviewed: 2026-08-30
---

# Advanced Relationships and Graph Loading

Real EF Core models quickly move beyond one parent and one collection. Multiple relationships introduce
join entities, composite keys, ownership decisions, cascade paths, graph materialization cost, tracking
fixup, and transactional consistency across split queries. This slice adds course tags beside modules
so these trade-offs are executable.

## Model Meaning Before Navigation Convenience

The expanded model is:

```text
Category 1 ── * Course 1 ── * CourseModule
                       1 ── * CourseTag * ── 1 Tag
```

Modules are ordered children of a course. Tags are shared reference entities connected through
`CourseTag`. These relationships look similar as C# collections but have different ownership and delete
semantics.

Ask before mapping:

- Is the related row independently meaningful or owned by the aggregate?
- Can it exist without the principal?
- Can multiple principals share it?
- Which side controls creation, update, and removal?
- What should happen when either side is deleted?
- Does the association itself carry business data?

## Explicit Join Entity

EF Core supports skip navigations for implicit many-to-many relationships. This phase uses explicit
`CourseTag` because the join is an important relational concept and may later gain `AddedAt`, ordering,
source, or auditing fields.

The composite primary key `(CourseId, TagId)` ensures one association per pair. Both foreign keys are
required. Course and tag deletion cascade only their join rows, not the entity on the other side.

An explicit join entity also makes querying, indexing, mutation, and migration review straightforward.
Use implicit many-to-many when the association truly has no behavior or metadata and that simplicity
is likely to remain appropriate.

## Defense in Depth for Duplicate Links

`Course.AddTag` rejects an existing tag ID immediately, producing a clear domain error before database
work. The composite key remains authoritative under concurrency or external database writers. Two
requests can both pass an in-memory check; only the database resolves simultaneous insert attempts.

Application validation and relational constraints serve different purposes and should often coexist.
Translate a provider-specific unique violation into a stable application conflict at the persistence
boundary rather than leaking a raw database exception to HTTP.

## Collection Encapsulation

`Course` stores modules and tag links in private lists and exposes read-only views. EF configurations
select field access for navigation materialization. Application code adds links through behavior rather
than replacing collections wholesale.

This protects common invariants, but EF must still be able to materialize and fix up relationships.
Test real queries after changing field/property access. Reflection-based assumptions can compile while
runtime model construction or materialization fails.

## Cascade and Restrict as Ownership Policy

The model deliberately applies:

- category → course: `Restrict`;
- course → module: `Cascade`;
- course → course-tag: `Cascade`;
- tag → course-tag: `Cascade`.

Deleting a course removes owned modules and association rows but preserves shared tags. The relational
test deletes a course from a fresh context without loading children, proving database cascade behavior
rather than client-side tracked cascade.

Cascade rules can create cycles or multiple cascade paths on some providers. Large cascades can lock or
delete far more data than expected. Review generated migrations and production-provider behavior; never
treat cascade as a cleanup convenience detached from domain ownership.

## Multiple Collection Includes

Loading modules and tag links as sibling collections in one query produces joins conceptually similar
to:

```text
course × modules × course_tags
```

For two modules and two tags, the database produces four rows for one course. EF identity resolution
reconstructs one course, two modules, and two links, so looking only at final CLR counts hides the extra
transfer. Ten-by-ten becomes 100 rows; adding another collection multiplies again.

The single-query test captures one SQL command with multiple joins and documents the expected four-row
cross product. In production, inspect actual execution statistics rather than estimating only from
small fixtures.

## Split Queries

`AsSplitQuery` executes separate commands for the principal and each included collection. The same graph
uses three commands:

1. course;
2. modules;
3. course-tag/tag rows.

This avoids sibling cross multiplication but adds network round trips, command overhead, and a
consistency window. Data can change between statements unless the transaction isolation level provides
the required snapshot.

Neither single nor split is universally correct:

| Shape | Often appropriate |
|---|---|
| Small principal + one small collection | Single query |
| Multiple potentially large sibling collections | Projection or split query |
| API read model | Direct projection, often without entities |
| Consistent mutable aggregate for command | Tracking query under explicit consistency rules |

Global query-splitting configuration can hide local trade-offs. Prefer explicit decisions for important
graphs and command-count tests.

## Filtered Include and Navigation Fixup

Filtered include appears to load only part of a collection:

```csharp
Include(course => course.Modules.Where(module => module.Order > 1))
```

In a tracking context, relationship fixup combines query results with compatible entities already in
the identity map. If all modules were loaded earlier, the filtered navigation can still contain all of
them. EF also considers that navigation loaded, so later loading may not retrieve missing rows.

The test proves both sides:

- an already tracking context returns two modules despite the filter;
- a fresh no-tracking context returns only module two.

Use a short-lived fresh context, no-tracking projection, or explicit read model when a filtered subset
is the contract. Do not reuse a broad context and assume every navigation reflects only the last query.

## Required Versus Optional Relationships

Non-nullable foreign keys make these relationships required. Optional relationships use nullable foreign
keys and need an explicit orphan policy. Nullable reference annotations help model intent but do not
replace Fluent configuration review.

Navigation nullability during entity construction/materialization can differ from database requiredness.
The `null!` navigation initialization acknowledges that EF sets required references during materialization;
application behavior should avoid accessing them before a graph is properly constructed or loaded.

## Alternate Keys and Natural Keys

Course slug and tag name are unique indexes, not primary keys. Foreign keys use stable surrogate GUIDs.
Natural values change, have collation semantics, and can be wider in every dependent index. Use an
alternate key only when another relationship truly targets that business identity; uniqueness alone
usually needs a unique index.

## Relationship Mutation in Disconnected Systems

Do not accept a complete submitted collection and infer deletes by calling `Update(graph)`. Instead:

1. load the current aggregate/links;
2. validate requested IDs and authorization;
3. compute additions and removals by key;
4. add/remove explicit join entities;
5. save under concurrency protection.

For large independent associations, set-based insert/delete commands may be more appropriate than
loading every link. The application must still define idempotency, missing IDs, and concurrent edits.

## Migration Review

`AddCourseTags` adds `tags` and `course_tags`, the composite key, unique tag-name index, foreign-key
indexes, and cascade actions. Migration tests now expect both schema versions. Every model expansion
must update migration-chain tests so a missing committed migration cannot pass CI unnoticed.

## Executable Scenarios

The suite now verifies:

- duplicate tag association is rejected by aggregate behavior;
- a single query with two sibling collections uses one command and multiple joins;
- a split query reconstructs the graph using three commands;
- filtered include is affected by previously tracked navigation fixup;
- a fresh no-tracking filtered include returns the intended subset;
- deleting a course cascades modules/links while preserving shared tags and other courses;
- read-model projection returns ordered tag names without tracking entities;
- the second migration applies and leaves no pending schema changes.

## Review Checklist

- Is every relationship's ownership, requiredness, and delete behavior explicit?
- Does a many-to-many association need its own entity now or likely later?
- Are duplicate associations protected by both behavior and a database key/index?
- Can multiple collection includes multiply rows beyond acceptable limits?
- Would projection or split queries produce a better read contract?
- Is split-query consistency sufficient under the current transaction isolation?
- Can filtered include be polluted by previously tracked entities?
- Are navigation field access and encapsulation tested through materialization?
- Does deleting one side preserve every shared entity that should survive?
- Is the generated relationship migration committed and provider-reviewed?

## Further Reading

- [Many-to-many relationships](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many)
- [Cascade delete](https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete)
- [Eager loading and filtered include](https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager)
- [Single and split queries](https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries)
