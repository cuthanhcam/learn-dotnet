using CoreDotNet.Examples.DelegatesAndEvents;

namespace CoreDotNet.Tests.DelegatesAndEvents;

[Collection("Console")]
public class DelegatesAndEventsExampleTests
{
    [Fact]
    public void ClickableButton_Raises_Clicked_Event_With_Increasing_Counts()
    {
        var button = new ClickableButton("Submit");
        var clickCounts = new List<int>();

        button.Clicked += (_, args) => clickCounts.Add(args.ClickCount);

        button.Click();
        button.Click();

        Assert.Equal([1, 2], clickCounts);
    }

    [Fact]
    public void DataPublisher_Raises_DataReceived_With_Payload()
    {
        var publisher = new DataPublisher();
        DataReceivedEventArgs? received = null;

        publisher.DataReceived += (_, args) => received = args;

        publisher.PublishData("Important update");

        Assert.NotNull(received);
        Assert.Equal("Important update", received!.Data);
        Assert.NotEqual(default, received.ReceivedAt);
    }

    [Fact]
    public void Run_Prints_Delegate_And_Event_Sections()
    {
        string output = ConsoleCapture.Run(DelegatesAndEventsExample.Run);

        Assert.Contains("Delegates & Events Examples", output);
        Assert.Contains("Predicate-selected topics:", output);
        Assert.Contains("Publishing data...", output);
    }
}
