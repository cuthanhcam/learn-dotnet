namespace PART13STATIC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var c1 = new C() { }; 

            var c2 = new C() { };

            Console.WriteLine(C.x);

            C.x = 333;

            Console.WriteLine(C.x);

            F1();

            Console.WriteLine(C.x);
        }
    
        static void F1()
        {
            C.x = 123;
        }
    }
}
