using System.Collections;
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Reduces a value to the single number a size rule compares. What counts as its size depends on what it is, so one rule serves numbers,
/// strings, collections, and uploads without knowing which it was handed.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationSize
{
    /// <summary>
    /// Reduces a value to the single number a size rule compares, and reports which kind of size it is so the failure message can say
    /// whether it counted characters, items, kilobytes, or the value itself.
    /// </summary>
    ///
    /// <param name="value">The value to measure.</param>
    /// <param name="size">A file's kilobytes, a string's length, a collection's count, or the number itself.</param>
    /// <param name="type">Which of those four was measured, which picks the message the rule reports.</param>
    ///
    /// <returns><c>true</c> when the value was one of those four shapes; otherwise <c>false</c>.</returns>
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
    /// Writes both outputs of a size read in one expression, so each arm of the switch above stays a single arm.
    /// </summary>
    ///
    /// <param name="value">The measured size to hand back.</param>
    /// <param name="valueType">Which quantity was measured, which picks the message the failure reports.</param>
    /// <param name="size">Receives the size.</param>
    /// <param name="type">Receives the kind.</param>
    ///
    /// <returns>Always <c>true</c>, since reaching here means a size was measured.</returns>
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
    /// Maps a measured kind onto its message key segment, so one size rule resolves four sentences without four rules existing.
    /// </summary>
    ///
    /// <param name="type">The kind that was measured.</param>
    ///
    /// <returns>The segment appended to the rule's key, such as <c>string</c> or <c>file</c>.</returns>
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
    /// Measures a value that is none of the three sized shapes, by reading the number itself so a numeric limit compares against its
    /// magnitude rather than its digit count.
    /// </summary>
    ///
    /// <param name="value">The bound value to measure, accepted as text or as any numeric type.</param>
    /// <param name="size">The resolved comparable size.</param>
    /// <param name="type">The resolved validation value type.</param>
    ///
    /// <returns><c>true</c> when the numeric size can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetNumericComparableSize(object? value, out decimal size, out ValidationValueType type)
    {
        if (TryGetOrderableNumber(value, out decimal number))
            return SetComparableSize(number, ValidationValueType.Numeric, out size, out type);

        size = 0;
        type = ValidationValueType.String;

        return false;
    }

    /// <summary>
    /// Reads a value as a decimal for ordering alone, clamping a magnitude no decimal can hold to the nearest bound instead of refusing it.
    /// </summary>
    ///
    /// <param name="value">The value to read, of whatever numeric type the property declared.</param>
    /// <param name="number">The orderable number, or zero when the value has no place in an ordering at all.</param>
    ///
    /// <returns>
    /// <c>true</c> for anything <see cref="ValidationValue.TryGetNumber"/> reads exactly, and additionally for an infinity or a floating
    /// value beyond decimal's range. <c>false</c> for <c>NaN</c> , which no comparison against it can be true of, and for anything that is
    /// not numeric at all.
    /// </returns>
    ///
    /// <remarks>
    /// Clamping is safe here and only here. Every ordering against a bound a decimal can hold gives the same answer for the clamped value
    /// as for the original, so a minimum or maximum on an extreme double is answered correctly rather than reported as non-numeric. It
    /// would not be safe for arithmetic: a clamped value's remainder is not the original's, which is why the multiple-of rule reads the
    /// exact number instead.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetOrderableNumber(object? value, out decimal number)
    {
        if (ValidationValue.TryGetNumber(value, out number))
            return true;

        double? floatingValue = value switch
        {
            float typed => typed,
            double typed => typed,
            _ => null
        };

        if (floatingValue is not { } floating || double.IsNaN(floating))
        {
            number = 0;

            return false;
        }

        number = floating > 0 ? decimal.MaxValue : decimal.MinValue;

        return true;
    }

    /// <summary>
    /// Converts a byte count to kilobytes as a decimal, so a size limit compares against the same unit a person wrote it in and a part of a
    /// kilobyte is not truncated away.
    /// </summary>
    ///
    /// <param name="bytes">The file size in bytes, converted so a limit compares against the unit a person wrote it in.</param>
    ///
    /// <returns>The kilobyte value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static decimal ToKilobytes(long bytes) => bytes / 1024m;
}
