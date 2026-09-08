using System.Globalization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Reads loosely typed values as dates, normalizing every result to UTC. Comparing two of them is left to
/// <see cref="DateValidationRule{TRequest,TProperty}"/>, and the normalization is what lets that comparison order two dates written in
/// different offsets by the instant they name rather than by the text they were written in.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class ValidationDate
{
    /// <summary>
    /// The styles every parse here runs under. Text carrying no offset is read as UTC rather than in the machine's own offset, and text
    /// carrying one is converted, so the same text names the same instant wherever the application runs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const DateTimeStyles _dateStyles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;

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
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    private static bool TryGetDateFromText(string value, out DateTimeOffset date)
    {
        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, _dateStyles, out date))
            return true;

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, _dateStyles, out DateTime dateTime))
        {
            date = ToDateTimeOffset(dateTime);

            return true;
        }

        date = default;

        return false;
    }

    /// <summary>
    /// Parses the literal a comparison rule was written with, under the culture and styles a submitted value is already parsed under, so
    /// the two sides of a comparison read a date written without an offset the same way.
    /// </summary>
    ///
    /// <param name="value">The literal date the rule compares against, as it was written at the declaration site.</param>
    ///
    /// <returns>The date normalized to UTC.</returns>
    ///
    /// <exception cref="ArgumentNullException">
    /// <paramref name="value"/> is <c>null</c>. Every caller is a comparison attribute that takes the literal as a non-nullable parameter,
    /// so it takes a declaration written as <c>[After(null!)]</c> to reach this.
    /// </exception>
    /// <exception cref="FormatException">
    /// <paramref name="value"/> is not a date the invariant culture can read. <see cref="TryGetDate"/> reads text without throwing, for
    /// the callers that have a value rather than a declaration.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static DateTimeOffset ParseTargetDate(string value) => DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, _dateStyles);

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
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    private static bool TryGetExactDateFromText(string value, string format, out DateTimeOffset date)
    {
        if (DateTimeOffset.TryParseExact(value, format, CultureInfo.InvariantCulture, _dateStyles, out date))
            return true;

        if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, _dateStyles, out DateTime dateTime))
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
    /// <since>4.0.0</since>
    public static string ToMessageValue(DateTimeOffset date) => date.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    ///
    /// <param name="dateTime">The value to normalize, whose kind decides whether it is treated as local or already universal.</param>
    ///
    /// <returns>The UTC date time offset.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    private static bool Fail(out DateTimeOffset date)
    {
        date = default;

        return false;
    }
}
