namespace Learning.Auth.Application.Identity;

/// <summary>
/// Defines the account-level layer of sign-in abuse protection. Per-network throttling is configured
/// independently at the HTTP boundary because the two controls mitigate different attacks.
/// </summary>
public sealed class SignInSecurityOptions
{
    public const string SectionName = "SignInSecurity";

    public int MaxFailedAttempts { get; init; } = 5;

    public int LockoutMinutes { get; init; } = 15;

    public TimeSpan LockoutDuration => TimeSpan.FromMinutes(LockoutMinutes);

    public void Validate()
    {
        if (MaxFailedAttempts is < 2 or > 20)
            throw new InvalidOperationException("SignInSecurity:MaxFailedAttempts must be between 2 and 20.");
        if (LockoutMinutes is < 1 or > 1_440)
            throw new InvalidOperationException("SignInSecurity:LockoutMinutes must be between 1 and 1440.");
    }
}
