using CoreDotNet.Examples.LINQ;

namespace CoreDotNet.Benchmarks
{
    public sealed class LinqBenchmarkData
    {
        public required int[] Numbers { get; init; }

        public required Product[] Products { get; init; }

        public required string[] Sentences { get; init; }

        public required List<int> DeferredNumbers { get; init; }

        public required User[] Users { get; init; }

        public required Order[] Orders { get; init; }

        public static LinqBenchmarkData Create()
        {
            var products = CreateProducts();
            var users = Enumerable.Range(1, 250)
                .Select(id => new User(
                    Id: id,
                    Name: $"User {id}",
                    Department: id % 2 == 0 ? "Sales" : "Engineering"))
                .ToArray();

            return new LinqBenchmarkData
            {
                Numbers = Enumerable.Range(1, 10_000).ToArray(),
                Products = products,
                Sentences = Enumerable.Range(0, 2_000)
                    .Select(index => index % 3 == 0 ? "hello world" : "linq examples dotnet core")
                    .ToArray(),
                DeferredNumbers = Enumerable.Range(1, 10_000).ToList(),
                Users = users,
                Orders = users
                    .SelectMany(user => Enumerable.Range(1, 8)
                        .Select(orderIndex => new Order(
                            UserId: user.Id,
                            OrderId: user.Id * 100 + orderIndex,
                            Amount: 50m + (orderIndex * 10m) + (user.Id % 25))))
                    .ToArray()
            };
        }

        private static Product[] CreateProducts()
        {
            Product[] template =
            [
                new Product { Id = 1, Name = "Laptop", Price = 1200m, Category = "Electronics" },
            new Product { Id = 2, Name = "Mouse", Price = 25m, Category = "Electronics" },
            new Product { Id = 3, Name = "Book", Price = 15m, Category = "Books" },
            new Product { Id = 4, Name = "Monitor", Price = 300m, Category = "Electronics" },
            new Product { Id = 5, Name = "Notebook", Price = 8m, Category = "Books" }
            ];

            return Enumerable.Range(0, 1_000)
                .SelectMany(batch => template.Select((product, index) => new Product
                {
                    Id = batch * template.Length + index + 1,
                    Name = $"{product.Name} {batch}",
                    Price = product.Price + (batch % 17),
                    Category = product.Category
                }))
                .ToArray();
        }
    }

    public sealed record User(int Id, string Name, string Department);

    public sealed record Order(int UserId, int OrderId, decimal Amount);
}
