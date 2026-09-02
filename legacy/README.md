# Legacy Archive

This directory preserves earlier experiments and historical learning projects. It is intentionally
outside `learn-dotnet.slnx`, the phase documentation contract, CI build/test gates, and the root Central
Package Management catalog.

The local `Directory.Packages.props` boundary keeps package versions declared by these projects intact.
Without that boundary, repository-wide tools such as GitHub Automatic Dependency Submission discover
legacy project files but apply the root central-package policy, causing `NU1008` before dependency
analysis can complete. `Directory.Build.props` likewise prevents archived restores from generating
new lock files or inheriting maintained-workspace compiler policy.

Security and dependency scanners may still inventory this tracked source. A successful restore only
makes that inventory possible; it does not promote these projects into the maintained learning path or
promise that they build, test, or follow current architecture guidance. Move a project out of this
archive only through an explicit rehabilitation change with documentation, tests, package centralization,
and inclusion in the master solution.
