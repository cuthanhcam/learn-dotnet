using System;
using System.Threading.Tasks;

namespace AsyncDemo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"Main - 1 - Thread ID: {Environment.CurrentManagedThreadId}");

            Task t = Task.Run(AsyncFunc1);
            await AsyncFunc2();

            Console.WriteLine($"Main - 2 - Thread ID: {Environment.CurrentManagedThreadId}");

            t.Wait();
        }

        public static int AsyncFunc1()
        {
            Console.WriteLine($"AsyncFunc1 - Thread ID: {Environment.CurrentManagedThreadId}");

            return 0;
        }

        public static async Task<int> AsyncFunc2()
        {   
            Console.WriteLine($"AsyncFunc2 - 1 - Thread ID: {Environment.CurrentManagedThreadId}");

            var r = (await File.ReadAllTextAsync("sample.txt")).Length; 

            Console.WriteLine($"AsyncFunc2 - 2 - Thread ID: {Environment.CurrentManagedThreadId}");
            
            return r;
        }
    }
}