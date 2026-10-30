using Learning.Architecture.Application.Abstractions;
using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Domain.Courses;

namespace Learning.Architecture.Tests;

public sealed class EnrollmentCancellationTests
{
    [Fact]
    public async Task PreCanceledHandler_DoesNotInvokeTheStore()
    {
        using var cancellation = new CancellationTokenSource();
        var store = new IgnoringCancellationStore(cancellation, found: true);
        cancellation.Cancel();
        var handler = new EnrollLearnerHandler(store);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await handler.HandleAsync(new EnrollLearnerCommand(Guid.NewGuid(), Guid.NewGuid()), cancellation.Token));

        Assert.Equal(0, store.Reads);
        Assert.Equal(0, store.Writes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CancellationDuringLookup_DoesNotReturnSuccessOrStartSaving(bool found)
    {
        using var cancellation = new CancellationTokenSource();
        var store = new IgnoringCancellationStore(cancellation, found);
        var handler = new EnrollLearnerHandler(store);

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await handler.HandleAsync(new EnrollLearnerCommand(Guid.NewGuid(), Guid.NewGuid()), cancellation.Token));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Equal(1, store.Reads);
        Assert.Equal(0, store.Writes);
    }

    private sealed class IgnoringCancellationStore(CancellationTokenSource cancellation, bool found) : ICourseOfferingStore
    {
        public int Reads { get; private set; }
        public int Writes { get; private set; }

        public ValueTask<CourseOffering?> FindAsync(Guid id, CancellationToken cancellationToken)
        {
            Reads++;
            cancellation.Cancel();
            return ValueTask.FromResult<CourseOffering?>(found ? CourseOffering.Create(id, "Architecture", 2) : null);
        }

        public ValueTask<bool> TrySaveAsync(CourseOffering offering, long expectedVersion, CancellationToken cancellationToken)
        {
            Writes++;
            return ValueTask.FromResult(true);
        }
    }
}
