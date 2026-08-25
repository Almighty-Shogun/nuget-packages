using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Holds the format checks shared between rules, so the same definition of a valid address is used wherever one is needed.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationFormat
{
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    /// <summary>
    /// Checks an address by the shape a mail system will actually accept rather than by the full grammar the specification allows, since
    /// the latter admits addresses no provider would deliver to.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns><c>true</c> when the value is a valid email address; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static bool IsEmail(string value) => _emailAddressAttribute.IsValid(value);
}
