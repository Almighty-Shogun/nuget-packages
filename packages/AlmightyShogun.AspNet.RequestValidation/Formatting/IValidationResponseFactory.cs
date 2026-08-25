using Microsoft.AspNetCore.Mvc;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the result a failed validation returns. Registered so an application can substitute its own shape without touching the rules that
/// produced the failures.
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
    /// <param name="context">The failures and the context they arose in, so a replacement factory can shape the body as it needs.</param>
    ///
    /// <returns>The action result containing the validation error response.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IActionResult Create(ValidationResponseContext context);
}
