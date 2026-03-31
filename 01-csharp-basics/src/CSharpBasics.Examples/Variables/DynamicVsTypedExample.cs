using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBasics.Examples.Variables
{
    /// <summary>
    /// Demostrates the tradeoffs between static typing and dynamic behavior in C#.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Key topics:
    /// - Compile-time type checking vs runtime type resolution
    /// - Performance and safety implications of dynamic types
    /// - Exception handling for runtime failures
    /// - When to use typed vs dynamic approaches
    /// - Common pitfalls of dynamic programming
    /// </summary>
    public static class DynamicVsTypedExample
    {
        /// <summary>
        /// Entry point to run all demos.   
        /// </summary>
        public static void Run()
        {

        }

        // TYPED OPERATIONS

        /// <summary>
        /// Adds two integers using static typing.
        /// - Advantages: Compile-time checking, optimal performance, IntelliSense support
        /// - Best practice: Use typed methods for known types
        /// </summary>
        public static int AddTyped(int left, int right)
        { 
            return left + right;
        }

        /// <summary>
        /// Multiplies two doubles using static typing.
        /// </summary>
        public static double MultiplyTyped(double left, double right)
        {
            return left * right;
        }

        /// <summary>
        /// Concatenates two strings using static typing.
        /// </summary>
        public static string ConcatenateTyped(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left))
            {
                throw new ArgumentException("Left string cannot be null or whitespace.", nameof(left));
            }

            if (string.IsNullOrWhiteSpace(right))
            {
                throw new ArgumentException("Right string cannot be null or whitespace.", nameof(right));
            }

            return $"{left.Trim()} {right.Trim()}";
        }


    }
}
