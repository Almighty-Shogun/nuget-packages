using Microsoft.AspNetCore.Mvc;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the result a failed validation returns, keeping the shape decision away from the rules that produced the failures. Internal to
/// the package: the shape is not a substitution point an application can replace.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IValidationResponseFactory
{
    /// <summary>
    /// Builds the result for a set of field failures, including the status code it should be sent with.
    /// </summary>
    ///
    /// <param name="context">The failures and the request they arose in, which is everything the body is built from.</param>
    ///
    /// <returns>The action result containing the validation error response.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IActionResult Create(ValidationResponseContext context);
}
