namespace CoreDotNet.Examples.DelegatesAndEvents
{
    /// <summary>
    /// Comprehensive examples for delegates and events.
    ///
    /// This lesson focuses on how callbacks and events shape real APIs:
    /// - Delegate types for reusable behavior.
    /// - Action, Func, and Predicate as common built-in delegate forms.
    /// - Event publishing and subscription lifecycles.
    /// - Multicast invocation and unsubscription patterns.
    ///
    /// Best practices:
    /// - Unsubscribe from events when the subscriber outlives the publisher.
    /// - Use EventArgs subclasses to pass meaningful event payloads.
    /// - Keep delegates small and focused.
    /// - Prefer explicit method groups when the callback is reused.
    /// </summary>
    public static class DelegatesAndEventsExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} Delegates & Events Examples {new string('=', 5)}");

            PrintSection("DELEGATES BASICS");
            DemoDelegates();

            PrintSection("ACTION AND FUNC");
            DemoActionAndFunc();

            PrintSection("PREDICATE FILTERING");
            DemoPredicateFiltering();

            PrintSection("EVENT PATTERN");
            DemoEventPattern();

            PrintSection("MULTICAST DELEGATES");
            DemoMulticastDelegates();

            PrintSection("PUBLISHER-SUBSCRIBER PATTERN");
            DemoPublisherSubscriber();

            Console.WriteLine();
        }

        private static void DemoDelegates()
        {
            // Create delegate instance using Action
            Action<string> printer = (msg) => Console.WriteLine($"Message: {msg}");
            printer("Hello from delegate!");

            // Delegate as parameter - Execute callback
            Execute(msg => Console.WriteLine($"Callback: {msg}"));

            void Execute(Action<string> callback)
            {
                callback("Executing from method");
            }
        }

        private static void DemoActionAndFunc()
        {
            // Action: void return type
            Action<string, int> printRepeat = (text, count) =>
            {
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine($"  {i + 1}: {text}");
                }
            };
            printRepeat("Hello", 3);

            // Func: return value
            Func<int, int, int> add = (a, b) => a + b;
            Console.WriteLine($"Add(5, 3) = {add(5, 3)}");

            // Func with complex logic
            Func<string, bool> isValidEmail = email =>
            {
                return email.Contains("@") && email.Length > 5;
            };
            Console.WriteLine($"Is 'test@example.com' valid: {isValidEmail("test@example.com")}");
        }

        private static void DemoPredicateFiltering()
        {
            Predicate<string> isLearningTopic = topic => topic.Contains(".") || topic.Contains("&");
            string[] topics = new[] { "Collections", "File I/O", "Delegates & Events", "LINQ" };
            var selected = topics.Where(topic => isLearningTopic(topic)).ToList();

            Console.WriteLine($"Predicate-selected topics: {string.Join(", ", selected)}");

            Func<int, int, int> combineScores = (left, right) => left + right;
            Console.WriteLine($"Combined score: {combineScores(7, 5)}");
        }

        private static void DemoEventPattern()
        {
            var button = new ClickableButton("Submit");

            // Subscribe to events
            button.Clicked += (sender, e) =>
            {
                Console.WriteLine($"Button clicked: {e.ClickCount} times");
            };

            button.Clicked += (sender, e) =>
            {
                Console.WriteLine($"  Time: {e.ClickTime:HH:mm:ss}");
            };

            // Trigger events
            button.Click();
            button.Click();
        }

        private static void DemoMulticastDelegates()
        {
            Action<string> action = msg => Console.WriteLine($"Handler 1: {msg}");
            action += msg => Console.WriteLine($"Handler 2: {msg}");
            action += msg => Console.WriteLine($"Handler 3: {msg}");

            Console.WriteLine("Invoking multicast delegate:");
            action("All handlers will execute");

            // Remove handler - note: -= can result in null if all handlers are removed
            action = (action - (msg => Console.WriteLine($"Handler 2: {msg}")))!;
            Console.WriteLine("After removing handler 2:");
            action?.Invoke("Now only 1 and 3 execute");
        }

        private static void DemoPublisherSubscriber()
        {
            var publisher = new DataPublisher();
            var subscriber1 = new DataSubscriber("Subscriber1");
            var subscriber2 = new DataSubscriber("Subscriber2");

            // Subscribe
            publisher.DataReceived += subscriber1.OnDataReceived;
            publisher.DataReceived += subscriber2.OnDataReceived;

            Console.WriteLine("Publishing data...");
            publisher.PublishData("Important update");

            // Unsubscribe
            publisher.DataReceived -= subscriber1.OnDataReceived;
            Console.WriteLine("\nSubscriber1 unsubscribed");
            publisher.PublishData("Second update");
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }

    // Event examples
    public class ClickableButton
    {
        public string Name { get; }
        private int _clickCount = 0;

        public event EventHandler<ButtonClickedEventArgs>? Clicked;

        public ClickableButton(string name)
        {
            Name = name;
        }

        public void Click()
        {
            _clickCount++;
            OnClicked(new ButtonClickedEventArgs { ClickCount = _clickCount, ClickTime = DateTime.Now });
        }

        protected void OnClicked(ButtonClickedEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }

    public class ButtonClickedEventArgs : EventArgs
    {
        public int ClickCount { get; set; }
        public DateTime ClickTime { get; set; }
    }

    public class DataPublisher
    {
        public event EventHandler<DataReceivedEventArgs>? DataReceived;

        public void PublishData(string data)
        {
            Console.WriteLine($"Publishing payload: {data}");
            OnDataReceived(new DataReceivedEventArgs { Data = data, ReceivedAt = DateTime.Now });
        }

        protected void OnDataReceived(DataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }
    }

    public class DataReceivedEventArgs : EventArgs
    {
        public string Data { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; }
    }

    public class DataSubscriber
    {
        public string Name { get; }

        public DataSubscriber(string name)
        {
            Name = name;
        }

        public void OnDataReceived(object? sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"  {Name} received: {e.Data} at {e.ReceivedAt:HH:mm:ss}");
        }
    }
}
