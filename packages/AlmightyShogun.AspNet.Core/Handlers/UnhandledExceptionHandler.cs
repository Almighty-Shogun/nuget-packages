using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Handles otherwise unhandled exceptions with a standardized internal server error response.
/// </summary>
///
/// <param name="serviceScopeFactory"> The factory used to create scopes for resolving localized messages. </param>
/// <param name="responseWriter"> The writer used to produce HTTP error responses. </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class UnhandledExceptionHandler(
    IServiceScopeFactory serviceScopeFactory,
    IHttpErrorResponseWriter responseWriter,
    ILogger<UnhandledExceptionHandler> logger
) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
            return false;

        logger.LogError(exception, "Unhandled exception while processing request");
        
        string description;

        try
        {
            await using AsyncServiceScope scope = serviceScopeFactory.CreateAsyncScope();

            description = scope.ServiceProvider
                .GetRequiredService<IMessageResolver>()
                .Resolve($"http-error.{StatusCodes.Status500InternalServerError}");
        }
        catch (Exception)
        {
            description = $"http-error.{StatusCodes.Status500InternalServerError}";
        }

        await responseWriter.WriteAsync(
            httpContext,
            StatusCodes.Status500InternalServerError,
            HttpErrorCodes.FromStatusCode(StatusCodes.Status500InternalServerError),
            description,
            cancellationToken
        );

        return true;
    }
}
