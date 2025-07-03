namespace Part10LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] scores = { 3, 1, 4, 1, 5, 9 };

            IEnumerable<int> scoreQuery = from score in scores
                                          where score % 2 == 1
                                          orderby score ascending
                                          select score;
            foreach (var item in scoreQuery)
            {
                Console.WriteLine(item);
            }
        }
    }
}
