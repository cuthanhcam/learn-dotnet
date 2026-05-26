using CoreDotNet.Examples.DateTimeAndTimeZone;

namespace CoreDotNet.Tests.DateTimeAndTimeZone;

[Collection("Console")]
public class DateTimeAndTimeZoneExampleTests
{
    [Fact]
    public void Run_Prints_Key_DateTime_and_TimeZone_Examples()
    {
        string output = ConsoleCapture.Run(DateTimeAndTimeZoneExample.Run);

        Assert.Contains("DateTime & TimeZone Examples", output);
        Assert.Contains("1 hour == 60 minutes: True", output);
        Assert.Contains("Spring-forward time invalid: True", output);
        Assert.Contains("Age from 1990-05-15:", output);
        Assert.Contains("3 days ago displayed as:", output);
    }
}
