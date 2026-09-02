using Microsoft.AspNetCore.Mvc.Testing;

namespace Learning.Auth.IntegrationTests.Infrastructure;

public sealed class AuthApiFactory : WebApplicationFactory<Program>
{
    // Test-only key material. Production keys belong in a secret manager or identity provider.
    public const string SigningKey = "integration-tests-only-signing-key-32-bytes-minimum";

    public AuthApiFactory()
    {
        // Program validates security configuration during bootstrap, before ConfigureWebHost runs.
        // The test process therefore supplies its non-production key through the same environment
        // variable channel used by a deployment platform before the in-memory host is created.
        Environment.SetEnvironmentVariable("Jwt__SigningKey", SigningKey);
    }
}
