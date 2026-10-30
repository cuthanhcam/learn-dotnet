using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Application.Offerings;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Messaging;
using Learning.Architecture.Infrastructure.Persistence;
using Learning.Architecture.Notifications;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.ConsoleApp;

/// <summary>
/// An offline, repeatable walkthrough of both module databases. SQLite runs real SQL but these
/// in-memory connections intentionally disappear at exit; the API uses file-backed databases.
/// </summary>
internal static class RelationalEnrollmentLab
{
    public static async Task RunAsync()
    {
        await using var enrollmentConnection = new SqliteConnection("Data Source=:memory:");
        await using var welcomeConnection = new SqliteConnection("Data Source=:memory:");
        await enrollmentConnection.OpenAsync();
        await welcomeConnection.OpenAsync();
        var options = new DbContextOptionsBuilder<EnrollmentDbContext>().UseSqlite(enrollmentConnection).Options;
        var welcomeContexts = new WelcomeContexts(welcomeConnection);
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Relational architecture lab", 1);

        await using (var setup = new EnrollmentDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.Offerings.Add(EfCourseOfferingStore.ToRow(offering));
            await setup.SaveChangesAsync();
        }
        await using (WelcomeDbContext setup = welcomeContexts.CreateDbContext())
            await setup.Database.EnsureCreatedAsync();

        var request = new EnrollmentRequest(Guid.NewGuid(), offering.Id, Guid.NewGuid());
        await using (var first = new EnrollmentDbContext(options))
            Console.WriteLine(await new DurableEnrollmentRequests(first).ExecuteAsync(request));
        await using (var replay = new EnrollmentDbContext(options))
            Console.WriteLine(await new DurableEnrollmentRequests(replay).ExecuteAsync(request));

        await using var dispatch = new EnrollmentDbContext(options);
        var dispatcher = new OutboxDispatcher(dispatch, new WelcomeEnrollmentConsumer(welcomeContexts));
        Console.WriteLine($"Delivered integration messages: {await dispatcher.DispatchAsync()}");
        Console.WriteLine($"Pending on second dispatch: {await dispatcher.DispatchAsync()}");
        OfferingPage page = await new ListOfferingsHandler(new EfCourseOfferingStore(dispatch))
            .HandleAsync(new ListOfferingsQuery());
        Console.WriteLine($"Available seats: {page.Items.Single().AvailableSeats}");
        await using WelcomeDbContext verify = welcomeContexts.CreateDbContext();
        Console.WriteLine($"Welcome work items: {await verify.WorkItems.CountAsync()}");
    }

    private sealed class WelcomeContexts(SqliteConnection connection) : IDbContextFactory<WelcomeDbContext>
    {
        public WelcomeDbContext CreateDbContext() => new(
            new DbContextOptionsBuilder<WelcomeDbContext>().UseSqlite(connection).Options);
    }
}
