---
title: "Authentication and Authorization Learning Roadmap"
description: "An ordered security path from threat modeling and credential verification to token lifecycle, authorization, abuse resistance, and operations."
slug: auth-authorization-roadmap
phase: 9
order: 0
difficulty: intermediate
article-type: roadmap
estimated-reading-minutes: 18
topics: [authentication, authorization, security, roadmap]
prerequisites: [ef-core-phase-08-completion-audit, aspnet-core-phase-07-completion-audit]
status: maintained
last-reviewed: 2026-08-31
---

# Authentication and Authorization Learning Roadmap

## Goal

Build a precise mental model for who establishes identity, which evidence is trusted, how credentials
and sessions expire or are revoked, where authorization is evaluated, and how failures remain secure
under replay, concurrency, key compromise, and hostile input.

## Progressive Layers

1. Threat actors, assets, entry points, trust boundaries, and security invariants.
2. Identity normalization, credential hashing, verification, and account lifecycle.
3. ASP.NET Core authentication schemes, handlers, challenges, and forbids.
4. JWT issuance and strict validation of signature, issuer, audience, lifetime, and algorithm.
5. Refresh-token digest storage, one-time rotation, token families, replay response, and revocation.
6. Claims transformation, roles, policies, custom requirements, and least privilege.
7. Resource-based authorization after loading the authoritative resource.
8. OAuth 2.0 and OpenID Connect roles and flows, including Authorization Code with PKCE.
9. Cookie, CSRF, CORS, browser storage, HTTPS, proxy, and secret-management boundaries.
10. Enumeration resistance, throttling, lockout, audit events, key rotation, and incident response.

## Study Loop

For each slice:

1. Name the protected asset and attacker capability.
2. State the invariant before selecting an API or protocol.
3. Identify every source of identity and authorization data.
4. Implement the smallest explicit boundary and its failure contract.
5. Test malformed, expired, replayed, duplicated, and concurrent inputs.
6. Record what is safe for learning only and what is suitable for production deployment.
7. Add observability without logging credentials, tokens, passwords, or sensitive claims.

## Security Honesty

This phase implements a compact first-party token system to make cryptographic and session invariants
observable. That is not a recommendation to build a general OAuth 2.0 or OpenID Connect provider.
Production systems should normally delegate protocol implementation, federation, MFA, recovery, and
key lifecycle to a maintained identity platform and validate the tokens it issues.

## Navigation

- Previous: [Phase 08 — Entity Framework Core](../../08-ef-core/README.md)
- Next: [Security architecture and project structure](01-security-architecture-project-structure.md)
