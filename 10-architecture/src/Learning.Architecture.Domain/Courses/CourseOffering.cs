using System.Collections.Immutable;

namespace Learning.Architecture.Domain.Courses;

/// <summary>
/// An immutable aggregate representing one scheduled offering, rather than the catalog definition
/// of a course. Capacity and enrolled learner identities must change together as one consistency unit.
/// No HTTP, persistence, dependency injection, or token type belongs in this model.
/// </summary>
public sealed class CourseOffering
{
    private readonly ImmutableHashSet<Guid> _learners;

    private CourseOffering(Guid id, string title, int capacity, long version,
        ImmutableHashSet<Guid> learners)
    {
        Id = id;
        Title = title;
        Capacity = capacity;
        Version = version;
        _learners = learners;
    }

    public Guid Id { get; }
    public string Title { get; }
    public int Capacity { get; }
    public long Version { get; }
    public int EnrolledCount => _learners.Count;
    public int AvailableSeats => Capacity - EnrolledCount;

    /// <summary>A detached persistence snapshot; the immutable array cannot mutate this aggregate.</summary>
    public ImmutableArray<Guid> LearnerIds => _learners.Order().ToImmutableArray();

    /// <summary>
    /// Reconstitutes stored state without replaying business actions. Storage is not automatically
    /// trusted: corrupt counts, duplicate identities, and impossible versions must fail loudly.
    /// This is not a public API contract and must never bind directly to an HTTP request body.
    /// </summary>
    public static CourseOffering Restore(Guid id, string title, int capacity, long version,
        IEnumerable<Guid> learnerIds)
    {
        ArgumentNullException.ThrowIfNull(learnerIds);
        CourseOffering empty = Create(id, title, capacity);
        Guid[] learners = learnerIds.ToArray();
        if (learners.Any(learner => learner == Guid.Empty) || learners.Distinct().Count() != learners.Length)
            throw new ArgumentException("Stored learners must be nonempty and unique.", nameof(learnerIds));
        if (learners.Length > capacity || version < learners.Length)
            throw new ArgumentException("Stored state violates capacity or version invariants.", nameof(version));
        return new CourseOffering(empty.Id, empty.Title, empty.Capacity, version, learners.ToImmutableHashSet());
    }

    public static CourseOffering Create(Guid id, string title, int capacity)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("An offering identifier is required.", nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        if (title.Trim().Length > 200)
            throw new ArgumentException("The title must contain at most 200 characters.", nameof(title));
        if (capacity is < 1 or > 1_000)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be between 1 and 1000.");

        return new CourseOffering(id, title.Trim(), capacity, 0, ImmutableHashSet<Guid>.Empty);
    }

    /// <summary>
    /// Produces a new valid state without modifying a snapshot held by another caller. Duplicate
    /// enrollment is idempotent even when the offering is full; no additional seat is consumed.
    /// Persistence must still compare Version atomically to prevent two snapshots claiming the last seat.
    /// </summary>
    public EnrollmentDecision Enroll(Guid learnerId)
    {
        if (learnerId == Guid.Empty)
            throw new ArgumentException("A learner identifier is required.", nameof(learnerId));
        if (_learners.Contains(learnerId))
            return new EnrollmentDecision(EnrollmentOutcome.AlreadyEnrolled, this);
        if (AvailableSeats == 0)
            return new EnrollmentDecision(EnrollmentOutcome.Full, this);

        return new EnrollmentDecision(EnrollmentOutcome.Enrolled,
            new CourseOffering(Id, Title, Capacity, checked(Version + 1), _learners.Add(learnerId)));
    }
}

public enum EnrollmentOutcome { Enrolled, AlreadyEnrolled, Full }

public sealed record EnrollmentDecision(EnrollmentOutcome Outcome, CourseOffering Offering);
