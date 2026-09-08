namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Decides whether two bound values are the same value, for the rules that pair one request field against another on the same request.
/// Numeric values that can be widened to decimal are matched on their magnitude rather than on the type they were boxed as, so ordinary
/// integral and decimal pairings across different property types still agree.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationComparison
{
    /// <summary>
    /// Compares two bound values, widening both to a decimal when each reads as a number and deferring to
    /// <see cref="object.Equals(object, object)"/> otherwise.
    /// </summary>
    ///
    /// <param name="left">
    /// The first value, of whatever type the property declared, so the runtime type decides which comparison runs.
    /// </param>
    /// <param name="right">The second value, read on the same terms as <paramref name="left"/>.</param>
    ///
    /// <returns>
    /// <c>true</c> when both read as numbers of equal magnitude, or when neither does and <see cref="object.Equals(object, object)"/>
    /// accepts them, two <c>null</c> values included.
    /// </returns>
    ///
    /// <remarks>
    /// One side failing to read as a number is enough to fall through to plain equality, which is what keeps the text <c>"5"</c> from
    /// matching the number <c>5</c>, and what leaves two <c>NaN</c> values matching each other as <see cref="double"/> already does.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool AreEqual(object? left, object? right)
    {
        if (TryGetComparableNumber(left, out decimal leftNumber) && TryGetComparableNumber(right, out decimal rightNumber))
            return leftNumber == rightNumber;

        return Equals(left, right);
    }

    /// <summary>
    /// Reads a value as the number this comparison widens to, refusing text before anything is parsed.
    /// </summary>
    ///
    /// <param name="value">The bound value to read.</param>
    /// <param name="number">The number when one could be read; otherwise zero, which callers must not read as a result.</param>
    ///
    /// <returns>
    /// <c>true</c> for a numeric value <see cref="ValidationValue.TryGetNumber"/> can widen exactly, which excludes <c>NaN</c>, the
    /// infinities, and a floating value beyond decimal's range. A string always reports <c>false</c>, however numeric its digits, so two
    /// text fields are matched character for character and a text field never matches a numeric one.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetComparableNumber(object? value, out decimal number)
    {
        if (value is string)
        {
            number = 0m;

            return false;
        }

        return ValidationValue.TryGetNumber(value, out number);
    }
}
