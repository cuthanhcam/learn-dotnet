---
title: "Identity and Credential Storage"
description: "Model normalized identity, password hashing, verification, uniqueness, enumeration resistance, and credential lifecycle without inventing cryptography."
slug: auth-identity-credential-storage
phase: 9
order: 2
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 28
topics: [identity, passwords, password-hashing, account-security]
prerequisites: [auth-security-architecture-project-structure]
status: maintained
last-reviewed: 2026-08-31
---

# Identity and Credential Storage

## Learning Objectives

By the end of this article, you should be able to separate an account identifier from its display
form, explain why password hashing is not encryption, use an adaptive framework password hasher,
enforce uniqueness atomically, avoid obvious account-enumeration responses, and plan credential
rehashing and account lifecycle behavior.

## Identity Is a Domain Contract

An email address is often used as both contact data and a login identifier, but those responsibilities
are different. Store the value presented to the user and a documented normalized lookup value. Put a
unique constraint on the normalized value in the authoritative database. An application-level
`Find` followed by `Insert` has a race: two requests can both observe absence before either writes.

The in-memory adapter uses `ConcurrentDictionary.TryAdd` to demonstrate one atomic decision. A
relational implementation should translate uniqueness violations into the same application result.

Syntax validation cannot prove ownership. Registration should later issue a single-use,
purpose-bound, expiring verification token and avoid granting sensitive permissions until ownership
is established.

## Password Hashing Mental Model

Passwords are low-entropy secrets chosen by people. Store a slow, salted, adaptive one-way hash—not
plaintext and not reversible encryption. A random per-password salt prevents equal passwords from
sharing equal stored values and frustrates precomputed tables. A work factor makes every guess more
expensive and can increase as hardware improves.

Use `PasswordHasher<TUser>` for new ASP.NET Core password-login systems rather than designing a hash
format around low-level PBKDF2 calls. Its encoded result contains the format and parameters required
for verification. `SuccessRehashNeeded` is a migration signal: after a successful sign-in, create a
new hash under the current policy and persist it.

Never log the password, encoded password hash, reset token, verification token, access token, or
refresh token. Secret redaction after logging is too late.

## Authentication Failure and Enumeration

These externally visible cases should normally share a generic failure contract:

- unknown identity;
- malformed identity;
- wrong password;
- account state that should not be disclosed before authentication.

Messages alone are not enough. Immediately returning for an unknown account while running an
expensive hash for a known account creates a timing signal. The example verifies the submitted
password against an in-memory dummy hash when no account exists. This reduces a simple timing gap;
it does not guarantee constant-time behavior across networks, caches, stores, or lockout checks.

## Password Policy

This learning module accepts 12–128 characters and does not require arbitrary mixtures of uppercase,
lowercase, digits, and symbols. Length, breached-password screening, password-manager compatibility,
rate limits, and MFA are generally more useful than composition puzzles. Do not silently trim or
normalize passwords: every character is credential material.

The maximum length bounds denial-of-service cost. Apply request-size limits before binding and avoid
performing multiple expensive hashes in one request.

## Account State

Credential correctness is necessary but not sufficient. Before issuing a session, evaluate disabled,
locked, deleted, unverified, or risk-challenged state according to an explicit policy. The example
distinguishes internal results while the eventual HTTP endpoint will map sensitive failures to a
generic public response.

Changing a password, disabling an account, or detecting compromise should revoke relevant refresh
sessions. Short access-token lifetime bounds how long already-issued stateless access tokens remain
usable unless the system adds online revocation or a security-stamp check.

## Implementation Map

| Concern | Code |
|---|---|
| Display and lookup identity | `Domain/Users/EmailAddress.cs` |
| Account state and roles | `Domain/Users/UserAccount.cs` |
| Atomic uniqueness port | `Application/Abstractions/IUserAccountRepository.cs` |
| Password hashing port | `Application/Abstractions/IPasswordHashService.cs` |
| Registration orchestration | `Application/Identity/RegistrationService.cs` |
| Generic credential verification | `Application/Identity/CredentialSignInService.cs` |
| Supported framework hasher | `Infrastructure/Identity/AspNetCorePasswordHashService.cs` |
| Concurrency-safe learning store | `Infrastructure/Identity/InMemoryUserAccountRepository.cs` |

## Common Pitfalls

- Encrypting passwords so they can be recovered.
- Using one global salt or a fast general-purpose hash such as SHA-256.
- Comparing password hashes as a login shortcut instead of using the hasher verifier.
- Lowercasing or trimming password input.
- Returning “email not found” from sign-in or password-reset endpoints.
- Treating email syntax as ownership proof.
- Relying on a pre-insert lookup instead of a unique persistence constraint.
- Ignoring `SuccessRehashNeeded` after increasing the work factor or format version.

## Exercises

1. Add an email-ownership state without leaking it through the sign-in response.
2. Design a database unique constraint and map its provider-specific exception at the adapter edge.
3. Add a breached-password port whose external dependency fails closed or degrades according to a
   documented product decision.
4. Measure password verification cost in a dedicated benchmark; do not turn the unit test into a
   fragile timing assertion.

## References

- [Hash passwords in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-10.0)
- [ASP.NET Core security topics](https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-10.0)

## Navigation

- Previous: [Security architecture and project structure](01-security-architecture-project-structure.md)
- Next: Authentication schemes and JWT validation (planned)
