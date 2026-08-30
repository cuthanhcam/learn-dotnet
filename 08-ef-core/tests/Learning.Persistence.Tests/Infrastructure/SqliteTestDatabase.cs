using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Tests.Infrastructure;

public sealed class SqliteTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public async Task InitializeAsync()
    {
        // SQLite in-memory databases live only while their connection remains open. Keeping this
        // owner connection open lets multiple short-lived DbContext units of work share one schema.
        await _connection.OpenAsync();
        await using LearningDbContext context = CreateContext();
        // Exercise the same migration chain that deployment will apply. EnsureCreated bypasses
        // migration history and therefore cannot prove that incremental schema evolution works.
        await context.Database.MigrateAsync();
    }

    public LearningDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<LearningDbContext>()
            .UseSqlite(_connection)
            .EnableDetailedErrors()
            .Options;
        return new LearningDbContext(options);
    }

    public async ValueTask DisposeAsync() => await _connection.DisposeAsync();
}
