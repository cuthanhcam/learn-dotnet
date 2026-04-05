using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBasics.Examples.ControlFlow
{
    /// <summary>
    /// Comprehensive lesson for switch expression and switch statement usage.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Switch statements vs switch expressions:
    /// - Switch statement: Traditional if-like syntax, useful for side effects
    /// - Switch expression: Modern, concise, returns values, C# 8.0+
    /// 
    /// Key topics:
    /// - Pattern matching with switch expressions
    /// - Guard clauses in switch patterns
    /// - Default/fallback cases
    /// - Relational patterns (>, <, >=, <=)
    /// - Logical patterns (or, and, not)
    /// - Type matching
    /// 
    /// Best practices:
    /// - Use switch expressions for value returns
    /// - Use switch statements when multiple actions needed
    /// - Always handle default case explicitly
    /// - Use pattern matching for complex conditions
    /// - Guard clauses add readability to switch expressions
    /// </summary>
    public static class SwitchExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} SwitchExample {new string('=', 5)}");

            PrintSection("SWITCH EXPRESSION WITH ENUM");
            DemoDayTypeClassification();

            PrintSection("SWITCH STATEMENT WITH CHAR");
            DemoGradeDescription();

            PrintSection("RELATIONAL PATTERNS");
            DemoNumberClassification();

            PrintSection("SWITCH EXPRESSION WITH TUPLES");
            DemoMonthToQuarter();

            PrintSection("LOGICAL PATTERNS");
            DemoSeasonFromMonth();

            PrintSection("COMPLEX PATTERN MATCHING");
            DemoComplexPatterns();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Classifies day of week using switch expression.
        /// Demonstrates pattern matching with enums.
        /// </summary>
        public static string GetDayType(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Saturday or DayOfWeek.Sunday => "Weekend", // Multiple patterns combined with 'or'
                _ => "Weekday"
            };
        }

        /// <summary>
        /// Describes grade using traditional switch statement.
        /// Demonstrates fall-through and default case.
        /// Traditional approach when multiple statements needed.
        /// </summary>
        public static string DescribeGrade(char grade)
        {
            switch (char.ToUpperInvariant(grade)) // Normalize input to uppercase for case-insensitive matching
            {
                case 'A':
                    return "Outstanding achievement";
                case 'B':
                    return "Very good performance";
                case 'C':
                    return "Good work";
                case 'D':
                    return "Satisfactory, needs improvement";
                case 'F':
                    return "Fail, major revision required";
                default:
                    return "Invalid grade";
            }
        }

        /// <summary>
        /// Classifies numbers using relational patterns.
        /// Demonstrates range patterns with comparisons.
        /// </summary>
        public static string ClassifyNumber(int value)
        {
            return value switch
            {
                < -100 => "Very negative",
                >= -100 and < 0 => "Negative",
                0 => "Zero",
                > 0 and <= 100 => "Small positive",
                > 100 => "Large positive"
            };
        }

        /// <summary>
        /// Determines quarter from month number.
        /// Demonstrates range patterns with inclusive bounds.
        /// </summary>
        public static int QuarterFromMonth(int month)
        {
            return month switch
            {
                >= 1 and <= 3 => 1,
                >= 4 and <= 6 => 2,
                >= 7 and <= 9 => 3,
                >= 10 and <= 12 => 4,
                _ => 0  // Invalid month
            };
        }

        /// <summary>
        /// Determines season from month using logical patterns.
        /// Demonstrates OR pattern for multiple values.
        /// </summary>
        public static string SeasonFromMonth(int month)
        {
            return month switch // 
            {
                12 or 1 or 2 => "Winter",
                3 or 4 or 5 => "Spring",
                6 or 7 or 8 => "Summer",
                9 or 10 or 11 => "Autumn",
                _ => "Invalid month"
            };
        }

        /// <summary>
        /// Calculates ticket price based on age and type.
        /// Demonstrates pattern matching with guard clauses.
        /// </summary>
        public static decimal GetTicketPrice(int age, string ticketType)
        {
            if (string.IsNullOrWhiteSpace(ticketType))
            {
                throw new ArgumentException("Ticket type is required.", nameof(ticketType));
            }

            return (age, ticketType.ToLowerInvariant()) switch
            {
                // Children
                ( < 5, _) => 0m,                        // Free for very young
                ( >= 5 and < 13, _) => 5m,              // Child price
                                                        // Standard
                ( >= 13 and < 65, "standard") => 10m,   // Adult price
                ( >= 13 and < 65, "premium") => 15m,    // Premium price
                                                        // Senior
                ( >= 65, "standard") => 7m,             // Senior discount
                ( >= 65, "premium") => 12m,             // Senior premium
                                                        // Fallback
                _ => 10m
            };
        }

        /// <summary>
        /// Classifies person based on age with guard clause conditions.
        /// </summary>
        public static string ClassifyPerson(int age)
        {
            return age switch
            {
                < 0 => "Invalid age",
                0 => "Newborn",
                < 13 => "Child",
                < 18 => "Teenager",
                < 65 => "Adult",
                _ => "Senior"
            };
        }

        /// <summary>
        /// Validates and categorizes input using complex patterns.
        /// Demonstrates not pattern and combination patterns.
        /// </summary>
        public static bool TryValidateUserStatus(int age, bool isActive, out string status)
        {
            if (age < 0)
            {
                status = "Invalid age";
                return false;
            }

            status = (age, isActive) switch
            {
                (0, false) => "Inactive newborn",
                (0, true) => "Active newborn",
                ( > 0 and < 18, false) => "Inactive minor",
                ( > 0 and < 18, true) => "Active minor",
                ( >= 18 and < 65, false) => "Inactive adult",
                ( >= 18 and < 65, true) => "Active adult",
                ( >= 65, false) => "Inactive senior",
                ( >= 65, true) => "Active senior",
                _ => "Unknown status"
            };

            return true;
        }

        // PRIVATE METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"{new string('-', 3)} {title} {new string('-', 3)}");
        }

        /// <summary>
        /// Demonstrates switch expression with enum pattern matching.
        /// </summary>
        private static void DemoDayTypeClassification()
        {
            var days = new[]
            {
            DayOfWeek.Monday,
            DayOfWeek.Wednesday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        };

            foreach (DayOfWeek day in days)
            {
                Console.WriteLine($"  {day,-10} => {GetDayType(day)}");
            }
        }

        /// <summary>
        /// Demonstrates switch statement with char matching.
        /// </summary>
        private static void DemoGradeDescription()
        {
            char[] grades = ['A', 'B', 'C', 'D', 'F', 'Z'];

            foreach (char grade in grades)
            {
                Console.WriteLine($"  Grade {grade} => {DescribeGrade(grade)}");
            }
        }

        /// <summary>
        /// Demonstrates relational patterns for number classification.
        /// </summary>
        private static void DemoNumberClassification()
        {
            int[] numbers = [-150, -50, 0, 25, 150];

            foreach (int num in numbers)
            {
                Console.WriteLine($"  {num:D4} => {ClassifyNumber(num)}");
            }
        }

        /// <summary>
        /// Demonstrates conversion with switch expressions.
        /// </summary>
        private static void DemoMonthToQuarter()
        {
            Console.WriteLine("Month to Quarter mapping:");
            for (int month = 1; month <= 12; month++)
            {
                int quarter = QuarterFromMonth(month);
                Console.WriteLine($"  Month {month:D2} => Q{quarter}");
            }
        }

        /// <summary>
        /// Demonstrates logical OR patterns in switch expressions.
        /// </summary>
        private static void DemoSeasonFromMonth()
        {
            Console.WriteLine("Month to Season mapping:");
            for (int month = 1; month <= 12; month++)
            {
                string season = SeasonFromMonth(month);
                Console.WriteLine($"  Month {month:D2} => {season}");
            }
        }

        /// <summary>
        /// Demonstrates complex pattern matching with tuples and guards.
        /// </summary>
        private static void DemoComplexPatterns()
        {
            Console.WriteLine("Ticket pricing (age, type):");
            var testCases = new[] { (5, "standard"), (12, "standard"), (25, "premium"), (65, "standard"), (70, "premium") };

            foreach (var (age, type) in testCases)
            {
                decimal price = GetTicketPrice(age, type);
                Console.WriteLine($"  Age {age:D2}, {type} => ${price}");
            }

            Console.WriteLine();
            Console.WriteLine("Person classification:");
            int[] testAges = [-1, 5, 13, 18, 45, 65];

            foreach (int age in testAges)
            {
                string classification = ClassifyPerson(age);
                Console.WriteLine($"  Age {age:D2} => {classification}");
            }

            Console.WriteLine();
            Console.WriteLine("User status validation:");
            if (TryValidateUserStatus(25, true, out string status))
            {
                Console.WriteLine($"  Valid: {status}");
            }

            if (!TryValidateUserStatus(-5, true, out string invalidStatus))
            {
                Console.WriteLine($"  Invalid: {invalidStatus}");
            }
        }
    }
}
