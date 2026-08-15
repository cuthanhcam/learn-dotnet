using OopBasics.Exercises;

namespace OopBasics.Tests.Exercises;

public sealed class OopExercisesTests
{
    [Fact]
    public void Person_CelebrateBirthday_PreservesNameAndIncrementsAge()
    {
        var person = new ClassesExercises.Person("Ada", 27);

        person.CelebrateBirthday();

        Assert.Equal("Ada", person.Name);
        Assert.Equal(28, person.Age);
    }

    [Fact]
    public void BankAccount_WithdrawMoreThanBalance_RejectsInvalidTransition()
    {
        var account = new EncapsulationExercises.BankAccount("Ada", 100m);

        InvalidOperationException error = Assert.Throws<InvalidOperationException>(
            () => account.Withdraw(101m));

        Assert.Equal("Insufficient funds.", error.Message);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void AnimalSpeak_DispatchesThroughBaseType()
    {
        InheritanceExercises.Animal animal = new InheritanceExercises.Dog("Rex", 4);

        string sound = animal.Speak();

        Assert.Equal("Rex barks.", sound);
    }

    [Fact]
    public void SumAreas_UsesEachShapePolymorphically()
    {
        PolymorphismExercises.Shape[] shapes =
        [
            new PolymorphismExercises.Circle(2),
            new PolymorphismExercises.Rectangle(3, 4)
        ];

        double area = PolymorphismExercises.SumAreas(shapes);

        Assert.Equal((Math.PI * 4) + 12, area, precision: 10);
    }

    [Fact]
    public void SumAreas_NullSequence_ExplainsTheInvalidArgument()
    {
        ArgumentNullException error = Assert.Throws<ArgumentNullException>(
            () => PolymorphismExercises.SumAreas(null!));

        Assert.Equal("shapes", error.ParamName);
    }
}
