---
title: "Article Metadata Schema"
description: "The canonical front-matter contract used to map Learn .NET documents into a future blog or static-site article catalog."
slug: article-metadata-schema
phase: 0
order: 0
difficulty: reference
article-type: reference
estimated-reading-minutes: 8
topics: [documentation, metadata, blog]
prerequisites: []
status: maintained
last-reviewed: 2026-08-15
---

# Article Metadata Schema

Every publishable Markdown document starts with YAML front matter. The metadata is intentionally portable across GitHub, common static-site generators, and a future custom blog pipeline.

## Required Fields

| Field | Type | Meaning |
|---|---|---|
| `title` | string | Human-readable article title |
| `description` | string | Search/card summary written as one complete sentence |
| `slug` | kebab-case string | Stable URL identifier; changing it requires a redirect |
| `phase` | integer | Learning phase number; `0` is repository-wide reference material |
| `order` | integer | Position within the phase |
| `difficulty` | enum | `beginner`, `intermediate`, `advanced`, or `reference` |
| `article-type` | enum | `roadmap`, `tutorial`, `concept`, `deep-dive`, `reference`, or `pitfalls` |
| `estimated-reading-minutes` | integer | Approximate focused reading time, excluding exercises |
| `topics` | string array | Search/filter tags using stable lowercase slugs |
| `prerequisites` | string array | Article slugs that should normally be understood first |
| `status` | enum | `draft`, `reviewed`, or `maintained` |
| `last-reviewed` | ISO date | Date of the latest technical/content review |

Phase README files may additionally declare `target-framework`, `previous-phase`, and `next-phase`. Those fields describe runnable module navigation rather than article taxonomy.

## Stability Rules

- A slug is an external identifier; do not derive it from a translated title at runtime.
- `order` controls learning sequence, not publication date.
- Prerequisites reference slugs, not filesystem paths, so articles can move without breaking the knowledge graph.
- Topics describe concepts, not incidental code symbols.
- Review dates indicate when version-sensitive claims were checked, not when every sentence was originally written.
- `maintained` means the article has code/tests or an active review owner; it does not claim the topic will never change.

## Article Body Contract

A full tutorial or deep dive should normally contain:

1. context and learning objectives;
2. prerequisites and vocabulary;
3. mental model;
4. API or syntax walkthrough;
5. invariants, contracts, or correctness reasoning;
6. trade-offs and selection guidance;
7. runnable implementation map;
8. testing strategy;
9. common pitfalls;
10. exercises and review questions;
11. previous/next navigation;
12. primary official references.

Short roadmap and index documents may omit implementation details, but must still provide navigation and completion criteria.

## Reference Policy

Prefer primary sources: Microsoft Learn, .NET API reference, dotnet/runtime design or source material, C# language specification, and original research for algorithms. Secondary sources can provide intuition but should not be the sole authority for API contracts or version-sensitive behavior.
