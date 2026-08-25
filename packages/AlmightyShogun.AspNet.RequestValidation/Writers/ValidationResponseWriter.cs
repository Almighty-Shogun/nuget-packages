using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the validation error response body. Validation carries a per-field error dictionary, which the shared
/// <see cref="IHttpErrorResponseWriter"/> deliberately does not model, so this package shapes its own body in one place and leaves each
/// caller to decide how to return it.
/// </summary>
///
/// <param name="messageResolver">The resolver used to produce the description and the per-field messages.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationResponseWriter(IMessageResolver messageResolver)
{
    /// <summary>
    /// The status a validation failure is returned with, which is the unprocessable-content code rather than a plain bad request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const int StatusCode = StatusCodes.Status422UnprocessableEntity;

    /// <summary>
    /// The machine-readable identifier every validation failure is reported under, shared by the per-field response and the plain error
    /// body written for a request whose payload could not be read at all.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const string ErrorCode = "validation_error";

    /// <summary>
    /// Assembles the response body from the gathered failures, resolving each message key as it goes.
    /// </summary>
    ///
    /// <param name="errors">The failures gathered while the rules ran, one entry per field that failed.</param>
    ///
    /// <returns>The validation error response.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal ValidationErrorResponse CreateResponse(ValidationBag errors) => new()
    {
        Code = StatusCode,
        Error = ErrorCode,
        ErrorDescription = messageResolver.Resolve($"http-error.{StatusCode}"),
        Errors = errors.ToErrorDictionary(messageResolver)
    };
}
