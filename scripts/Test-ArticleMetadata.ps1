param(
    [string[]] $Roots = @(
        "01-csharp-basics/docs",
        "02-oop/docs",
        "03-core-dotnet/docs",
        "04-memory-performance/docs",
        "05-dsa/docs",
        "06-async-concurrency/docs",
        "07-aspnet-core/docs",
        "08-ef-core/docs",
        "docs"
    )
)

$ErrorActionPreference = "Stop"

$requiredKeys = @(
    "title",
    "description",
    "slug",
    "phase",
    "order",
    "difficulty",
    "article-type",
    "estimated-reading-minutes",
    "topics",
    "prerequisites",
    "status",
    "last-reviewed"
)

$allowedDifficulty = @("beginner", "intermediate", "advanced", "reference")
$allowedArticleType = @("roadmap", "tutorial", "concept", "deep-dive", "reference", "pitfalls")
$allowedStatus = @("draft", "reviewed", "maintained")
$articles = [System.Collections.Generic.List[object]]::new()
$errors = [System.Collections.Generic.List[string]]::new()

foreach ($root in $Roots) {
    if (-not (Test-Path -LiteralPath $root)) {
        continue
    }

    foreach ($file in Get-ChildItem -LiteralPath $root -Recurse -Filter *.md) {
        $lines = Get-Content -LiteralPath $file.FullName -Encoding utf8
        if ($lines.Count -lt 3 -or $lines[0] -ne "---") {
            $errors.Add("$($file.FullName): missing YAML front matter")
            continue
        }

        $closing = [Array]::IndexOf($lines, "---", 1)
        if ($closing -lt 2) {
            $errors.Add("$($file.FullName): front matter has no closing delimiter")
            continue
        }

        $metadata = @{}
        for ($index = 1; $index -lt $closing; $index++) {
            if ($lines[$index] -match '^([a-z][a-z0-9-]*):\s*(.*)$') {
                $metadata[$Matches[1]] = $Matches[2].Trim().Trim('"')
            }
        }

        foreach ($key in $requiredKeys) {
            if (-not $metadata.ContainsKey($key)) {
                $errors.Add("$($file.FullName): missing '$key'")
            }
        }

        if ($metadata.ContainsKey("slug") -and $metadata["slug"] -notmatch '^[a-z0-9]+(?:-[a-z0-9]+)*$') {
            $errors.Add("$($file.FullName): slug must be stable kebab-case")
        }

        if ($metadata.ContainsKey("difficulty") -and $metadata["difficulty"] -notin $allowedDifficulty) {
            $errors.Add("$($file.FullName): unsupported difficulty '$($metadata["difficulty"])'")
        }

        if ($metadata.ContainsKey("article-type") -and $metadata["article-type"] -notin $allowedArticleType) {
            $errors.Add("$($file.FullName): unsupported article-type '$($metadata["article-type"])'")
        }

        if ($metadata.ContainsKey("status") -and $metadata["status"] -notin $allowedStatus) {
            $errors.Add("$($file.FullName): unsupported status '$($metadata["status"])'")
        }

        if ($metadata.ContainsKey("last-reviewed") -and $metadata["last-reviewed"] -notmatch '^\d{4}-\d{2}-\d{2}$') {
            $errors.Add("$($file.FullName): last-reviewed must use YYYY-MM-DD")
        }

        foreach ($numericKey in @("phase", "order", "estimated-reading-minutes")) {
            $numericValue = 0
            if ($metadata.ContainsKey($numericKey) -and
                -not [int]::TryParse($metadata[$numericKey], [ref]$numericValue)) {
                $errors.Add("$($file.FullName): '$numericKey' must be an integer")
            }
        }

        $phaseNumber = 0
        if ($metadata.ContainsKey("phase") -and
            [int]::TryParse($metadata["phase"], [ref]$phaseNumber) -and
            $phaseNumber -lt 0) {
            $errors.Add("$($file.FullName): phase must be non-negative")
        }

        if ($metadata.ContainsKey("estimated-reading-minutes")) {
            $readingMinutes = 0
            if ([int]::TryParse($metadata["estimated-reading-minutes"], [ref]$readingMinutes) -and
                $readingMinutes -lt 1) {
                $errors.Add("$($file.FullName): estimated-reading-minutes must be at least 1")
            }
        }

        if ($metadata.ContainsKey("last-reviewed") -and
            $metadata["last-reviewed"] -match '^\d{4}-\d{2}-\d{2}$') {
            $reviewedDate = [datetime]::MinValue
            $validDate = [datetime]::TryParseExact(
                $metadata["last-reviewed"],
                "yyyy-MM-dd",
                [Globalization.CultureInfo]::InvariantCulture,
                [Globalization.DateTimeStyles]::None,
                [ref]$reviewedDate)
            if (-not $validDate) {
                $errors.Add("$($file.FullName): last-reviewed is not a valid calendar date")
            }
        }

        foreach ($arrayKey in @("topics", "prerequisites")) {
            if ($metadata.ContainsKey($arrayKey) -and $metadata[$arrayKey] -notmatch '^\[.*\]$') {
                $errors.Add("$($file.FullName): '$arrayKey' must use an inline YAML array")
            }
        }

        $articles.Add([pscustomobject]@{
            Path = $file.FullName
            Slug = $metadata["slug"]
            Prerequisites = $metadata["prerequisites"]
            Phase = $metadata["phase"]
            Order = $metadata["order"]
        })
    }
}

$articles |
    Where-Object { $_.Slug } |
    Group-Object Slug |
    Where-Object Count -gt 1 |
    ForEach-Object { $errors.Add("Duplicate slug '$($_.Name)'") }

$articles |
    Where-Object { $_.Phase -ne $null -and $_.Order -ne $null } |
    Group-Object { "$($_.Phase):$($_.Order)" } |
    Where-Object Count -gt 1 |
    ForEach-Object { $errors.Add("Duplicate phase/order '$($_.Name)'") }

$knownSlugs = @{}
foreach ($article in $articles | Where-Object Slug) {
    $knownSlugs[$article.Slug] = $true
}
foreach ($article in $articles) {
    if (-not $article.Prerequisites -or $article.Prerequisites -eq "[]") {
        continue
    }

    $dependencies = $article.Prerequisites.Trim('[', ']') -split ',' |
        ForEach-Object { $_.Trim().Trim('"', "'") } |
        Where-Object { $_ }

    foreach ($dependency in $dependencies) {
        if (-not $knownSlugs.ContainsKey($dependency)) {
            $errors.Add("$($article.Path): unknown prerequisite slug '$dependency'")
        }
    }
}

# Detect prerequisite cycles so a future blog can topologically order the learning graph.
$dependenciesBySlug = @{}
foreach ($article in $articles | Where-Object Slug) {
    $dependenciesBySlug[$article.Slug] = @()
    if ($article.Prerequisites -and $article.Prerequisites -ne "[]") {
        $dependenciesBySlug[$article.Slug] = @(
            $article.Prerequisites.Trim('[', ']') -split ',' |
                ForEach-Object { $_.Trim().Trim('"', "'") } |
                Where-Object { $_ }
        )
    }
}

$visitState = @{}
function Visit-Prerequisite([string]$slug, [System.Collections.Generic.List[string]]$path) {
    if ($visitState[$slug] -eq 2) {
        return
    }
    if ($visitState[$slug] -eq 1) {
        $cycle = @($path) + $slug
        $errors.Add("Prerequisite cycle detected: $($cycle -join ' -> ')")
        return
    }

    $visitState[$slug] = 1
    $path.Add($slug)
    foreach ($dependency in $dependenciesBySlug[$slug]) {
        if ($dependenciesBySlug.ContainsKey($dependency)) {
            Visit-Prerequisite $dependency $path
        }
    }
    $path.RemoveAt($path.Count - 1)
    $visitState[$slug] = 2
}

foreach ($slug in $dependenciesBySlug.Keys) {
    Visit-Prerequisite $slug ([System.Collections.Generic.List[string]]::new())
}

if ($errors.Count -gt 0) {
    $errors | Sort-Object | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Output "Validated $($articles.Count) article metadata files."
$global:LASTEXITCODE = 0
