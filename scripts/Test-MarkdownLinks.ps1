[CmdletBinding()]
param(
    [string[]]$Roots = @(
        "01-csharp-basics",
        "02-oop",
        "03-core-dotnet",
        "04-memory-performance",
        "05-dsa",
        "06-async-concurrency",
        "07-aspnet-core",
        "08-ef-core",
        "docs",
        "README.md",
        "CONTRIBUTING.md",
        "SECURITY.md",
        "CHANGELOG.md"
    )
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$failures = [System.Collections.Generic.List[string]]::new()
$checkedLinks = 0

foreach ($root in $Roots) {
    $absoluteRoot = Join-Path $repositoryRoot $root
    if (-not (Test-Path -LiteralPath $absoluteRoot)) {
        $failures.Add("Configured Markdown root does not exist: $root")
        continue
    }

    $markdownFiles = if (Test-Path -LiteralPath $absoluteRoot -PathType Leaf) {
        @(Get-Item -LiteralPath $absoluteRoot)
    }
    else {
        Get-ChildItem -LiteralPath $absoluteRoot -Recurse -File -Filter "*.md" |
            Where-Object { $_.FullName -notmatch '[\\/](bin|obj|artifacts|BenchmarkDotNet\.Artifacts)[\\/]' }
    }
    foreach ($file in $markdownFiles) {
        $lineNumber = 0
        foreach ($line in Get-Content -LiteralPath $file.FullName -Encoding utf8) {
            $lineNumber++

            # This intentionally validates local targets only. External URLs need a separate,
            # network-aware check because redirects, authentication, and rate limits are normal.
            $matches = [regex]::Matches(
                $line,
                '!?' + '\[[^\]]*\]\((?<target><[^>]+>|[^\s\)]+)')

            foreach ($match in $matches) {
                $target = $match.Groups['target'].Value.Trim('<', '>')
                if ($target.StartsWith('#') -or
                    $target.StartsWith('/') -or
                    $target -match '^[a-zA-Z][a-zA-Z0-9+.-]*:') {
                    continue
                }

                $checkedLinks++
                $pathWithoutFragment = ($target -split '[?#]', 2)[0]
                if ([string]::IsNullOrWhiteSpace($pathWithoutFragment)) {
                    continue
                }

                $decodedPath = [System.Uri]::UnescapeDataString($pathWithoutFragment)
                $candidate = Join-Path $file.DirectoryName $decodedPath
                if (-not (Test-Path -LiteralPath $candidate)) {
                    $relativeFile = $file.FullName.Substring($repositoryRoot.Length).TrimStart('\', '/')
                    $failures.Add("${relativeFile}:${lineNumber} -> $target")
                }
            }
        }
    }
}

if ($failures.Count -gt 0) {
    Write-Error ("Broken local Markdown links:`n- " + ($failures -join "`n- "))
    exit 1
}

Write-Host "Validated $checkedLinks local Markdown links."
$global:LASTEXITCODE = 0
