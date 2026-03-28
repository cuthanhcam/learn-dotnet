# Strings

Strings are immutable sequences of characters.

## String Literals

Regular strings:
```csharp
string text = "Hello World";
```

Verbatim strings (backslashes not escaped):
```csharp
string path = @"C:\Users\Name\Documents";  // Actual backslashes
string json = @"{""key"": ""value""}";     // Quotes inside
```

Raw strings (C# 11+, prefer for multiline):
```csharp
string json = """
{
    "name": "Alice",
    "age": 30
}
""";

string pattern = """
    Line 1
    Line 2
    Line 3
""";
```

## String Interpolation

Modern way to format strings:

```csharp
string name = "Alice";
int age = 30;

// Interpolation
string message = $"Name: {name}, Age: {age}";

// With formatting
string price = $"Price: {100.5:C}";           // Currency
string padded = $"Value: {value,10}";         // Right-aligned in 10 spaces
string precision = $"Pi: {3.14159:F2}";       // 2 decimal places

// Expressions
string calculation = $"Sum: {10 + 20}";
string conditional = $"Status: {(age >= 18 ? "Adult" : "Minor")}";
string method = $"Length: {name.Length}";
```

## String Methods

Case conversion:
```csharp
string upper = "hello".ToUpper();      // "HELLO"
string lower = "HELLO".ToLower();      // "hello"
```

Searching:
```csharp
string text = "Hello World";
bool contains = text.Contains("World");      // true
bool starts = text.StartsWith("Hello");      // true
bool ends = text.EndsWith("World");          // true
int index = text.IndexOf("World");           // 6
int lastIndex = text.LastIndexOf("o");       // 7
```

Extraction:
```csharp
string text = "Hello World";
string sub = text.Substring(0, 5);      // "Hello"
string sub2 = text.Substring(6);        // "World"
string first = text[0].ToString();      // "H"
```

Splitting and joining:
```csharp
string csv = "apple,banana,cherry";
string[] fruits = csv.Split(',');       // ["apple", "banana", "cherry"]

string joined = string.Join(" - ", fruits);  // "apple - banana - cherry"
string joined2 = string.Join(", ", fruits);  // "apple, banana, cherry"
```

Modification:
```csharp
string text = "Hello World";
string replaced = text.Replace("World", "C#");     // "Hello C#"
string trimmed = "  text  ".Trim();                // "text"
string trimStart = "  text  ".TrimStart();         // "text  "
string trimEnd = "  text  ".TrimEnd();             // "  text"
string padded = "5".PadLeft(3, '0');               // "005"
string padded2 = "5".PadRight(3, '0');             // "500"
```

Other:
```csharp
string text = "Hello";
int length = text.Length;
bool isEmpty = string.IsNullOrEmpty(text);
bool isWhiteSpace = string.IsNullOrWhiteSpace("   ");

string repeated = string.Concat("Hi", " ", "there");  // "Hi there"
string repeated2 = string.Repeat("ab", 3);             // "ababab"
```

## String Comparison

By default, strings compare case-sensitively:

```csharp
string a = "Hello";
string b = "hello";

bool equals = a == b;              // false
bool equals2 = a.Equals(b);        // false

// Case-insensitive
bool equalsIgnoreCase = a.Equals(b, StringComparison.OrdinalIgnoreCase);  // true

// Comparisons
if ("apple".CompareTo("banana") < 0)
    Console.WriteLine("apple comes first");
```

## String Concatenation vs StringBuilder

Concatenation with + creates new strings:

```csharp
string result = "";
for (int i = 0; i < 1000; i++)
    result += i;  // Creates 1000 string objects - SLOW

// Better: StringBuilder
var sb = new StringBuilder();
for (int i = 0; i < 1000; i++)
    sb.Append(i);
string result = sb.ToString();  // One final string - FAST
```

StringBuilder methods:

```csharp
var sb = new StringBuilder();
sb.Append("Hello");            // Append text
sb.AppendLine("World");        // Append with newline
sb.Insert(0, ">> ");           // Insert at position
sb.Remove(0, 3);               // Remove characters
sb.Replace("Hello", "Hi");     // Replace text
string result = sb.ToString();
```

Performance:
- Concatenation: Use for small number of operations
- StringBuilder: Use in loops or many operations

## Null and Empty Checks

```csharp
string? text = null;

// Check for null
if (text == null)
    Console.WriteLine("null");

if (text is null)
    Console.WriteLine("null");

// Check for null or empty
if (string.IsNullOrEmpty(text))
    Console.WriteLine("null or empty");

if (string.IsNullOrWhiteSpace(text))
    Console.WriteLine("null, empty, or whitespace");

// Safe access
string name = text?.ToUpper() ?? "Unknown";
```

## String vs char

Single character:
```csharp
char c = 'A';
string s = "A";

// Convert between
string fromChar = c.ToString();
char fromString = s[0];

// Character methods
bool isDigit = char.IsDigit('5');
bool isLetter = char.IsLetter('A');
bool isUpper = char.IsUpper('A');
bool isLower = char.IsLower('a');
```

## Regular Expressions

Pattern matching with Regex:

```csharp
using System.Text.RegularExpressions;

string email = "user@example.com";
string pattern = @"^[^@]+@[^@]+\.[^@]+$";

bool isValid = Regex.IsMatch(email, pattern);

// Replace
string text = "The year is 2024";
string replaced = Regex.Replace(text, @"\d+", "XXXX");  // "The year is XXXX"

// Extract
string numbers = "Call 555-1234";
Match match = Regex.Match(numbers, @"\d+-\d+");  // "555-1234"
```

## String Interning

Intern reuses string objects in memory:

```csharp
string a = "Hello";
string b = "Hello";

// By default, may be different objects
bool sameRef = ReferenceEquals(a, b);  // Maybe true or false

// Force interning
string a2 = string.Intern("Hello");
string b2 = string.Intern("Hello");
bool sameRef2 = ReferenceEquals(a2, b2);  // Always true
```

---

## Key Takeaways

- Strings are immutable - operations create new strings
- Use string interpolation for formatting
- Use StringBuilder in loops, not string concatenation
- String methods are powerful for manipulation
- Null/empty checks with IsNullOrEmpty or IsNullOrWhiteSpace
- Regular expressions for complex pattern matching
- verbatim (@) and raw (""") strings for special content
