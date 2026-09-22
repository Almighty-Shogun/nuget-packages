using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Handles framework exceptions that can be mapped to HTTP error responses.
/// </summary>
///
/// <param name="serviceScopeFactory"> The factory used to create scopes for resolving localized messages. </param>
/// <param name="responseWriter"> The writer used to produce HTTP error responses. </param>
/// <param name="logger">
/// The logger used for handled request cancellations.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class FrameworkExceptionHandler(
    IServiceScopeFactory serviceScopeFactory,
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
                logger.LogInformation("Request on route {Route} was aborted", ResolveRoute(httpContext));

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

        string description;
        var messageKey = $"http-error.{statusCode.Value}";

        try
        {
            await using AsyncServiceScope scope = serviceScopeFactory.CreateAsyncScope();
            description = scope.ServiceProvider
                .GetRequiredService<IMessageResolver>()
                .Resolve(messageKey);
        }
        catch (Exception)
        {
            description = messageKey;
        }
        
        await responseWriter.WriteAsync(
            httpContext,
            statusCode.Value,
            HttpErrorCodes.FromStatusCode(statusCode.Value),
            description,
            cancellationToken
        );

        return true;
    }

    /// <summary>
    /// Resolves the matched route pattern for logging.
    /// </summary>
    ///
    /// <param name="httpContext">The current HTTP context.</param>
    ///
    /// <returns>The route pattern, or <c>(unknown route)</c> when unavailable.</returns>
    ///
    /// <remarks>
    /// Uses <see cref="IExceptionHandlerFeature.Endpoint"/> because exception handling may clear the endpoint from the
    /// current <see cref="HttpContext"/>.
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
