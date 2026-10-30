using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Learning.Testing.PostgresTests;

[Trait("Category", "Postgres")]
public sealed class PostgresTransactionTests
{
    [Fact]
    public async Task CompetingConnections_OnlyOneStaleSnapshotClaimsTheLastSeat()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Last seat", 1);
        await using var fixture = await PostgresDatabase.CreateAsync(offering);
        await using var first = fixture.CreateContext();
        await using var second = fixture.CreateContext();
        var firstStore = new EfCourseOfferingStore(first);
        var secondStore = new EfCourseOfferingStore(second);
        CourseOffering a = (await firstStore.FindAsync(offering.Id, default))!;
        CourseOffering b = (await secondStore.FindAsync(offering.Id, default))!;
        Guid learnerA = Guid.NewGuid();
        Guid learnerB = Guid.NewGuid();

        // Both snapshots exist before either write. The connections then compete at the server;
        // the test promises one winner, without assuming a particular scheduler or lock-wait order.
        bool[] saved = await Task.WhenAll(
            firstStore.TrySaveAsync(a.Enroll(learnerA).Offering, 0, default).AsTask(),
            secondStore.TrySaveAsync(b.Enroll(learnerB).Offering, 0, default).AsTask());

        Assert.Single(saved, accepted => accepted);
        await using var verification = fixture.CreateContext();
        OfferingRow row = await verification.Offerings.SingleAsync();
        Assert.Equal(1, row.EnrolledCount);
        Assert.Equal(1, row.Version);
        CourseOffering persisted = (await new EfCourseOfferingStore(verification).FindAsync(offering.Id, default))!;
        Assert.Equal(saved[0] ? learnerA : learnerB, Assert.Single(persisted.LearnerIds));
    }

    [Fact]
    public async Task ReceiptFailure_RollsBackSqlMutationAndOutbox()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Rollback", 1);
        await using var fixture = await PostgresDatabase.CreateAsync(offering);
        await using (var failing = fixture.CreateContext(new RejectSave()))
            await Assert.ThrowsAsync<InjectedFailure>(async () =>
                await new DurableEnrollmentRequests(failing).ExecuteAsync(
                    new EnrollmentRequest(Guid.NewGuid(), offering.Id, Guid.NewGuid())));

        await using var verification = fixture.CreateContext();
        Assert.Equal(0, (await verification.Offerings.SingleAsync()).EnrolledCount);
        Assert.Empty(await verification.Receipts.ToArrayAsync());
        Assert.Empty(await verification.Outbox.ToArrayAsync());
    }

    [Fact]
    public async Task ReplayAcrossConnections_PreservesResultAndOneDeliveryIntent()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Replay", 1);
        await using var fixture = await PostgresDatabase.CreateAsync(offering);
        var request = new EnrollmentRequest(Guid.NewGuid(), offering.Id, Guid.NewGuid());
        await using (var first = fixture.CreateContext())
        {
            EnrollmentRequestResult original = await new DurableEnrollmentRequests(first).ExecuteAsync(request);
            Assert.False(original.Replayed);
            Assert.Equal(EnrollLearnerStatus.Enrolled, original.Enrollment!.Status);
        }
        await using var second = fixture.CreateContext();
        EnrollmentRequestResult replay = await new DurableEnrollmentRequests(second).ExecuteAsync(request);
        Assert.True(replay.Replayed);
        Assert.Equal(new EnrollLearnerResult(EnrollLearnerStatus.Enrolled, 1), replay.Enrollment);
        Assert.Single(await second.Receipts.ToArrayAsync());
        Assert.Single(await second.Outbox.ToArrayAsync());
        Assert.Equal(1, (await second.Offerings.SingleAsync()).EnrolledCount);
    }

    [Fact]
    public async Task InvalidCapacity_IsRejectedByTheServerConstraint()
    {
        await using var fixture = await PostgresDatabase.CreateAsync();
        await using var context = fixture.CreateContext();
        context.Offerings.Add(new OfferingRow { Id = Guid.NewGuid(), Title = "Invalid", Capacity = 0 });
        DbUpdateException failure = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        var provider = Assert.IsType<PostgresException>(failure.InnerException);
        Assert.Equal(PostgresErrorCodes.CheckViolation, provider.SqlState);
        Assert.Equal("CK_Offerings_Capacity", provider.ConstraintName);
    }

    private sealed class InjectedFailure : Exception;

    private sealed class RejectSave : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default) => throw new InjectedFailure();
    }
}
