
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Handles authentication exceptions using standardized error responses.
/// </summary>
/// <param name="exceptionMapper">The authentication exception mapper.</param>
/// <param name="messageResolver">The localized message resolver.</param>
/// <param name="responseWriter">The HTTP error response writer.</param>
/// <param name="logger">The exception logger.</param>
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
    /// Logs handled exceptions according to their HTTP status codes.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The handled exception.</param>
    /// <param name="mapping">The mapped error details.</param>
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
    /// Resolves the original request's route pattern for logging.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The route pattern or a placeholder if unavailable.</returns>
    /// <remarks>
    /// Uses the exception handler feature because exception middleware may clear the current endpoint.
    /// </remarks>
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ResolveRoute(HttpContext httpContext)
    {
        Endpoint? endpoint = httpContext.Features.Get<IExceptionHandlerFeature>()?.Endpoint ?? httpContext.GetEndpoint();

        return endpoint is RouteEndpoint { RoutePattern.RawText: { } rawText } ? rawText : "(unknown route)";
    }
}
