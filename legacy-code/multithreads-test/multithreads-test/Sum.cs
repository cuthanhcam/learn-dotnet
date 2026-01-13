using System;

namespace optimize_loop
{
    public class Sum
    {
        public decimal CalculateSum(decimal n, int numThreads)
        {
            decimal[] partialSums = new decimal[numThreads];
            Parallel.For(0, numThreads, p =>
            {
                decimal start = p * (n / numThreads);
                decimal end = (p == numThreads - 1) ? n : start + (n / numThreads);
                decimal subtotal = 0;
                for (decimal i = start; i < end; i++)
                {
                    subtotal += i;
                }
                partialSums[p] = subtotal;
            });
            return partialSums.Sum();
        }
    }
}