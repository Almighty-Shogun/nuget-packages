using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Maps common framework exceptions to their correct status code instead of letting them all become <c>500</c>: a
/// malformed request body becomes the <c>400</c> it is, and a client that hangs up mid-request is not a server fault.
/// </summary>
///
/// <param name="messageResolver">The resolver that turns the <c>http-error.{status}</c> key into a localized description.</param>
/// <param name="responseWriter">
/// The writer that produces the body, so a framework fault returns the same shape as an application one.
/// </param>
/// <param name="logger">
/// The logger a client-aborted request is recorded on, at information level. Nothing else is logged here; an exception
/// this handler declines falls through to the framework's own logging.
/// </param>
///
/// <remarks>
/// A client disconnect is answered with the non-standard <c>499</c> and no body, since there is no longer a client to
/// read one; the status exists only so the access log can tell an abort apart from a success. Registered between the
/// application and fallback handlers, and declines anything it has no specific mapping for.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class FrameworkExceptionHandler(
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter,
    ILogger<FrameworkExceptionHandler> logger
) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Request {Path} was aborted", httpContext.Request.Path);

            if (!httpContext.Response.HasStarted)
                httpContext.Response.StatusCode = 499;

            return true;
        }

        int? statusCode = exception switch
        {
            BadHttpRequestException badRequest => badRequest.StatusCode,
            _ => null
        };

        if (statusCode is null || httpContext.Response.HasStarted)
            return false;

        await responseWriter.WriteAsync(
            httpContext,
            statusCode.Value,
            HttpErrorCodes.FromStatusCode(statusCode.Value),
            messageResolver.Resolve($"http-error.{statusCode.Value}"),
            cancellationToken
        );

        return true;
    }
}
