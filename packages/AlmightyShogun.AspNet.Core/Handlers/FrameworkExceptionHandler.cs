using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Maps the one framework exception this package handles to its correct status code instead of letting it become
/// <c>500</c>: a malformed request body becomes the <c>400</c> it is.
/// </summary>
///
/// <param name="messageResolver">The resolver that turns the <c>http-error.{status}</c> key into a localized description.</param>
/// <param name="responseWriter">
/// The writer that produces the body, so a framework fault returns the same shape as an application one.
/// </param>
/// <param name="logger">
/// The logger the client-abort branch records on, at information level. Nothing else is logged here, and that branch
/// is unreachable under the exception handler middleware, so in practice this handler logs nothing.
/// </param>
///
/// <remarks>
/// Only <see cref="BadHttpRequestException"/> is mapped, to the status code it carries. Anything else is declined and
/// the fallback handler answers it. Registered between the application and fallback handlers.
/// </remarks>
///
/// <remarks>
/// The client-abort branch is never entered under <c>UseExceptionHandler</c>. <c>ExceptionHandlerMiddlewareImpl</c>
/// returns before running any <see cref="IExceptionHandler"/> when the exception is an
/// <see cref="OperationCanceledException"/> or an <see cref="IOException"/> and <c>HttpContext.RequestAborted</c> is
/// cancelled, logging the abort and setting <c>499</c> itself. That condition covers this handler's own, so an abort
/// never reaches <c>TryHandleAsync</c>.
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
