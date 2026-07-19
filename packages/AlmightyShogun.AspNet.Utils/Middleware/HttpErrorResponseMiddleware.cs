using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Writes standardized HTTP error bodies for empty error responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class HttpErrorResponseMiddleware(RequestDelegate next, IMessageResolver messageResolver)
{
    /// <summary>
    /// Writes a standardized HTTP error response when the pipeline leaves an empty error response.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    ///
    /// <returns>A task representing the asynchronous middleware operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (!ShouldWriteErrorResponse(context.Response))
        {
            return;
        }

        HttpErrorResponse response = HttpErrorResponseFactory.Create(context.Response.StatusCode, messageResolver);

        context.Response.ContentLength = null;
        context.Response.ContentType = "application/json; charset=utf-8";

        await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
    }

    /// <summary>
    /// Determines whether the middleware should write a standardized error body.
    /// </summary>
    ///
    /// <param name="response">The current HTTP response.</param>
    ///
    /// <returns><c>true</c> when the response is an unwritten error response; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool ShouldWriteErrorResponse(HttpResponse response)
    {
        if (response.HasStarted || response.StatusCode < StatusCodes.Status400BadRequest)
        {
            return false;
        }

        return response.ContentLength is null or 0;
    }
}
