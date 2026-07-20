using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;
using Microsoft.AspNetCore.Diagnostics;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Handles validation exceptions and writes standardized JSON responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationExceptionHandler(IMessageResolver messageResolver) : IExceptionHandler
{
    private const int _statusCode = StatusCodes.Status422UnprocessableEntity;

    /// <summary>
    /// Converts validation exceptions into standardized validation error responses.
    /// </summary>
    ///
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns><c>true</c> when the exception was handled; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException || httpContext.Response.HasStarted)
            return false;

        ValidationErrorResponse response = new()
        {
            Code = _statusCode,
            Error = "validation_error",
            ErrorDescription = messageResolver.Resolve($"http-error.{_statusCode}"),
            Errors = validationException.Errors.ToErrorDictionary(messageResolver)
        };

        httpContext.Response.StatusCode = response.Code;
        httpContext.Response.ContentLength = null;
        httpContext.Response.ContentType = "application/json; charset=utf-8";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
