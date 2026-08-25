using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// What a response factory is given: the failures and the context they arose in, so a replacement can shape the body however it needs.
/// </summary>
///
/// <param name="HttpContext">The HTTP context for the current request.</param>
/// <param name="StatusCode">The validation response status code.</param>
/// <param name="Errors">The collected validation errors.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record ValidationResponseContext(HttpContext HttpContext, int StatusCode, ValidationBag Errors);
