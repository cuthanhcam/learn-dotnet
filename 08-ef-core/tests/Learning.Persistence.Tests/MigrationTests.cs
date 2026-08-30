using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Learning.Persistence.Tests;

public sealed class MigrationTests
{
    [Fact]
    public async Task Migrate_AppliesCompleteChainAndLeavesNoPendingMigrations()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();

        string[] applied = (await context.Database.GetAppliedMigrationsAsync()).ToArray();
        string[] pending = (await context.Database.GetPendingMigrationsAsync()).ToArray();

        Assert.Equal(2, applied.Length);
        Assert.Contains(applied, migration => migration.EndsWith("_InitialCreate", StringComparison.Ordinal));
        Assert.Contains(applied, migration => migration.EndsWith("_AddCourseTags", StringComparison.Ordinal));
        Assert.Empty(pending);
    }

    [Fact]
    public async Task GeneratedIdempotentScript_IsRejectedBySqliteProviderWithClearLimitation()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();
        IMigrator migrator = context.GetService<IMigrator>();

        // SQLite cannot express the procedural existence checks needed by EF's idempotent script.
        // Production providers must have their own script/bundle verification tests.
        NotSupportedException exception = await Assert.ThrowsAsync<NotSupportedException>(() =>
            Task.FromResult(migrator.GenerateScript(options: MigrationsSqlGenerationOptions.Idempotent)));

        Assert.Contains("Idempotent", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GeneratedUpgradeScript_ContainsTablesConstraintsAndIndexes()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();
        IMigrator migrator = context.GetService<IMigrator>();

        string script = migrator.GenerateScript(fromMigration: Migration.InitialDatabase);

        Assert.Contains("CREATE TABLE \"categories\"", script, StringComparison.Ordinal);
        Assert.Contains("CREATE TABLE \"courses\"", script, StringComparison.Ordinal);
        Assert.Contains("FOREIGN KEY", script, StringComparison.Ordinal);
        Assert.Contains("CREATE UNIQUE INDEX", script, StringComparison.Ordinal);
    }
}
