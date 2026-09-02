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
last-reviewed: 2026-08-31
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

## References

- [Dependency Review Action](https://github.com/actions/dependency-review-action)
- [GitHub Labeler Action](https://github.com/actions/labeler)
- [Gitleaks Action](https://github.com/gitleaks/gitleaks-action)
- [OpenSSF Scorecard Action](https://github.com/ossf/scorecard-action)

## Navigation

- Previous: [Article metadata schema](article-metadata-schema.md)
- Next: [Repository security policy](../SECURITY.md)
