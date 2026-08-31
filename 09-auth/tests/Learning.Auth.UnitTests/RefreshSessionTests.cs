using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Sessions;
using Learning.Auth.Infrastructure.Sessions;
using Learning.Auth.Infrastructure.Tokens;

namespace Learning.Auth.UnitTests;

public sealed class RefreshSessionTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 31, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Issue_ProducesOpaqueRandomValueAndStableDigest()
    {
        var tokens = new CryptographicRefreshTokenService(CreateOptions());

        IssuedRefreshToken first = tokens.Issue(Now);
        IssuedRefreshToken second = tokens.Issue(Now);

        Assert.NotEqual(first.Value, second.Value);
        Assert.NotEqual(first.Value, first.Digest);
        Assert.Equal(first.Digest, tokens.ComputeDigest(first.Value));
        Assert.Equal(Now.AddDays(14), first.ExpiresAt);
    }

    [Fact]
    public async Task ConcurrentRotation_AllowsOneWinnerAndReplayRevokesItsReplacement()
    {
        var store = new InMemoryRefreshSessionStore();
        RefreshSession original = RefreshSession.Issue(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "ORIGINAL", Now, Now.AddDays(14));
        await store.AddAsync(original, CancellationToken.None);

        // Run on separate workers so both callers genuinely contend for the same atomic transition.
        Task<RefreshRotationResult> first = Task.Run(async () => await store.RotateAsync(
            "ORIGINAL", "REPLACEMENT-A", Now.AddDays(14), Now.AddMinutes(1), CancellationToken.None));
        Task<RefreshRotationResult> second = Task.Run(async () => await store.RotateAsync(
            "ORIGINAL", "REPLACEMENT-B", Now.AddDays(14), Now.AddMinutes(1), CancellationToken.None));
        RefreshRotationResult[] results = await Task.WhenAll(first, second);

        Assert.Single(results, result => result.Status == RefreshRotationStatus.Rotated);
        Assert.Single(results, result => result.Status == RefreshRotationStatus.ReplayDetected);

        string winningDigest = results[0].Status == RefreshRotationStatus.Rotated
            ? "REPLACEMENT-A"
            : "REPLACEMENT-B";
        RefreshRotationResult afterReplay = await store.RotateAsync(winningDigest, "NEXT",
            Now.AddDays(14), Now.AddMinutes(2), CancellationToken.None);
        Assert.Equal(RefreshRotationStatus.Revoked, afterReplay.Status);
    }

    [Fact]
    public async Task Rotation_ExpiredSessionFailsAndNeverCreatesReplacement()
    {
        var store = new InMemoryRefreshSessionStore();
        RefreshSession expired = RefreshSession.Issue(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "EXPIRED", Now.AddDays(-2), Now.AddDays(-1));
        await store.AddAsync(expired, CancellationToken.None);

        RefreshRotationResult result = await store.RotateAsync("EXPIRED", "NEVER-CREATED",
            Now.AddDays(14), Now, CancellationToken.None);

        Assert.Equal(RefreshRotationStatus.Expired, result.Status);
        Assert.Null(result.UserId);
    }

    private static JwtOptions CreateOptions() => new()
    {
        Issuer = "https://issuer.example",
        Audience = "learning-api",
        SigningKey = "unit-test-only-signing-key-at-least-32-bytes"
    };
}
