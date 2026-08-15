param(
    [string[]] $Roots = @(
        "01-csharp-basics/docs",
        "02-oop/docs",
        "03-core-dotnet/docs",
        "04-memory-performance/docs",
        "05-dsa/docs",
        "06-async-concurrency/docs",
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

        $articles.Add([pscustomobject]@{
            Path = $file.FullName
            Slug = $metadata["slug"]
            Prerequisites = $metadata["prerequisites"]
        })
    }
}

$articles |
    Where-Object { $_.Slug } |
    Group-Object Slug |
    Where-Object Count -gt 1 |
    ForEach-Object { $errors.Add("Duplicate slug '$($_.Name)'") }

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

if ($errors.Count -gt 0) {
    $errors | Sort-Object | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Output "Validated $($articles.Count) article metadata files."
