using BenchmarkDotNet.Attributes;
using OopBasics.Examples.Inheritance;

namespace OopBasics.Benchmarks;

[MemoryDiagnoser]
public class InheritanceBenchmarks
{
    private List<Person>? _people;

    [GlobalSetup]
    public void Setup()
    {
        _people = new List<Person>();
        for (int i = 0; i < 10000; i++)
            _people.Add(new Employee($"Name{i}", i % 100, "Role"));
    }

    [Benchmark]
    public int SumAges_Person()
    {
        int sum = 0;
        foreach (var p in _people!)
            sum += p.Age;
        return sum;
    }

    [Benchmark]
    public int SumAges_EmployeeCast()
    {
        int sum = 0;
        foreach (var p in _people!)
            sum += ((Employee)p).Age;
        return sum;
    }
}
