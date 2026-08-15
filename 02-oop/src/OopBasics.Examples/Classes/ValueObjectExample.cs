namespace OopBasics.Examples.Classes;

public static class ValueObjectExample
{
    public static void Run()
    {
        Console.WriteLine("ValueObjectExample: Encapsulating domain values");
        var email = new Email("user@Example.COM");
        Console.WriteLine($"Normalized email: {email}");

        if (!Email.TryCreate("invalid-email", out _, out string? error))
        {
            Console.WriteLine($"Validation error: {error}");
        }

        Console.WriteLine("- Value objects enforce validation and normalization at creation.");
        Console.WriteLine("- Equality follows the domain value rather than object identity.");
    }
}

/// <summary>
/// A deliberately small email-address value object for teaching equality and invariants.
/// It is not intended to implement every address form allowed by the complete email RFCs.
/// </summary>
public sealed class Email : IEquatable<Email>
{
    public Email(string value)
    {
        (LocalPart, Domain) = Parse(value);
        Value = $"{LocalPart}@{Domain}";
    }

    public string LocalPart { get; }
    public string Domain { get; }
    public string Value { get; }

    public static bool TryCreate(string? value, out Email? email, out string? error)
    {
        try
        {
            email = new Email(value!);
            error = null;
            return true;
        }
        catch (ArgumentException exception)
        {
            email = null;
            error = exception.Message;
            return false;
        }
    }

    public bool Equals(Email? other) =>
        other is not null &&
        StringComparer.Ordinal.Equals(LocalPart, other.LocalPart) &&
        StringComparer.OrdinalIgnoreCase.Equals(Domain, other.Domain);

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(
        StringComparer.Ordinal.GetHashCode(LocalPart),
        StringComparer.OrdinalIgnoreCase.GetHashCode(Domain));

    public override string ToString() => Value;

    public static bool operator ==(Email? left, Email? right) => Equals(left, right);
    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);

    private static (string LocalPart, string Domain) Parse(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("An email address cannot contain whitespace.", nameof(value));
        }

        int separator = value.IndexOf('@');
        if (separator <= 0 ||
            separator != value.LastIndexOf('@') ||
            separator == value.Length - 1)
        {
            throw new ArgumentException(
                "An email address must contain one non-edge '@' separator.",
                nameof(value));
        }

        return (value[..separator], value[(separator + 1)..].ToLowerInvariant());
    }
}
