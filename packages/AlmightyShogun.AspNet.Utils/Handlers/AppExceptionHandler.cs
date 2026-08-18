using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Diagnostics;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Converts any <see cref="IAppException"/> from any package into the standardized error response, so a domain
/// exception reaches the client as the status code and error code it named rather than as a <c>500</c>.
/// </summary>
///
/// <remarks>
/// Registered first by <c>AddExceptionHandling</c>, ahead of the framework and fallback handlers, because those would
/// otherwise claim the exception before its status code is read. Declines anything that is not an
/// <see cref="IAppException"/>, and anything arriving once the response has started, so the next handler gets a turn.
/// </remarks>
///
/// <param name="messageResolver">
/// The resolver that turns <see cref="IAppException.MessageKey"/> into localized text, falling back to the key itself
/// when no message file defines it.
/// </param>
/// <param name="responseWriter">
/// The writer that produces the body, so the shape here matches every other error the application returns.
/// </param>
/// <param name="errorOptions">The settings deciding whether the handled error is logged, and from which status upwards.</param>
/// <param name="logger">The logger the handled error is written to, at a level chosen from the exception's status code.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class AppExceptionHandler(
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter,
    IOptions<HttpErrorSettings> errorOptions,
    ILogger<AppExceptionHandler> logger
) : IExceptionHandler
{
    /// <summary>
    /// The logging thresholds, read once at construction rather than through <see cref="IOptions{TOptions}"/> on every
    /// exception, since the handler outlives individual requests and the settings do not change while it runs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly HttpErrorSettings _settings = errorOptions.Value;

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not IAppException appException || httpContext.Response.HasStarted)
            return false;

        Log(httpContext, exception, appException);

        await responseWriter.WriteAsync(
            httpContext,
            appException.StatusCode,
            appException.Code,
            messageResolver.Resolve(appException.MessageKey, appException.MessageParameters),
            cancellationToken
        );

        return true;
    }

    /// <summary>
    /// Writes the handled error to the log, at warning for a client fault and at error for a server fault.
    /// </summary>
    ///
    /// <param name="httpContext">The context the method and path are read from for the log line.</param>
    /// <param name="exception">The exception itself, attached to the log entry only for a server fault.</param>
    /// <param name="appException">
    /// The same exception seen through its interface, supplying the status code and error code the log line reports.
    /// </param>
    ///
    /// <remarks>
    /// The exception itself is only attached to a server fault. A 4xx is the caller's mistake, so the stack trace
    /// describes where the application detected it rather than anything worth acting on.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void Log(HttpContext httpContext, Exception exception, IAppException appException)
    {
        if (!_settings.LogErrors || appException.StatusCode < _settings.MinimumLogStatusCode) return;

        if (appException.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Request {Method} {Path} failed with {StatusCode} {Code}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                appException.StatusCode,
                appException.Code
            );

            return;
        }

        logger.LogWarning(
            "Request {Method} {Path} was rejected with {StatusCode} {Code}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            appException.StatusCode,
            appException.Code
        );
    }
}
