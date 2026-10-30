using System.Net;
using System.Text.Json;

namespace Learning.Testing.IntegrationTests;

// These requests do not mutate the fixture. Write-path tests below each own a fresh host instead.
[Trait("Category", "Integration")]
public sealed class ReadOnlyHostTests(ReminderApiFactory factory) : IClassFixture<ReminderApiFactory>
{
    [Fact]
    public async Task GetHealth_RunningHost_ReturnsJsonStatus()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("ok", body.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task GetUnknownRoute_ReturnsNotFound()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/not-a-route");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Empty(factory.Sender.Attempts);
    }
}
