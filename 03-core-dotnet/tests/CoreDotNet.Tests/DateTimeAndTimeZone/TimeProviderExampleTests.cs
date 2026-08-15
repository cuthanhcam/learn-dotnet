using CoreDotNet.Examples.DateTimeAndTimeZone;

namespace CoreDotNet.Tests.DateTimeAndTimeZone;

public class TimeProviderExampleTests
{
    private static readonly DateTimeOffset ReferenceTime =
        new(2026, 8, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void IsActive_BeforeExpiration_ReturnsTrue()
    {
        var policy = new SubscriptionPolicy(new FixedTimeProvider(ReferenceTime));

        bool isActive = policy.IsActive(ReferenceTime.AddMinutes(1));

        Assert.True(isActive);
    }

    [Fact]
    public void IsActive_AtExactExpiration_ReturnsFalse()
    {
        var policy = new SubscriptionPolicy(new FixedTimeProvider(ReferenceTime));

        bool isActive = policy.IsActive(ReferenceTime);

        Assert.False(isActive);
    }

    [Fact]
    public void Remaining_AfterExpiration_ClampsToZero()
    {
        var policy = new SubscriptionPolicy(new FixedTimeProvider(ReferenceTime));

        TimeSpan remaining = policy.Remaining(ReferenceTime.AddSeconds(-1));

        Assert.Equal(TimeSpan.Zero, remaining);
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
