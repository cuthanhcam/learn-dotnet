---
title: "OpenAPI Contracts with ASP.NET Core 10"
description: "Generate and verify OpenAPI 3.1 JSON and YAML documents, enrich endpoint metadata, and treat API descriptions as tested contracts."
slug: aspnet-core-openapi-contracts-dotnet-10
phase: 7
order: 6
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 36
topics: [aspnet-core, dotnet-10, openapi, json-schema, api-contracts, testing]
prerequisites: [aspnet-core-controllers-model-validation-filters]
status: maintained
last-reviewed: 2026-08-15
---

# OpenAPI Contracts with ASP.NET Core 10

An OpenAPI document is a machine-readable description of an HTTP API. It can drive documentation,
client generation, contract review, gateways, security analysis, and compatibility checks. It is not
automatically correct merely because the application generated it. The quality of the document
depends on endpoint metadata, public DTO design, and tests that catch accidental omissions.

## Why the Phase Targets .NET 10

Phase 07 targets `net10.0` and uses the matching ASP.NET Core 10 packages. .NET 10 is an LTS release,
and its built-in OpenAPI generator emits OpenAPI 3.1 with JSON Schema draft 2020-12 semantics. ASP.NET
Core 10 can also serve the generated document as YAML and exposes the document provider through DI.

Keep target frameworks and framework-coupled packages on the same major version. In particular,
`Microsoft.AspNetCore.Mvc.Testing` hosts the application using ASP.NET Core infrastructure and should
match the application's major runtime. The repository pins stable package versions so a restore is
repeatable; dependency-update automation can propose reviewed upgrades later.

The root `global.json` selects a .NET 10 SDK feature band while allowing an installed later feature
band. CI installs .NET 10 and discovers both `.sln` and `.slnx`, ensuring this phase is not skipped just
because it uses the modern solution format.

## Runtime Document Generation

The API registers the generator and maps document endpoints:

```csharp
builder.Services.AddOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapOpenApi("/openapi/{documentName}.yaml");
}
```

The default document is available as:

```text
/openapi/v1.json
/openapi/v1.yaml
```

The YAML support is document serialization, not an interactive documentation UI. Swagger UI,
Scalar, and similar tools are separate consumers and introduce their own packages, assets, security
configuration, and update lifecycle.

This sample exposes documents only in Development. API descriptions can reveal internal endpoints,
model names, and security requirements. Public production exposure should therefore be an explicit
product decision. A common alternative is generating the document during CI and publishing a vetted
static artifact to a documentation site.

## Where Metadata Comes From

ASP.NET Core combines endpoint metadata, routing, binding rules, result declarations, controller
attributes, and JSON serialization information. Minimal APIs and controllers feed the same document
through different authoring styles.

The product mappings declare:

- stable operation identifiers through `WithName`;
- human-readable intent through `WithSummary`;
- success body types and status codes through `Produces<T>`;
- validation errors through `ProducesValidationProblem`;
- other Problem Details outcomes through `ProducesProblem`.

The order-quote controller uses `[ApiController]`, route/action attributes, `ActionResult<T>`, and
`ProducesResponseType`. ASP.NET Core API Explorer converts those conventions into descriptions.

Metadata must match runtime behavior. Declaring `409` does not make the implementation return it,
and returning `412` without declaring it leaves clients with an incomplete description. Treat the
endpoint code, generated contract, and HTTP integration tests as three views of one behavior.

## OpenAPI 3.1 and JSON Schema

OpenAPI 3.1 aligns its schema dialect with modern JSON Schema. This improves expressiveness and
reduces special-case differences from general JSON Schema tooling. Consumers must nevertheless
support the emitted version. If an older gateway or generator supports only OpenAPI 3.0, configure
the generator intentionally or add a compatible transformation step; do not silently hand it a 3.1
document and assume every downstream tool interprets it correctly.

Schema compatibility is broader than C# source compatibility. These changes can break clients:

- removing or renaming a property;
- making an optional property required;
- narrowing an allowed range or pattern;
- changing nullability;
- changing an enum representation;
- removing a response or media type;
- changing an operation identifier used for generated method names.

Additive changes are often safer but are not universally harmless. Strict consumers may reject
unknown properties, and adding a new enum member can break exhaustive client switches.

## JSON and YAML Are Two Serializations

The JSON and YAML endpoints describe the same logical document. YAML is convenient for human review
and supports concise, multi-line text. JSON has a simpler built-in parser ecosystem and is often
better for automated assertions. Do not maintain independent hand-written copies because they will
drift.

The test suite verifies both surfaces. It parses JSON structurally and asserts that routes from both
Minimal APIs and controllers exist. The YAML test verifies the .NET 10 endpoint, declared OpenAPI
version, and representative path. These are focused smoke contracts; a mature service can snapshot
the normalized document or run a semantic breaking-change tool against the previous release.

## Document and Operation Transformers

Built-in OpenAPI supports transformers for changes that cannot be expressed by ordinary endpoint
metadata. Common uses include:

- setting consistent title, description, license, or contact information;
- adding a correlation header to every relevant operation;
- describing standardized authentication schemes;
- removing internal endpoints from a public document;
- applying schema examples or organization-specific extensions.

Transformers execute during document generation and may resolve services from DI. Keep them
deterministic, cancellation-aware, and free from per-request secrets. Prefer endpoint-local metadata
for endpoint-local facts; use a transformer for genuinely cross-cutting document policy.

## Multiple Documents and Audiences

One application can generate multiple named documents. This is useful for public versus internal
audiences or independently versioned surfaces. Inclusion is controlled from API descriptions and
endpoint metadata. Multiple documents are not an authorization boundary: hiding an operation from a
document does not make its route unreachable. Routing and authorization must still protect it.

Avoid introducing versioned documents without an API evolution policy. A `v2` label alone does not
define URL strategy, deprecation, supported lifetimes, or how clients migrate.

## Build-Time Generation and Contract Review

Runtime generation is ideal for development inspection. Build-time generation is useful when the
document must be:

- stored as a build artifact;
- reviewed in pull requests;
- published without running the API;
- compared against the previous released contract;
- consumed by client-generation jobs.

Build-time generation uses an additional MSBuild package and runs application startup code. Startup
must not require unavailable production secrets or mutate external systems. Configuration validation
should remain strict, but contract generation needs an intentional, safe build environment.

## Testing Strategy

Use several test levels rather than relying on a single large snapshot:

1. Endpoint integration tests prove actual statuses, headers, and bodies.
2. OpenAPI smoke tests prove important routes and versions are present.
3. Schema assertions protect particularly important required fields and formats.
4. Semantic diff tooling detects breaking changes across released documents.
5. Generated-client tests prove at least one real consumer can call the service.

Snapshots are useful only when reviewers understand changes. A huge regenerated file approved
without inspection provides weak protection. Normalize unstable ordering and timestamps before a diff.

## Review Checklist

- Do the application TFM, ASP.NET Core packages, test host, local SDK, and CI SDK agree?
- Is the document exposed only in intended environments?
- Do operation names remain stable for generated clients?
- Are every normal error status and Problem Details shape described?
- Do schemas reflect nullability, validation, and serialization behavior?
- Can every downstream consumer process OpenAPI 3.1?
- Are JSON and YAML generated from one source rather than maintained separately?
- Do tests verify both runtime HTTP behavior and the generated contract?
- Is breaking-change detection part of the release workflow for a public API?

## Further Reading

- [Generate OpenAPI documents with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-10.0)
- [Include OpenAPI metadata](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-10.0)
- [What's new in ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0?view=aspnetcore-10.0)
- [What's new in .NET 10](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview)

