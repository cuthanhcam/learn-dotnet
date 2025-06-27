namespace Part9Lambda
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Func<int, int, string> sum = (int a, int b) => (a + b).ToString();
            //Action<string> printUpper = s => Console.WriteLine(s.ToUpper());

            var sum = (int a, int b = 10) => (a + b).ToString();
            var printUpper = (string s) => Console.WriteLine(s.ToUpper());

            var t = object (int a, int b) => a > b ? 0 : "A";

            Console.WriteLine(t(1, 2));
            Console.WriteLine(t(2, 1));

            Console.WriteLine(sum(3, 5));
            Console.WriteLine(sum(3));

            printUpper("hello world");

            int A = 100;
            int B = 200;

            Call((a, b) => a + b, A, B);
            Call((a, b) => a * b, A, B);

            int[] arr = [111, 2424, 5435, 231, 2131, 12321, 213213];

            Print((x) => x > 300, arr);
            Console.WriteLine();
            Print((x) => x <= 300, arr);
        }

        static void Call(Func<int, int, int> f, int a, int b)
        {
            Console.WriteLine(f(a, b));
        }

        static void Print(Func<int, bool> f, int[] arr)
        {
            foreach (var item in arr)
            {
                if (f(item))
                {
                    Console.Write($"{item} ");
                }
            }   
        }
    }
}
