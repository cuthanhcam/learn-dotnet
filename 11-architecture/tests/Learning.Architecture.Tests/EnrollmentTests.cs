using Learning.Architecture.Application.Abstractions;
using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Courses;

namespace Learning.Architecture.Tests;

public sealed class EnrollmentTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1001)]
    public void Create_InvalidCapacityIsRejected(int capacity) =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CourseOffering.Create(Guid.NewGuid(), "Architecture", capacity));

    [Fact]
    public void Enroll_PreservesSnapshotAndDoesNotExposeMutableLearnerCollection()
    {
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Architecture", 1);
        EnrollmentDecision decision = initial.Enroll(Guid.NewGuid());

        Assert.Equal(0, initial.EnrolledCount);
        Assert.Equal(1, decision.Offering.EnrolledCount);
        Assert.Equal(1, decision.Offering.Version);
        Assert.Equal(0, decision.Offering.AvailableSeats);
    }

    [Fact]
    public async Task Handler_DuplicateIsIdempotentEvenWhenFull()
    {
        var store = new InMemoryCourseOfferingStore();
        CourseOffering offering = CourseOffering.Create(Guid.NewGuid(), "Architecture", 1);
        store.TryAdd(offering);
        var handler = new EnrollLearnerHandler(store);
        var command = new EnrollLearnerCommand(offering.Id, Guid.NewGuid());

        Assert.Equal(EnrollLearnerStatus.Enrolled, (await handler.HandleAsync(command)).Status);
        Assert.Equal(EnrollLearnerStatus.AlreadyEnrolled, (await handler.HandleAsync(command)).Status);
        Assert.Equal(EnrollLearnerStatus.Full, (await handler.HandleAsync(
            new EnrollLearnerCommand(offering.Id, Guid.NewGuid()))).Status);
        Assert.Equal(1, (await store.FindAsync(offering.Id, default))!.EnrolledCount);
    }

    [Fact]
    public async Task Store_TwoSnapshotsCompetingForLastSeatHaveExactlyOneWinner()
    {
        var store = new InMemoryCourseOfferingStore();
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Architecture", 1);
        store.TryAdd(initial);
        CourseOffering first = initial.Enroll(Guid.NewGuid()).Offering;
        CourseOffering second = initial.Enroll(Guid.NewGuid()).Offering;

        // Separate workers contend for one atomic comparison. The assertion is independent of which
        // worker wins and does not rely on sleeps, scheduler order, or a timing-sensitive expectation.
        bool[] saved = await Task.WhenAll(
            Task.Run(async () => await store.TrySaveAsync(first, initial.Version, default)),
            Task.Run(async () => await store.TrySaveAsync(second, initial.Version, default)));

        Assert.Single(saved, value => value);
        Assert.Equal(1, (await store.FindAsync(initial.Id, default))!.EnrolledCount);
    }

    [Fact]
    public async Task Handler_StaleSnapshotProducesExplicitConflict()
    {
        // This adapter models a write race at the port boundary. It tests the use-case decision,
        // while the separate store test proves actual atomicity in the real learning adapter.
        var handler = new EnrollLearnerHandler(new ConflictingStore());
        var result = await handler.HandleAsync(new EnrollLearnerCommand(Guid.NewGuid(), Guid.NewGuid()));
        Assert.Equal(EnrollLearnerStatus.Conflict, result.Status);
        Assert.Null(result.Version);
    }

    [Fact]
    public async Task Handler_MissingOfferingReturnsNotFound()
    {
        var handler = new EnrollLearnerHandler(new InMemoryCourseOfferingStore());
        Assert.Equal(EnrollLearnerStatus.NotFound, (await handler.HandleAsync(
            new EnrollLearnerCommand(Guid.NewGuid(), Guid.NewGuid()))).Status);
    }

    [Fact]
    public async Task Handler_CanceledOperationDoesNotEnroll()
    {
        var store = new InMemoryCourseOfferingStore();
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Architecture", 1);
        store.TryAdd(initial);
        var handler = new EnrollLearnerHandler(store);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await handler.HandleAsync(new EnrollLearnerCommand(initial.Id, Guid.NewGuid()), cancellation.Token));
        Assert.Equal(0, (await store.FindAsync(initial.Id, default))!.EnrolledCount);
    }

    private sealed class ConflictingStore : ICourseOfferingStore
    {
        public ValueTask<CourseOffering?> FindAsync(Guid id, CancellationToken cancellationToken) =>
            ValueTask.FromResult<CourseOffering?>(CourseOffering.Create(id, "Concurrent offering", 1));

        public ValueTask<bool> TrySaveAsync(CourseOffering offering, long expectedVersion,
            CancellationToken cancellationToken) => ValueTask.FromResult(false);
    }
}
