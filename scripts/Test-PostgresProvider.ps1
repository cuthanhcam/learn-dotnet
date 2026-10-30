#requires -Version 7.0
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$project = "learn-dotnet-tests-$([Guid]::NewGuid().ToString('N'))"
$compose = Join-Path $repositoryRoot "12-testing/compose.postgres.yml"
$testProject = Join-Path $repositoryRoot "12-testing/tests/Learning.Testing.PostgresTests"
$previousPassword = $env:POSTGRES_PASSWORD
$previousConnection = $env:LEARN_DOTNET_POSTGRES
$env:POSTGRES_PASSWORD = [Guid]::NewGuid().ToString("N")
$results = Join-Path $repositoryRoot "artifacts/postgres/$project"
$null = New-Item -ItemType Directory -Path $results

function Invoke-ProviderCommand {
    param([string]$Command, [string[]]$Arguments)
    & $Command @Arguments
    # Do not print arguments: a future command could contain a connection credential.
    if ($LASTEXITCODE -ne 0) { throw "$Command failed with exit code $LASTEXITCODE." }
}

Push-Location $repositoryRoot
try {
    Invoke-ProviderCommand docker @("info", "--format", "{{.ServerVersion}}")
    Invoke-ProviderCommand docker @("compose", "-f", $compose, "-p", $project, "up", "--detach", "--wait", "--wait-timeout", "90")
    $endpoint = & docker compose -f $compose -p $project port postgres 5432
    if ($LASTEXITCODE -ne 0 -or $endpoint -notmatch '^127\.0\.0\.1:(\d+)$') { throw "Cannot determine PostgreSQL loopback port." }
    $env:LEARN_DOTNET_POSTGRES = "Host=127.0.0.1;Port=$($Matches[1]);Username=postgres;Password=$env:POSTGRES_PASSWORD;Database=postgres;Timeout=10;Command Timeout=15;Pooling=false"
    Invoke-ProviderCommand dotnet @("restore", $testProject, "--locked-mode")
    Invoke-ProviderCommand dotnet @("test", $testProject, "--configuration", "Release", "--no-restore",
        "--logger", "trx", "--results-directory", $results)
}
finally {
    try {
        & docker compose -f $compose -p $project logs --no-color *> (Join-Path $results "postgres.log")
        & docker compose -f $compose -p $project down --volumes --remove-orphans
        if ($LASTEXITCODE -ne 0) { Write-Warning "Fixture cleanup failed for Compose project $project." }
    }
    finally {
        $env:POSTGRES_PASSWORD = $previousPassword
        $env:LEARN_DOTNET_POSTGRES = $previousConnection
        Pop-Location
    }
}
