using Learning.Architecture.Domain.Courses;

namespace Learning.Architecture.Tests;

public sealed class DomainRestorationTests
{
    [Fact]
    public void Restore_RejectsDuplicateLearners()
    {
        Guid learner = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => CourseOffering.Restore(
            Guid.NewGuid(), "Corrupt", 3, 2, new[] { learner, learner }));
    }

    [Fact]
    public void Restore_RejectsEmptyLearnerIdentity() => Assert.Throws<ArgumentException>(() =>
        CourseOffering.Restore(Guid.NewGuid(), "Corrupt", 1, 1, new[] { Guid.Empty }));

    [Fact]
    public void Restore_RejectsOverCapacity() => Assert.Throws<ArgumentException>(() =>
        CourseOffering.Restore(Guid.NewGuid(), "Corrupt", 1, 2, new[] { Guid.NewGuid(), Guid.NewGuid() }));

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Restore_RejectsVersionBehindEnrollmentCount(long version) => Assert.Throws<ArgumentException>(() =>
        CourseOffering.Restore(Guid.NewGuid(), "Corrupt", 1, version, new[] { Guid.NewGuid() }));

    [Fact]
    public void Restore_DoesNotAliasCallerArray()
    {
        Guid learner = Guid.NewGuid();
        Guid[] input = [learner];
        CourseOffering offering = CourseOffering.Restore(Guid.NewGuid(), "Snapshot", 2, 1, input);
        input[0] = Guid.NewGuid();
        Assert.Equal(learner, Assert.Single(offering.LearnerIds));
        Assert.Equal(EnrollmentOutcome.AlreadyEnrolled, offering.Enroll(learner).Outcome);
    }
}
