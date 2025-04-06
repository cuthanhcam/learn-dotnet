using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace optimize_loop
{
    class Program
    {
        static void Main()
        {
            long n = 10_000_000_000;
            int numThreads = 12;
        
            Stopwatch sw = Stopwatch.StartNew();

            Sum sum = new Sum();
            decimal total = sum.CalculateSum(n, numThreads);
            sw.Stop();

            Console.WriteLine($"Tổng: {total}");
            Console.WriteLine($"Thời gian: {sw.Elapsed.TotalSeconds} giây");
        }
    }
}