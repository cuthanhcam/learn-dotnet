---
title: "Dependency Injection, Configuration, and Options"
description: "Service lifetimes, scopes, captive dependencies, configuration precedence, typed options, validation, secrets, and test overrides."
slug: aspnet-core-di-configuration-options
phase: 7
order: 2
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 34
topics: [aspnet-core, dependency-injection, configuration, options]
prerequisites: [aspnet-core-hosting-request-pipeline]
status: maintained
last-reviewed: 2026-08-15
---

# Dependency Injection, Configuration, and Options

## Service Lifetimes

| Lifetime | Meaning | Typical example |
|---|---|---|
| Transient | new instance per resolution | small stateless operation |
| Scoped | one instance per request scope | unit of work / DbContext |
| Singleton | one instance from the root provider | thread-safe shared service |

A singleton must be safe for concurrent requests and must not directly capture a scoped dependency.
Creating scopes manually is an ownership decision normally reserved for background processing or
infrastructure boundaries—not a workaround for arbitrary lifetime mismatches.

The in-memory repository is singleton and uses a concurrent dictionary because all requests share it.
Its methods still receive cancellation and return snapshots with deterministic ordering.

## Configuration

ASP.NET Core composes providers such as JSON files, environment-specific JSON, user secrets in
development, environment variables, and command-line arguments. Later providers can override earlier
ones. Treat configuration as hierarchical external input and understand deployment-specific keys.

Do not store production secrets in committed JSON. Use a secret manager or platform facility and
avoid logging bound option objects that may contain credentials.

## Typed and Validated Options

`LearningOptions` binds one section, uses data annotations, and validates on startup. Startup validation
is appropriate because an empty catalog name or invalid maximum page size makes the service
misconfigured before it handles traffic.

Choose the interface by update semantics:

- `IOptions<T>`: stable singleton-style value;
- `IOptionsSnapshot<T>`: scoped recomputation, commonly per request;
- `IOptionsMonitor<T>`: current values and change notifications for long-lived services.

Reloadability does not make every dependency reconfigure safely. Validate changes, define failure
behavior, and dispose change registrations owned by long-lived components.

## Testing Overrides

Integration tests can replace registrations or configuration in the test host. Prefer overriding the
public composition boundary rather than adding production-only setters. Validate both correct startup
and deliberately invalid configuration.

## References

- [Dependency injection in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Options pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)

## Navigation

- Previous: [Hosting and request pipeline](01-hosting-request-pipeline.md)
- Next: [Routing, validation, and Problem Details](03-routing-validation-problem-details.md)
