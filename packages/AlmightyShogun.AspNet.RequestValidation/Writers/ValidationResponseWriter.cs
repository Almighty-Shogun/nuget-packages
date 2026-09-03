using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Returns a validation failure to the client. Validation carries a per-field error dictionary, which the shared
/// <see cref="IHttpErrorResponseWriter"/> deliberately does not model, so this package returns its own body and leaves each caller to
/// choose between writing it and handing back a result. The body itself is built by
/// <see cref="ValidationErrorResponseFactory"/> , which every other path shares.
/// </summary>
///
/// <param name="messageResolver">The resolver used to produce the description and the per-field messages.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationResponseWriter(IMessageResolver messageResolver)
{
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
    internal ValidationErrorResponse CreateResponse(ValidationBag errors)
        => ValidationErrorResponseFactory.Create(messageResolver, errors);

    /// <summary>
    /// Builds the result for the gathered failures, for the callers that return a result rather than writing the response themselves.
    /// </summary>
    ///
    /// <param name="errors">The failures gathered while the rules ran, one entry per field that failed.</param>
    ///
    /// <returns>The result carrying the validation body, whose status comes from the body rather than being set separately.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal HttpErrorResult CreateResult(ValidationBag errors) => new(CreateResponse(errors));

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
    /// The serializer options are left to the framework to resolve, which reads the application's <c>JsonOptions</c> from the container
    /// rather than MVC's own. That is the right source for a middleware, which runs outside MVC and may run where MVC is not registered
    /// at all.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal async Task WriteAsync(HttpContext context, int statusCode, string messageKey, CancellationToken cancellationToken)
    {
        if (context.Response.HasStarted) return;

        context.Response.ContentLength = null;
        context.Response.StatusCode = statusCode;

        ValidationErrorResponse response = ValidationErrorResponseFactory.Create(messageResolver, messageKey, statusCode);

        await context.Response.WriteAsJsonAsync(
            response,
            options: null,
            contentType: "application/json; charset=utf-8",
            cancellationToken
        );
    }
}
