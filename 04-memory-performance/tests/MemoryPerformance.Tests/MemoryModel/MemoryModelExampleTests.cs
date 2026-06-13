using MemoryPerformance.Examples.MemoryModel;

namespace MemoryPerformance.Tests.MemoryModel;

public class MemoryModelExampleTests
{
    [Fact]
    public void ValueTypeCopyExample_Keeps_Original_Independent()
    {
        string result = MemoryModelExample.ValueTypeCopyExample();

        Assert.Contains("original=Point { X = 2, Y = 4 }", result);
        Assert.Contains("copy=Point { X = 99, Y = 4 }", result);
    }

    [Fact]
    public void ReferenceAliasExample_Mutates_Same_Object()
    {
        string result = MemoryModelExample.ReferenceAliasExample();

        Assert.Contains("first=Updated:15", result);
        Assert.Contains("same=True", result);
    }

    [Fact]
    public void DistanceFromOrigin_Uses_Readonly_Input()
    {
        double distance = MemoryModelExample.DistanceFromOrigin(new Point(3, 4));

        Assert.Equal(5, distance);
    }
}
