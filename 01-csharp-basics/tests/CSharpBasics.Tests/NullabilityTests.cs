using CSharpBasics.Examples.Nullability;

namespace CSharpBasics.Tests;

public class NullabilityTests
{
    [Fact]
    public void ParseNullableInteger_HandlesValidAndInvalid()
    {
        Assert.Equal(123, NullabilityExample.ParseNullableInteger("123"));
        Assert.Null(NullabilityExample.ParseNullableInteger("abc"));
        Assert.Null(NullabilityExample.ParseNullableInteger(null));
    }

    [Fact]
    public void GetFirstNonNull_ReturnsFirstAvailableOrFallback()
    {
        Assert.Equal("B", NullabilityExample.GetFirstNonNull(null, "B", "C", "F"));
        Assert.Equal("F", NullabilityExample.GetFirstNonNull(null, null, null, "F"));
    }

    [Fact]
    public void GetUserAgeOrNull_HandlesNullChains()
    {
        Assert.Null(NullabilityExample.GetUserAgeOrNull(null));
        Assert.Null(NullabilityExample.GetUserAgeOrNull(new NullabilityExample.User { Name = "Cam", Profile = null }));
        Assert.Equal(30, NullabilityExample.GetUserAgeOrNull(new NullabilityExample.User
        {
            Name = "Cam",
            Profile = new NullabilityExample.UserProfile { Age = 30 }
        }));
    }

    [Fact]
    public void ClassifyValue_CoversPatterns()
    {
        Assert.Equal("Value is null", NullabilityExample.ClassifyValue(null));
        Assert.Equal("Empty string", NullabilityExample.ClassifyValue(""));
        Assert.Equal("Non-empty string (3 chars)", NullabilityExample.ClassifyValue("abc"));
        Assert.Equal("Integer: 42", NullabilityExample.ClassifyValue(42));
        Assert.Equal("User: Cam", NullabilityExample.ClassifyValue(new NullabilityExample.User { Name = "Cam" }));
    }

    [Fact]
    public void ProcessName_ValidatesInput()
    {
        NullabilityExample.ProcessName("Cam");
        Assert.Throws<ArgumentNullException>(() => NullabilityExample.ProcessName(null));
        Assert.Throws<ArgumentException>(() => NullabilityExample.ProcessName("   "));
    }
}
