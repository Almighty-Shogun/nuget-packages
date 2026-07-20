namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Represents an exception containing validation errors.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the collected validation errors.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal ValidationBag Errors { get; } = new();

    /// <summary>
    /// Creates a validation exception for a single field.
    /// </summary>
    ///
    /// <param name="field">The validation field.</param>
    /// <param name="key">The validation message key.</param>
    /// <param name="parameters">The validation message parameters.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public ValidationException(string field, string key, params object?[] parameters) : base(key)
        => Errors.Add(ToFieldName(field), key, parameters);

    /// <summary>
    /// Converts a property name into the validation field name used in responses.
    /// </summary>
    ///
    /// <param name="field">The field or property name.</param>
    ///
    /// <returns>The normalized field name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToFieldName(string field)
    {
        if (string.IsNullOrEmpty(field) || char.IsLower(field[0]))
            return field;

        return char.ToLowerInvariant(field[0]) + field[1..];
    }
}
