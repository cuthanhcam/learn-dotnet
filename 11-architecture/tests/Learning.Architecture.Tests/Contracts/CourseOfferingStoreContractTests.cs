using Learning.Architecture.Application.Abstractions;
using Learning.Architecture.Domain.Courses;

namespace Learning.Architecture.Tests.Contracts;

// The same behavior specification must hold for every adapter. SQL-only and real connection-race
// cases remain in the relational suite; this contract is not a substitute for provider-specific tests.
public abstract class CourseOfferingStoreContractTests
{
    protected abstract Task<StoreLease> OpenAsync(CourseOffering initial);

    [Fact]
    public async Task Save_ValidNextSnapshot_PreservesDefinitionAndOriginalSnapshot()
    {
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Architecture", 2);
        await using StoreLease lease = await OpenAsync(initial);
        CourseOffering loaded = (await lease.Store.FindAsync(initial.Id, default))!;
        Guid learner = Guid.NewGuid();

        Assert.True(await lease.Store.TrySaveAsync(loaded.Enroll(learner).Offering, loaded.Version, default));

        CourseOffering persisted = (await lease.Store.FindAsync(initial.Id, default))!;
        Assert.Equal(initial.Title, persisted.Title);
        Assert.Equal(initial.Capacity, persisted.Capacity);
        Assert.Equal(learner, Assert.Single(persisted.LearnerIds));
        Assert.Equal(1, persisted.Version);
        Assert.Equal(0, loaded.EnrolledCount);
        Assert.Equal(0, initial.Version);
    }

    [Fact]
    public async Task Save_MissingOffering_DoesNotInsertAnAggregate()
    {
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Architecture", 2);
        await using StoreLease lease = await OpenAsync(initial);
        CourseOffering missing = CourseOffering.Create(Guid.NewGuid(), "Missing", 1).Enroll(Guid.NewGuid()).Offering;

        Assert.False(await lease.Store.TrySaveAsync(missing, 0, default));
        Assert.Null(await lease.Store.FindAsync(missing.Id, default));
    }

    [Fact]
    public async Task Save_StaleSnapshot_DoesNotReplaceTheWinningLearner()
    {
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Architecture", 1);
        await using StoreLease lease = await OpenAsync(initial);
        CourseOffering first = (await lease.Store.FindAsync(initial.Id, default))!;
        CourseOffering second = (await lease.Store.FindAsync(initial.Id, default))!;
        Guid winner = Guid.NewGuid();

        Assert.True(await lease.Store.TrySaveAsync(first.Enroll(winner).Offering, 0, default));
        Assert.False(await lease.Store.TrySaveAsync(second.Enroll(Guid.NewGuid()).Offering, 0, default));

        CourseOffering persisted = (await lease.Store.FindAsync(initial.Id, default))!;
        Assert.Equal(winner, Assert.Single(persisted.LearnerIds));
        Assert.Equal(1, persisted.Version);
    }

    [Theory]
    [InlineData("Different title", 2)]
    [InlineData("architecture", 2)]
    [InlineData("Architecture", 3)]
    public async Task Save_DifferentImmutableDefinition_RejectsWithoutChangingStoredState(string title, int capacity)
    {
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Architecture", 2);
        await using StoreLease lease = await OpenAsync(initial);
        CourseOffering incompatible = CourseOffering.Restore(initial.Id, title, capacity, 1, [Guid.NewGuid()]);

        Assert.False(await lease.Store.TrySaveAsync(incompatible, 0, default));

        CourseOffering persisted = (await lease.Store.FindAsync(initial.Id, default))!;
        Assert.Equal(initial.Title, persisted.Title);
        Assert.Equal(initial.Capacity, persisted.Capacity);
        Assert.Equal(0, persisted.Version);
        Assert.Empty(persisted.LearnerIds);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task Save_InvalidVersionTransition_IsRejectedBeforeMutation(long expectedVersion)
    {
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Architecture", 2);
        await using StoreLease lease = await OpenAsync(initial);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await lease.Store.TrySaveAsync(initial.Enroll(Guid.NewGuid()).Offering, expectedVersion, default));

        Assert.Equal(0, (await lease.Store.FindAsync(initial.Id, default))!.EnrolledCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CanceledOperation_DoesNotReadOrSaveSuccessfully(bool save)
    {
        CourseOffering initial = CourseOffering.Create(Guid.NewGuid(), "Architecture", 2);
        await using StoreLease lease = await OpenAsync(initial);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            if (save)
                await lease.Store.TrySaveAsync(initial.Enroll(Guid.NewGuid()).Offering, 0, cancellation.Token);
            else
                await lease.Store.FindAsync(initial.Id, cancellation.Token);
        });

        Assert.Equal(0, (await lease.Store.FindAsync(initial.Id, default))!.EnrolledCount);
    }

    protected sealed class StoreLease(ICourseOfferingStore store,
        IAsyncDisposable? context = null, IAsyncDisposable? database = null) : IAsyncDisposable
    {
        public ICourseOfferingStore Store { get; } = store;

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (context is not null)
                    await context.DisposeAsync();
            }
            finally
            {
                if (database is not null)
                    await database.DisposeAsync();
            }
        }
    }
}
