using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace optimize_loop
{
    class Program
    {
        static void Main()
        {
            decimal n = 10_000_000_000_000;
            int numThreads = 12;

            Stopwatch sw = Stopwatch.StartNew();

            Sum sum = new Sum();
            decimal total = sum.CalculateSum(n, numThreads);
            sw.Stop();

            Console.WriteLine($"Total: {total}");
            Console.WriteLine($"Time: {sw.Elapsed.TotalSeconds} seconds");
        }
    }
}