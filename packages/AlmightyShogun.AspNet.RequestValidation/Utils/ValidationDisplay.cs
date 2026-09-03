using System.Globalization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Renders values for a failure message, so what a client reads is written the way a person would write it rather than the way the runtime
/// prints it.
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
    /// Renders one value for a failure message, so a boolean or an absent value reads as a person would write it.
    /// </summary>
    ///
    /// <param name="value">The value to render.</param>
    ///
    /// <returns>The text a message shows for it.</returns>
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
