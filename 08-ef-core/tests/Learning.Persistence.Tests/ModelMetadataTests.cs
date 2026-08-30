using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Learning.Persistence.Tests;

/// <summary>
/// Protects intentional relational mapping decisions from accidental convention changes. These tests
/// inspect EF's finalized model; migration tests separately prove that the model can become a schema.
/// </summary>
public sealed class ModelMetadataTests
{
    [Fact]
    public async Task CourseMapping_DefinesLengthsPrecisionAndConcurrencyContract()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();
        IEntityType course = context.Model.FindEntityType(typeof(Course))!;

        Assert.Equal("courses", course.GetTableName());
        Assert.Equal(160, course.FindProperty(nameof(Course.Title))!.GetMaxLength());
        Assert.Equal(180, course.FindProperty(nameof(Course.Slug))!.GetMaxLength());
        Assert.Equal(18, course.FindProperty(nameof(Course.Price))!.GetPrecision());
        Assert.Equal(2, course.FindProperty(nameof(Course.Price))!.GetScale());
        Assert.True(course.FindProperty(nameof(Course.Version))!.IsConcurrencyToken);
    }

    [Fact]
    public async Task CourseMapping_DefinesBusinessAndQuerySupportingIndexes()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();
        IEntityType course = context.Model.FindEntityType(typeof(Course))!;
        IReadOnlyList<IIndex> indexes = course.GetIndexes().ToArray();

        IIndex slug = Assert.Single(
            indexes,
            index => index.Properties.Select(property => property.Name).SequenceEqual([nameof(Course.Slug)]));
        Assert.True(slug.IsUnique);

        Assert.Contains(
            indexes,
            index => index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(Course.IsPublished), nameof(Course.PublishedAt)]));
    }

    [Fact]
    public async Task RelationshipMapping_UsesRestrictForAggregateOwnerAndCascadeForOwnedLifecycle()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();
        IEntityType course = context.Model.FindEntityType(typeof(Course))!;
        IEntityType module = context.Model.FindEntityType(typeof(CourseModule))!;

        IForeignKey categoryRelationship = Assert.Single(
            course.GetForeignKeys(),
            key => key.PrincipalEntityType.ClrType == typeof(Category));
        IForeignKey moduleRelationship = Assert.Single(
            module.GetForeignKeys(),
            key => key.PrincipalEntityType.ClrType == typeof(Course));

        Assert.Equal(DeleteBehavior.Restrict, categoryRelationship.DeleteBehavior);
        Assert.Equal(DeleteBehavior.Cascade, moduleRelationship.DeleteBehavior);
        Assert.True(moduleRelationship.IsRequired);
    }

    [Fact]
    public async Task TestFixture_UsesRelationalSqliteInsteadOfTheInMemoryProvider()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();

        Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", context.Database.ProviderName);
        Assert.True(context.Database.IsRelational());
    }
}
