namespace CoreDotNet.Examples.LINQ
{
    /// <summary>
    /// Comprehensive examples for LINQ (Language Integrated Query).
    ///
    /// This lesson uses realistic data-shaping examples:
    /// - Filtering catalog items and projecting display models.
    /// - Grouping, joining, and summarizing operational data.
    /// - Understanding deferred execution and when to materialize queries.
    /// - Using lookup-style collections and batching helpers.
    ///
    /// Best practices:
    /// - Prefer method syntax for complex chains.
    /// - Materialize results with ToList() when you need a stable snapshot.
    /// - Avoid repeated enumeration of the same deferred query.
    /// - Pick operators that match the shape of the data you need.
    /// </summary>
    public static class LINQExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} LINQ Examples {new string('=', 5)}");

            PrintSection("BASIC OPERATORS");
            DemoBasicOperators();

            PrintSection("QUERY SYNTAX VS METHOD SYNTAX");
            DemoQuerySyntax();

            PrintSection("FILTERING AND PROJECTION");
            DemoFilteringProjection();

            PrintSection("GROUPING AND AGGREGATION");
            DemoGroupingAggregation();

            PrintSection("LOOKUPS, DISTINCT VALUES, AND BATCHING");
            DemoLookupDistinctBatching();

            PrintSection("JOINING DATA");
            DemoJoining();

            PrintSection("DEFERRED VS IMMEDIATE EXECUTION");
            DemoDeferredExecution();

            PrintSection("SORTING AND PAGINATION");
            DemoSortingPagination();

            Console.WriteLine();
        }

        private static void DemoBasicOperators()
        {
            var numbers = Enumerable.Range(1, 10).ToList();

            // Where - filter
            var evens = numbers.Where(n => n % 2 == 0).ToList();
            Console.WriteLine($"Even numbers: {string.Join(", ", evens)}");

            // Select - transform
            var squared = numbers.Select(n => n * n).ToList();
            Console.WriteLine($"Squared: {string.Join(", ", squared.Take(5))}...");

            // First, Last, Single
            Console.WriteLine($"First: {numbers.First()}, Last: {numbers.Last()}");
            Console.WriteLine($"First > 5: {numbers.First(n => n > 5)}");

            // Any, All
            Console.WriteLine($"Any > 100: {numbers.Any(n => n > 100)}, All > 0: {numbers.All(n => n > 0)}");
        }

        private static void DemoQuerySyntax()
        {
            var products = CreateProducts();

            var querySyntax =
                from product in products
                where product.Category == "Electronics" && product.Price >= 100
                orderby product.Price descending
                select $"{product.Name} (${product.Price})";

            Console.WriteLine($"Query syntax result: {string.Join(", ", querySyntax)}");

            var methodSyntax = products
                .Where(product => product.Category == "Books")
                .Select(product => product.Name)
                .ToList();

            Console.WriteLine($"Method syntax result: {string.Join(", ", methodSyntax)}");
        }

        private static void DemoFilteringProjection()
        {
            var products = new[]
            {
            new Product { Id = 1, Name = "Laptop", Price = 1200, Category = "Electronics" },
            new Product { Id = 2, Name = "Mouse", Price = 25, Category = "Electronics" },
            new Product { Id = 3, Name = "Book", Price = 15, Category = "Books" },
            new Product { Id = 4, Name = "Monitor", Price = 300, Category = "Electronics" }
        };

            // Filter expensive items
            var expensive = products.Where(p => p.Price > 100).ToList();
            Console.WriteLine($"Expensive items: {string.Join(", ", expensive.Select(p => p.Name))}");

            // Project to new shape
            var productNames = products.Select(p => $"{p.Name} (${p.Price})").ToList();
            Console.WriteLine($"Products: {string.Join(", ", productNames)}");

            // SelectMany - flatten nested collections
            var allWords = new[] { "hello world", "linq examples", "dotnet core" }
                .SelectMany(s => s.Split(' '))
                .ToList();
            Console.WriteLine($"All words flattened: {string.Join(", ", allWords)}");
        }

        private static void DemoGroupingAggregation()
        {
            var products = CreateProducts();

            // Group by category
            var byCategory = products.GroupBy(p => p.Category).ToList();
            foreach (var group in byCategory)
            {
                decimal total = group.Sum(p => p.Price);
                Console.WriteLine($"{group.Key}: {group.Count()} items, Total: ${total}");
            }

            // Aggregation functions
            Console.WriteLine($"Total price: ${products.Sum(p => p.Price)}");
            Console.WriteLine($"Average price: ${products.Average(p => p.Price):F2}");
            Console.WriteLine($"Max price: ${products.Max(p => p.Price)}");
            Console.WriteLine($"Min price: ${products.Min(p => p.Price)}");
        }

        private static void DemoLookupDistinctBatching()
        {
            var products = CreateProducts();

            var categories = products.Select(product => product.Category).Distinct().OrderBy(category => category);
            Console.WriteLine($"Distinct categories: {string.Join(", ", categories)}");

            var lookup = products.ToLookup(product => product.Category, product => product.Name);
            foreach (var category in lookup)
            {
                Console.WriteLine($"{category.Key}: {string.Join(", ", category)}");
            }

            var batches = products
                .OrderBy(product => product.Id)
                .Chunk(2)
                .Select(chunk => string.Join(" | ", chunk.Select(product => product.Name)));

            Console.WriteLine($"Batches of 2: {string.Join("; ", batches)}");
        }

        private static void DemoJoining()
        {
            var users = new[]
            {
            new { Id = 1, Name = "Alice" },
            new { Id = 2, Name = "Bob" }
        };

            var orders = new[]
            {
            new { UserId = 1, OrderId = 101, Amount = 250 },
            new { UserId = 1, OrderId = 102, Amount = 150 },
            new { UserId = 2, OrderId = 103, Amount = 500 }
        };

            // Inner join
            var joined = users.Join(
                orders,
                u => u.Id,
                o => o.UserId,
                (u, o) => $"{u.Name}: Order #{o.OrderId} (${o.Amount})"
            ).ToList();

            Console.WriteLine("User Orders (Join):");
            foreach (string item in joined)
            {
                Console.WriteLine($"  {item}");
            }

            // Left outer join using GroupJoin
            var userWithOrders = users.GroupJoin(
                orders,
                u => u.Id,
                o => o.UserId,
                (u, userOrders) => new
                {
                    User = u.Name,
                    OrderCount = userOrders.Count(),
                    Total = userOrders.Sum(o => o.Amount)
                }
            ).ToList();

            Console.WriteLine("User Order Summary (GroupJoin):");
            foreach (var item in userWithOrders)
            {
                Console.WriteLine($"  {item.User}: {item.OrderCount} orders, Total: ${item.Total}");
            }
        }

        private static void DemoDeferredExecution()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5 };

            // Query is not executed yet
            var query = numbers.Where(n => n % 2 == 0);
            Console.WriteLine("Query created (not executed yet)");

            // Execution happens when enumerated
            var result = query.ToList();
            Console.WriteLine($"After ToList(): {string.Join(", ", result)}");

            // Modifying source affects deferred queries
            numbers.Add(6);
            var deferredQuery = numbers.Where(n => n % 2 == 0);
            Console.WriteLine($"Deferred query includes newly added 6: {string.Join(", ", deferredQuery)}");
        }

        private static void DemoSortingPagination()
        {
            var products = CreateProducts();

            // Single sort
            var byPrice = products.OrderBy(p => p.Price).ToList();
            Console.WriteLine($"Sorted by price: {string.Join(", ", byPrice.Select(p => $"{p.Name}(${p.Price})"))}");

            // Multiple sort criteria
            var sorted = products.OrderBy(p => p.Category).ThenByDescending(p => p.Price).ToList();
            Console.WriteLine("By category, then price descending:");
            foreach (var p in sorted)
            {
                Console.WriteLine($"  {p.Category}: {p.Name} ${p.Price}");
            }

            // Pagination: Skip and Take
            int pageSize = 2;
            int page = 0;
            var pageData = products.OrderBy(p => p.Id).Skip(page * pageSize).Take(pageSize).ToList();
            Console.WriteLine($"Page {page}: {string.Join(", ", pageData.Select(p => p.Name))}");
        }

        private static Product[] CreateProducts()
        {
            return
            [
                new Product { Id = 1, Name = "Laptop", Price = 1200, Category = "Electronics" },
            new Product { Id = 2, Name = "Mouse", Price = 25, Category = "Electronics" },
            new Product { Id = 3, Name = "Book", Price = 15, Category = "Books" },
            new Product { Id = 4, Name = "Monitor", Price = 300, Category = "Electronics" },
            new Product { Id = 5, Name = "Notebook", Price = 8, Category = "Books" }
            ];
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
