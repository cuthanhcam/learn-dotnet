namespace Learning.Auth.Application.Abstractions;

public enum PasswordVerification
{
    Failed,
    Succeeded,
    SucceededRehashNeeded
}

public interface IPasswordHashService
{
    string Hash(string password);

    PasswordVerification Verify(string passwordHash, string suppliedPassword);

    /// <summary>
    /// Performs equivalent expensive work when no account exists, reducing a timing-based
    /// username-enumeration signal without inventing a fake account in the application layer.
    /// </summary>
    void VerifyUnknownAccount(string suppliedPassword);
}
