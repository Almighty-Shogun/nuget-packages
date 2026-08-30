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

    /// <summary>
    /// Writes an error that has no per-field detail, such as a body that could not be read, in the same envelope a rule failure uses.
    /// </summary>
    ///
    /// <param name="context">The response written to. Left untouched once the response has started.</param>
    /// <param name="statusCode">The status the response is sent with and repeated inside the body.</param>
    /// <param name="messageKey">The message key describing the failure, resolved into the negotiated language here.</param>
    /// <param name="cancellationToken">Cancels the write, normally the request's own abort token.</param>
    ///
    /// <returns>A task that completes once the body is written, or immediately when the response had already started.</returns>
    ///
    /// <remarks>
    /// The <c>Errors</c> dictionary is present but empty, so a client reading it finds nothing to report rather than finding the field
    /// missing altogether. That is what lets one envelope cover both a rule failure and a body that never parsed.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal async Task WriteAsync(HttpContext context, int statusCode, string messageKey, CancellationToken cancellationToken)
    {
        if (context.Response.HasStarted) return;

        context.Response.ContentLength = null;
        context.Response.StatusCode = statusCode;

        var response = new ValidationErrorResponse
        {
            Code = statusCode,
            Error = ErrorCode,
            ErrorDescription = messageResolver.Resolve(messageKey),
            Errors = new Dictionary<string, ValidationRuleError>()
        };
        
        await context.Response.WriteAsJsonAsync(
            response,
            options: null,
            contentType: "application/json; charset=utf-8",
            cancellationToken
        );
    }
}
