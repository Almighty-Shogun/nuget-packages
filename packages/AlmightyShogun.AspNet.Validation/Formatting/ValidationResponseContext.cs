using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Contains the data needed to create a validation error response.
/// </summary>
///
/// <param name="HttpContext">The HTTP context for the current request.</param>
/// <param name="StatusCode">The validation response status code.</param>
/// <param name="Errors">The collected validation errors.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record ValidationResponseContext(HttpContext HttpContext, int StatusCode, ValidationBag Errors);
