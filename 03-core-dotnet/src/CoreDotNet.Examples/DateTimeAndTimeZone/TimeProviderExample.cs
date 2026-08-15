namespace CoreDotNet.Examples.DateTimeAndTimeZone;

/// <summary>
/// Evaluates time-based subscription rules without reading the system clock directly.
/// Injecting <see cref="TimeProvider"/> makes the same production policy deterministic in tests.
/// </summary>
public sealed class SubscriptionPolicy
{
    private readonly TimeProvider _timeProvider;

    public SubscriptionPolicy(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    /// <summary>
    /// A subscription is active until, but not including, its expiration instant.
    /// Defining this boundary explicitly prevents ambiguous behavior at exact expiry.
    /// </summary>
    public bool IsActive(DateTimeOffset expiresAt) => expiresAt > _timeProvider.GetUtcNow();

    /// <summary>
    /// Returns a non-negative duration so callers never need to interpret a negative value.
    /// </summary>
    public TimeSpan Remaining(DateTimeOffset expiresAt)
    {
        TimeSpan remaining = expiresAt - _timeProvider.GetUtcNow();
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
}

public static class TimeProviderExample
{
    public static void Run()
    {
        var policy = new SubscriptionPolicy(TimeProvider.System);
        DateTimeOffset expiresAt = TimeProvider.System.GetUtcNow().AddDays(7);

        Console.WriteLine("TimeProvider example:");
        Console.WriteLine($"Subscription active: {policy.IsActive(expiresAt)}");
        Console.WriteLine($"Whole days remaining: {(int)policy.Remaining(expiresAt).TotalDays}");
    }
}
