using OopBasics.Examples.Classes;
using Xunit;

namespace OopBasics.Tests.Classes;

public class ClassBasicsExampleTests
{
    [Fact]
    public void Person_CelebrateBirthday_IncrementsAge()
    {
        var person = new Person("Charlie", 22);
        person.CelebrateBirthday();
        Assert.Equal(23, person.Age);
    }

    [Fact]
    public void Person_ChangeName_UpdatesName()
    {
        var person = new Person("Charlie", 22);
        person.ChangeName("Charles");
        Assert.Equal("Charles", person.Name);
    }
}
