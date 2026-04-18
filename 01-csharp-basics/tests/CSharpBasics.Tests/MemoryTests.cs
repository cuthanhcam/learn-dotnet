using CSharpBasics.Examples.Memory;

namespace CSharpBasics.Tests;

public class MemoryTests
{
    [Fact]
    public void StackAllocationExample_ReturnsExpectedValue()
    {
        Assert.Equal(42, MemoryConceptsExample.StackAllocationExample());
    }

    [Fact]
    public void HeapAllocationExample_ReturnsPersonObject()
    {
        var person = MemoryConceptsExample.HeapAllocationExample();
        Assert.Equal("Alice", person.Name);
        Assert.Equal(30, person.Age);
        Assert.Equal("Alice (30)", person.ToString());
    }

    [Fact]
    public void MeasureAllocations_ReturnsNonNegativeValue()
    {
        long allocated = MemoryConceptsExample.MeasureAllocations(() =>
        {
            var list = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                list.Add($"item-{i}");
            }
        });

        Assert.True(allocated >= 0);
    }

    [Fact]
    public void SupportingTypes_WorkCorrectly()
    {
        var p = new MemoryConceptsExample.Point { X = 5, Y = 7 };
        Assert.Equal("(5, 7)", p.ToString());

        using var resource = new MemoryConceptsExample.ManagedResource("test");
        resource.Dispose();
        resource.Dispose();
        Assert.Equal("test", resource.Name);
    }
}
