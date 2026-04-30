using BenchmarkDotNet.Attributes;
using OopBasics.Examples.Polymorphism;

namespace OopBasics.Benchmarks;

[MemoryDiagnoser]
public class PolymorphismBenchmarks
{
    private Shape[]? _shapes;

    [GlobalSetup]
    public void Setup()
    {
        _shapes = new Shape[10000];
        for (int i = 0; i < _shapes.Length; i++)
            _shapes[i] = (i % 2 == 0) ? new Circle(5) : new Rectangle(4, 6);
    }

    [Benchmark]
    public double SumAreas_VirtualDispatch()
    {
        double sum = 0;
        foreach (var s in _shapes!)
            sum += s.GetArea();
        return sum;
    }
}
