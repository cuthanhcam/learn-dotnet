namespace Part10LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dataSource = GetIntNumbers();

            Print(dataSource);

            // Query syntax example
            //var filteredData = from n in dataSource
            //                   where GreaterThanZero(n) && n % 2 == 0
            //                   select n;

            // Method syntax example
            var filteredData = dataSource.Where(n => GreaterThanZero(n) && n % 2 == 0);

            Print(filteredData);

            Console.WriteLine($"Count: {filteredData.Count()}, Sum: {filteredData.Sum()}");
        }

        static bool GreaterThanZero(int n)
        {
            //Console.WriteLine($"{n} > 0 = {n > 0}");
            return n > 0;
        }

        static IEnumerable<int> GetIntNumbers()
        {
            var ns = new int[] { 1, 2, 32, 4324, 3242, 324234, 231, 0, -123, -123123, 0, 2321321 };
            return ns;
        }

        static void Print(IEnumerable<int> numbers)
        {
            foreach (var value in numbers)
            {
                Console.WriteLine($"{value} ");
            }
            Console.WriteLine();
        }
    }
}
