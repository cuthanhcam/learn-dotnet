using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Contracts;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Messaging;
using Learning.Architecture.Infrastructure.Persistence;
using Learning.Architecture.Notifications;
using Learning.Architecture.Tests.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.Tests.Messaging;

public sealed class OutboxDeliveryTests
{
    [Fact]
    public async Task CancellationDuringDelivery_LeavesMessagePending()
    {
        await using var fixture = await EnrollmentDatabase.CreateAsync();
        await using var database = fixture.CreateContext();
        database.Outbox.Add(new OutboxRow
        {
            MessageId = Guid.NewGuid(),
            OfferingId = Guid.NewGuid(),
            LearnerId = Guid.NewGuid()
        });
        await database.SaveChangesAsync();
        using var cancellation = new CancellationTokenSource();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new OutboxDispatcher(database, new CancelDelivery(cancellation))
                .DispatchAsync(cancellationToken: cancellation.Token));
        Assert.False((await database.Outbox.AsNoTracking().SingleAsync()).Dispatched);
    }

    [Fact]
    public async Task Dispatch_DoesNotExceedTheBatchBudget()
    {
        await using var fixture = await EnrollmentDatabase.CreateAsync();
        await using var database = fixture.CreateContext();
        for (int index = 0; index < 3; index++)
            database.Outbox.Add(new OutboxRow
            {
                MessageId = Guid.NewGuid(),
                OfferingId = Guid.NewGuid(),
                LearnerId = Guid.NewGuid()
            });
        await database.SaveChangesAsync();
        Assert.Equal(2, await new OutboxDispatcher(database, new AcceptDelivery()).DispatchAsync(limit: 2));
        Assert.Equal(1, await database.Outbox.CountAsync(row => !row.Dispatched));
    }

    [Fact]
    public async Task FailureAfterConsumerCommit_RedeliversWithoutDuplicatingLocalEffect()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Outbox", 1);
        await using var fixture = await EnrollmentDatabase.CreateAsync(offering);
        await using (var commandDb = fixture.CreateContext())
            await new DurableEnrollmentRequests(commandDb).ExecuteAsync(
                new EnrollmentRequest(Guid.NewGuid(), offering.Id, Guid.NewGuid()));

        // Separate databases make the lack of a distributed transaction observable. Notifications
        // commit first; enrollment acknowledgement is deliberately interrupted on the first attempt.
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var contexts = new WelcomeContexts(connection);
        await using (var initialize = contexts.CreateDbContext())
            await initialize.Database.EnsureCreatedAsync();
        var sink = new LoseAcknowledgementOnce(new WelcomeEnrollmentConsumer(contexts));
        await using (var first = fixture.CreateContext())
        {
            await Assert.ThrowsAsync<AcknowledgementLostException>(() =>
                new OutboxDispatcher(first, sink).DispatchAsync());
            Assert.False((await first.Outbox.SingleAsync()).Dispatched);
        }
        await using (var second = fixture.CreateContext())
        {
            Assert.Equal(1, await new OutboxDispatcher(second, sink).DispatchAsync());
            Assert.Equal(0, await new OutboxDispatcher(second, sink).DispatchAsync());
            Assert.True((await second.Outbox.SingleAsync()).Dispatched);
        }
        await using var verify = contexts.CreateDbContext();
        Assert.Equal(1, await verify.WorkItems.CountAsync());
    }

    [Fact]
    public async Task Consumer_RejectsMessageIdReuseWithDifferentData()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var contexts = new WelcomeContexts(connection);
        await using (var initialize = contexts.CreateDbContext())
            await initialize.Database.EnsureCreatedAsync();
        var consumer = new WelcomeEnrollmentConsumer(contexts);
        var message = new LearnerEnrolledV1(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        await consumer.AcceptAsync(message, default);
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await consumer.AcceptAsync(message with { LearnerId = Guid.NewGuid() }, default));
    }

    [Fact]
    public async Task UnknownSchema_RemainsPendingInsteadOfBeingSilentlyDiscarded()
    {
        await using var fixture = await EnrollmentDatabase.CreateAsync();
        await using var database = fixture.CreateContext();
        database.Outbox.Add(new OutboxRow
        {
            MessageId = Guid.NewGuid(),
            OfferingId = Guid.NewGuid(),
            LearnerId = Guid.NewGuid(),
            SchemaVersion = 99
        });
        await database.SaveChangesAsync();
        var sink = new RejectUnexpectedDelivery();
        await Assert.ThrowsAsync<InvalidOperationException>(() => new OutboxDispatcher(database, sink).DispatchAsync());
        Assert.False((await database.Outbox.AsNoTracking().SingleAsync()).Dispatched);
    }

    private sealed class WelcomeContexts(SqliteConnection connection) : IDbContextFactory<WelcomeDbContext>
    {
        public WelcomeDbContext CreateDbContext() => new(
            new DbContextOptionsBuilder<WelcomeDbContext>().UseSqlite(connection).Options);
    }

    private sealed class AcknowledgementLostException : Exception;

    private sealed class LoseAcknowledgementOnce(IEnrollmentEventSink inner) : IEnrollmentEventSink
    {
        private bool _first = true;
        public async ValueTask AcceptAsync(LearnerEnrolledV1 message, CancellationToken cancellationToken)
        {
            await inner.AcceptAsync(message, cancellationToken);
            if (_first)
            {
                _first = false;
                throw new AcknowledgementLostException();
            }
        }
    }

    private sealed class RejectUnexpectedDelivery : IEnrollmentEventSink
    {
        public ValueTask AcceptAsync(LearnerEnrolledV1 message, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("An unknown schema must never reach this sink.");
    }

    private sealed class CancelDelivery(CancellationTokenSource cancellation) : IEnrollmentEventSink
    {
        public ValueTask AcceptAsync(LearnerEnrolledV1 message, CancellationToken cancellationToken)
        {
            cancellation.Cancel();
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.CompletedTask;
        }
    }

    private sealed class AcceptDelivery : IEnrollmentEventSink
    {
        public ValueTask AcceptAsync(LearnerEnrolledV1 message, CancellationToken cancellationToken) => ValueTask.CompletedTask;
    }
}
