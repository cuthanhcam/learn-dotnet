using OopBasics.Examples.Polymorphism;
using Xunit;

namespace OopBasics.Tests.Polymorphism;

public class AbstractClassExampleTests
{
    [Fact]
    public void EmailNotification_And_SmsNotification_Send_DoesNotThrow()
    {
        Notification email = new EmailNotification();
        Notification sms = new SmsNotification();
        email.Send("Hello via Email");
        sms.Send("Hello via SMS");
    }
}
