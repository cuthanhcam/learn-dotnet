using OopBasics.Examples.Inheritance;
using Xunit;

namespace OopBasics.Tests.Inheritance;

public class BaseConstructorExampleTests
{
    [Fact]
    public void Employee_Constructor_InitializesBaseAndDerived()
    {
        var employee = new Employee("Alice", 30, "Software Engineer");
        Assert.Equal("Alice", employee.Name);
        Assert.Equal(30, employee.Age);
        Assert.Equal("Software Engineer", employee.Role);
    }

    [Fact]
    public void Person_Constructor_ThrowsOnInvalidInput()
    {
        Assert.Throws<ArgumentException>(() => new Person("", 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Person("Bob", -1));
    }
}
