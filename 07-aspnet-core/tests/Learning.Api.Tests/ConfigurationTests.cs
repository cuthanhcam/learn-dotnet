using Learning.Api.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Learning.Api.Tests;

public sealed class ConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ConfigurationTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public void Options_AreBoundFromApplicationConfiguration()
    {
        LearningOptions options = _factory.Services
            .GetRequiredService<IOptions<LearningOptions>>()
            .Value;

        Assert.Equal("Learn .NET Catalog", options.CatalogName);
        Assert.Equal(100, options.MaximumPageSize);
    }

    [Fact]
    public void InvalidOptions_PreventHostFromStarting()
    {
        using WebApplicationFactory<Program> invalidFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    [$"{LearningOptions.SectionName}:MaximumPageSize"] = "0"
                });
            });
        });

        Assert.Throws<OptionsValidationException>(invalidFactory.CreateClient);
    }
}
