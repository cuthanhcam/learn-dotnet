using BenchmarkDotNet.Attributes;
using OopBasics.Examples.Classes;

namespace OopBasics.Benchmarks;

[MemoryDiagnoser]
public class ObjectCreationBenchmarks
{
    [Benchmark]
    public List<Person> CreateManyPersons()
    {
        var list = new List<Person>();
        for (int i = 0; i < 10000; i++)
            list.Add(new Person($"Name{i}", i % 100));
        return list;
    }

    [Benchmark]
    public List<User> CreateManyUsers()
    {
        var list = new List<User>();
        for (int i = 0; i < 10000; i++)
            list.Add(new User($"Name{i}", i % 100));
        return list;
    }
}
