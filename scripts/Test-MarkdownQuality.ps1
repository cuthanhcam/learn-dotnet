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
        "09-auth",
        "10-architecture",
        "11-testing",
        "docs",
        "README.md",
        "START-HERE.md",
        "CONTRIBUTING.md",
        "SECURITY.md",
        "CHANGELOG.md"
    )
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$errors = [System.Collections.Generic.List[string]]::new()
$checkedFiles = 0

foreach ($root in $Roots) {
    $absoluteRoot = Join-Path $repositoryRoot $root
    if (-not (Test-Path -LiteralPath $absoluteRoot)) {
        throw "Configured Markdown root does not exist: $root"
    }

    $markdownFiles = if (Test-Path -LiteralPath $absoluteRoot -PathType Leaf) {
        @(Get-Item -LiteralPath $absoluteRoot)
    }
    else {
        Get-ChildItem -LiteralPath $absoluteRoot -Recurse -File -Filter "*.md" |
            Where-Object { $_.FullName -notmatch '[\\/](bin|obj|artifacts|BenchmarkDotNet\.Artifacts)[\\/]' }
    }

    foreach ($file in $markdownFiles) {
        $checkedFiles++
        $lines = Get-Content -LiteralPath $file.FullName -Encoding utf8
        $relativePath = $file.FullName.Substring($repositoryRoot.Length).TrimStart('\', '/')
        $h1Count = 0
        $fence = $null

        for ($index = 0; $index -lt $lines.Count; $index++) {
            $line = $lines[$index]
            $lineNumber = $index + 1

            if ($line -match '^\s*(```+|~~~+)') {
                $marker = $Matches[1].Substring(0, 3)
                if ($null -eq $fence) {
                    $fence = $marker
                }
                elseif ($fence -eq $marker) {
                    $fence = $null
                }
                continue
            }

            if ($null -eq $fence -and $line.Contains("`t")) {
                $errors.Add("${relativePath}:${lineNumber}: tab character is not allowed outside code blocks")
            }

            if ($null -eq $fence -and $line -match '^#\s+\S') {
                $h1Count++
            }
        }

        if ($h1Count -ne 1) {
            $errors.Add("${relativePath}: expected exactly one level-one heading; found $h1Count")
        }
        if ($null -ne $fence) {
            $errors.Add("${relativePath}: unclosed fenced code block")
        }
    }
}

if ($errors.Count -gt 0) {
    $errors | Sort-Object | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Output "Validated Markdown structure for $checkedFiles publishable articles."
$global:LASTEXITCODE = 0
