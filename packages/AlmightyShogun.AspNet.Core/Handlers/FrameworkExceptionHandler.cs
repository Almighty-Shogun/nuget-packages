using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
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
/// <since>4.0.0</since>
internal sealed class FrameworkExceptionHandler(
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter,
    ILogger<FrameworkExceptionHandler> logger
) : IExceptionHandler
{
    /// <inheritdoc />
    ///
    /// <exception cref="DirectoryNotFoundException">
    /// A message directory was removed between being found and being enumerated while the description was resolved.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// A message directory became unreadable between being found and being enumerated while the description was
    /// resolved.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// A consumer-supplied <see cref="ILanguageProvider"/> returned <c>null</c> from
    /// <see cref="ILanguageProvider.GetLanguages"/>.
    /// </exception>
    ///
    /// <remarks>
    /// Nothing here catches what <see cref="IMessageResolver.Resolve(string)"/> throws, so a resolver failure escapes
    /// into the middleware while the error body is being built, in place of the response this would have written. Only
    /// the <see cref="BadHttpRequestException"/> path resolves at all; every other exception is declined first.
    /// </remarks>
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

        await responseWriter.WriteAsync(
            httpContext,
            statusCode.Value,
            HttpErrorCodes.FromStatusCode(statusCode.Value),
            messageResolver.Resolve($"http-error.{statusCode.Value}"),
            cancellationToken
        );

        return true;
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
