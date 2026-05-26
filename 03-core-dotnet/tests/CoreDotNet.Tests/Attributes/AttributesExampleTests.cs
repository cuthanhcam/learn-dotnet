using System.Reflection;
using CoreDotNet.Examples.Attributes;

namespace CoreDotNet.Tests.Attributes;

[Collection("Console")]
public class AttributesExampleTests
{
    [Fact]
    public void Run_Prints_Key_Metadata_Sections()
    {
        string output = ConsoleCapture.Run(AttributesExample.Run);

        Assert.Contains("Attributes Examples", output);
        Assert.Contains("Built-in attributes:", output);
        Assert.Contains("Endpoint: /api/users [GET]", output);
        Assert.Contains("User is valid: True", output);
    }

    [Fact]
    public void ApiEndpoint_And_Validation_Attributes_Are_Declared_As_Expected()
    {
        var endpoint = typeof(ApiEndpoint).GetCustomAttribute<EndpointAttribute>();

        Assert.NotNull(endpoint);
        Assert.Equal("/api/users", endpoint!.Route);
        Assert.Equal("GET", endpoint.Method);
        Assert.Equal("User management endpoint", endpoint.Description);

        var getUserMethod = typeof(ApiEndpoint).GetMethod(nameof(ApiEndpoint.GetUser));
        var permission = getUserMethod!.GetCustomAttribute<RequiredPermissionAttribute>();

        Assert.NotNull(permission);
        Assert.Equal("read:users", permission!.Permission);

        var nameProperty = typeof(User).GetProperty(nameof(User.Name));
        Assert.NotNull(nameProperty);
        Assert.NotNull(nameProperty!.GetCustomAttribute<RequiredAttribute>());
        Assert.Equal("Min length 3", nameProperty.GetCustomAttribute<MinLengthAttribute>()?.Rule);
        Assert.Equal("Full name", nameProperty.GetCustomAttribute<DisplayNameAttribute>()?.Name);

        var ageProperty = typeof(User).GetProperty(nameof(User.Age));
        Assert.NotNull(ageProperty);
        Assert.Equal("Age in years", ageProperty!.GetCustomAttribute<DisplayNameAttribute>()?.Name);
        Assert.Equal("Range 0-150", ageProperty.GetCustomAttribute<RangeAttribute>()?.Rule);
    }
}
