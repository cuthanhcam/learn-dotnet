using CSharpBasics.Examples.Methods;

namespace CSharpBasics.Tests;

public class MethodsTests
{
    [Fact]
    public void MethodBasics_Methods_WorkCorrectly()
    {
        Assert.Equal(7, MethodBasicsExample.Add(3, 4));
        Assert.Equal(10.0, MethodBasicsExample.Multiply(2.5, 4.0));
        Assert.Equal(80m, MethodBasicsExample.ApplyDiscount(100m, 20m));
        Assert.Throws<ArgumentOutOfRangeException>(() => MethodBasicsExample.ApplyDiscount(-1m, 10m));
        Assert.Throws<ArgumentOutOfRangeException>(() => MethodBasicsExample.ApplyDiscount(100m, 101m));

        Assert.Equal("Welcome Cam! You are studying C#.", MethodBasicsExample.BuildWelcomeMessage("Cam", "C#"));
        Assert.Throws<ArgumentException>(() => MethodBasicsExample.BuildWelcomeMessage("", "C#"));

        Assert.Equal(29, MethodBasicsExample.ParseAgeOrDefault("29", 18));
        Assert.Equal(18, MethodBasicsExample.ParseAgeOrDefault("abc", 18));
        Assert.Throws<ArgumentOutOfRangeException>(() => MethodBasicsExample.ParseAgeOrDefault("29", 0));

        var (min, max) = MethodBasicsExample.GetMinMax([5, 2, 10]);
        Assert.Equal(2, min);
        Assert.Equal(10, max);
        Assert.Throws<ArgumentException>(() => MethodBasicsExample.GetMinMax(Array.Empty<int>()));

        var stats = MethodBasicsExample.GetStatistics([2, 4, 6]);
        Assert.Equal(2, stats.Min);
        Assert.Equal(6, stats.Max);
        Assert.Equal(4.0, stats.Average);

        Assert.True(MethodBasicsExample.TryDivide(10m, 2m, out var result));
        Assert.Equal(5m, result);
        Assert.False(MethodBasicsExample.TryDivide(10m, 0m, out _));

        var ok = MethodBasicsExample.DivideResult(10m, 2m);
        Assert.True(ok.Success);
        Assert.Equal(5m, ok.Value);

        var fail = MethodBasicsExample.DivideResult(10m, 0m);
        Assert.False(fail.Success);
        Assert.NotNull(fail.Error);
    }

    [Fact]
    public void ParamModifiers_Methods_WorkCorrectly()
    {
        int x = 1;
        ParamModifiersExample.Increment(ref x);
        Assert.Equal(2, x);

        double d = 2.0;
        ParamModifiersExample.Double(ref d);
        Assert.Equal(4.0, d);

        int a = 1, b = 2;
        ParamModifiersExample.Swap(ref a, ref b);
        Assert.Equal(2, a);
        Assert.Equal(1, b);

        Assert.True(ParamModifiersExample.TryDivide(17, 5, out int q, out int r));
        Assert.Equal(3, q);
        Assert.Equal(2, r);
        Assert.False(ParamModifiersExample.TryDivide(17, 0, out _, out _));

        var modern = ParamModifiersExample.TryDivideModern(17, 5);
        Assert.True(modern.Success);
        Assert.Equal(3, modern.Quotient);
        Assert.Equal(2, modern.Remainder);

        Assert.True(ParamModifiersExample.TryParseNameAge("John:25", out string name, out int age));
        Assert.Equal("John", name);
        Assert.Equal(25, age);
        Assert.False(ParamModifiersExample.TryParseNameAge("bad", out _, out _));

        int p = 3, q2 = 4;
        Assert.Equal(7, ParamModifiersExample.Sum(in p, in q2));

        var c1 = new ParamModifiersExample.ComplexValue(1, "A", 1.0, 1m, 2m, 3m);
        var c2 = new ParamModifiersExample.ComplexValue(1, "A", 1.0, 9m, 8m, 7m);
        Assert.True(ParamModifiersExample.AreEqual(in c1, in c2));

        int[] arr = [10, 20, 30];
        ref int middle = ref ParamModifiersExample.GetElementRef(arr, 1);
        middle += 5;
        Assert.Equal(25, arr[1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => ParamModifiersExample.GetElementRef(arr, 10));
    }

    [Fact]
    public void Overloading_Methods_WorkCorrectly()
    {
        Assert.Equal(12, OverloadingExample.Multiply(3, 4));
        Assert.Equal(10.0, OverloadingExample.Multiply(2.5, 4.0));
        Assert.Equal(10m, OverloadingExample.Multiply(2.5m, 4m));
        Assert.Equal(24, OverloadingExample.Multiply(2, 3, 4));
        Assert.Equal(1, OverloadingExample.Multiply());

        Assert.Equal(3, OverloadingExample.Add(1, 2));
        Assert.Equal(4.0, OverloadingExample.Add(1.5, 2.5));
        Assert.Equal(10, OverloadingExample.Add(1, 2, 3, 4));
        Assert.Equal(30m, OverloadingExample.Add<decimal>(10m, 20m));

        Assert.Equal("John Doe", OverloadingExample.FormatPair(" John ", " Doe "));
        Assert.Equal("John-Doe", OverloadingExample.FormatPair(" John ", " Doe ", "-"));
        Assert.Throws<ArgumentNullException>(() => OverloadingExample.FormatPair(null!, "x"));
    }

    [Fact]
    public void OptionalParameters_Methods_WorkCorrectly()
    {
        Assert.Equal("Cam (Student) - Active", OptionalParametersExample.CreateUserLabel("Cam"));
        Assert.Equal("Cam (Mentor) - Inactive", OptionalParametersExample.CreateUserLabel("Cam", "Mentor", false));

        var fixedTime = new DateTime(2026, 4, 19, 12, 0, 0, DateTimeKind.Utc);
        Assert.Contains("created [doc-1]", OptionalParametersExample.FormatAuditMessage("created", "doc-1", fixedTime));
        Assert.Throws<ArgumentException>(() => OptionalParametersExample.FormatAuditMessage("", "doc-1"));

        Assert.Equal("https://api/users", OptionalParametersExample.BuildQueryString("https://api/users"));
        var query = OptionalParametersExample.BuildQueryString("https://api/users", pageNumber: 2, pageSize: 20, sortBy: "created date");
        Assert.Contains("page=2", query);
        Assert.Contains("pageSize=20", query);
        Assert.Contains("sortBy=created%20date", query);
        Assert.Throws<ArgumentOutOfRangeException>(() => OptionalParametersExample.BuildQueryString("x", pageNumber: 0));

        var notification = OptionalParametersExample.SendNotification("u@example.com", "hello");
        Assert.Contains("retries=3", notification);
        Assert.Throws<ArgumentOutOfRangeException>(() => OptionalParametersExample.SendNotification("u@example.com", "hello", delayMs: -1));

        var withDefaults = OptionalParametersExample.SendNotificationWithOptions("u@example.com", "hello");
        Assert.Contains("via email", withDefaults);

        var custom = OptionalParametersExample.SendNotificationWithOptions(
            "u@example.com",
            "hello",
            new OptionalParametersExample.NotificationOptions { Channel = "sms", IsHighPriority = true, DelayMs = 100, MaxRetries = 5 });
        Assert.Contains("high-priority", custom);
        Assert.Contains("via sms", custom);

        Assert.Throws<ArgumentOutOfRangeException>(() => OptionalParametersExample.SendNotificationWithOptions(
            "u@example.com",
            "hello",
            new OptionalParametersExample.NotificationOptions { DelayMs = 70_000 }));

        var logWithoutTime = OptionalParametersExample.LogMessage("message", includeTimestamp: false);
        Assert.Contains("[Info] [General]", logWithoutTime);
    }
}
