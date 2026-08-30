[CmdletBinding()]
param(
    [string]$Solution = "learn-dotnet.slnx"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $Solution)) {
    throw "Solution does not exist: $Solution"
}

$output = & dotnet list $Solution package --vulnerable --include-transitive 2>&1
$exitCode = $LASTEXITCODE
$output | Write-Host

if ($exitCode -ne 0) {
    throw "NuGet vulnerability audit failed with exit code $exitCode."
}

# `dotnet list package --vulnerable` reports findings in text while still returning zero. Treat any
# vulnerability section as a failing quality gate instead of accepting a successful process exit.
if ($output -match "has the following vulnerable packages") {
    throw "Vulnerable NuGet packages were detected. Review the advisory output above."
}

Write-Output "Validated NuGet dependency graph: no known vulnerabilities reported."
