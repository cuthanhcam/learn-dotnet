using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpBasics.Examples.Collections
{
    /// <summary>
    /// Comprehensive lesson for HashSet and set operations.
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// HashSet<T> characteristics:
    /// - Unordered collection
    /// - No duplicates (unique values only)
    /// - O(1) average lookup
    /// - No indexing (can't access by index)
    /// - Case-sensitive for strings (unless StringComparer provided)
    /// 
    /// Set operations:
    /// - Intersection: Common elements in both sets
    /// - Union: All elements from both sets (no duplicates)
    /// - Difference/Except: Elements in first set but not in second
    /// - IsSubsetOf: All elements of first in second
    /// - IsSupersetOf: First contains all elements of second
    /// 
    /// Key topics:
    /// - Removing duplicates
    /// - Set operations (intersection, union, difference)
    /// - Efficient membership testing (Contains)
    /// - Case-insensitive comparison
    /// - Performance characteristics vs List<T>
    /// 
    /// When to use HashSet<T>:
    /// - Removing duplicates efficiently
    /// - Set operations needed
    /// - Fast membership testing
    /// - Don't need to preserve order
    /// - Don't need index access
    /// 
    /// When NOT to use HashSet<T>:
    /// - Need ordering → use SortedSet<T>
    /// - Need index access → use List<T>
    /// - Duplicates meaningful → use List<T>
    /// - Very small collections → List<T> may be faster
    /// 
    /// Best practices:
    /// - Specify StringComparer for case-insensitive comparisons
    /// - Use Contains for membership testing (efficient)
    /// - Use set operations instead of manual loops
    /// - Remember: unordered (order not guaranteed)
    /// - Initialize with capacity when size known
    /// </summary>
    public static class HashSetExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} HashSetExample {new string('=', 5)}");

            PrintSection("REMOVING DUPLICATES");
            DemoRemovingDuplicates();

            PrintSection("INTERSECTION (Common elements)");
            DemoIntersection();

            PrintSection("UNION (All elements)");
            DemoUnion();

            PrintSection("DIFFERENCE (First - Second)");
            DemoDifference();

            PrintSection("SYMMETRIC DIFFERENCE");
            DemoSymmetricDifference();

            PrintSection("SUBSET & SUPERSET");
            DemoSubsetSuperset();

            PrintSection("MEMBERSHIP TESTING");
            DemoMembershipTesting();

            PrintSection("CASE-INSENSITIVE COMPARISON");
            DemoCaseInsensitive();

            PrintSection("PERFORMANCE NOTES");
            DemoPerformanceNotes();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Removes duplicates from a sequence.
        /// Average time complexity: O(n).
        /// </summary>
        public static HashSet<int> RemoveDuplicates(IEnumerable<int> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return new HashSet<int>(values);
        }

        /// <summary>
        /// Intersection: elements that exist in both collections.
        /// Average time complexity: O(n + m).
        /// </summary>
        public static HashSet<string> IntersectTags(IEnumerable<string> left, IEnumerable<string> right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            HashSet<string> result = new(left, StringComparer.OrdinalIgnoreCase);
            result.IntersectWith(right);
            return result;
        }

        /// <summary>
        /// Union: all unique elements from both collections.
        /// Average time complexity: O(n + m).
        /// </summary>
        public static HashSet<string> UnionTags(IEnumerable<string> left, IEnumerable<string> right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            HashSet<string> result = new(left, StringComparer.OrdinalIgnoreCase);
            result.UnionWith(right);
            return result;
        }

        /// <summary>
        /// Difference: elements in left but not in right.
        /// Average time complexity: O(n + m).
        /// </summary>
        public static HashSet<string> DifferenceTags(IEnumerable<string> left, IEnumerable<string> right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            HashSet<string> result = new(left, StringComparer.OrdinalIgnoreCase);
            result.ExceptWith(right);
            return result;
        }

        /// <summary>
        /// Symmetric difference: elements in either set, but not both.
        /// Useful for "changed between versions" style comparisons.
        /// </summary>
        public static HashSet<string> SymmetricDifferenceTags(IEnumerable<string> left, IEnumerable<string> right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            HashSet<string> result = new(left, StringComparer.OrdinalIgnoreCase);
            result.SymmetricExceptWith(right);
            return result;
        }

        public static bool IsSubset(IEnumerable<string> first, IEnumerable<string> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            HashSet<string> firstSet = new(first, StringComparer.OrdinalIgnoreCase);
            HashSet<string> secondSet = new(second, StringComparer.OrdinalIgnoreCase);
            return firstSet.IsSubsetOf(secondSet);
        }

        public static bool IsSuperset(IEnumerable<string> first, IEnumerable<string> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            HashSet<string> firstSet = new(first, StringComparer.OrdinalIgnoreCase);
            HashSet<string> secondSet = new(second, StringComparer.OrdinalIgnoreCase);
            return firstSet.IsSupersetOf(secondSet);
        }

        /// <summary>
        /// Checks whether two sets share at least one common element.
        /// Stops early as soon as overlap is detected.
        /// </summary>
        public static bool HasAnyCommon(IEnumerable<string> first, IEnumerable<string> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            HashSet<string> firstSet = new(first, StringComparer.OrdinalIgnoreCase);
            return firstSet.Overlaps(second);
        }

        /// <summary>
        /// Adds item and returns true only when item was not present.
        /// For case-insensitive behavior, set must be created with StringComparer.OrdinalIgnoreCase.
        /// </summary>
        public static bool TryAddUnique(HashSet<string> set, string item)
        {
            ArgumentNullException.ThrowIfNull(set);
            ArgumentException.ThrowIfNullOrWhiteSpace(item);
            return set.Add(item);
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        private static void DemoRemovingDuplicates()
        {
            int[] original = [1, 1, 2, 3, 3, 3, 4, 5, 5];
            HashSet<int> unique = RemoveDuplicates(original);

            Console.WriteLine($"Original: {string.Join(", ", original)}");
            Console.WriteLine($"Unique: {string.Join(", ", unique.OrderBy(x => x))}");
            Console.WriteLine($"Duplicates removed: {original.Length - unique.Count}");
        }

        private static void DemoIntersection()
        {
            string[] tags1 = ["csharp", "dotnet", "api"];
            string[] tags2 = ["dotnet", "azure", "api"];

            HashSet<string> common = IntersectTags(tags1, tags2);

            Console.WriteLine($"Tags 1: {string.Join(", ", tags1)}");
            Console.WriteLine($"Tags 2: {string.Join(", ", tags2)}");
            Console.WriteLine($"Common: {string.Join(", ", common.OrderBy(x => x))}");
        }

        private static void DemoUnion()
        {
            string[] tags1 = ["csharp", "dotnet"];
            string[] tags2 = ["azure", "dotnet"];

            HashSet<string> combined = UnionTags(tags1, tags2);

            Console.WriteLine($"Tags 1: {string.Join(", ", tags1)}");
            Console.WriteLine($"Tags 2: {string.Join(", ", tags2)}");
            Console.WriteLine($"Union: {string.Join(", ", combined.OrderBy(x => x))}");
        }

        private static void DemoDifference()
        {
            string[] owned = ["csharp", "dotnet", "api"];
            string[] removed = ["dotnet"];

            HashSet<string> remaining = DifferenceTags(owned, removed);

            Console.WriteLine($"Owned: {string.Join(", ", owned)}");
            Console.WriteLine($"Removed: {string.Join(", ", removed)}");
            Console.WriteLine($"Remaining: {string.Join(", ", remaining.OrderBy(x => x))}");
        }

        private static void DemoSymmetricDifference()
        {
            string[] teamA = ["csharp", "dotnet", "sql"];
            string[] teamB = ["dotnet", "azure", "devops"];

            HashSet<string> onlyOneTeamHas = SymmetricDifferenceTags(teamA, teamB);
            Console.WriteLine($"Only one team has: {string.Join(", ", onlyOneTeamHas.OrderBy(x => x))}");
        }

        private static void DemoSubsetSuperset()
        {
            string[] coreSkills = ["csharp"];
            string[] allSkills = ["csharp", "dotnet", "sql", "javascript"];

            Console.WriteLine($"coreSkills ⊆ allSkills: {IsSubset(coreSkills, allSkills)}");
            Console.WriteLine($"allSkills ⊇ coreSkills: {IsSuperset(allSkills, coreSkills)}");
        }

        private static void DemoMembershipTesting()
        {
            string[] backend = ["csharp", "dotnet", "sql"];
            string[] mobile = ["kotlin", "swift", "dotnet"];

            bool overlaps = HasAnyCommon(backend, mobile);
            Console.WriteLine($"Has any common skill: {overlaps}");

            HashSet<string> tags = new(StringComparer.OrdinalIgnoreCase) { "api", "cloud" };
            // Add returns false when element already exists under comparer rules.
            Console.WriteLine($"Add 'API' first time: {TryAddUnique(tags, "API")}");
            Console.WriteLine($"Add 'api' second time: {TryAddUnique(tags, "api")}");
        }

        private static void DemoCaseInsensitive()
        {
            string[] items1 = ["CSHARP", "DOTNET"];
            string[] items2 = ["csharp", "azure"];

            var intersection = IntersectTags(items1, items2);

            Console.WriteLine($"Intersection (case-insensitive): {string.Join(", ", intersection.OrderBy(x => x))}");
        }

        private static void DemoPerformanceNotes()
        {
            Console.WriteLine("HashSet.Contains is O(1) average and usually faster than List.Contains for large sets.");
            Console.WriteLine("HashSet is unordered; if order matters, use SortedSet or sort before output.");
            Console.WriteLine("Use set operations (UnionWith/IntersectWith/ExceptWith) over manual loops.");
            Console.WriteLine("StringComparer.OrdinalIgnoreCase prevents casing duplicates like 'API' vs 'api'.");
        }
    }
}
