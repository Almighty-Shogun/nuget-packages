using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Fills in the standardized body for an error status that reached the client with no content, covering the responses
/// produced below MVC: authentication challenges, routing failures, and anything short-circuited by earlier middleware.
/// </summary>
///
/// <param name="next">The rest of the pipeline, run to completion before the response is inspected.</param>
/// <param name="messageResolver">The resolver that turns the <c>http-error.{status}</c> key into a localized description.</param>
/// <param name="responseWriter">The writer that produces the body, matching every other error the application returns.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class HttpErrorResponseMiddleware(
    RequestDelegate next,
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter
)
{
    /// <summary>
    /// Runs the pipeline, then writes the error body if what came back was an empty error.
    /// </summary>
    ///
    /// <param name="context">The context whose response is inspected and, when it qualifies, written to.</param>
    ///
    /// <returns>A task that completes once the pipeline has run and any error body has been written.</returns>
    ///
    /// <remarks>
    /// Register it inside <c>UseHttpErrorResponses</c> and therefore below the exception handler, so an exception has
    /// already been turned into a written response by the time the status code is examined here.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (!ShouldWriteErrorResponse(context.Response)) return;

        int statusCode = context.Response.StatusCode;

        await responseWriter.WriteAsync(
            context,
            statusCode,
            HttpErrorCodes.FromStatusCode(statusCode),
            messageResolver.Resolve($"http-error.{statusCode}"),
            context.RequestAborted
        );
    }

    /// <summary>
    /// Determines whether the middleware should write a standardized error body.
    /// </summary>
    ///
    /// <param name="response">The response as the pipeline left it.</param>
    ///
    /// <returns>
    /// <c>true</c> for an error status that has not started and declares no content; otherwise <c>false</c>, so a
    /// handler that already wrote its own error body keeps it.
    /// </returns>
    ///
    /// <remarks>
    /// An unset <see cref="HttpResponse.ContentLength"/> counts as empty alongside zero, since nothing in the pipeline
    /// announced a body. A non-zero length means a body is on its way, and the response is left alone.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool ShouldWriteErrorResponse(HttpResponse response)
    {
        if (response.HasStarted || response.StatusCode < StatusCodes.Status400BadRequest)
            return false;

        return response.ContentLength is null or 0;
    }
}
