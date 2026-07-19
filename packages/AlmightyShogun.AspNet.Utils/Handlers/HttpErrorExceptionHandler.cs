using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Handles HTTP error exceptions and writes standardized JSON responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class HttpErrorExceptionHandler(IMessageResolver messageResolver) : IExceptionHandler
{
    /// <summary>
    /// Converts HTTP error exceptions into standardized JSON responses.
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
        if (exception is not HttpErrorException httpException || httpContext.Response.HasStarted)
        {
            return false;
        }

        HttpErrorResponse response = httpException.MessageKey is null
            ? HttpErrorResponseFactory.Create(httpException.StatusCode, messageResolver)
            : HttpErrorResponseFactory.Create(httpException.StatusCode, messageResolver, httpException.MessageKey, httpException.MessageParameters);

        httpContext.Response.ContentLength = null;
        httpContext.Response.StatusCode = response.Code;
        httpContext.Response.ContentType = "application/json; charset=utf-8";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
