namespace Part10LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dataSource = GetIntNumbers();

            Print(dataSource);

            var filteredData = from n in dataSource
                               where n <= 0
                               select n;

            Print(filteredData);
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
                Console.Write($"{value} ");
            }
            Console.WriteLine();
        }
    }
}
