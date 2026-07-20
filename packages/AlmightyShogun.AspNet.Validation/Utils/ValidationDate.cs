using System.Globalization;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Provides date parsing and comparison helpers used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationDate
{
    /// <summary>
    /// Attempts to read a value as a UTC date.
    /// </summary>
    ///
    /// <param name="value">The value to read.</param>
    /// <param name="date">The resolved UTC date.</param>
    ///
    /// <returns><c>true</c> when the date can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetDate(object? value, out DateTimeOffset date) => value switch
    {
        DateTimeOffset typed => SetDate(typed.ToUniversalTime(), out date),
        DateTime typed => SetDate(ToDateTimeOffset(typed), out date),
        DateOnly typed => SetDate(new DateTimeOffset(typed.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)), out date),
        string typed => TryGetDateFromText(typed, out date),
        _ => Fail(out date)
    };

    /// <summary>
    /// Attempts to parse text as a UTC date.
    /// </summary>
    ///
    /// <param name="value">The text value to parse.</param>
    /// <param name="date">The resolved UTC date.</param>
    ///
    /// <returns><c>true</c> when the date can be parsed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetDateFromText(string value, out DateTimeOffset date)
    {
        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out date))
            return true;

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime dateTime))
        {
            date = ToDateTimeOffset(dateTime);
            return true;
        }

        date = default;

        return false;
    }

    /// <summary>
    /// Attempts to read a value as a UTC date using an exact format.
    /// </summary>
    ///
    /// <param name="value">The value to read.</param>
    /// <param name="format">The expected date format.</param>
    /// <param name="date">The resolved UTC date.</param>
    ///
    /// <returns><c>true</c> when the exact date can be read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool TryGetExactDate(object? value, string format, out DateTimeOffset date) => value switch
    {
        string typed => TryGetExactDateFromText(typed, format, out date),
        _ => Fail(out date)
    };

    /// <summary>
    /// Attempts to parse text as a UTC date using an exact format.
    /// </summary>
    ///
    /// <param name="value">The text value to parse.</param>
    /// <param name="format">The expected date format.</param>
    /// <param name="date">The resolved UTC date.</param>
    ///
    /// <returns><c>true</c> when the exact date can be parsed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetExactDateFromText(string value, string format, out DateTimeOffset date)
    {
        if (DateTimeOffset.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out date))
            return true;

        if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime dateTime))
        {
            date = ToDateTimeOffset(dateTime);
            return true;
        }

        if (!DateOnly.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateOnly))
            return false;

        date = new DateTimeOffset(dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));

        return true;
    }

    /// <summary>
    /// Converts a date to its UTC validation message value.
    /// </summary>
    ///
    /// <param name="date">The date to format.</param>
    ///
    /// <returns>The UTC date message value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string ToMessageValue(DateTimeOffset date)
        => date.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    ///
    /// <param name="dateTime">The date time to convert.</param>
    ///
    /// <returns>The UTC date time offset.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static DateTimeOffset ToDateTimeOffset(DateTime dateTime) => dateTime.Kind switch
    {
        DateTimeKind.Local => new DateTimeOffset(dateTime).ToUniversalTime(),
        DateTimeKind.Utc => new DateTimeOffset(dateTime),
        _ => new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc))
    };

    /// <summary>
    /// Writes a date output value.
    /// </summary>
    ///
    /// <param name="value">The date value.</param>
    /// <param name="date">The output date.</param>
    ///
    /// <returns><c>true</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool SetDate(DateTimeOffset value, out DateTimeOffset date)
    {
        date = value;

        return true;
    }

    /// <summary>
    /// Sets the date output to its default value and returns failure.
    /// </summary>
    ///
    /// <param name="date">The output date.</param>
    ///
    /// <returns><c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool Fail(out DateTimeOffset date)
    {
        date = default;

        return false;
    }
}
