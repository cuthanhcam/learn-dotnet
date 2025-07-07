namespace Part10LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dataSource = GetIntNumbers();

            Print(dataSource);

            Console.WriteLine("\n\nFiltered numbers greater than 1000:\n");

            var query = from n in dataSource
                        where n > 1000
                        select n;
            Print(query);
        }

        static IEnumerable<int> GetIntNumbers()
        {
            var ns = new int[] { 1, 2, 32, 4324, 3242, 324234, 231, -123, -123123, 2321321 };
            return ns;
        }

        static void Print(IEnumerable<int> numbers)
        {
            foreach (var value in numbers)
            {
                Console.Write($"{value} ");
            }
        }
    }
}
