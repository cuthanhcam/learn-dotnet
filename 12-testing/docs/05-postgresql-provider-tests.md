---
title: "PostgreSQL Provider Tests with Disposable Docker Fixtures"
description: "Run shared persistence contracts and transaction tests against a real PostgreSQL server with isolated databases, bounded startup, cleanup, and dedicated CI."
slug: testing-postgresql-provider-tests
phase: 12
order: 5
difficulty: advanced
article-type: tutorial
estimated-reading-minutes: 18
topics: [testing, postgresql, docker, transactions, concurrency]
prerequisites: [testing-coverage-test-quality, architecture-persistence-transactions-idempotency]
status: maintained
last-reviewed: 2026-10-30
---

# PostgreSQL Provider Tests with Disposable Docker Fixtures

## Why Another Database Boundary?

SQLite tests establish behavior on SQLite. They cannot prove how another server resolves identifiers,
translates updates, enforces constraints, or coordinates separate connections. This lesson selects
PostgreSQL as a second provider for the existing Phase 11 enrollment adapter. The API remains a
SQLite learning host; adding provider tests does not silently change its deployment or HTTP error policy.

The first PostgreSQL run exposed a real schema issue: EF created quoted `Capacity` columns while the
handwritten check expression used unquoted `Capacity`, which PostgreSQL resolves as lowercase.
All eleven shared contract cases failed during setup. Quoting the constraint identifiers made the
same schema work on both providers. This is evidence a fake or SQLite-only test could not supply.

## Run the Fixture

Install PowerShell 7, the repository SDK, and Docker with Compose v2. Start the Linux Docker engine
(Docker Desktop on Windows). From the repository root:

```powershell
docker info
docker compose version
pwsh -NoProfile -File scripts/Test-PostgresProvider.ps1
```

Expect fifteen passing cases: eleven shared store contracts and four provider transaction/constraint
cases. The command requires image-registry access on its first run. Reports live in a fresh directory
under `artifacts/postgres/`; the runner prints the TRX path and preserves server logs there.
If Docker is stopped or setup fails, the command fails. No provider test reports success by skipping.

The [runner](../../scripts/Test-PostgresProvider.ps1) creates a unique Compose project, generates a
temporary password, starts the [pinned server image](../compose.postgres.yml), and waits up to ninety
seconds for health. Docker allocates a loopback-only host port, avoiding a fixed-port collision with
your own database. The image digest identifies the reviewed artifact; updating it is a separate
maintenance change requiring a fixture run.

The password and connection are process environment values restored afterward. Do not run
`docker compose config` into a shared log while these values are set; it expands configuration.
The runner does not print connection arguments. This local fixture uses a privileged database user
to create/drop databases; production application roles should not have that privilege.

## Isolation and Ownership

[PostgresDatabase](../tests/Learning.Testing.PostgresTests/PostgresDatabase.cs) creates a uniquely named
database per test. Names contain only a fixed prefix and generated hexadecimal characters; request
input never becomes a SQL identifier. Each context uses its own connection and disables pooling so
fixture disposal does not leave pooled sessions attached to a database being removed.

The fixture drops only its generated database. The outer runner captures logs and removes only its
generated Compose project, including temporary storage, in `finally`. It restores prior environment
values even after failures. Container data lives in tmpfs; this is disposable test data, not a persistent
development database. Do not attach existing data volumes to this fixture.

If a process is forcibly killed, `finally` cannot run. Inspect `docker compose ls` and remove only the
`learn-dotnet-tests-<generated-id>` project printed for that failed run, using this Compose file and
that exact project name. Avoid global Docker prune commands.

## What Each Test Establishes

| Suite | Evidence | Limit |
|---|---|---|
| Shared store contracts | Identical immutable definition, version, missing-row, and cancellation promises | Does not test every SQL/provider feature |
| Competing connections | Both readers see the old version; exactly one conditional save wins the last seat | Scheduler/lock order is unspecified; no claim of exhaustive race exploration |
| Injected receipt-save failure | Already executed SQL mutation rolls back with receipt/outbox intent | Does not simulate network loss during commit |
| Replay across connections | Stable original result, one receipt, one outbox intent | Does not test simultaneous duplicate request-key races |
| Invalid capacity | PostgreSQL reports the expected server check constraint | Does not replace domain input validation |

The [PostgreSQL contract class](../tests/Learning.Testing.PostgresTests/PostgresOfferingStoreTests.cs)
inherits the same abstract specifications compiled by the Phase 11 suites. The project links that
source file rather than copying assertions or referencing an entire test assembly. Concrete dictionary
and SQLite fixtures stay in their original project. A change to the shared contract therefore reaches
all three implementations.

[Transaction tests](../tests/Learning.Testing.PostgresTests/PostgresTransactionTests.cs) construct
independent contexts and assert durable state through a fresh context. They use no timing sleeps and
do not assume the same connection or change tracker can represent concurrent clients.

## Portable Validation and Provider CI

```powershell
pwsh -NoProfile -File scripts/Validate-Repository.ps1
pwsh -NoProfile -File scripts/Test-PostgresProvider.ps1
```

The first command builds all maintained projects and runs portable cases using
`--filter "Category!=Postgres"`. The provider-only assembly may print that no tests match that filter;
that is deliberate selection, not a skipped or successful provider run. The second command provisions
the required server and executes every provider case. Running the test project directly without
`LEARN_DOTNET_POSTGRES` fails with fixture instructions. Use the runner rather than setting credentials
manually.

[CI](../../.github/workflows/ci.yml) retains portable Windows/Linux jobs and adds a required-by-workflow
Linux PostgreSQL job. It executes the same runner and uploads TRX/server logs even on failure.
This does not modify remote branch rules; inspect all observed checks before merging a PR.
The `/providers/` solution folder also keeps this boundary separate from focused coverage runs.

## Exercise and Remaining Work

Temporarily remove the SQL expected-version predicate on a scratch branch. The competing-write case
must fail because both stale snapshots can report acceptance. Restore the implementation and confirm
all fifteen cases pass again. Inspect the persisted learner set, not only the count of successful calls.

This baseline uses `EnsureCreated` on empty disposable databases. It does **not** deliver reviewed
migrations, populated-schema upgrades, downgrade/recovery, production permissions/TLS, connection
outages, deadlock recovery, or uncertain-commit behavior. Schema evolution remains a separate roadmap
slice; neither this lesson nor Phase 12 is marked fully complete because the baseline passes.

## References

- [Npgsql EF Core provider](https://www.npgsql.org/efcore/)
- [PostgreSQL identifier rules](https://www.postgresql.org/docs/current/sql-syntax-lexical.html)
- [Official PostgreSQL container image](https://hub.docker.com/_/postgres/)

## Navigation

- Previous: [Coverage and test quality](04-coverage-and-test-quality.md)
- Return: [Testing roadmap](00-roadmap.md)
