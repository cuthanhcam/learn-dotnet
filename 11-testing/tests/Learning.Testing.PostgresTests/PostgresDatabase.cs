using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Learning.Testing.PostgresTests;

internal sealed class PostgresDatabase(string adminConnection, string databaseName, string connection) : IAsyncDisposable
{
    public static async Task<PostgresDatabase> CreateAsync(CourseOffering? offering = null)
    {
        string adminConnection = Environment.GetEnvironmentVariable("LEARN_DOTNET_POSTGRES")
            ?? throw new InvalidOperationException("Run scripts/Test-PostgresProvider.ps1 to provision the disposable PostgreSQL fixture.");
        // Identifiers contain only a fixed prefix and generated hex, never request/configuration text.
        string databaseName = $"learn_dotnet_{Guid.NewGuid():N}";
        await using var admin = new NpgsqlConnection(adminConnection);
        await admin.OpenAsync();
        await using var create = new NpgsqlCommand($"CREATE DATABASE {databaseName}", admin);
        await create.ExecuteNonQueryAsync();
        var connection = new NpgsqlConnectionStringBuilder(adminConnection) { Database = databaseName, Pooling = false };
        var fixture = new PostgresDatabase(adminConnection, databaseName, connection.ConnectionString);
        try
        {
            await using EnrollmentDbContext context = fixture.CreateContext();
            // Disposable fixture only; this deliberately does not claim migration/upgrade coverage.
            await context.Database.EnsureCreatedAsync();
            if (offering is not null)
            {
                context.Offerings.Add(EfCourseOfferingStore.ToRow(offering));
                await context.SaveChangesAsync();
            }
            return fixture;
        }
        catch
        {
            await fixture.DisposeAsync();
            throw;
        }
    }

    public EnrollmentDbContext CreateContext(params IInterceptor[] interceptors) => new(
        new DbContextOptionsBuilder<EnrollmentDbContext>().UseNpgsql(connection)
            .AddInterceptors(interceptors).Options);

    public async ValueTask DisposeAsync()
    {
        await using var admin = new NpgsqlConnection(adminConnection);
        await admin.OpenAsync();
        await using var drop = new NpgsqlCommand($"DROP DATABASE {databaseName} WITH (FORCE)", admin);
        await drop.ExecuteNonQueryAsync();
    }
}
