namespace Part5Method
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int x = 10;
            int y = 20;
            DemoParameters(x, ref y);
            Console.WriteLine($"Main.x: {x}, Main.y: {y}");

            var mc = new MyClass() { M = 100 };
            Print(mc);
            UpdateMyClass(mc);
            Print(mc);
        }

        static void Print(MyClass mc)
        {
            Console.WriteLine($"MyClass.M: {mc.M}");
        }

        static void UpdateMyClass(MyClass mc)
        {
            mc = new MyClass() { M = 200 };
            //mc.M = 300; // This will modify the original object, not the reference
            Print(mc);
        }

        public static int DemoParameters(int x, ref int y)
        {
            Console.WriteLine($"x: {x}, y: {y}");

            x = 100;
            y = 200;

            Console.WriteLine($"x: {x}, y: {y}");

            return x + y;
        }

        public static int Add(int a, int b)
        {
            Console.WriteLine($"{a}");
            Console.WriteLine($"{b}");
            return a + b;
        }

        public static int Subtract(int a, int b)
        {
            return a - b;
        }

        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        public static int Divide(int a, int b)
        {
            if (b == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }
            return a / b;
        }

        private static void DisplayResult(int result)
        {
            Console.WriteLine($"The result is: {result}");
        }
    }
}
