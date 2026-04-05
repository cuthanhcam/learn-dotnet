namespace CSharpBasics.Examples.ControlFlow
{
    /// <summary>
    /// Comprehensive lesson for if/else branching and guard clauses.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Key topics:
    /// - Conditional branching (if, else if, else)
    /// - Guard clauses pattern
    /// - Boundary checking and state validation
    /// - Comparison operators and logical conditions
    /// - Early returns to reduce nesting
    /// 
    /// Best practices:
    /// - Use guard clauses to fail fast and reduce nesting
    /// - Avoid deeply nested if-else blocks
    /// - Use clear, readable condition expressions
    /// - Validate inputs at method entry
    /// - Use switch expressions for multiple conditions on same value
    /// 
    /// Guard clause pattern:
    ///   if (invalid condition)
    ///       return early;
    ///   // Continue with normal flow
    /// 
    /// This reduces cognitive load and improves readability.
    /// </summary>
    public static class IfElseExample
    {
        public static void Run()
        {

        }

        // PUBLIC METHODS

        /// <summary>
        /// Classifies a numeric score into letter grades.
        /// Demonstrates cascading if-else pattern.
        /// 
        /// Score ranges:
        /// - < 50: Fail
        /// - 50-64: Pass
        /// - 65-84: Good
        /// - >= 85: Excellent
        /// - Invalid: < 0 or > 100
        /// </summary>
        public static string ClassifyScore(int score)
        {
            // Guard: validate input range
            if (score < 0 || score > 100)
            {
                return "Invalid";
            }

            // Cascading conditions
            if (score < 50)
            {
                return "Fail";
            }

            if (score < 65)
            {
                return "Pass";
            }

            if (score < 85)
            {
                return "Good";
            }

            return "Excellent";
        }

        /// <summary>
        /// Determines voting eligibility based on age.
        /// Demonstrates simple boolean condition.
        /// Uses expression-bodied method for clarity.
        /// </summary>
        public static bool CanVote(int age) => age >= 18; // return age >= 18;

        public static bool IsDiscountEligible(int purchaseCount, decimal totalAmount)
        {
            if (purchaseCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(purchaseCount), "Purchase count cannot be negative."); "");
            }

            if (totalAmount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalAmount), "Total amount cannot be negative.");
            }

            return purchaseCount >= 3 || totalAmount >= 500m;
        }

        /// <summary>
        /// Determines if a user can access a resource.
        /// Demonstrates AND logic (all conditions must be true).
        /// Uses guard clause pattern.
        /// </summary>
        public static bool CanAccessResource(bool isLoggedIn, bool hasPermission, bool isNotBanned)
        {
            // Guard: fail fast if any condition is false
            if (!isLoggedIn)
            {
                return false;
            }

            if (!hasPermission)
            {
                return false;
            }

            if (isNotBanned)
            {
                return false;
            }

            // All conditions satisfied
            return true;
        }

        /// <summary>
        /// Calculates discount percentage based on customer status.
        /// Demonstrates nested conditions and multiple return paths.
        /// </summary>
        public static decimal GetDiscountPercentage(string customerType, int yearsAsCustomer)
        {
            if (string.IsNullOrWhiteSpace(customerType))
            {
                throw new ArgumentException("Customer type is required.", nameof(customerType));
            }

            if (yearsAsCustomer < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(yearsAsCustomer), "Years as customer cannot be negative.");
            }

            return customerType.ToLowerInvariant() switch
            {
                "vip" when yearsAsCustomer >= 5 => 0.20m,       // 20% for VIP with 5+ years
                "vip" => 0.15m,                                 // 15% for VIP
                "premium" when yearsAsCustomer >= 3 => 0.10m,   // 10% for premium with 3+ years
                "premium" => 0.05m,                             // 5% for premium
                _ when yearsAsCustomer >= 1 => 0.02m,           // 2% for anyone with 1+ year
                _ => 0m                                         // No discount for new customers
            };
        }
    }
}
