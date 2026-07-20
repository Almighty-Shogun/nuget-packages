using System.Text;
using System.Collections;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Provides value conversion helpers used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationValue
{
    private static readonly string[] _acceptedTexts = ["yes", "on", "1", "true"];

    private static readonly string[] _declinedTexts = ["no", "off", "0", "false"];

    /// <summary>
    /// Checks whether a value is considered empty for validation.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is empty; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsEmpty(object? value) => value switch
    {
        null => true,
        string text => text.Length == 0,
        ICollection collection => collection.Count == 0,
        _ => false
    };

    /// <summary>
    /// Checks whether a value is present.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is not <c>null</c>; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsPresent(object? value) => value is not null;

    /// <summary>
    /// Checks whether a value represents an accepted state.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is accepted; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsAccepted(object? value) => value switch
    {
        bool boolValue => boolValue,
        string text => IsAcceptedText(text),
        _ => IsDecimalValue(value, 1)
    };

    /// <summary>
    /// Checks whether a value represents a declined state.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is declined; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsDeclined(object? value) => value switch
    {
        bool boolValue => !boolValue,
        string text => IsDeclinedText(text),
        _ => IsDecimalValue(value, 0)
    };

    /// <summary>
    /// Checks whether a value is either <c>null</c> or a string.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is string-compatible; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsString(object? value) => value is null or string;

    /// <summary>
    /// Checks whether text is one of the accepted text values.
    /// </summary>
    ///
    /// <param name="text">The text to check.</param>
    ///
    /// <returns><c>true</c> when the text is accepted; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAcceptedText(string text)
        => _acceptedTexts.Contains(text, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether text is one of the declined text values.
    /// </summary>
    ///
    /// <param name="text">The text to check.</param>
    ///
    /// <returns><c>true</c> when the text is declined; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsDeclinedText(string text)
        => _declinedTexts.Contains(text, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a value can be read as the expected decimal value.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    /// <param name="expected">The expected decimal value.</param>
    ///
    /// <returns><c>true</c> when the value matches the expected number; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsDecimalValue(object? value, decimal expected)
    {
        if (!TryGetNumber(value, out decimal number))
            return false;

        return number == expected;
    }

    /// <summary>
    /// Attempts to read a value as text.
    /// </summary>
    ///
    /// <param name="value">The value to read.</param>
    /// <param name="text">The resolved text.</param>
    ///
    /// <returns><c>true</c> when the value is text; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetText(object? value, out string text)
    {
        (bool isValid, string resolvedText) = value switch
        {
            string typed => (true, typed),
            _ => (false, string.Empty)
        };

        text = resolvedText;

        return isValid;
    }

    /// <summary>
    /// Checks whether a value can be treated as a boolean.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is boolean-compatible; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsBoolean(object? value) => value switch
    {
        bool => true,
        null => true,
        string { Length: 0 } => true,
        ICollection { Count: 0 } => true,
        string typed => bool.TryParse(typed, out _),
        _ => false
    };

    /// <summary>
    /// Checks whether a value can be treated as an integer.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is integer-compatible; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsInteger(object? value) => value switch
    {
        null => true,
        string { Length: 0 } => true,
        ICollection { Count: 0 } => true,
        byte or sbyte or short or ushort or int or uint or long or ulong => true,
        string typed => long.TryParse(typed, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
        _ => false
    };

    /// <summary>
    /// Checks whether a value can be treated as numeric.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is numeric-compatible; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsNumeric(object? value) => value switch
    {
        null => true,
        string { Length: 0 } => true,
        ICollection { Count: 0 } => true,
        _ => TryGetNumber(value, out _)
    };

    /// <summary>
    /// Attempts to read a value as a decimal number.
    /// </summary>
    ///
    /// <param name="value">The value to read.</param>
    /// <param name="number">The resolved number.</param>
    ///
    /// <returns><c>true</c> when the value can be read as a number; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetNumber(object? value, out decimal number)
    {
        (bool isValid, decimal resolvedNumber) = value switch
        {
            string typed => decimal.TryParse(typed, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsed)
                ? (true, parsed) : (false, 0m),
            byte typed => (true, typed),
            sbyte typed => (true, typed),
            short typed => (true, typed),
            ushort typed => (true, typed),
            int typed => (true, typed),
            uint typed => (true, typed),
            long typed => (true, typed),
            ulong typed => (true, typed),
            float typed => (true, (decimal)typed),
            double typed => (true, (decimal)typed),
            decimal typed => (true, typed),
            _ => (false, 0m)
        };

        number = resolvedNumber;

        return isValid;
    }

    /// <summary>
    /// Attempts to read a value as digit-only text.
    /// </summary>
    ///
    /// <param name="value">The value to read.</param>
    /// <param name="text">The resolved digit text.</param>
    ///
    /// <returns><c>true</c> when the value can be read as digit-only text; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetDigitText(object? value, out string text)
    {
        text = value switch
        {
            string typed => typed,
            byte typed => typed.ToString(CultureInfo.InvariantCulture),
            sbyte typed => typed.ToString(CultureInfo.InvariantCulture),
            short typed => typed.ToString(CultureInfo.InvariantCulture),
            ushort typed => typed.ToString(CultureInfo.InvariantCulture),
            int typed => typed.ToString(CultureInfo.InvariantCulture),
            uint typed => typed.ToString(CultureInfo.InvariantCulture),
            long typed => typed.ToString(CultureInfo.InvariantCulture),
            ulong typed => typed.ToString(CultureInfo.InvariantCulture),
            _ => string.Empty
        };

        return text.Length > 0 && text.All(IsAsciiDigit);
    }

    /// <summary>
    /// Attempts to read the number of decimal places in a value.
    /// </summary>
    ///
    /// <param name="value">The value to inspect.</param>
    /// <param name="places">The resolved decimal place count.</param>
    ///
    /// <returns><c>true</c> when the decimal place count can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetDecimalPlaces(object? value, out int places) => value switch
    {
        string typed => TryGetTextDecimalPlaces(typed, out places),
        _ => TryGetNumericDecimalPlaces(value, out places)
    };

    /// <summary>
    /// Attempts to read decimal places from text without losing trailing zeros.
    /// </summary>
    ///
    /// <param name="value">The text value to inspect.</param>
    /// <param name="places">The resolved decimal place count.</param>
    ///
    /// <returns><c>true</c> when the text is numeric and decimal places can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetTextDecimalPlaces(string value, out int places)
    {
        if (!TryGetNumber(value, out _))
        {
            places = 0;
            return false;
        }

        int separatorIndex = value.IndexOf('.', StringComparison.Ordinal);

        if (separatorIndex < 0)
        {
            places = 0;
            return true;
        }

        string decimals = value[(separatorIndex + 1)..];

        if (!decimals.All(IsAsciiDigit))
        {
            places = 0;
            return false;
        }

        places = decimals.Length;

        return true;
    }

    /// <summary>
    /// Reads the decimal scale from a decimal value.
    /// </summary>
    ///
    /// <param name="value">The decimal value.</param>
    /// <param name="places">The resolved decimal place count.</param>
    ///
    /// <returns><c>true</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool SetDecimalPlaces(decimal value, out int places)
    {
        places = (decimal.GetBits(value)[3] >> 16) & 0x7F;

        return true;
    }

    /// <summary>
    /// Attempts to read a value as a comparable validation size.
    /// </summary>
    ///
    /// <param name="value">The value to inspect.</param>
    /// <param name="size">The resolved comparable size.</param>
    /// <param name="type">The resolved validation value type.</param>
    ///
    /// <returns><c>true</c> when a comparable size can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetComparableSize(object? value, out decimal size, out ValidationValueType type) => value switch
    {
        IFormFile typed => SetComparableSize(ToKilobytes(typed.Length), ValidationValueType.File, out size, out type),
        string typed => SetComparableSize(typed.Length, ValidationValueType.String, out size, out type),
        ICollection typed => SetComparableSize(typed.Count, ValidationValueType.Array, out size, out type),
        _ => TryGetNumericComparableSize(value, out size, out type)
    };

    /// <summary>
    /// Writes a comparable size result.
    /// </summary>
    ///
    /// <param name="value">The comparable size.</param>
    /// <param name="valueType">The validation value type.</param>
    /// <param name="size">The resolved comparable size.</param>
    /// <param name="type">The resolved validation value type.</param>
    ///
    /// <returns><c>true</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool SetComparableSize(decimal value, ValidationValueType valueType, out decimal size, out ValidationValueType type)
    {
        size = value;
        type = valueType;

        return true;
    }

    /// <summary>
    /// Converts a validation value type to the message key segment used by size rules.
    /// </summary>
    ///
    /// <param name="type">The validation value type.</param>
    ///
    /// <returns>The message key segment.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string ToMessageType(ValidationValueType type) => type switch
    {
        ValidationValueType.Array => "array",
        ValidationValueType.File => "file",
        ValidationValueType.Numeric => "numeric",
        _ => "string"
    };

    /// <summary>
    /// Joins validation values for message parameters.
    /// </summary>
    ///
    /// <param name="values">The values to join.</param>
    ///
    /// <returns>The joined values.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string JoinValues(IEnumerable<string> values) => string.Join(", ", values);

    /// <summary>
    /// Joins display values for message parameters.
    /// </summary>
    ///
    /// <param name="values">The values to join.</param>
    ///
    /// <returns>The joined display values.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string JoinDisplayValues(IEnumerable<object?> values) => string.Join(", ", values.Select(ToDisplayValue));

    /// <summary>
    /// Converts a value to its validation display representation.
    /// </summary>
    ///
    /// <param name="value">The value to display.</param>
    ///
    /// <returns>The display value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string ToDisplayValue(object? value)
    {
        if (value is null)
            return "null";

        return value is IFormattable formattable
            ? formattable.ToString(null, CultureInfo.InvariantCulture)
            : value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Checks whether a value contains only ASCII characters.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is ASCII-only; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsAscii(string value) => Encoding.UTF8.GetByteCount(value) == value.Length;

    /// <summary>
    /// Checks whether a character is an ASCII digit.
    /// </summary>
    ///
    /// <param name="character">The character to check.</param>
    ///
    /// <returns><c>true</c> when the character is an ASCII digit; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAsciiDigit(char character) => character is >= '0' and <= '9';

    /// <summary>
    /// Converts bytes to kilobytes for file size validation.
    /// </summary>
    ///
    /// <param name="bytes">The byte count.</param>
    ///
    /// <returns>The kilobyte value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static decimal ToKilobytes(long bytes) => bytes / 1024m;

    /// <summary>
    /// Attempts to read decimal places from a numeric value.
    /// </summary>
    ///
    /// <param name="value">The value to inspect.</param>
    /// <param name="places">The resolved decimal place count.</param>
    ///
    /// <returns><c>true</c> when the decimal place count can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetNumericDecimalPlaces(object? value, out int places)
    {
        return !TryGetNumber(value, out decimal number)
            ? Fail(out places)
            : SetDecimalPlaces(number, out places);
    }

    /// <summary>
    /// Attempts to read a numeric comparable size.
    /// </summary>
    ///
    /// <param name="value">The value to inspect.</param>
    /// <param name="size">The resolved comparable size.</param>
    /// <param name="type">The resolved validation value type.</param>
    ///
    /// <returns><c>true</c> when the numeric size can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetNumericComparableSize(object? value, out decimal size, out ValidationValueType type)
    {
        if (TryGetNumber(value, out decimal number))
            return SetComparableSize(number, ValidationValueType.Numeric, out size, out type);

        size = 0;
        type = ValidationValueType.String;

        return false;

    }

    /// <summary>
    /// Sets an output value to its default value and returns failure.
    /// </summary>
    ///
    /// <param name="value">The output value.</param>
    ///
    /// <returns><c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool Fail<T>(out T value)
    {
        value = default!;

        return false;
    }
}
