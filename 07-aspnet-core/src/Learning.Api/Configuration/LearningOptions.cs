using System.ComponentModel.DataAnnotations;

namespace Learning.Api.Configuration;

public sealed class LearningOptions
{
    public const string SectionName = "Learning";

    [Required]
    public string CatalogName { get; init; } = string.Empty;

    [Range(1, 500)]
    public int MaximumPageSize { get; init; } = 100;

    [MinLength(1)]
    public string[] AllowedOrigins { get; init; } = [];
}
