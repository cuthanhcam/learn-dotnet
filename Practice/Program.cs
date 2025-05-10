using System;
using System.Dynamic;

namespace Practice
{
    interface ITest
    {
        int A { get; }
        public void displayText(string str);
    }

    public class Test : ITest
    {
        public int A => throw new NotImplementedException();

        public void displayText(string str)
        {
            Console.WriteLine(str);
        }
    }
    internal class Program
    {
        public static void Main(string[] args)
        {
            ITest test = new Test();
            test.displayText("Hello World!");
        }
    }
}