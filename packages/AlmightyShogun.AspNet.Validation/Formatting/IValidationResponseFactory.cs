using Microsoft.AspNetCore.Mvc;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Creates action results for validation error responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IValidationResponseFactory
{
    /// <summary>
    /// Creates a validation error response.
    /// </summary>
    ///
    /// <param name="context">The validation response context.</param>
    ///
    /// <returns>The action result containing the validation error response.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IActionResult Create(ValidationResponseContext context);
}
