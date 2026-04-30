using OopBasics.Examples.Classes;
using Xunit;

namespace OopBasics.Tests.Classes;

public class PropertiesExampleTests
{
    [Fact]
    public void Car_PropertySettersAndValidation()
    {
        var car = new Car { Make = "Toyota", Model = "Corolla", Year = 2022 };
        Assert.Equal("Toyota", car.Make);
        Assert.Equal("Corolla", car.Model);
        Assert.Equal(2022, car.Year);
        car.Year = 2024;
        Assert.Equal(2024, car.Year);
        Assert.Throws<ArgumentException>(() => car.Year = 1500);
    }
}
