namespace Learning.Architecture.Api.Security;

/// <summary>
/// Configure a real OAuth/OIDC authority. The API validates access tokens; it does not collect
/// passwords or mint tokens. No development authentication handler is compiled into this project.
/// </summary>
public sealed class IdentitySettings
{
    public string Authority { get; set; } = "";
    public string Audience { get; set; } = "";

    public bool HasHttpsAuthority() => Uri.TryCreate(Authority, UriKind.Absolute, out Uri? authority)
        && authority.Scheme == Uri.UriSchemeHttps && string.IsNullOrEmpty(authority.UserInfo);
}
