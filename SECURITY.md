# Security Policy

## Reporting A Vulnerability

If you discover a security vulnerability, do not open a public issue.

Please report it by email to: cuthanhcam04@gmail.com
Maintainer profile: https://github.com/cuthanhcam

Include:

- A clear description of the issue.
- Reproduction steps.
- Potential impact.
- Suggested fix (if available).

## Response Process

- We will acknowledge receipt within 3 business days.
- We will investigate and provide status updates.
- We will coordinate disclosure and remediation responsibly.

## Scope

This policy applies to all code and configuration in this repository.

Historical samples under `legacy/` are excluded from maintained builds and may demonstrate outdated
dependencies or practices. Reports showing that maintained documentation directs readers toward an
unsafe pattern remain in scope even when the original example is historical.

## Dependency and Secret Policy

- Maintained NuGet versions are centralized in `Directory.Packages.props`.
- CI rejects dependency graphs with known NuGet vulnerabilities.
- Dependency Review rejects pull requests that introduce high or critical vulnerable dependencies.
- CodeQL analyzes maintained C# code on changes and on a weekly schedule.
- Gitleaks scans pull requests, protected branches, and repository history for credential patterns.
- OpenSSF Scorecard audits repository and workflow security posture without blocking ordinary PRs.
- Secrets must not be committed to `appsettings` files. Use user-secrets, environment variables, or a
  managed secret store and rotate any credential that is accidentally exposed.

Automation ownership, permission boundaries, blocking behavior, and the narrow historical Gitleaks
baseline are documented in [Repository Automation and Security Gates](docs/repository-automation.md).

GitHub advisories and dependency updates do not replace manual review of authentication,
authorization, cryptography, data exposure, and operational configuration.
