using System.Diagnostics.CodeAnalysis;

namespace CoreDotNet.Examples.Generics
{
    /// <summary>
    /// Comprehensive examples for generic programming in C#.
    ///
    /// This lesson focuses on how generics improve API clarity and reuse:
    /// - Generic classes that work across multiple domain types.
    /// - Generic methods with constraints and type inference.
    /// - Variance to make APIs more flexible without losing safety.
    /// - Reusable repository and factory patterns that mirror production code.
    ///
    /// Best practices:
    /// - Use appropriate constraints to enable operations.
    /// - Prefer compile-time checks over runtime type inspection.
    /// - Keep generic APIs narrow and explicit.
    /// - Use variance where the direction of assignment matters.
    /// </summary>
    public static class GenericsExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} Generics Examples {new string('=', 5)}");

            PrintSection("GENERIC CLASS BASICS");
            DemoGenericClass();

            PrintSection("GENERIC METHODS");
            DemoGenericMethods();

            PrintSection("GENERIC CONSTRAINTS");
            DemoGenericConstraints();

            PrintSection("VARIANCE IN PRACTICE");
            DemoVariance();

            PrintSection("PRACTICAL PATTERNS");
            DemoPracticalPatterns();

            Console.WriteLine();
        }

        private static void DemoGenericClass()
        {
            // Generic container for any type
            var stringBox = new Box<string> { Value = "Hello" };
            var intBox = new Box<int> { Value = 42 };
            var listBox = new Box<List<string>> { Value = new List<string> { "a", "b" } };

            Console.WriteLine($"String box: {stringBox.Value}");
            Console.WriteLine($"Int box: {intBox.Value}");
            Console.WriteLine($"List box count: {listBox.Value?.Count ?? 0}");

            // Generic stack implementation
            var stack = new GenericStack<int>();
            stack.Push(10);
            stack.Push(20);
            stack.Push(30);
            Console.WriteLine($"Stack peek: {stack.Peek()}, Pop: {stack.Pop()}");
        }

        private static void DemoGenericMethods()
        {
            // Type inference in generic methods
            int result = FindMax(new[] { 1, 5, 3, 9, 2 });
            Console.WriteLine($"Max of [1, 5, 3, 9, 2]: {result}");

            // Works with any comparable type
            string[] words = { "apple", "zebra", "banana" };
            Console.WriteLine($"Last word alphabetically: {FindMax(words)}");

            // Generic swap
            int a = 10, b = 20;
            Swap(ref a, ref b);
            Console.WriteLine($"After swap: a={a}, b={b}");

            if (TryGetLast(words, out string? lastWord))
            {
                Console.WriteLine($"Last word in array: {lastWord}");
            }
        }

        private static void DemoGenericConstraints()
        {
            // Constraint examples
            var intComparer = new Comparer<int>();
            Console.WriteLine($"10 > 5: {intComparer.IsGreater(10, 5)}");

            var person1 = new Person { Name = "Alice", Age = 30 };
            var person2 = new Person { Name = "Bob", Age = 25 };
            var personComparer = new Comparer<Person>();
            Console.WriteLine($"Alice > Bob by age: {personComparer.IsGreater(person1, person2)}");

            // New() constraint - requires parameterless constructor
            var factory = new GenericFactory<Config>();
            var config = factory.Create();
            Console.WriteLine($"Created config: {config.GetType().Name}");

            var page = new PagedResult<Person>
            {
                Items = [person1, person2],
                PageNumber = 1,
                PageSize = 10,
                TotalItems = 2
            };

            Console.WriteLine($"Paged result: page {page.PageNumber} with {page.Items.Count} user(s)");
        }

        private static void DemoVariance()
        {
            IEnumerable<string> courseNames = ["C# Basics", "LINQ", "File I/O"];
            IEnumerable<object> boxedCourses = courseNames;
            Console.WriteLine($"Covariance via IEnumerable<object>: {string.Join(", ", boxedCourses)}");

            Func<string> createTitle = () => "Core .NET";
            Func<object> createObject = createTitle;
            Console.WriteLine($"Covariant Func<object> result: {createObject()}");

            Action<object> logAnyObject = value => Console.WriteLine($"Contravariant Action<object>: {value}");
            Action<string> logText = logAnyObject;
            logText("Variance lets the same logger handle strings safely");
        }

        private static void DemoPracticalPatterns()
        {
            // Repository pattern with generics
            var userRepo = new Repository<User>();
            userRepo.Add(new User { Id = 1, Name = "Alice" });
            userRepo.Add(new User { Id = 2, Name = "Bob" });

            var users = userRepo.GetAll();
            foreach (var user in users)
            {
                Console.WriteLine($"User: {user.Name}");
            }

            // Get single item
            if (userRepo.TryGetById(1, out User? found))
            {
                Console.WriteLine($"Found user: {found.Name}");
            }

            Console.WriteLine($"Repository exposes read-only list count: {userRepo.GetAll().Count}");
        }

        private static T FindMax<T>(T[] items) where T : IComparable<T>
        {
            ArgumentNullException.ThrowIfNull(items);
            if (items.Length == 0) throw new ArgumentException("Array is empty");

            var max = items[0];
            for (int i = 1; i < items.Length; i++)
            {
                if (items[i].CompareTo(max) > 0)
                    max = items[i];
            }
            return max;
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        private static bool TryGetLast<T>(IReadOnlyList<T> items, out T? value)
        {
            ArgumentNullException.ThrowIfNull(items);

            if (items.Count == 0)
            {
                value = default;
                return false;
            }

            value = items[items.Count - 1];
            return true;
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }

    // Generic helpers
    public class Box<T>
    {
        public T? Value { get; set; }
    }

    public class GenericStack<T>
    {
        private readonly List<T> _items = new();

        public void Push(T item) => _items.Add(item);
        public T Pop()
        {
            if (_items.Count == 0) throw new InvalidOperationException("Stack is empty");
            var item = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            return item;
        }
        public T Peek()
        {
            if (_items.Count == 0) throw new InvalidOperationException("Stack is empty");
            return _items[_items.Count - 1];
        }
    }

    public class Comparer<T> where T : IComparable<T>
    {
        public bool IsGreater(T a, T b) => a.CompareTo(b) > 0;
    }

    public class GenericFactory<T> where T : new()
    {
        public T Create() => new T();
    }

    public class Repository<T> where T : Entity
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);
        public IReadOnlyList<T> GetAll() => _items.AsReadOnly();
        public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);
        public bool TryGetById(int id, [NotNullWhen(true)] out T? item)
        {
            item = GetById(id);
            return item is not null;
        }
    }

    public abstract class Entity
    {
        public int Id { get; set; }
    }

    public class Config
    {
        public string Setting { get; set; } = "default";
    }

    public class User : Entity
    {
        public string Name { get; set; } = string.Empty;
    }

    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalItems { get; init; }
    }

    public class Person : IComparable<Person>
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }

        public int CompareTo(Person? other)
        {
            return other == null ? 1 : Age.CompareTo(other.Age);
        }
    }
}
