using System.Globalization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Renders values for a failure message, as the single comma-separated string a message template substitutes, and with an absent value
/// written out as <c>null</c> rather than left blank.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationDisplay
{
    /// <summary>
    /// Joins values into the comma-separated list a message template substitutes as one parameter.
    /// </summary>
    ///
    /// <param name="values">The values to list, already in the order the message should read.</param>
    ///
    /// <returns>The values separated by a comma and a space.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string JoinValues(IEnumerable<string> values) => string.Join(", ", values);

    /// <summary>
    /// Joins arbitrary values into a message list, rendering each the way a reader expects rather than the way it prints by default.
    /// </summary>
    ///
    /// <param name="values">The values to list, each passed through the display conversion first.</param>
    ///
    /// <returns>The rendered values separated by a comma and a space.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string JoinDisplayValues(IEnumerable<object?> values) => string.Join(", ", values.Select(ToDisplayValue));

    /// <summary>
    /// Renders one value for a failure message. An absent value is the only special case; everything else is written out under the
    /// invariant culture where it can be, so the same value reads the same whichever culture the server runs under.
    /// </summary>
    ///
    /// <param name="value">The value to render.</param>
    ///
    /// <returns>
    /// The text a message shows for it: <c>null</c> for an absent value, and otherwise the value's own text, empty when it has none.
    /// </returns>
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
}
