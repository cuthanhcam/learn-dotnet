namespace Learning.Auth.Domain.Users;

public enum AccountStatus
{
    Active,
    Disabled
}

/// <summary>
/// Represents authoritative account state. It deliberately contains no JWT or HTTP concepts.
/// </summary>
public sealed class UserAccount
{
    private readonly Lock _signInStateLock = new();
    private readonly HashSet<string> _roles = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _permissions = new(StringComparer.Ordinal);
    private int _failedSignInCount;
    private DateTimeOffset? _lockoutEnd;

    private UserAccount(Guid id, EmailAddress email, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        Status = AccountStatus.Active;
        _roles.Add(RoleNames.Member);
        _permissions.Add(PermissionNames.ProfileRead);
    }

    public Guid Id { get; }

    public EmailAddress Email { get; }

    public string PasswordHash { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public AccountStatus Status { get; private set; }

    /// <summary>
    /// Gets the number of consecutive failed password verifications. The learning repository returns
    /// shared account objects, so access is synchronized just like the state transitions below.
    /// A relational implementation should use an atomic update or optimistic concurrency token.
    /// </summary>
    public int FailedSignInCount
    {
        get
        {
            lock (_signInStateLock)
                return _failedSignInCount;
        }
    }

    public DateTimeOffset? LockoutEnd
    {
        get
        {
            lock (_signInStateLock)
                return _lockoutEnd;
        }
    }

    public IReadOnlySet<string> Roles => _roles;

    public IReadOnlySet<string> Permissions => _permissions;

    public static UserAccount Register(
        Guid id,
        EmailAddress email,
        string passwordHash,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("A user identifier is required.", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        return new UserAccount(id, email, passwordHash, createdAt);
    }

    public void ReplacePasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
    }

    public void Disable() => Status = AccountStatus.Disabled;

    public bool IsLockedOut(DateTimeOffset now)
    {
        lock (_signInStateLock)
            return _lockoutEnd is { } lockoutEnd && lockoutEnd > now;
    }

    /// <summary>
    /// Atomically records a failed password verification and starts a bounded lockout after the
    /// configured threshold. The count is capped so hostile traffic cannot overflow it.
    /// </summary>
    public void RecordFailedSignIn(DateTimeOffset now, int threshold, TimeSpan lockoutDuration)
    {
        if (threshold < 1)
            throw new ArgumentOutOfRangeException(nameof(threshold), "The threshold must be positive.");
        if (lockoutDuration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(lockoutDuration), "The duration must be positive.");

        lock (_signInStateLock)
        {
            // Attempts during an active lockout do not extend it indefinitely. Rate limiting handles
            // sustained abuse, while a legitimate owner gets a predictable recovery time.
            if (_lockoutEnd is { } activeEnd && activeEnd > now)
                return;

            // An expired window starts a new sequence rather than immediately locking the account again.
            if (_lockoutEnd is not null)
            {
                _lockoutEnd = null;
                _failedSignInCount = 0;
            }

            _failedSignInCount = Math.Min(_failedSignInCount + 1, threshold);
            if (_failedSignInCount >= threshold)
                _lockoutEnd = now.Add(lockoutDuration);
        }
    }

    /// <summary>
    /// Clears stale failure state only after a valid password is presented outside an active lockout.
    /// </summary>
    public void RecordSuccessfulSignIn()
    {
        lock (_signInStateLock)
        {
            _failedSignInCount = 0;
            _lockoutEnd = null;
        }
    }

    public void GrantRole(string role)
    {
        if (role is not (RoleNames.Member or RoleNames.Administrator))
            throw new ArgumentOutOfRangeException(nameof(role), "Role is not part of the application vocabulary.");
        _roles.Add(role);
    }

    public void GrantPermission(string permission)
    {
        if (permission is not PermissionNames.ProfileRead)
            throw new ArgumentOutOfRangeException(nameof(permission), "Permission is not part of the application vocabulary.");
        _permissions.Add(permission);
    }
}

public static class PermissionNames
{
    public const string ProfileRead = "profile.read";
}

public static class RoleNames
{
    public const string Member = "member";
    public const string Administrator = "administrator";
}
