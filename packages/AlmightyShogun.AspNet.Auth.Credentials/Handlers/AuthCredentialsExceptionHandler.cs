using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Handles credential authentication exceptions using standardized error responses.
/// </summary>
///
/// <param name="exceptionMapper">Maps authentication exceptions to error responses.</param>
/// <param name="messageResolver"> Resolves localized error messages. </param>
/// <param name="responseWriter"> Writes standardized HTTP error responses. </param>
/// <param name="logger">Logs handled exceptions.</param>
///
/// <remarks>
/// Register before the fallback exception handler.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class AuthCredentialsExceptionHandler(
    AuthCredentialsExceptionMapper exceptionMapper,
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter,
    ILogger<AuthCredentialsExceptionHandler> logger
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
    /// Logs handled exceptions according to their HTTP status code.
    /// </summary>
    ///
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The handled exception.</param>
    /// <param name="mapping">The mapped error details.</param>
    ///
    /// <remarks>
    /// Server errors include the exception; client errors are logged as warnings.
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
    /// Resolves the matched route pattern for logging.
    /// </summary>
    ///
    /// <param name="httpContext">The current HTTP context.</param>
    ///
    /// <returns>The route pattern, or "(unknown route)" if unavailable.</returns>
    ///
    /// <remarks>
    /// Uses the original endpoint from exception handler features when available.
    /// Logs route patterns instead of request paths to avoid exposing sensitive values.
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
