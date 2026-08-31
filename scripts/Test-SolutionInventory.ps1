[CmdletBinding()]
param(
    [string]$Solution = "learn-dotnet.slnx",
    [string[]]$Roots = @(
        "01-csharp-basics",
        "02-oop",
        "03-core-dotnet",
        "04-memory-performance",
        "05-dsa",
        "06-async-concurrency",
        "07-aspnet-core",
        "08-ef-core",
        "09-auth"
    )
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$solutionPath = Join-Path $repositoryRoot $Solution

if (-not (Test-Path -LiteralPath $solutionPath)) {
    throw "Master solution does not exist: $Solution"
}

$expected = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($root in $Roots) {
    $absoluteRoot = Join-Path $repositoryRoot $root
    foreach ($project in Get-ChildItem -LiteralPath $absoluteRoot -Recurse -File -Filter "*.csproj" |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj|artifacts)[\\/]' }) {
        $relative = $project.FullName.Substring($repositoryRoot.Length).TrimStart('\', '/').Replace('\', '/')
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

Write-Output "Validated master solution inventory: $($actual.Count) maintained projects."
$global:LASTEXITCODE = 0
