namespace Learning.Testing.Examples.Loans;

public sealed record Loan
{
    public Loan(Guid id, Guid memberId, DateTimeOffset dueAt, bool isReturned = false)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("A loan ID is required.", nameof(id));
        if (memberId == Guid.Empty)
            throw new ArgumentException("A member ID is required.", nameof(memberId));

        Id = id;
        MemberId = memberId;
        DueAt = dueAt;
        IsReturned = isReturned;
    }

    public Guid Id { get; }
    public Guid MemberId { get; }
    public DateTimeOffset DueAt { get; }
    public bool IsReturned { get; }
}
