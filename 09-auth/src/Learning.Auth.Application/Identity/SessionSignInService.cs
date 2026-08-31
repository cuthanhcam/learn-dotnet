using Learning.Auth.Application.Abstractions;

namespace Learning.Auth.Application.Identity;

public sealed record SessionSignInResult(SignInStatus Status, AccessToken? AccessToken = null);

public sealed class SessionSignInService(
    CredentialSignInService credentials,
    IAccessTokenIssuer accessTokens)
{
    public async Task<SessionSignInResult> SignInAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        SignInResult result = await credentials.VerifyAsync(email, password, cancellationToken)
            .ConfigureAwait(false);

        // Token issuance follows every credential and account-state check; it never bypasses them.
        return result is { Status: SignInStatus.Succeeded, Account: not null }
            ? new SessionSignInResult(result.Status, accessTokens.Issue(result.Account))
            : new SessionSignInResult(result.Status);
    }
}
