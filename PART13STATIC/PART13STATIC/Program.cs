namespace PART13STATIC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var c1 = new C() { x = 111 }; 

            var c2 = new C() { x = 222 };

            Console.WriteLine(c1.x);

            Console.WriteLine(c2.x);

            c2.x = 333;

            Console.WriteLine(c1.x);

            Console.WriteLine(c2.x);
        }
    }
}
