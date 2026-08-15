---
title: "Date, Time, and Time Zones"
description: "DateTime, DateTimeOffset, TimeSpan, time-zone conversion, parsing, formatting, and testable clocks."
phase: 3
order: 7
topics: [dotnet, datetime, timezones]
---

# 🕐 DateTime & TimeZone: Temporal Operations

## Overview

Working with dates, times, and timezones requires careful consideration. This section covers DateTime operations, timezone handling, and common patterns.

## Table of Contents

1. [DateTime Basics](#datetime-basics)
2. [DateTime Operations](#datetime-operations)
3. [TimeSpan](#timespan)
4. [Timezone Handling](#timezone-handling)
5. [Formatting and Parsing](#formatting-and-parsing)
6. [Best Practices](#best-practices)
7. [Common Pitfalls](#common-pitfalls)

## DateTime Basics

### DateTime Structure

```csharp
// Create DateTime
var now = DateTime.Now;              // Current local time
var utcNow = DateTime.UtcNow;        // Current UTC time
var specific = new DateTime(2024, 5, 15, 14, 30, 0); // May 15, 2024, 2:30 PM

// DateTime components
var year = now.Year;
var month = now.Month;
var day = now.Day;
var hour = now.Hour;
var minute = now.Minute;
var second = now.Second;
var dayOfWeek = now.DayOfWeek;      // Monday, Tuesday, etc.
var dayOfYear = now.DayOfYear;
```

### DateTime vs DateTimeOffset

```csharp
// DateTime - No timezone information
var dt = DateTime.Now;
// Ambiguous if offset changes

// DateTimeOffset - Includes timezone offset
var dto = DateTimeOffset.Now;
// Clear: local time with UTC offset
// Example: 2024-05-15 14:30:00 -07:00
```

### Comparing DateTime Values

```csharp
var date1 = new DateTime(2024, 5, 15);
var date2 = new DateTime(2024, 5, 20);

bool isEqual = date1 == date2;           // false
bool isBefore = date1 < date2;           // true
int comparison = date1.CompareTo(date2); // -1 (less than)
```

## DateTime Operations

### Adding/Subtracting Time

```csharp
var date = DateTime.Now;

// Add/subtract time
var tomorrow = date.AddDays(1);
var nextWeek = date.AddDays(7);
var nextYear = date.AddYears(1);
var in5Minutes = date.AddMinutes(5);

// Also: AddMonths, AddHours, AddSeconds, AddMilliseconds, etc.

// Using TimeSpan
var timeSpan = TimeSpan.FromDays(3);
var threedays = date + timeSpan;
```

### Date Boundaries

```csharp
var now = DateTime.Now;

// Beginning of day
var startOfDay = now.Date; // Midnight
// Or: new DateTime(now.Year, now.Month, now.Day);

// End of day
var endOfDay = now.Date.AddDays(1).AddTicks(-1);

// Beginning of year
var startOfYear = new DateTime(now.Year, 1, 1);

// Beginning of month
var startOfMonth = new DateTime(now.Year, now.Month, 1);
```

### Last Day of Month

```csharp
public static DateTime GetLastDayOfMonth(DateTime date)
{
    return new DateTime(date.Year, date.Month, 1)
        .AddMonths(1)
        .AddDays(-1);
}

// Alternative
int daysInMonth = DateTime.DaysInMonth(2024, 5); // 31
var lastDay = new DateTime(2024, 5, daysInMonth);
```

## TimeSpan

### Creating TimeSpans

```csharp
// Various constructors
var ts1 = new TimeSpan(hours: 2, minutes: 30, seconds: 0);
var ts2 = TimeSpan.FromDays(1);
var ts3 = TimeSpan.FromHours(2.5);
var ts4 = TimeSpan.FromMinutes(150);
var ts5 = TimeSpan.FromMilliseconds(1000);

// From DateTime subtraction
var date1 = new DateTime(2024, 5, 15, 10, 0, 0);
var date2 = new DateTime(2024, 5, 15, 12, 30, 0);
var diff = date2 - date1; // TimeSpan of 2.5 hours
```

### TimeSpan Properties

```csharp
var ts = new TimeSpan(days: 2, hours: 3, minutes: 45, seconds: 30);

var days = ts.Days;           // 2
var hours = ts.Hours;         // 3
var minutes = ts.Minutes;     // 45
var seconds = ts.Seconds;     // 30
var totalDays = ts.TotalDays; // 2.15625
var totalHours = ts.TotalHours;
var totalSeconds = ts.TotalSeconds;

var isNegative = ts < TimeSpan.Zero;
var abs = ts.Duration();
```

## Timezone Handling

### TimeZoneInfo

```csharp
// Get all available timezones
var timezones = TimeZoneInfo.GetSystemTimeZones();

// Get specific timezone
var pst = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
var utc = TimeZoneInfo.Utc;

// Convert between timezones
var utcTime = DateTime.UtcNow;
var localTime = TimeZoneInfo.ConvertTime(utcTime, pst);

// Create custom timezone (fixed offset)
var customTz = TimeZoneInfo.CreateCustomTimeZone(
    id: "CustomZone",
    baseUtcOffset: TimeSpan.FromHours(5),
    displayName: "Custom Time Zone"
);
```

### Avoiding Timezone Issues

```csharp
// ❌ BAD - Ambiguous
var time = DateTime.Now; // Local time, unclear which timezone

// ✅ GOOD - Clear intent
var utcTime = DateTime.UtcNow;  // Always UTC
var localTime = TimeZoneInfo.ConvertTime(utcTime, desiredTimeZone);

// ✅ GOOD - DateTimeOffset
var offset = DateTimeOffset.Now; // Includes UTC offset
```

### Daylight Saving Time

```csharp
var tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
var date = new DateTime(2024, 6, 15);

bool isDaylight = tz.IsDaylightSavingTime(date);

// Get DST transitions
var rules = tz.GetAdjustmentRules();
foreach (var rule in rules)
{
    var startTransition = rule.DaylightTransitionStart;
    var endTransition = rule.DaylightTransitionEnd;
}
```

## Formatting and Parsing

### DateTime Formatting

```csharp
var date = new DateTime(2024, 5, 15, 14, 30, 0);

// Standard format strings
string formats = date.ToString();              // "5/15/2024 2:30:00 PM"
string short_ = date.ToString("d");            // "5/15/2024"
string shortTime = date.ToString("t");         // "2:30 PM"
string longDate = date.ToString("D");          // "Wednesday, May 15, 2024"
string rfc1123 = date.ToString("R");           // "Wed, 15 May 2024 14:30:00 GMT"
string iso8601 = date.ToString("O");           // "2024-05-15T14:30:00.0000000"

// Custom format strings
string custom = date.ToString("yyyy-MM-dd HH:mm:ss");    // "2024-05-15 14:30:00"
string custom2 = date.ToString("MM/dd/yyyy");            // "05/15/2024"
string custom3 = date.ToString("dddd, MMMM d, yyyy");    // "Wednesday, May 15, 2024"
```

### DateTime Parsing

```csharp
// Parse (throws if invalid)
var date = DateTime.Parse("2024-05-15");
var dateTime = DateTime.Parse("5/15/2024 2:30 PM");

// TryParse (safer)
if (DateTime.TryParse("2024-05-15", out var parsed))
{
    // Use parsed
}

// Parse with specific format
var date = DateTime.ParseExact("15-05-2024", "dd-MM-yyyy",
    System.Globalization.CultureInfo.InvariantCulture);
```

### Culture-Aware Formatting

```csharp
var date = new DateTime(2024, 5, 15);
var culture = new System.Globalization.CultureInfo("fr-FR");

string french = date.ToString("D", culture);  // "mercredi 15 mai 2024"

// Use InvariantCulture for serialization
string invariant = date.ToString("O",
    System.Globalization.CultureInfo.InvariantCulture);
```

## Best Practices

### 1. Store UTC, Display Local

```csharp
// ✅ GOOD - Store UTC
var utcTime = DateTime.UtcNow;
_database.SaveTime(utcTime);

// When displaying to user
var tz = TimeZoneInfo.FindSystemTimeZoneById(userTimezone);
var localTime = TimeZoneInfo.ConvertTime(utcTime, tz);
Console.WriteLine(localTime);
```

### 2. Use DateTimeOffset When Timezone Matters

```csharp
// ✅ GOOD for APIs
public struct Event
{
    public DateTimeOffset OccurredAt { get; set; }
}

// Over DateTime when timezone is important
```

### 3. Use Invariant Culture for Serialization

```csharp
// ✅ GOOD - Consistent across cultures
string json = date.ToString("O",
    CultureInfo.InvariantCulture);

// Use InvariantCulture for DateTime.Parse
DateTime parsed = DateTime.Parse(json,
    CultureInfo.InvariantCulture);
```

### 4. Validate Before Using

```csharp
// ✅ GOOD
if (DateTime.TryParse(input, out var date))
{
    if (date > DateTime.MinValue && date < DateTime.MaxValue)
    {
        UseDate(date);
    }
}
```

## Common Pitfalls

### Pitfall 1: Mixing UTC and Local

```csharp
// ❌ WRONG - Ambiguous
var time1 = DateTime.Now;     // Local
var time2 = DateTime.UtcNow;  // UTC
if (time1 > time2) { }        // Wrong comparison!

// ✅ CORRECT
var utc1 = DateTime.Now.ToUniversalTime();
var utc2 = DateTime.UtcNow;
if (utc1 > utc2) { }
```

### Pitfall 2: DST Not Considered

```csharp
// ❌ BAD - Ignores DST
var noon = new DateTime(2024, 3, 10, 12, 0, 0);
var plus2Hours = noon.AddHours(2); // May not be 2 hours later!

// ✅ GOOD - Use TimeZoneInfo
var tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
var noon = DateTime.Parse("2024-03-10 12:00:00");
var converted = TimeZoneInfo.ConvertTime(
    noon.AddHours(2),
    tz
);
```

### Pitfall 3: DateTime.MinValue/MaxValue

```csharp
// ❌ BAD - Uninitialized
DateTime date = default; // 0001-01-01, might be invalid
if (date == DateTime.MinValue) return;

// ✅ GOOD
DateTime? date = null;
if (date.HasValue)
{
    UseDate(date.Value);
}
```

### Pitfall 4: Comparing Dates with Different Kinds

```csharp
// ❌ WRONG
var local = DateTime.Now;
var utc = DateTime.UtcNow;
if (local > utc) { } // Meaningless comparison

// ✅ CORRECT
var localAsUtc = local.ToUniversalTime();
if (localAsUtc > utc) { }
```

## Key Takeaways

- Use DateTime.UtcNow for storing dates internally
- Use DateTimeOffset when timezone matters
- Store UTC, display in local timezone
- Handle DST transitions appropriately
- Use invariant culture for serialization
- Use TryParse instead of Parse when possible
- Consider TimeZoneInfo for conversions
- Be aware of DateTime.MinValue/MaxValue
- Document timezone expectations clearly
- Test around DST transitions
