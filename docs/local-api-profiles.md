---
title: "Local API Launch Profiles"
description: "Use consistent tracked HTTP/HTTPS profiles, stable loopback ports, and explicit configuration for each maintained API."
slug: local-api-launch-profiles
phase: 0
order: 5
difficulty: reference
article-type: reference
estimated-reading-minutes: 6
topics: [development, configuration, aspnet-core, tooling]
prerequisites: []
status: maintained
last-reviewed: 2026-10-30
---

# Local API Launch Profiles

Each maintained API tracks `Properties/launchSettings.json`. The files share `http` and `https`
profile names, use `Development`, and leave browser launching off. Choose the profile explicitly in
commands or the IDE. The first/default profile is `http`, so basic local runs do not require a certificate.

| API project | `http` URL | Additional `https` URL |
|---|---|---|
| `07-aspnet-core/src/Learning.Api` | `http://127.0.0.1:5070` | `https://localhost:7070` |
| `09-auth/src/Learning.Auth.Api` | `http://127.0.0.1:5090` | `https://localhost:7090` |
| `11-architecture/src/Learning.Architecture.Api` | `http://127.0.0.1:5180` | `https://localhost:7180` |
| `12-testing/src/Learning.Testing.Api` | `http://127.0.0.1:5111` | `https://localhost:7111` |

The `https` profile binds its HTTPS address and the same HTTP loopback address. It needs a local
ASP.NET Core development certificate. For HTTPS lessons, follow the SDK's `dotnet dev-certs https`
instructions and trust the certificate on your own machine if needed. Do not disable TLS validation.
These profiles are development tooling, not production deployment configuration.

## Run from the Repository Root

```powershell
dotnet run --project 07-aspnet-core/src/Learning.Api --launch-profile http
dotnet run --project 12-testing/src/Learning.Testing.Api --launch-profile http
```

Run one server per terminal. Use `--launch-profile https` for the corresponding HTTPS configuration.
Stop with Ctrl+C. No browser is opened automatically; use the documented endpoints or HTTP client.

Phase 09 still needs `Jwt__SigningKey` from local configuration, with at least 256 bits of key material;
see [the token lesson](../09-auth/docs/03-authentication-schemes-jwt.md). Phase 11 still requires a valid
HTTPS identity authority and audience; follow [its configuration tutorial](../11-architecture/docs/06-http-security-errors-versioning.md).
Launch profiles contain no credentials, token issuer, or authorization bypass.

The Phase 12 host is an unauthenticated offline loopback example. Phase 09 security exercises can
require HTTPS; do not treat a successful local HTTP smoke as proof of browser security or deployment readiness.

## CLI Overrides and Shared Configuration

`--no-launch-profile` deliberately ignores this file. To choose your own address, make that explicit:

```powershell
dotnet run --project 12-testing/src/Learning.Testing.Api --no-launch-profile -- --urls http://127.0.0.1:5211
```

Shared launch settings belong in Git so IDE and CLI users start from the same contract. Keep secrets
in environment variables, local user secrets, or a secret manager. Do not put them into a launch profile.
`bin/`, `obj/`, `.vs/`, and `artifacts/` remain ignored generated/local output.

## Old Phase Folders After Pulling

Git removes tracked files during a rename but leaves ignored build output behind. An older checkout
may therefore show `10-architecture/` or `11-testing/` even though the maintained source moved to
`11-architecture/` and `12-testing/`. Inspect these folders before cleaning them: never assume every
leftover directory contains only generated files. A safe reversible option is to archive verified
old `bin/obj` output under ignored `artifacts/local-archive/`.

## Navigation

- [Start Here](../START-HERE.md)
- [Contributing](../CONTRIBUTING.md)
- [Phase reorganization map](phase-reorganization.md)
