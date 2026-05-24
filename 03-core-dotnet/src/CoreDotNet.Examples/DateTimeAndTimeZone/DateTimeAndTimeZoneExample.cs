namespace CoreDotNet.Examples.DateTimeAndTimeZone
{
    /// <summary>
    /// Comprehensive examples for DateTime and timezone handling.
    ///
    /// This lesson emphasizes decisions developers make in real systems:
    /// - Choosing DateTimeOffset for timestamps that cross machine boundaries.
    /// - Handling local-time ambiguity around daylight-saving transitions.
    /// - Formatting, parsing, and comparing dates in a predictable way.
    /// - Calculating durations and business-friendly relative times.
    ///
    /// Best practices:
    /// - Store timestamps in UTC or DateTimeOffset.
    /// - Use TimeZoneInfo for conversions, not manual offsets.
    /// - Be explicit about culture when parsing and formatting.
    /// - Treat ambiguous or invalid local times as a data quality issue.
    /// </summary>
    public static class DateTimeAndTimeZoneExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} DateTime & TimeZone Examples {new string('=', 5)}");

            PrintSection("DATETIME BASICS");
            DemoDateTimeBasics();

            PrintSection("DATETIME KINDS");
            DemoDateTimeKinds();

            PrintSection("DATETIMEOFFSET BASICS");
            DemoDateTimeOffset();

            PrintSection("TIMESPAN OPERATIONS");
            DemoTimeSpanOperations();

            PrintSection("TIMEZONE CONVERSIONS");
            DemoTimeZoneConversions();

            PrintSection("DAYLIGHT-SAVING EDGE CASES");
            DemoDaylightSavingEdgeCases();

            PrintSection("FORMATTING AND PARSING");
            DemoFormattingParsing();

            PrintSection("PRACTICAL PATTERNS");
            DemoPracticalPatterns();

            Console.WriteLine();
        }

        private static void DemoDateTimeBasics()
        {
            // Current date and time
            var now = DateTime.Now;
            var utcNow = DateTime.UtcNow;

            Console.WriteLine($"Local now: {now}");
            Console.WriteLine($"UTC now: {utcNow}");

            // Creating specific dates
            var specificDate = new DateTime(2024, 5, 15, 14, 30, 0);
            Console.WriteLine($"Specific date: {specificDate}");

            // DateTime arithmetic
            var tomorrow = now.AddDays(1);
            var nextWeek = now.AddDays(7);
            var nextMonth = now.AddMonths(1);

            Console.WriteLine($"Tomorrow: {tomorrow:yyyy-MM-dd}");
            Console.WriteLine($"Next week: {nextWeek:yyyy-MM-dd}");
            Console.WriteLine($"Next month: {nextMonth:yyyy-MM-dd}");

            // Date components
            Console.WriteLine($"Year: {now.Year}, Month: {now.Month}, Day: {now.Day}");
            Console.WriteLine($"DayOfWeek: {now.DayOfWeek}, DayOfYear: {now.DayOfYear}");
        }

        private static void DemoDateTimeKinds()
        {
            // Unspecified (default)
            var unspecified = new DateTime(2024, 5, 15);
            Console.WriteLine($"Unspecified kind: {unspecified.Kind}");

            // UTC
            var utcDate = DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
            Console.WriteLine($"UTC date: {utcDate} (Kind: {utcDate.Kind})");

            // Local
            var localDate = DateTime.SpecifyKind(unspecified, DateTimeKind.Local);
            Console.WriteLine($"Local date: {localDate} (Kind: {localDate.Kind})");

            // Converting between kinds
            var utcNow = DateTime.UtcNow;
            var localNow = utcNow.ToLocalTime();
            Console.WriteLine($"UTC to Local: {utcNow:HH:mm} UTC -> {localNow:HH:mm} Local");
        }

        private static void DemoTimeSpanOperations()
        {
            // Create TimeSpan
            var duration1 = TimeSpan.FromHours(2.5);
            var duration2 = new TimeSpan(1, 30, 0); // 1 hour 30 minutes

            Console.WriteLine($"Duration 1: {duration1.TotalHours} hours");
            Console.WriteLine($"Duration 2: {duration2.TotalMinutes} minutes");

            // TimeSpan arithmetic
            var start = new DateTime(2024, 5, 15, 10, 0, 0);
            var end = new DateTime(2024, 5, 15, 14, 30, 0);
            var elapsed = end - start;

            Console.WriteLine($"Elapsed: {elapsed.Hours} hours, {elapsed.Minutes} minutes");

            // TimeSpan components
            Console.WriteLine($"Total seconds: {elapsed.TotalSeconds}");
            Console.WriteLine($"Total milliseconds: {elapsed.TotalMilliseconds}");

            // Comparisons
            var ts1 = TimeSpan.FromHours(1);
            var ts2 = TimeSpan.FromMinutes(60);
            Console.WriteLine($"1 hour == 60 minutes: {ts1 == ts2}");
        }

        private static void DemoTimeZoneConversions()
        {
            var utcTime = new DateTime(2024, 5, 15, 12, 0, 0, DateTimeKind.Utc);

            // Get timezone info
            var pacificTz = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var easternTz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            // Convert to different timezones
            var pacificTime = TimeZoneInfo.ConvertTime(utcTime, pacificTz);
            var easternTime = TimeZoneInfo.ConvertTime(utcTime, easternTz);

            Console.WriteLine($"UTC:      {utcTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Pacific:  {pacificTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Eastern:  {easternTime:yyyy-MM-dd HH:mm:ss}");

            // Check DST
            bool isDstPacific = pacificTz.IsDaylightSavingTime(pacificTime);
            Console.WriteLine($"Pacific time is DST: {isDstPacific}");

            // DateTimeOffset for API communication
            var offset = new DateTimeOffset(utcTime);
            Console.WriteLine($"DateTimeOffset: {offset} (Offset: {offset.Offset})");
        }

        private static void DemoDateTimeOffset()
        {
            var scheduledMeeting = new DateTimeOffset(2024, 5, 15, 14, 30, 0, TimeSpan.FromHours(7));
            var utcMeeting = scheduledMeeting.ToUniversalTime();

            Console.WriteLine($"Scheduled meeting: {scheduledMeeting:yyyy-MM-dd HH:mm zzz}");
            Console.WriteLine($"UTC equivalent: {utcMeeting:yyyy-MM-dd HH:mm 'UTC'}");
            Console.WriteLine($"Offset minutes: {scheduledMeeting.Offset.TotalMinutes}");
        }

        private static void DemoDaylightSavingEdgeCases()
        {
            var pacificTz = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

            var springForwardGap = new DateTime(2024, 3, 10, 2, 30, 0, DateTimeKind.Unspecified);
            var fallBackAmbiguous = new DateTime(2024, 11, 3, 1, 30, 0, DateTimeKind.Unspecified);

            Console.WriteLine($"Spring-forward time invalid: {pacificTz.IsInvalidTime(springForwardGap)}");
            Console.WriteLine($"Fall-back time ambiguous: {pacificTz.IsAmbiguousTime(fallBackAmbiguous)}");
        }

        private static void DemoFormattingParsing()
        {
            var date = new DateTime(2024, 5, 15, 14, 30, 45);
            var invariantCulture = System.Globalization.CultureInfo.InvariantCulture;

            // Formatting
            Console.WriteLine("Formatting examples:");
            Console.WriteLine($"Short date:  {date:d}");
            Console.WriteLine($"Long date:   {date:D}");
            Console.WriteLine($"Full date:   {date:F}");
            Console.WriteLine($"ISO 8601:    {date:o}");
            Console.WriteLine($"Custom:      {date:yyyy-MM-dd HH:mm:ss}");

            // Parsing
            string dateString = "2024-05-15";
            if (DateTime.TryParse(dateString, out var parsed))
            {
                Console.WriteLine($"Parsed '{dateString}': {parsed:yyyy-MM-dd}");
            }

            // Parse with specific format
            string dateWithTime = "15/05/2024 14:30";
            if (DateTime.TryParseExact(dateWithTime, "dd/MM/yyyy HH:mm",
                invariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var preciseDate))
            {
                Console.WriteLine($"Parsed precise: {preciseDate}");
            }

            string roundTrip = date.ToString("O", invariantCulture);
            Console.WriteLine($"Round-trip format: {roundTrip}");
        }

        private static void DemoPracticalPatterns()
        {
            // Pattern: Store UTC, display local
            var eventUtc = new DateTime(2024, 12, 25, 12, 0, 0, DateTimeKind.Utc);
            var eventLocal = eventUtc.ToLocalTime();
            Console.WriteLine($"Event (stored UTC): {eventUtc:O}");
            Console.WriteLine($"Event (display local): {eventLocal:yyyy-MM-dd HH:mm}");

            // Pattern: Age calculation
            var birthDate = new DateTime(1990, 5, 15);
            int age = CalculateAge(birthDate);
            Console.WriteLine($"Age from {birthDate:yyyy-MM-dd}: {age} years");

            // Pattern: Business hours check
            bool inBusinessHours = IsInBusinessHours(DateTime.Now);
            Console.WriteLine($"Current time in business hours (9-17): {inBusinessHours}");

            // Pattern: Relative time display
            var pastDate = DateTime.Now.AddDays(-3);
            string relative = GetRelativeTime(pastDate);
            Console.WriteLine($"3 days ago displayed as: {relative}");
        }

        private static int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Today.Year - birthDate.Year;
            if (birthDate.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }
            return age;
        }

        private static bool IsInBusinessHours(DateTime dateTime)
        {
            return dateTime.Hour >= 9 && dateTime.Hour < 17;
        }

        private static string GetRelativeTime(DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;
            if (span.TotalSeconds < 60) return "just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            return dateTime.ToString("yyyy-MM-dd");
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }
}
