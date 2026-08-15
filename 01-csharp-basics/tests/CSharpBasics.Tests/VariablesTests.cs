using CSharpBasics.Examples.Variables;
using System.Globalization;
using Xunit;

namespace CSharpBasics.Tests;

public class VariablesTests
{
    [Fact]
    public void GetPrimitiveValues_ReturnsValidSnapshot()
    {
        var snapshot = VariablesExamples.GetPrimitiveValues();
        Assert.NotEqual(default, snapshot.Id);
        Assert.True(snapshot.IsValid());
        Assert.Equal(13, snapshot.IntegerValue);
        Assert.Equal('C', snapshot.Initial);
    }

    [Fact]
    public void BuildDisplayName_TrimsInput()
    {
        var value = VariablesExamples.BuildDisplayName("  Cam  ", "  Cu  ");
        Assert.Equal("Cam Cu", value);
    }

    [Fact]
    public void BuildDisplayName_WithMissingNames_Throws()
    {
        Assert.Throws<ArgumentException>(() => VariablesExamples.BuildDisplayName("", "Cu"));
        Assert.Throws<ArgumentException>(() => VariablesExamples.BuildDisplayName("Cam", ""));
    }

    [Fact]
    public void InferTypeWithVar_AddsOffset()
    {
        Assert.Equal(30, VariablesExamples.InferTypeWithVar(10));
    }

    [Fact]
    public void ParseNullableInt_ValidAndInvalid()
    {
        Assert.Equal(123, VariablesExamples.ParseNullableInt("123"));
        Assert.Null(VariablesExamples.ParseNullableInt("abc"));
    }

    [Fact]
    public void Config_ValidatesAndTrims()
    {
        var config = new VariablesExamples.Config("  Development ");
        Assert.Equal("Development", config.EnvironmentName);
        Assert.Throws<ArgumentException>(() => new VariablesExamples.Config("   "));
    }

    [Fact]
    public void AddTyped_AndMultiplyTyped_WorkAsExpected()
    {
        Assert.Equal(12, DynamicVsTypedExample.AddTyped(7, 5));
        Assert.Equal(10.0, DynamicVsTypedExample.MultiplyTyped(2.5, 4.0));
    }

    [Fact]
    public void ConcatenateTyped_ValidatesInputs()
    {
        Assert.Equal("Hello C#", DynamicVsTypedExample.ConcatenateTyped("Hello", "C#"));
        Assert.Throws<ArgumentException>(() => DynamicVsTypedExample.ConcatenateTyped("", "x"));
        Assert.Throws<ArgumentException>(() => DynamicVsTypedExample.ConcatenateTyped("x", ""));
    }

    [Fact]
    public void AddDynamic_ReturnsValuesAndThrowsOnInvalidOperation()
    {
        Assert.Equal(30, DynamicVsTypedExample.AddDynamic(10, 20));
        Assert.Equal("Hello World", DynamicVsTypedExample.AddDynamic("Hello", " World"));
        Assert.Throws<InvalidOperationException>(() => DynamicVsTypedExample.AddDynamic(new object(), 10));
    }

    [Fact]
    public void TryMultiplyDynamic_HandlesSuccessAndFailure()
    {
        var ok = DynamicVsTypedExample.TryMultiplyDynamic(3, 4, out dynamic? result);
        Assert.True(ok);
        Assert.Equal(12, result);

        var fail = DynamicVsTypedExample.TryMultiplyDynamic("text", 4, out dynamic? badResult);
        Assert.False(fail);
        Assert.Null(badResult);
    }

    [Fact]
    public void DynamicPropertyAndMethodHelpers_WorkAsExpected()
    {
        Assert.True(DynamicVsTypedExample.TryAccessProperty("Name", out object? name));
        Assert.Equal("Charlie Cu", name);

        Assert.False(DynamicVsTypedExample.TryAccessProperty("Department", out _));

        Assert.False(DynamicVsTypedExample.TryInvokeDynamicMethod(out string invokeError));
        Assert.NotEmpty(invokeError);

        Assert.False(DynamicVsTypedExample.TryAccessMissingProperty());

        Assert.False(DynamicVsTypedExample.TryCallUnknownMethod(out string callError));
        Assert.NotEmpty(callError);
    }

    [Fact]
    public void TrySafeAddNumbers_ParsesAndAddsSafely()
    {
        Assert.True(DynamicVsTypedExample.TrySafeAddNumbers("25.5", 10, out decimal sum));
        Assert.Equal(35.5m, sum);

        Assert.False(DynamicVsTypedExample.TrySafeAddNumbers("abc", 10, out _));
        Assert.False(DynamicVsTypedExample.TrySafeAddNumbers(null, 10, out _));
    }

    [Fact]
    public void ClassifyRuntimeType_CoversCommonTypes()
    {
        Assert.Equal("int", DynamicVsTypedExample.ClassifyRuntimeType(1));
        Assert.Equal("string", DynamicVsTypedExample.ClassifyRuntimeType("hi"));
        Assert.Equal("bool", DynamicVsTypedExample.ClassifyRuntimeType(true));
        dynamic? nullValue = null;
        Assert.Equal("null", DynamicVsTypedExample.ClassifyRuntimeType(nullValue));
    }

    [Fact]
    public void PrimitiveSnapshot_Create_ProducesDistinctIds()
    {
        var s1 = VariablesExamples.PrimitiveSnapshot.Create(1, 2, 3m, 4.0, "a", true, 'A');
        var s2 = VariablesExamples.PrimitiveSnapshot.Create(1, 2, 3m, 4.0, "a", true, 'A');
        Assert.NotEqual(s1.Id, s2.Id);
    }

    [Fact]
    public void TryParseAmount_UsesExplicitCultureAndRejectsCurrencySymbol()
    {
        CultureInfo german = CultureInfo.GetCultureInfo("de-DE");

        Assert.True(NumericConversionExample.TryParseAmount("1.234,56", german, out decimal amount));
        Assert.Equal(1234.56m, amount);
        Assert.False(NumericConversionExample.TryParseAmount("€1.234,56", german, out _));
    }

    [Fact]
    public void ToInt32Checked_RejectsNarrowingOverflow()
    {
        Assert.Equal(42, NumericConversionExample.ToInt32Checked(42));
        Assert.Throws<OverflowException>(() =>
            NumericConversionExample.ToInt32Checked((long)int.MaxValue + 1));
    }

    [Theory]
    [InlineData((long)int.MinValue, true)]
    [InlineData((long)int.MaxValue, true)]
    [InlineData((long)int.MinValue - 1, false)]
    [InlineData((long)int.MaxValue + 1, false)]
    public void TryToInt32_HandlesBoundaries(long value, bool expectedSuccess)
    {
        bool success = NumericConversionExample.TryToInt32(value, out int result);

        Assert.Equal(expectedSuccess, success);
        if (success)
        {
            Assert.Equal((int)value, result);
        }
    }
}
