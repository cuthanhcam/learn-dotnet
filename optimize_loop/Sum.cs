using System;

namespace optimize_loop
{
    public class Sum
    {
        public decimal CalculateSum(long n, int numThreads)
        {
            decimal[] partialSums = new decimal[numThreads];
            Parallel.For(0, numThreads, p =>
            {
                long start = p * (n / numThreads);
                long end = (p == numThreads - 1) ? n : start + (n / numThreads);
                decimal subtotal = 0;
                for (decimal i = start; i < end; i++)
                {
                    subtotal += i % 10;
                }
                partialSums[p] = subtotal;
            });
            return partialSums.Sum();
        }
    }
}