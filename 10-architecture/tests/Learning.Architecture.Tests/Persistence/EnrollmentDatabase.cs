using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Learning.Architecture.Tests.Persistence;

/// <summary>
/// Holds one real SQLite database alive for a test. Each CreateContext call gets an independent
/// tracker/unit of work; no EF InMemory provider substitutes for SQL predicates or transactions.
/// Tests are isolated from one another and do not share filesystem or global application state.
/// </summary>
internal sealed class EnrollmentDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public static async Task<EnrollmentDatabase> CreateAsync(CourseOffering? offering = null)
    {
        var fixture = new EnrollmentDatabase();
        await fixture._connection.OpenAsync();
        await using var database = fixture.CreateContext();
        await database.Database.EnsureCreatedAsync();
        if (offering is not null)
        {
            database.Offerings.Add(EfCourseOfferingStore.ToRow(offering));
            await database.SaveChangesAsync();
        }
        return fixture;
    }

    public EnrollmentDbContext CreateContext(params IInterceptor[] interceptors) => new(
        new DbContextOptionsBuilder<EnrollmentDbContext>().UseSqlite(_connection)
            .AddInterceptors(interceptors).Options);

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
