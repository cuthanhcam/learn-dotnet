using CSharpBasics.Exercises.Easy;
using CSharpBasics.Exercises.Hard;
using CSharpBasics.Exercises.Medium;

Console.WriteLine("CSharpBasics.Exercises");
Console.WriteLine(new string('=', 40));

Console.WriteLine($"SumNumbers(1, 2, 3, 4) = {SumNumbers.CalculateSum(1, 2, 3, 4)}");
Console.WriteLine($"MaxOfThree(3, 10, 7) = {MaxOfThree.GetMax(3, 10, 7)}");
Console.WriteLine($"MethodBasics(Alex) = {MethodBasics.Greet("Alex")}");
Console.WriteLine($"EvenOdd(42) = {EvenOdd.IsEven(42)}");
Console.WriteLine($"TemperatureConverter(25) = {TemperatureConverter.CelsiusToFahrenheit(25):F1}");
Console.WriteLine($"SimpleLoop(2, 5) = [{string.Join(", ", SimpleLoop.GenerateRange(2, 5))}]");
Console.WriteLine($"VariableTypes(123) = {VariableTypes.GetTypeName(123)}");
Console.WriteLine($"ReverseString(hello) = {ReverseString.Reverse("hello")}");
Console.WriteLine($"Palindrome(radar) = {Palindrome.IsPalindrome("radar")}");
Console.WriteLine($"NullDisplay(null) = {NullDisplay.GetDisplayName(null)}");
Console.WriteLine($"FibonacciSequence(7) = [{string.Join(", ", FibonacciSequence.Generate(7))}]");
Console.WriteLine($"PrimeNumbers(10) = [{string.Join(", ", PrimeNumbers.GetPrimesUpTo(10))}]");
Console.WriteLine($"BasicCalculator(10 / 2) = {BasicCalculator.Evaluate(10, 2, '/')}");
Console.WriteLine($"RemoveDuplicates([1,1,2,3]) = [{string.Join(", ", RemoveDuplicates.GetDistinctValues(new[] { 1, 1, 2, 3 }))}]");

var counts = CountWords.Count("C# is fun, c# is fast");
Console.WriteLine($"CountWords = {string.Join(", ", counts.Select(pair => $"{pair.Key}:{pair.Value}"))}");

var groups = new Dictionary<string, List<int>>
{
    ["A"] = new List<int> { 3, 1 },
    ["B"] = new List<int> { 2, 3 },
    ["C"] = new List<int> { 9 }
};

Console.WriteLine($"NestedCollections = [{string.Join(", ", NestedCollections.FlattenDistinctSorted(groups))}]");
Console.WriteLine($"StudentReport = {StudentReport.BuildReport(new Dictionary<string, int?> { ["Ann"] = 10, ["Ben"] = null })}");
Console.WriteLine($"MemoryBucket = {MemoryBucket.Analyze()}");
