#requires -Version 7.0
[CmdletBinding()]
param(
    [ValidateSet(10, 11)]
    [int]$Phase = 11
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$phaseDirectory = if ($Phase -eq 10) { "10-architecture" } else { "11-testing" }
$solution = Join-Path $repositoryRoot "$phaseDirectory/$phaseDirectory.slnx"
$settings = Join-Path $repositoryRoot "11-testing/coverage.runsettings"
# Never scan old artifacts: each invocation owns a fresh directory, even when tests fail.
$runId = "{0}-{1}" -f [DateTime]::UtcNow.ToString("yyyyMMddTHHmmssZ"), [Guid]::NewGuid().ToString("N").Substring(0, 8)
$results = Join-Path $repositoryRoot "artifacts/coverage/phase-$Phase/$runId"
$null = New-Item -ItemType Directory -Path $results

function Invoke-CoverageCommand {
    param([string[]]$Arguments)
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE. Results: $results"
    }
}

Push-Location $repositoryRoot
try {
    [xml]$inventory = Get-Content -LiteralPath $solution -Raw
    $projects = @($inventory.SelectNodes('/Solution/Folder[@Name="/tests/"]/Project'))
    if ($projects.Count -eq 0) { throw "No test projects declared in $solution." }
    Invoke-CoverageCommand -Arguments @("restore", $solution, "--locked-mode", "--nologo")
    Invoke-CoverageCommand -Arguments @("build", $solution, "--configuration", "Release", "--no-restore", "--nologo")

    $rows = [System.Collections.Generic.List[string]]::new()
    foreach ($project in $projects) {
        $projectPath = Join-Path (Split-Path -Parent $solution) $project.Path
        $projectName = [IO.Path]::GetFileNameWithoutExtension($projectPath)
        $projectResults = Join-Path $results $projectName
        Invoke-CoverageCommand -Arguments @("test", $projectPath, "--configuration", "Release",
            "--no-build", "--no-restore", "--collect", "XPlat Code Coverage", "--settings", $settings,
            "--logger", "trx", "--results-directory", $projectResults)

        # VSTest stores the collector attachment in a GUID directory. The TRX logger can also copy
        # it under <run>/In/<machine>; counting that copy would duplicate the same measurements.
        $attachmentDirectories = @(Get-ChildItem -LiteralPath $projectResults -Directory | Where-Object {
            $attachmentId = [Guid]::Empty
            [Guid]::TryParse($_.Name, [ref]$attachmentId)
        })
        $reports = @($attachmentDirectories | ForEach-Object {
            Get-ChildItem -LiteralPath $_.FullName -Filter coverage.cobertura.xml -File
        })
        if ($reports.Count -ne 1) {
            throw "Expected one fresh coverage report for $projectName; found $($reports.Count)."
        }
        [xml]$coverage = Get-Content -LiteralPath $reports[0].FullName -Raw
        $modules = @($coverage.SelectNodes('/coverage/packages/package'))
        if ($modules.Count -eq 0) { throw "No covered modules reported for $projectName." }
        foreach ($module in $modules) {
            $line = [double]::Parse($module.GetAttribute("line-rate"), [Globalization.CultureInfo]::InvariantCulture)
            $branch = [double]::Parse($module.GetAttribute("branch-rate"), [Globalization.CultureInfo]::InvariantCulture)
            $lineText = $line.ToString("P1", [Globalization.CultureInfo]::InvariantCulture)
            # A module with no instrumented branches has no meaningful branch percentage.
            $branchLines = @($module.SelectNodes('.//line') | Where-Object { $_.GetAttribute("branch") -ieq "true" })
            $branchText = if ($branchLines.Count -eq 0) { "n/a" }
                else { $branch.ToString("P1", [Globalization.CultureInfo]::InvariantCulture) }
            $rows.Add("| $projectName | $($module.name) | $lineText | $branchText |")
        }
    }

    $summary = @(
        "# Phase $Phase coverage", "", "Run: $runId", "",
        "Each row is one test project's view of a module. Rows overlap; do not sum or average them.",
        "Only loaded, instrumented modules appear. Missing modules are not evidence of full coverage.", "",
        "| Test project | Module | Lines | Branches |", "|---|---|---:|---:|"
    ) + $rows.ToArray()
    $summary | Set-Content -LiteralPath (Join-Path $results "summary.md") -Encoding utf8
    $summary | Write-Output
    Write-Output "Reports and TRX: $results"
}
finally {
    Pop-Location
}
