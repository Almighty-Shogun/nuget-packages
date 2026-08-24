using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Writes the standardized <c>500</c> body for any exception the earlier handlers did not recognize. Registered last,
/// so it is the fallback rather than a competitor to the handlers that map a specific exception to a status code.
/// </summary>
///
/// <param name="messageResolver">The resolver that turns the <c>http-error.500</c> key into a localized description.</param>
/// <param name="responseWriter">
/// The writer that produces the body. Nothing from the exception reaches it, so an internal fault never leaks a message
/// or a stack trace to the caller.
/// </param>
///
/// <remarks>
/// The exception itself is not logged here. The framework's exception handler middleware already logs every unhandled
/// exception with its stack trace, so logging again would duplicate it.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class UnhandledExceptionHandler(
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter
) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
            return false;

        await responseWriter.WriteAsync(
            httpContext,
            StatusCodes.Status500InternalServerError,
            HttpErrorCodes.FromStatusCode(StatusCodes.Status500InternalServerError),
            messageResolver.Resolve($"http-error.{StatusCodes.Status500InternalServerError}"),
            cancellationToken
        );

        return true;
    }
}
