using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Provides format helpers used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationFormat
{
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    /// <summary>
    /// Checks whether a value is a valid email address.
    /// </summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns><c>true</c> when the value is a valid email address; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsEmail(string value) => _emailAddressAttribute.IsValid(value);
}
