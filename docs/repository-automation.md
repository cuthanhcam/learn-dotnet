---
title: "Repository Automation and Security Gates"
description: "The ownership, permissions, failure policy, and maintenance contract for bots and GitHub Actions used by Learn .NET."
slug: repository-automation-security-gates
phase: 0
order: 1
difficulty: reference
article-type: reference
estimated-reading-minutes: 14
topics: [github-actions, automation, supply-chain-security, maintenance]
prerequisites: [article-metadata-schema]
status: maintained
last-reviewed: 2026-09-02
---

# Repository Automation and Security Gates

## Purpose

Automation should reduce review load without silently changing learning content or making ordinary
contributions fragile. Every workflow therefore has a narrow trigger, explicit timeout, least-privilege
token permissions, and a documented decision about whether failure blocks a pull request.

## Automation Inventory

| Automation | Trigger | Blocking policy | Responsibility |
|---|---|---|---|
| CI | PRs and pushes to `main`/`develop` | Blocking | restore locks, formatting, build, tests, docs, inventory, NuGet audit |
| CodeQL | PRs, protected pushes, weekly | Blocking on PR when required | C# data-flow and security analysis |
| Dependency Review | PRs into `main`/`develop` | High/critical additions block | review dependency diff before merge |
| Dependabot | Weekly | Never auto-merges | grouped NuGet and GitHub Actions update PRs |
| External links | Weekly/manual | Scheduled job only | verify external documentation and maintain one failure issue |
| Gitleaks | PRs, protected pushes, weekly | New findings block | detect credential patterns in committed content and history |
| PR Labeler | PR metadata changes | Non-content mutation only | apply phase and change-type labels |
| OpenSSF Scorecard | `main`, weekly/manual | Does not block PRs | publish repository security-posture findings as SARIF |

## Permission and Trust Rules

- Build and analysis workflows receive read-only repository contents unless an output requires more.
- Code-scanning upload jobs receive only `security-events: write`.
- Scorecard alone receives `id-token: write` because publishing results uses GitHub OIDC.
- The labeler uses `pull_request_target` so fork PRs can be labeled, but it never checks out or executes
  pull-request content. Adding a script or checkout step to that workflow requires a security review.
- No automation auto-merges, rewrites commits, pushes formatting changes, or closes inactive work.
- Third-party actions are updated by Dependabot. Security-sensitive Scorecard actions are pinned to
  immutable commits, with version comments retained for auditability.

## Failure Policy

Dependency Review blocks only newly introduced vulnerabilities rated high or critical. Lower
severity findings remain visible without preventing educational maintenance. The existing NuGet audit
still checks the resolved maintained dependency graph.

Dependency Review requires GitHub's native Dependency Graph to be enabled under **Settings → Advanced
Security**. The workflow remains blocking when that repository capability is unavailable because
silently skipping the action would present a green supply-chain gate that performed no review. It runs
only for pull requests: invoking the action manually without explicit base and head revisions is not a
valid dependency comparison.

GitHub-hosted runners use Node 24 for JavaScript actions during the Node 20 retirement window. Some
actions can still declare `node20` in their internal metadata and therefore produce a migration notice;
do not set `ACTIONS_ALLOW_USE_UNSECURE_NODE_VERSION`. Keep the action on its current supported major
version and let the hosted runner apply the secure runtime until the action publishes native Node 24
metadata.

GitHub Automatic Dependency Submission discovers supported manifests across the entire repository,
including the historical `legacy/` archive. The archive has its own `Directory.Packages.props` and
`Directory.Build.props` boundaries so discovery can restore explicit historical package versions
without inheriting root Central Package Management or generating maintained lock files. The resulting
inventory does not make legacy projects part of `learn-dotnet.slnx` or the CI build/test contract.

External-link failures do not affect every PR because remote sites are transient and rate-limit
automation. The scheduled workflow updates one open issue instead of creating weekly duplicates.

Scorecard assesses repository settings and supply-chain practices. It runs on `main`, not on PRs, so a
temporary platform or publishing failure cannot block feature work.

## Secret-Scanning Baseline

`.gitleaksignore` contains one fingerprint for an old documentation sample already preserved in Git
history. A fingerprint suppresses only that exact historic finding. It must never be used to broadly
exclude a path, rule, phase, or future credential. Ignoring a finding is not revocation: if the
historic value was ever active, rotate it independently.

Generated `.vs`, `bin`, `obj`, `artifacts`, and `temp` content is excluded from Git and therefore from
the hosted scan. A local directory scan may still inspect these files and should interpret generated
development session keys separately from committed credentials.

## Maintainer Checklist

1. Review Dependabot PR release notes and allow CI/security checks to finish before merge.
2. Investigate scanner findings before adding any narrow suppression.
3. Rotate a real credential first; deleting or ignoring it does not invalidate it.
4. Keep action permissions explicit when adding a new API call.
5. Test workflow YAML with `actionlint` and the relevant CLI before push.
6. Do not make a scheduled network audit a required PR check.
7. Review repository rules on GitHub because branch protection and native secret scanning are settings,
   not files that workflows can fully enforce.
8. Keep Dependency Graph enabled; Dependabot alerts and Dependency Review depend on that native graph.

## References

- [Dependency Review Action](https://github.com/actions/dependency-review-action)
- [Enabling the dependency graph](https://docs.github.com/en/code-security/how-tos/secure-your-supply-chain/secure-your-dependencies/enable-dependency-graph)
- [GitHub Labeler Action](https://github.com/actions/labeler)
- [Gitleaks Action](https://github.com/gitleaks/gitleaks-action)
- [OpenSSF Scorecard Action](https://github.com/ossf/scorecard-action)

## Navigation

- Previous: [Article metadata schema](article-metadata-schema.md)
- Next: [Repository security policy](../SECURITY.md)
