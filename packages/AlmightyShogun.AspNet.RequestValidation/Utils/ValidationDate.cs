using System.Globalization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Reads loosely typed values as dates and compares them. Everything is normalized to UTC first, so two dates written in different offsets
/// order by the instant they name rather than by the text they were written in.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationDate
{
    /// <summary>
    /// Reads a value as a date, accepting the date types directly and text by parsing, then normalizing the result to UTC.
    /// </summary>
    ///
    /// <param name="value">The bound value to convert, which may already be the target type or may be text that has to be parsed.</param>
    /// <param name="date">
    /// Receives the date normalized to UTC, so two values written in different offsets order by the instant they name.
    /// </param>
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
    /// Parses text as a date under the invariant culture, so the same payload validates identically wherever the application runs.
    /// </summary>
    ///
    /// <param name="value">The text value to parse.</param>
    /// <param name="date">
    /// Receives the date normalized to UTC, so two values written in different offsets order by the instant they name.
    /// </param>
    ///
    /// <returns><c>true</c> when the date can be parsed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetDateFromText(string value, out DateTimeOffset date)
    {
        const DateTimeStyles flags = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, flags, out date))
            return true;

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, flags, out DateTime dateTime))
        {
            date = ToDateTimeOffset(dateTime);

            return true;
        }

        date = default;

        return false;
    }

    /// <summary>
    /// Reads a value as a date that must match one exact format, for the rule that constrains how a date is written and not only what it
    /// means.
    /// </summary>
    ///
    /// <param name="value">The bound value, accepted only as text; a real date instance is rejected here.</param>
    /// <param name="format">
    /// The one format the value must match, so a date written any other way fails even when it names a real instant.
    /// </param>
    /// <param name="date">
    /// Receives the date normalized to UTC, so two values written in different offsets order by the instant they name.
    /// </param>
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
    /// Parses text against one exact format, rejecting anything the format does not describe rather than falling back to a loose parse.
    /// </summary>
    ///
    /// <param name="value">The text value to parse.</param>
    /// <param name="format">
    /// The one format the value must match, so a date written any other way fails even when it names a real instant.
    /// </param>
    /// <param name="date">
    /// Receives the date normalized to UTC, so two values written in different offsets order by the instant they name.
    /// </param>
    ///
    /// <returns><c>true</c> when the exact date can be parsed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetExactDateFromText(string value, string format, out DateTimeOffset date)
    {
        const DateTimeStyles flags = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;

        if (DateTimeOffset.TryParseExact(value, format, CultureInfo.InvariantCulture, flags, out date))
            return true;

        if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, flags, out DateTime dateTime))
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
    /// Renders a date for a failure message in UTC, so the sentence a client reads names the same instant the comparison used.
    /// </summary>
    ///
    /// <param name="date">The date to render, normalized to UTC first so the sentence names the same instant the comparison used.</param>
    ///
    /// <returns>The UTC date message value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string ToMessageValue(DateTimeOffset date) => date.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a UTC <see cref="DateTimeOffset"/> .
    /// </summary>
    ///
    /// <param name="dateTime">The value to normalize, whose kind decides whether it is treated as local or already universal.</param>
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
    /// Writes the output and reports success in one expression, keeping the try-pattern methods above expression-bodied.
    /// </summary>
    ///
    /// <param name="value">The value already read as a date, normalized to UTC before any comparison.</param>
    /// <param name="date">Receives the date; left at its default when none could be read, which callers must not treat as a result.</param>
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
    /// Clears the output and reports failure in one expression, the counterpart of the success writer above.
    /// </summary>
    ///
    /// <param name="date">Receives the date; left at its default when none could be read, which callers must not treat as a result.</param>
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
