using CSharpBasics.Examples.ControlFlow;

namespace CSharpBasics.Tests;

public class ControlFlowTests
{
    [Fact]
    public void IfElse_ClassifyScore_HandlesBoundaries()
    {
        Assert.Equal("Invalid", IfElseExample.ClassifyScore(-1));
        Assert.Equal("Fail", IfElseExample.ClassifyScore(49));
        Assert.Equal("Pass", IfElseExample.ClassifyScore(50));
        Assert.Equal("Good", IfElseExample.ClassifyScore(65));
        Assert.Equal("Excellent", IfElseExample.ClassifyScore(85));
        Assert.Equal("Invalid", IfElseExample.ClassifyScore(101));
    }

    [Fact]
    public void IfElse_BooleanRules_WorkCorrectly()
    {
        Assert.False(IfElseExample.CanVote(17));
        Assert.True(IfElseExample.CanVote(18));

        Assert.True(IfElseExample.IsDiscountEligible(3, 100m));
        Assert.True(IfElseExample.IsDiscountEligible(1, 500m));
        Assert.False(IfElseExample.IsDiscountEligible(1, 100m));

        Assert.Throws<ArgumentOutOfRangeException>(() => IfElseExample.IsDiscountEligible(-1, 100m));
        Assert.Throws<ArgumentOutOfRangeException>(() => IfElseExample.IsDiscountEligible(1, -1m));

        Assert.True(IfElseExample.CanAccessResource(true, true, true));
        Assert.False(IfElseExample.CanAccessResource(false, true, true));
        Assert.False(IfElseExample.CanAccessResource(true, false, true));
        Assert.False(IfElseExample.CanAccessResource(true, true, false));
    }

    [Fact]
    public void IfElse_DiscountPercentage_HandlesCases()
    {
        Assert.Equal(0.20m, IfElseExample.GetDiscountPercentage("vip", 5));
        Assert.Equal(0.15m, IfElseExample.GetDiscountPercentage("vip", 2));
        Assert.Equal(0.10m, IfElseExample.GetDiscountPercentage("premium", 3));
        Assert.Equal(0.05m, IfElseExample.GetDiscountPercentage("premium", 1));
        Assert.Equal(0.02m, IfElseExample.GetDiscountPercentage("standard", 2));
        Assert.Equal(0.00m, IfElseExample.GetDiscountPercentage("standard", 0));

        Assert.Throws<ArgumentException>(() => IfElseExample.GetDiscountPercentage("", 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => IfElseExample.GetDiscountPercentage("vip", -1));
    }

    [Fact]
    public void SwitchExample_Methods_WorkCorrectly()
    {
        Assert.Equal("Weekend", SwitchExample.GetDayType(DayOfWeek.Sunday));
        Assert.Equal("Weekday", SwitchExample.GetDayType(DayOfWeek.Monday));

        Assert.Equal("Outstanding achievement", SwitchExample.DescribeGrade('a'));
        Assert.Equal("Invalid grade", SwitchExample.DescribeGrade('z'));

        Assert.Equal("Very negative", SwitchExample.ClassifyNumber(-101));
        Assert.Equal("Negative", SwitchExample.ClassifyNumber(-10));
        Assert.Equal("Zero", SwitchExample.ClassifyNumber(0));
        Assert.Equal("Small positive", SwitchExample.ClassifyNumber(10));
        Assert.Equal("Large positive", SwitchExample.ClassifyNumber(101));

        Assert.Equal(1, SwitchExample.QuarterFromMonth(1));
        Assert.Equal(4, SwitchExample.QuarterFromMonth(12));
        Assert.Equal(0, SwitchExample.QuarterFromMonth(13));

        Assert.Equal("Winter", SwitchExample.SeasonFromMonth(12));
        Assert.Equal("Summer", SwitchExample.SeasonFromMonth(7));
        Assert.Equal("Invalid month", SwitchExample.SeasonFromMonth(13));

        Assert.Equal(0m, SwitchExample.GetTicketPrice(3, "standard"));
        Assert.Equal(5m, SwitchExample.GetTicketPrice(7, "premium"));
        Assert.Equal(10m, SwitchExample.GetTicketPrice(30, "standard"));
        Assert.Equal(12m, SwitchExample.GetTicketPrice(65, "premium"));
        Assert.Throws<ArgumentException>(() => SwitchExample.GetTicketPrice(20, ""));

        Assert.Equal("Invalid age", SwitchExample.ClassifyPerson(-1));
        Assert.Equal("Newborn", SwitchExample.ClassifyPerson(0));
        Assert.Equal("Child", SwitchExample.ClassifyPerson(12));
        Assert.Equal("Teenager", SwitchExample.ClassifyPerson(17));
        Assert.Equal("Adult", SwitchExample.ClassifyPerson(20));
        Assert.Equal("Senior", SwitchExample.ClassifyPerson(70));

        Assert.True(SwitchExample.TryValidateUserStatus(25, true, out var status));
        Assert.Equal("Active adult", status);
        Assert.False(SwitchExample.TryValidateUserStatus(-1, true, out var invalid));
        Assert.Equal("Invalid age", invalid);
    }

    [Fact]
    public void LoopsExample_Methods_WorkCorrectly()
    {
        Assert.Equal([0, 1, 4, 9], LoopsExample.GenerateSquares(4));
        Assert.Throws<ArgumentOutOfRangeException>(() => LoopsExample.GenerateSquares(-1));

        Assert.Equal(6, LoopsExample.SumWithWhile([1, 2, 3]));
        Assert.Throws<ArgumentNullException>(() => LoopsExample.SumWithWhile(null!));

        Assert.Equal(1, LoopsExample.CountDigitsWithDoWhile(0));
        Assert.Equal(3, LoopsExample.CountDigitsWithDoWhile(123));
        Assert.Equal(2, LoopsExample.CountDigitsWithDoWhile(-45));

        Assert.Equal(2, LoopsExample.CountPositiveWithForeach([-1, 0, 2, 3]));
        Assert.Equal("a,b", LoopsExample.JoinWithSeparator([" a ", "", "b"], ","));

        Assert.Equal(2, LoopsExample.FindFirstEven([1, 2, 3, 4]));
        Assert.Null(LoopsExample.FindFirstEven([1, 3, 5]));
        Assert.Equal(3, LoopsExample.CountNonNegative([-1, 0, 2, 3]));

        var table = LoopsExample.GenerateMultiplicationTable(3);
        Assert.Equal(1, table[0, 0]);
        Assert.Equal(9, table[2, 2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => LoopsExample.GenerateMultiplicationTable(0));
    }
}
