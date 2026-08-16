using Learning.Api.Features.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Learning.Api.Tests.Infrastructure;

public sealed class LearningApiFactory : WebApplicationFactory<Program>
{
    public ManualTimeProvider Clock { get; } =
        new(DateTimeOffset.Parse("2030-01-02T03:04:05Z"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Learning:CatalogName"] = "Integration Test Catalog"
            }));
        builder.ConfigureTestServices(services =>
        {
            // Every factory instance owns fresh mutable infrastructure. Test classes can run in
            // parallel without sharing repository state or depending on execution order.
            services.RemoveAll<IProductRepository>();
            services.AddSingleton<IProductRepository, InMemoryProductRepository>();
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(Clock);
        });
    }
}
