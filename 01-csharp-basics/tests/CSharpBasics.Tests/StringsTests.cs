using CSharpBasics.Examples.Strings;
using Xunit;

namespace CSharpBasics.Tests;

public class StringsTests
{
    [Fact]
    public void AreEqualIgnoreCase_WithDifferentCases_ReturnsTrue()
    {
        bool result = StringBasicsExample.AreEqualIgnoreCase("DOTNET", "dotnet");
        Assert.True(result);
    }

    [Fact]
    public void StringBasics_Methods_WorkCorrectly()
    {
        Assert.Equal("Name: Cam, Age: 30", StringBasicsExample.BuildProfileLine("Cam", 30));
        Assert.Equal(@"C:\Root\file.txt", StringBasicsExample.BuildEscapedPath(@"C:\Root", "file.txt"));
        Assert.Equal(@"C:\Root\file.txt", StringBasicsExample.BuildPathVerbatim(@"C:\Root", "file.txt"));
        Assert.Equal("Cu Thanh Cam", StringBasicsExample.NormalizeName("  cU  tHaNh cAm  "));
        Assert.Equal("CJC", StringBasicsExample.BuildInitials("Cam Jane Cu"));
        Assert.True(StringBasicsExample.LooksLikeEmail("cam@example.com"));
        Assert.False(StringBasicsExample.LooksLikeEmail("cam-example"));
        Assert.Equal("Hello...", StringBasicsExample.Truncate("Hello World", 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => StringBasicsExample.Truncate("abc", 2));
    }

    [Fact]
    public void StringBuilder_Methods_WorkCorrectly()
    {
        string[] values = ["a", "b", "c"];
        Assert.Equal("a,b,c", StringBuilderExample.BuildCsvLineNaive(values));
        Assert.Equal("a,b,c", StringBuilderExample.BuildCsvLineOptimal(values));
        Assert.Equal(@"C:\Users\cam", StringBuilderExample.BuildPath("C:", "Users", "cam"));
        Assert.Equal("<tr><td>A</td><td>B</td></tr>", StringBuilderExample.BuildTableRow("A", "B"));
        Assert.Equal("hi-hi-hi", StringBuilderExample.RepeatWord("hi", 3, "-"));
        Assert.Equal("", StringBuilderExample.RepeatWord("", 3));

        var kv = StringBuilderExample.BuildKeyValueTable(new List<(string key, string value)>
        {
            ("Name", "Cam"),
            ("Role", "Dev")
        });
        Assert.Contains("Name: Cam", kv);
        Assert.Contains("Role: Dev", kv);

        Assert.Equal("abcabc", StringBuilderExample.Repeat("abc", 2));
        Assert.Equal("", StringBuilderExample.Repeat("abc", 0));

        var debug = StringBuilderExample.BuildDebugInfo("User", new Dictionary<string, object>
        {
            ["Id"] = 1,
            ["Name"] = "Cam"
        });
        Assert.Contains("User", debug);
        Assert.Contains("Id = 1", debug);
    }

    [Fact]
    public void StringMethods_Methods_WorkCorrectly()
    {
        Assert.Equal(["hello", "world"], StringMethodsExample.SplitWords("  hello   world "));
        Assert.Equal("a-b-c", StringMethodsExample.JoinWords(["a", "", "b", "c"], "-"));
        Assert.True(StringMethodsExample.ContainsIgnoreCase("Hello World", "world"));
        Assert.Equal("hi hi", StringMethodsExample.ReplaceWord("hello HELLO", "hello", "hi"));
        Assert.Equal(2, StringMethodsExample.CountTokens("x y"));
        Assert.True(StringMethodsExample.StartsWithIgnoreCase("DotNet", "dot"));
        Assert.True(StringMethodsExample.EndsWithIgnoreCase("report.PDF", ".pdf"));
        Assert.Equal(4, StringMethodsExample.FindIndexIgnoreCase("abc DEF ghi", "def"));
        Assert.Equal(6, StringMethodsExample.FindLastIndexIgnoreCase("ab ab AB", "ab"));
        Assert.Equal("cde", StringMethodsExample.ExtractSlice("abcdef", 2, 5));
        Assert.Equal("", StringMethodsExample.ExtractSlice("abc", 5, 8));

        Assert.True(StringMethodsExample.TryExtractBetween("a[123]b", "[", "]", out string between));
        Assert.Equal("123", between);
        Assert.False(StringMethodsExample.TryExtractBetween("abc", "[", "]", out _));

        Assert.Equal("0012", StringMethodsExample.PadLeft("12", 4, '0'));
    }

    [Fact]
    public void StringPerformance_Methods_WorkCorrectly()
    {
        Assert.Equal(StringPerformanceExample.BuildWithConcatenation(10), StringPerformanceExample.BuildWithStringBuilder(10));

        var (concatMs, builderMs) = StringPerformanceExample.MeasureExecution(100);
        Assert.True(concatMs >= 0);
        Assert.True(builderMs >= 0);

        var (avgConcat, avgBuilder) = StringPerformanceExample.MeasureAverage(100, 2);
        Assert.True(avgConcat >= 0);
        Assert.True(avgBuilder >= 0);

        Assert.Equal(2.0, StringPerformanceExample.ComputeSpeedupRatio(10.0, 5.0));
        Assert.Equal(double.MaxValue, StringPerformanceExample.ComputeSpeedupRatio(10.0, 0.0));

        Assert.Throws<ArgumentOutOfRangeException>(() => StringPerformanceExample.BuildWithConcatenation(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => StringPerformanceExample.BuildWithStringBuilder(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => StringPerformanceExample.MeasureExecution(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => StringPerformanceExample.MeasureAverage(0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => StringPerformanceExample.MeasureAverage(1, 0));
    }
}
