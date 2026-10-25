using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Application.Offerings;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Learning.Architecture.Tests.Persistence;

public sealed class RelationalEnrollmentTests
{
    [Fact]
    public async Task IndependentContexts_RejectTheSecondStaleSnapshot()
    {
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Last seat", 1);
        await using var fixture = await EnrollmentDatabase.CreateAsync(initial);
        await using var firstDb = fixture.CreateContext();
        await using var secondDb = fixture.CreateContext();
        var first = new EfCourseOfferingStore(firstDb);
        var second = new EfCourseOfferingStore(secondDb);
        CourseOffering a = (await first.FindAsync(initial.Id, default))!;
        CourseOffering b = (await second.FindAsync(initial.Id, default))!;

        // Both readers see version zero before either save. Ordered writes deterministically reproduce
        // the stale-read race without trying to run two operations on one connection simultaneously.
        Assert.True(await first.TrySaveAsync(a.Enroll(Guid.NewGuid()).Offering, 0, default));
        Assert.False(await second.TrySaveAsync(b.Enroll(Guid.NewGuid()).Offering, 0, default));
        Assert.Equal(1, (await second.FindAsync(initial.Id, default))!.EnrolledCount);
    }

    [Fact]
    public async Task Query_ProjectsReadContractFromRelationalRows()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Catalog", 3).Enroll(Guid.NewGuid()).Offering;
        await using var fixture = await EnrollmentDatabase.CreateAsync(offering);
        await using var database = fixture.CreateContext();
        OfferingPage page = await new ListOfferingsHandler(new EfCourseOfferingStore(database))
            .HandleAsync(new ListOfferingsQuery());
        Assert.Equal(2, Assert.Single(page.Items).AvailableSeats);
        Assert.Empty(database.ChangeTracker.Entries());
    }

    [Fact]
    public async Task SameKeyAcrossScopes_ReplaysOriginalResultAndCreatesOneOutboxMessage()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Replay", 1);
        await using var fixture = await EnrollmentDatabase.CreateAsync(offering);
        var request = new EnrollmentRequest(Guid.NewGuid(), offering.Id, Guid.NewGuid());
        await using (var first = fixture.CreateContext())
        {
            EnrollmentRequestResult result = await new DurableEnrollmentRequests(first).ExecuteAsync(request);
            Assert.Equal(EnrollLearnerStatus.Enrolled, result.Enrollment!.Status);
            Assert.False(result.Replayed);
        }
        await using var second = fixture.CreateContext();
        EnrollmentRequestResult replay = await new DurableEnrollmentRequests(second).ExecuteAsync(request);
        Assert.True(replay.Replayed);
        Assert.Equal(EnrollLearnerStatus.Enrolled, replay.Enrollment!.Status);
        Assert.Equal(1, await second.Receipts.CountAsync());
        Assert.Equal(1, await second.Outbox.CountAsync());
        Assert.Equal(1, (await second.Offerings.SingleAsync()).EnrolledCount);
    }

    [Fact]
    public async Task KeyWithDifferentPayload_IsRejectedWithoutAnotherMutation()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Key reuse", 2);
        await using var fixture = await EnrollmentDatabase.CreateAsync(offering);
        var request = new EnrollmentRequest(Guid.NewGuid(), offering.Id, Guid.NewGuid());
        await using (var first = fixture.CreateContext())
            await new DurableEnrollmentRequests(first).ExecuteAsync(request);
        await using var second = fixture.CreateContext();
        EnrollmentRequestResult result = await new DurableEnrollmentRequests(second)
            .ExecuteAsync(request with { LearnerId = Guid.NewGuid() });
        Assert.Equal(EnrollmentRequestStatus.KeyReused, result.Status);
        Assert.Equal(1, (await second.Offerings.SingleAsync()).EnrolledCount);
    }

    [Fact]
    public async Task NewKeyForSameLearner_IsBusinessIdempotentWithoutAnotherEvent()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Same learner", 1);
        await using var fixture = await EnrollmentDatabase.CreateAsync(offering);
        var request = new EnrollmentRequest(Guid.NewGuid(), offering.Id, Guid.NewGuid());
        await using (var first = fixture.CreateContext())
            await new DurableEnrollmentRequests(first).ExecuteAsync(request);
        await using var second = fixture.CreateContext();
        EnrollmentRequestResult result = await new DurableEnrollmentRequests(second)
            .ExecuteAsync(request with { RequestId = Guid.NewGuid() });
        Assert.Equal(EnrollLearnerStatus.AlreadyEnrolled, result.Enrollment!.Status);
        Assert.False(result.Replayed);
        Assert.Equal(2, await second.Receipts.CountAsync());
        Assert.Equal(1, await second.Outbox.CountAsync());
    }

    [Fact]
    public async Task FailureAfterSqlUpdate_RollsBackSeatReceiptAndOutboxTogether()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Atomic failure", 1);
        await using var fixture = await EnrollmentDatabase.CreateAsync(offering);
        await using (var failing = fixture.CreateContext(new RejectReceiptSave()))
        {
            await Assert.ThrowsAsync<InjectedPersistenceException>(async () =>
                await new DurableEnrollmentRequests(failing).ExecuteAsync(
                    new EnrollmentRequest(Guid.NewGuid(), offering.Id, Guid.NewGuid())));
        }
        await using var verification = fixture.CreateContext();
        Assert.Equal(0, (await verification.Offerings.SingleAsync()).EnrolledCount);
        Assert.Empty(await verification.Receipts.ToArrayAsync());
        Assert.Empty(await verification.Outbox.ToArrayAsync());
    }

    [Fact]
    public async Task FullResult_IsReceiptedButDoesNotPublishEnrollment()
    {
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Full", 1).Enroll(Guid.NewGuid()).Offering;
        await using var fixture = await EnrollmentDatabase.CreateAsync(offering);
        await using var database = fixture.CreateContext();
        EnrollmentRequestResult result = await new DurableEnrollmentRequests(database).ExecuteAsync(
            new EnrollmentRequest(Guid.NewGuid(), offering.Id, Guid.NewGuid()));
        Assert.Equal(EnrollLearnerStatus.Full, result.Enrollment!.Status);
        Assert.Equal(1, await database.Receipts.CountAsync());
        Assert.Equal(0, await database.Outbox.CountAsync());
    }

    [Fact]
    public async Task CancellationBeforeTransaction_DoesNotWriteAnything()
    {
        await using var fixture = await EnrollmentDatabase.CreateAsync();
        await using var database = fixture.CreateContext();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await new DurableEnrollmentRequests(database).ExecuteAsync(
                new EnrollmentRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()), cancellation.Token));
        Assert.Equal(0, await database.Receipts.CountAsync());
    }

    private sealed class InjectedPersistenceException : Exception;

    private sealed class RejectReceiptSave : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default) =>
            throw new InjectedPersistenceException();
    }
}
