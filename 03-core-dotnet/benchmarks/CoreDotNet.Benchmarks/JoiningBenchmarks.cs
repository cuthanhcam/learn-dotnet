using BenchmarkDotNet.Attributes;

namespace CoreDotNet.Benchmarks
{
    [MemoryDiagnoser]
    public class JoiningBenchmarks : LinqBenchmarkBase
    {
        [Benchmark]
        public int JoinUsersAndOrders()
        {
            return Data.Users
                .Join(
                    Data.Orders,
                    user => user.Id,
                    order => order.UserId,
                    (_, order) => order.OrderId)
                .Count();
        }

        [Benchmark]
        public decimal GroupJoinUsersAndOrders()
        {
            return Data.Users
                .GroupJoin(
                    Data.Orders,
                    user => user.Id,
                    order => order.UserId,
                    (_, orders) => orders.Sum(order => order.Amount))
                .Sum();
        }
    }
}
