[CmdletBinding()]
param(
    [string]$Solution = "learn-dotnet.slnx",
    [string]$WorkspaceSettings = ".vscode/settings.json",
    [string]$ExpectedTargetFramework = "net10.0",
    [string[]]$Roots = @(
        "01-csharp-basics",
        "02-oop",
        "03-core-dotnet",
        "04-memory-performance",
        "05-dsa",
        "06-async-concurrency",
        "07-aspnet-core",
        "08-ef-core",
        "09-auth",
        "10-architecture",
        "11-testing"
    )
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$solutionPath = Join-Path $repositoryRoot $Solution
$workspaceSettingsPath = Join-Path $repositoryRoot $WorkspaceSettings

if (-not (Test-Path -LiteralPath $solutionPath)) {
    throw "Master solution does not exist: $Solution"
}

if (-not (Test-Path -LiteralPath $workspaceSettingsPath)) {
    throw "VS Code workspace settings do not exist: $WorkspaceSettings"
}

$workspaceConfiguration = Get-Content -LiteralPath $workspaceSettingsPath -Raw -Encoding utf8 | ConvertFrom-Json
if ($workspaceConfiguration.'dotnet.defaultSolution' -ne $Solution) {
    throw "VS Code dotnet.defaultSolution must reference the master solution '$Solution'."
}

$expected = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($root in $Roots) {
    $absoluteRoot = Join-Path $repositoryRoot $root
    $nestedSdkSelectors = @(Get-ChildItem -LiteralPath $absoluteRoot -Recurse -File -Filter "global.json" |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj|artifacts|BenchmarkDotNet\.Artifacts)[\\/]' })
    if ($nestedSdkSelectors.Count -gt 0) {
        throw "Maintained phases must inherit the root SDK selection; remove nested global.json: $($nestedSdkSelectors.FullName -join ', ')"
    }

    foreach ($project in Get-ChildItem -LiteralPath $absoluteRoot -Recurse -File -Filter "*.csproj" |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj|artifacts)[\\/]' }) {
        $relative = $project.FullName.Substring($repositoryRoot.Length).TrimStart('\', '/').Replace('\', '/')
        # Keep the learning runtime explicit in each project, rather than silently inheriting a
        # different target or adding a second runtime that the course does not validate.
        [xml]$projectXml = Get-Content -LiteralPath $project.FullName -Raw -Encoding utf8
        $targetFrameworks = @($projectXml.GetElementsByTagName("TargetFramework"))
        if ($targetFrameworks.Count -ne 1 -or
            $targetFrameworks[0].InnerText -ne $ExpectedTargetFramework -or
            $projectXml.GetElementsByTagName("TargetFrameworks").Count -ne 0) {
            throw "${relative}: maintained projects must declare exactly one TargetFramework '$ExpectedTargetFramework'."
        }
        [void]$expected.Add($relative)
    }
}

$solutionXml = Get-Content -LiteralPath $solutionPath -Raw -Encoding utf8
$matches = [regex]::Matches($solutionXml, '<Project\s+Path="(?<path>[^"]+)"\s*/>')
$actual = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($match in $matches) {
    [void]$actual.Add($match.Groups['path'].Value.Replace('\', '/'))
}

$missing = @($expected | Where-Object { -not $actual.Contains($_) } | Sort-Object)
$unexpected = @($actual | Where-Object { -not $expected.Contains($_) } | Sort-Object)

if ($missing.Count -gt 0 -or $unexpected.Count -gt 0) {
    if ($missing.Count -gt 0) {
        Write-Error "Projects missing from master solution:`n- $($missing -join "`n- ")"
    }
    if ($unexpected.Count -gt 0) {
        Write-Error "Unexpected projects in master solution:`n- $($unexpected -join "`n- ")"
    }
    exit 1
}

Write-Output "Validated master solution inventory, VS Code default, and root SDK inheritance: $($actual.Count) maintained $ExpectedTargetFramework projects."
$global:LASTEXITCODE = 0
