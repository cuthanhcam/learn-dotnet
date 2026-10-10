using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;

namespace Learning.Architecture.IntegrationTests;

internal sealed class EnrollmentApiFactory : WebApplicationFactory<Program>
{
    public static readonly Guid SampleOffering = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private readonly SqliteConnection _enrollment;
    private readonly SqliteConnection _notifications;
    private readonly RSA _signer = RSA.Create(2048);
    private readonly bool _realJwt;

    public EnrollmentApiFactory(bool realJwt = false)
    {
        _realJwt = realJwt;
        // Keepers preserve isolated named in-memory databases while scoped contexts open their own
        // connections. Parallel tests never share a database, external identity server, or filesystem.
        string suffix = Guid.NewGuid().ToString("N");
        _enrollment = new SqliteConnection($"Data Source=enrollment-{suffix};Mode=Memory;Cache=Shared");
        _notifications = new SqliteConnection($"Data Source=notifications-{suffix};Mode=Memory;Cache=Shared");
        _enrollment.Open();
        _notifications.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["ConnectionStrings:Enrollment"] = _enrollment.ConnectionString,
                ["ConnectionStrings:Notifications"] = _notifications.ConnectionString,
                ["Identity:Authority"] = "https://identity.example.test",
                ["Identity:Audience"] = "architecture-tests",
                ["Database:Initialize"] = "true",
                ["Outbox:Enabled"] = "false"
            }));
        if (_realJwt)
        {
            builder.ConfigureServices(services => services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    // Deterministic OIDC metadata replaces the remote discovery document, NOT the
                    // bearer handler. Signature, issuer, audience, expiry, and authorization run for real.
                    var configuration = new OpenIdConnectConfiguration { Issuer = "https://identity.example.test" };
                    configuration.SigningKeys.Add(new RsaSecurityKey(_signer));
                    options.Configuration = configuration;
                    options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration);
                    options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                }));
            return;
        }
        builder.ConfigureServices(services => services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "TestOnly";
            options.DefaultChallengeScheme = "TestOnly";
            options.DefaultForbidScheme = "TestOnly";
        }).AddScheme<AuthenticationSchemeOptions, TestIdentityHandler>("TestOnly", _ => { }));
    }

    public string CreateAccessToken(string audience = "architecture-tests", bool expired = false,
        bool wrongSignature = false, string issuer = "https://identity.example.test", string scope = "enrollment.write")
    {
        using RSA otherSigner = RSA.Create(2048);
        var key = new RsaSecurityKey(wrongSignature ? otherSigner : _signer);
        var token = new JwtSecurityToken(issuer, audience,
            [new Claim("learner_id", Guid.NewGuid().ToString()), new Claim("scope", scope)],
            DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(expired ? -1 : 5),
            new SigningCredentials(key, SecurityAlgorithms.RsaSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _enrollment.Dispose();
            _notifications.Dispose();
            _signer.Dispose();
        }
    }
}

/// <summary>
/// A transport/authORIZATION fixture only. This class lives in the TEST assembly, is not a JWT
/// validator, and must never be copied into the API. Cryptographic token validation is a separate test.
/// </summary>
internal sealed class TestIdentityHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-Learner", out var learner))
            return Task.FromResult(AuthenticateResult.NoResult());
        var claims = new List<Claim> { new("scope", Request.Headers["X-Test-Scope"].ToString()) };
        if (learner != "missing")
            claims.Add(new Claim("learner_id", learner.ToString()));
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
    }
}
