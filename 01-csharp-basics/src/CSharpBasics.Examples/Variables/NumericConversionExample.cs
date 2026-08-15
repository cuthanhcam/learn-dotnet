using System.Globalization;

namespace CSharpBasics.Examples.Variables;

public static class NumericConversionExample
{
    /// <summary>
    /// Parses a decimal using an explicit culture and a deliberately limited number style.
    /// </summary>
    public static bool TryParseAmount(
        string? text,
        CultureInfo culture,
        out decimal amount)
    {
        ArgumentNullException.ThrowIfNull(culture);

        // NumberStyles.Number accepts a sign, decimal separator, grouping separators, and
        // surrounding whitespace. Currency symbols are rejected because they are not part
        // of this method's input contract.
        return decimal.TryParse(text, NumberStyles.Number, culture, out amount);
    }

    /// <summary>
    /// Narrows a 64-bit value only when it is representable by Int32.
    /// </summary>
    public static int ToInt32Checked(long value)
    {
        // An explicit cast documents narrowing; checked makes overflow visible rather than
        // silently truncating high-order bits in an unchecked context.
        return checked((int)value);
    }

    public static bool TryToInt32(long value, out int result)
    {
        if (value is < int.MinValue or > int.MaxValue)
        {
            result = default;
            return false;
        }

        result = (int)value;
        return true;
    }
}
