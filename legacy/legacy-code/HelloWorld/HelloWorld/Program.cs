namespace HelloWorld
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int? x;
            x = 5;
            Console.WriteLine(x.HasValue ? x.Value.ToString() : "x is null");
        }
    }
}