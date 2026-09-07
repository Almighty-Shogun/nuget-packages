using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Answers this package's exceptions with the standardized error response, using <see cref="JwtAuthExceptionMapper"/> to
/// decide the status, code, and message for each. Declines everything else, so the rest of the handler chain still gets
/// a turn.
/// </summary>
///
/// <param name="exceptionMapper">
/// The mapper naming the exceptions this package owns and the response each becomes. One it declines is left to the
/// handlers behind this one.
/// </param>
/// <param name="messageResolver">
/// The resolver that turns the mapped message key into localized text, falling back to the key itself when no message
/// file defines it.
/// </param>
/// <param name="responseWriter">
/// The writer that produces the body, so these failures match every other error the application returns.
/// </param>
/// <param name="logger">The logger the handled error is written to, at a level chosen from the mapped status code.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class JwtAuthExceptionHandler(
    JwtAuthExceptionMapper exceptionMapper,
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter,
    ILogger<JwtAuthExceptionHandler> logger
) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted || exceptionMapper.Map(exception) is not { } mapping)
            return false;

        Log(httpContext, exception, mapping);

        await responseWriter.WriteAsync(
            httpContext,
            mapping.StatusCode,
            mapping.Code,
            messageResolver.Resolve(mapping.MessageKey, mapping.MessageParameters),
            cancellationToken
        );

        return true;
    }

    /// <summary>
    /// Writes the handled error to the log, at warning for a client fault and at error for a server fault.
    /// </summary>
    ///
    /// <param name="httpContext">The failing request, read for the method and the matched route the log line reports.</param>
    /// <param name="exception">
    /// The exception being reported. Attached to the log entry only for a server fault, since a client mistake needs no
    /// stack trace.
    /// </param>
    /// <param name="mapping">The mapping chosen for it, supplying the status code and error code the log line reports.</param>
    ///
    /// <remarks>
    /// The exception itself is only attached to a server fault. A 4xx is the caller's mistake, so the stack trace
    /// describes where the application detected it rather than anything worth acting on.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void Log(HttpContext httpContext, Exception exception, ErrorMapping mapping)
    {
        if (mapping.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Request {Method} on route {Route} failed with {StatusCode} {Code}",
                httpContext.Request.Method,
                ResolveRoute(httpContext),
                mapping.StatusCode,
                mapping.Code
            );

            return;
        }

        logger.LogWarning(
            "Request {Method} on route {Route} was rejected with {StatusCode} {Code}",
            httpContext.Request.Method,
            ResolveRoute(httpContext),
            mapping.StatusCode,
            mapping.Code
        );
    }

    /// <summary>
    /// Resolves the route pattern the failing request matched, so a log line can name the route without carrying the
    /// request path.
    /// </summary>
    ///
    /// <param name="httpContext">The failing request, read for the endpoint it matched before the failure.</param>
    ///
    /// <returns>The matched route pattern's raw text, or <c>(unknown route)</c> when no pattern is available.</returns>
    ///
    /// <remarks>
    /// <see cref="EndpointHttpContextExtensions.GetEndpoint(HttpContext)"/> alone does not work here. Under
    /// <c>UseExceptionHandler</c>, <c>ExceptionHandlerMiddlewareImpl</c> clears the endpoint and the route values before
    /// it runs any <see cref="IExceptionHandler"/>, so the context reports no endpoint at that point. The original one
    /// survives on <see cref="IExceptionHandlerFeature.Endpoint"/>, which is read first, leaving the context's own
    /// endpoint as the fallback for a direct call made outside that middleware.
    /// </remarks>
    ///
    /// <remarks>
    /// Only a <see cref="RouteEndpoint"/> carries a pattern, and <see cref="RouteEndpoint.RoutePattern"/> may have no raw
    /// text. Both cases return the placeholder rather than the path, which can hold a single-use token a consumer placed
    /// in a route segment.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ResolveRoute(HttpContext httpContext)
    {
        Endpoint? endpoint = httpContext.Features.Get<IExceptionHandlerFeature>()?.Endpoint ?? httpContext.GetEndpoint();

        return endpoint is RouteEndpoint { RoutePattern.RawText: { } rawText } ? rawText : "(unknown route)";
    }
}
