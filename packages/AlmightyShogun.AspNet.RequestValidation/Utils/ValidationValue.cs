using System.Text;
using System.Collections;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Reads loosely typed bound values the way validation rules need them. A request property may arrive as the declared type or as text from
/// a form post, so every helper here accepts <see cref="object"/> and decides what the value actually is rather than trusting a cast.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationValue
{
    /// <summary>
    /// The textual spellings counted as accepted, matched case-insensitively. Covers what an HTML checkbox and a JSON boolean each post.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly string[] _acceptedTexts = ["yes", "on", "1", "true"];

    /// <summary>
    /// The textual spellings counted as declined. A value in neither list is neither accepted nor declined, so both checks report
    /// <c>false</c> rather than one implying the other.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly string[] _declinedTexts = ["no", "off", "0", "false"];

    /// <summary>
    /// Checks whether a value is empty, which is what every value rule short-circuits on so it never implies the field is required.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns>
    /// <c>true</c> for <c>null</c> , an empty string, a zero-length uploaded file, and an empty collection. A <c>false</c> boolean and a
    /// zero number are values, not absences, so both come back <c>false</c> .
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsEmpty(object? value) => value switch
    {
        null => true,
        string text => text.Length == 0,
        IFormFile file => file.Length == 0,
        ICollection collection => collection.Count == 0,
        _ => false
    };

    /// <summary>
    /// Checks whether a value was supplied at all, which is a weaker test than not being empty.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns>
    /// <c>true</c> for anything that is not <c>null</c> , an empty string included. Presence rules need that distinction: a field posted
    /// blank was still posted.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsPresent(object? value) => value is not null;

    /// <summary>
    /// Checks whether a value reads as a yes, across the spellings a checkbox, a JSON boolean, and a numeric flag each produce.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns><c>true</c> for <c>true</c>, one of the accepted spellings, or the number <c>1</c>; otherwise <c>false</c>.</returns>
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
    /// Checks whether a value reads as a no. Not the negation of the accepted check: a value in neither vocabulary is neither.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns><c>true</c> for <c>false</c>, one of the declined spellings, or the number <c>0</c>; otherwise <c>false</c>.</returns>
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
    /// Checks whether a value is text or absent, which is what the string type rule accepts.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns>
    /// <c>true</c> for a string or <c>null</c> ; otherwise <c>false</c> , which includes a number bound to an object property.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsString(object? value) => value is null or string;

    /// <summary>
    /// Matches text against the accepted vocabulary, case-insensitively, so <c>Yes</c> and <c>YES</c> read the same as <c>yes</c> .
    /// </summary>
    ///
    /// <param name="text">The text to check.</param>
    ///
    /// <returns><c>true</c> when the text is accepted; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAcceptedText(string text) => _acceptedTexts.Contains(text, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Matches text against the declined vocabulary, case-insensitively, on the same terms as the accepted check.
    /// </summary>
    ///
    /// <param name="text">The text to check.</param>
    ///
    /// <returns><c>true</c> when the text is declined; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsDeclinedText(string text) => _declinedTexts.Contains(text, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Compares a non-boolean, non-text value against an exact number, which is how a numeric <c>1</c> or <c>0</c> is read as a yes or a no
    /// without accepting every other number.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
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
    /// Reads a value as text, without converting anything that is not already text, so a rule meant for strings declines a number rather
    /// than validating its digits.
    /// </summary>
    ///
    /// <param name="value">The bound value to convert, which may already be the target type or may be text that has to be parsed.</param>
    /// <param name="text">The text when the value was a string; otherwise an empty string rather than <c>null</c>.</param>
    ///
    /// <returns>
    /// <c>true</c> only when the value was already a string. A number is not stringified here, so a rule meant for text declines it instead
    /// of silently validating its digits.
    /// </returns>
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
    /// Checks whether a value is a boolean or parses as one. Absent and empty values pass, so the rule never implies the field is required.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns><c>true</c> for a boolean, <c>null</c>, an empty string or collection, or text <c>bool.TryParse</c> accepts.</returns>
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
    /// Checks whether a value is a whole number or parses as one. Absent and empty values pass, as with every type check here.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns>
    /// <c>true</c> for any integral type, an absent or empty value, or text parsed under the invariant culture, so a grouped or localized
    /// spelling is rejected rather than accepted by whichever culture the server runs under.
    /// </returns>
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
    /// Checks whether a value is a number or parses as one, integral or fractional. Absent and empty values pass, as with every type check
    /// here, so the rule never implies the field is required.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns>
    /// <c>true</c> for an absent or empty value, for any numeric type whatever magnitude it holds, and for text that parses under the
    /// invariant culture.
    /// </returns>
    ///
    /// <remarks>
    /// The runtime type is tested rather than the value converted, so a double outside decimal's range, an infinity, and <c>NaN</c> all
    /// report as numeric. They were bound to a numeric property and are numbers; whether they can be <em>compared</em> is a separate
    /// question, which <see cref="TryGetNumber"/> and <see cref="ValidationSize.TryGetComparableSize"/> answer.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsNumeric(object? value) => value switch
    {
        null => true,
        string { Length: 0 } => true,
        ICollection { Count: 0 } => true,
        byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal => true,
        string typed => decimal.TryParse(typed, NumberStyles.Number, CultureInfo.InvariantCulture, out _),
        _ => false
    };

    /// <summary>
    /// Reads a value as a decimal, widening every numeric type to one so a comparison never depends on which one the model declared.
    /// </summary>
    ///
    /// <param name="value">The bound value to convert, which may already be the target type or may be text that has to be parsed.</param>
    /// <param name="number">The number when one could be read; otherwise zero, which callers must not read as a result.</param>
    ///
    /// <returns>
    /// <c>true</c> for any numeric type whose value a decimal can hold exactly, or text parsed under the invariant culture. Text is
    /// deliberately not parsed under the request's culture, so the same payload validates identically wherever the application runs.
    /// <c>NaN</c> , the infinities, and a floating value beyond decimal's range all report <c>false</c> , since no decimal stands for
    /// them.
    /// </returns>
    ///
    /// <remarks>
    /// This is the exact read, for the rules that do arithmetic on the result rather than only order it, such as the multiple-of check and
    /// the decimal-place count. Ordering rules use <see cref="ValidationSize.TryGetComparableSize"/> instead, which accepts values this
    /// one refuses.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetNumber(object? value, out decimal number)
    {
        (bool isValid, decimal resolvedNumber) = value switch
        {
            string typed => decimal.TryParse(typed, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsed)
                ? (true, parsed)
                : (false, 0m),
            byte typed => (true, typed),
            sbyte typed => (true, typed),
            short typed => (true, typed),
            ushort typed => (true, typed),
            int typed => (true, typed),
            uint typed => (true, typed),
            long typed => (true, typed),
            ulong typed => (true, typed),
            float typed => TryFromFloatingPoint(typed),
            double typed => TryFromFloatingPoint(typed),
            decimal typed => (true, typed),
            _ => (false, 0m)
        };

        number = resolvedNumber;

        return isValid;
    }

    /// <summary>
    /// Reads a value as a run of digits, for the rules that count digits rather than compare magnitudes.
    /// </summary>
    ///
    /// <param name="value">The value to read. An integral type is written out first, so the digits of a number can be counted.</param>
    /// <param name="text">The digit text when every character was a digit; otherwise an empty string.</param>
    ///
    /// <returns>
    /// <c>true</c> only when the result is non-empty and entirely ASCII digits, so a negative sign, a decimal point, or a thousands
    /// separator all decline rather than being counted.
    /// </returns>
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
    /// Reads how many decimal places a value carries, for the rule that constrains scale rather than magnitude.
    /// </summary>
    ///
    /// <param name="value">The value to inspect, as text or as any numeric type.</param>
    /// <param name="places">Receives the count; zero when none could be read, which callers must not treat as a result.</param>
    ///
    /// <returns><c>true</c> when the value was a number or numeric text; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetDecimalPlaces(object? value, out int places) => value switch
    {
        string typed => TryGetTextDecimalPlaces(typed, out places),
        _ => TryGetNumericDecimalPlaces(value, out places)
    };

    /// <summary>
    /// Counts the decimal places written in the text itself rather than in the parsed number, so a trailing zero the writer typed is
    /// counted as the place it is.
    /// </summary>
    ///
    /// <param name="value">The text value to inspect.</param>
    /// <param name="places">The resolved decimal place count.</param>
    ///
    /// <returns><c>true</c> when the text parsed as a number; otherwise <c>false</c>.</returns>
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
    /// Reads the scale a decimal carries in its own representation, which is what preserves a trailing zero that a conversion to double
    /// would discard.
    /// </summary>
    ///
    /// <param name="value">The decimal to inspect.</param>
    /// <param name="places">Receives the scale, taken from the flags word of the decimal's bit representation.</param>
    ///
    /// <returns>Always <c>true</c>, so this can be the tail of a try-pattern expression rather than a statement.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool SetDecimalPlaces(decimal value, out int places)
    {
        places = (decimal.GetBits(value)[3] >> 16) & 0x7F;

        return true;
    }

    /// <summary>
    /// Checks whether text is entirely single-byte, by comparing its UTF-8 byte count against its length rather than inspecting characters.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns><c>true</c> when every character encodes to one byte; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsAscii(string value) => Encoding.UTF8.GetByteCount(value) == value.Length;

    /// <summary>
    /// Checks whether a character is an ASCII digit, deliberately excluding the digits of other scripts that <c>char.IsDigit</c> would
    /// accept but a digit-count rule should not.
    /// </summary>
    ///
    /// <param name="character">One character of the value, tested on its own rather than through a culture-aware string comparison.</param>
    ///
    /// <returns><c>true</c> when the character is an ASCII digit; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAsciiDigit(char character) => character is >= '0' and <= '9';


    /// <summary>
    /// Reads decimal places from a value that is not text, by widening it to a decimal and reading the scale it carries.
    /// </summary>
    ///
    /// <param name="value">The numeric value to inspect.</param>
    /// <param name="places">Receives the scale of the widened decimal.</param>
    ///
    /// <returns><c>true</c> when the value read as a number; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetNumericDecimalPlaces(object? value, out int places) => !TryGetNumber(value, out decimal number)
        ? Fail(out places)
        : SetDecimalPlaces(number, out places);



    /// <summary>
    /// Converts a floating value to a decimal only when one can hold it, so a conversion that would throw reports a failed read instead.
    /// </summary>
    ///
    /// <param name="value">The floating value to convert, which may be <c>NaN</c> , infinite, or simply too large.</param>
    ///
    /// <returns>
    /// The converted number paired with <c>true</c> , or zero paired with <c>false</c> when no decimal stands for the value.
    /// </returns>
    ///
    /// <remarks>
    /// The bound is compared against rather than the conversion being attempted and caught, and it sits just inside
    /// <see cref="decimal.MaxValue"/> because the nearest double to that constant rounds above it. A value the check admits therefore
    /// converts without overflowing.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static (bool IsValid, decimal Number) TryFromFloatingPoint(double value)
    {
        const double maximum = 7.9228162514264337e28;

        if (!double.IsFinite(value) || Math.Abs(value) >= maximum)
            return (false, 0m);

        return (true, (decimal)value);
    }

    /// <summary>
    /// Clears an output and reports failure in one expression, so every try-pattern here stays a single expression rather than growing a
    /// statement body just to assign a default.
    /// </summary>
    ///
    /// <param name="value">Receives the default for its type, so a failed read leaves nothing a caller could mistake for a result.</param>
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
