#requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot

function Invoke-DotNetGate {
    param([string[]]$Arguments)

    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

# Resolve paths from the script, so invoking this gate from another directory is safe.
Push-Location $repositoryRoot
try {
    Invoke-DotNetGate -Arguments @("tool", "restore")
    Invoke-DotNetGate -Arguments @("restore", "learn-dotnet.slnx", "--locked-mode", "--nologo")
    Invoke-DotNetGate -Arguments @("format", "whitespace", "learn-dotnet.slnx", "--verify-no-changes", "--no-restore")
    Invoke-DotNetGate -Arguments @("build", "learn-dotnet.slnx", "--configuration", "Release", "--no-restore", "--nologo")
    Invoke-DotNetGate -Arguments @("test", "learn-dotnet.slnx", "--configuration", "Release", "--no-build", "--no-restore", "--nologo", "--logger", "trx", "--results-directory", "artifacts/test-results", "--collect", "XPlat Code Coverage")

    foreach ($gate in @("Test-ArticleMetadata.ps1", "Test-MarkdownLinks.ps1", "Test-MarkdownQuality.ps1", "Test-SolutionInventory.ps1", "Test-PackageVulnerabilities.ps1")) {
        & (Join-Path $PSScriptRoot $gate)
        if ($LASTEXITCODE -ne 0) {
            throw "$gate failed with exit code $LASTEXITCODE."
        }
    }
    Write-Output "All maintained repository gates passed."
}
finally {
    Pop-Location
}
