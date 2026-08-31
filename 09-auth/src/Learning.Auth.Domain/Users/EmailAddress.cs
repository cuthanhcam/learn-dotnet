namespace Learning.Auth.Domain.Users;

/// <summary>
/// Stores both the user-facing address and the stable lookup key used for uniqueness checks.
/// Full deliverability still requires an ownership-verification flow; syntax alone is not proof.
/// </summary>
public sealed record EmailAddress
{
    private EmailAddress(string value, string normalizedValue)
    {
        Value = value;
        NormalizedValue = normalizedValue;
    }

    public string Value { get; }

    public string NormalizedValue { get; }

    public static EmailAddress Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        string trimmed = value.Trim();
        if (trimmed.Length > 254 || trimmed.Contains('\r') || trimmed.Contains('\n'))
        {
            throw new ArgumentException("Email must be a single-line value of at most 254 characters.", nameof(value));
        }

        int separator = trimmed.LastIndexOf('@');
        if (separator <= 0 || separator == trimmed.Length - 1)
        {
            throw new ArgumentException("Email must contain a local part and domain.", nameof(value));
        }

        // This repository uses an ordinal, case-insensitive account lookup policy. Real systems must
        // document their provider-specific normalization and migration rules before changing it.
        return new EmailAddress(trimmed, trimmed.ToUpperInvariant());
    }
}
