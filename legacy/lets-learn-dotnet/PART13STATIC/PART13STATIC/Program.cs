namespace PART13STATIC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(AccessCounter.GetInstance().Inc());
            }
        }
    }
}
