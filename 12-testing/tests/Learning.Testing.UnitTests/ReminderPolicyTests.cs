using Learning.Testing.Examples.Loans;

namespace Learning.Testing.UnitTests;

[Trait("Category", "Unit")]
public sealed class ReminderPolicyTests
{
    private static readonly DateTimeOffset DueAt = new(2026, 10, 25, 0, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    public void IsDue_AtDeadlineBoundary_UsesInclusiveInstantComparison(long offsetTicks, bool expected)
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), DueAt);

        bool actual = ReminderPolicy.IsDue(loan, DueAt.AddTicks(offsetTicks));

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void IsDue_ReturnedLoan_NeverRequiresReminder(int offsetDays)
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), DueAt, isReturned: true);

        Assert.False(ReminderPolicy.IsDue(loan, DueAt.AddDays(offsetDays)));
    }

    [Fact]
    public void IsDue_SameInstantWithDifferentOffsets_IsDue()
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), DueAt.ToOffset(TimeSpan.FromHours(7)));

        Assert.True(ReminderPolicy.IsDue(loan, DueAt));
    }

    [Fact]
    public void IsDue_NullLoan_RejectsInvalidInput()
    {
        Assert.Throws<ArgumentNullException>(() => ReminderPolicy.IsDue(null!, DueAt));
    }

    [Theory]
    [InlineData(true, "id")]
    [InlineData(false, "memberId")]
    public void Loan_EmptyIdentifier_ReportsTheInvalidParameter(bool emptyLoanId, string parameter)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Loan(
            emptyLoanId ? Guid.Empty : Guid.NewGuid(), emptyLoanId ? Guid.NewGuid() : Guid.Empty, DueAt));

        Assert.Equal(parameter, exception.ParamName);
    }
}
