using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using AlmightyShogun.AspNet.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Writes the standardized <c>500</c> body for any exception the earlier handlers did not recognize. Registered last,
/// so it is the fallback rather than a competitor to the handlers that map a specific exception to a status code.
/// </summary>
///
/// <param name="serviceScopeFactory">
/// The factory a scope is created from for each exception answered here, the scope the <see cref="IMessageResolver"/>
/// that turns the <c>http-error.500</c> key into a localized description is taken from. <c>AddExceptionHandler</c>
/// registers this handler as a singleton, so taking the factory rather than the resolver is what keeps a resolver
/// registered with a scoped lifetime resolvable, and takes it fresh for each exception rather than once for the
/// process.
/// </param>
/// <param name="responseWriter">
/// The writer that produces the body. Nothing from the exception reaches it, so an internal fault never leaks a message
/// or a stack trace to the caller.
/// </param>
///
/// <remarks>
/// The exception itself is not logged here, and claiming it stops the framework from logging it either: with no
/// <c>SuppressDiagnosticsCallback</c> configured, and <c>UseHttpErrorResponses</c> configures none,
/// <c>ExceptionHandlerMiddlewareImpl</c> suppresses its own unhandled-exception diagnostics for any exception an
/// <see cref="IExceptionHandler"/> claimed. This one claims everything that reaches it, so an application that needs
/// the stack trace recorded has to log it itself.
///
/// The one case it declines is a response that has already started, whose status and body can no longer be replaced.
/// <c>ExceptionHandlerMiddlewareImpl</c> rethrows that case before the chain runs at all, so the guard is not reached
/// under <c>UseHttpErrorResponses</c>.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class UnhandledExceptionHandler(
    IServiceScopeFactory serviceScopeFactory,
    IHttpErrorResponseWriter responseWriter
) : IExceptionHandler
{
    /// <inheritdoc />
    ///
    /// <remarks>
    /// Whatever creating the scope, resolving <see cref="IMessageResolver"/> out of it, or
    /// <see cref="IMessageResolver.Resolve(string)"/> itself throws is caught, and the message key is written as the
    /// description instead, which is what the resolver itself returns for a key it cannot resolve. This is the last
    /// handler, so an exception escaping here would replace the <c>500</c> with a second unhandled exception and the
    /// caller would receive no error response at all. A missing or unreadable message file, or no registered resolver at
    /// all, therefore costs the localized text, not the response.
    /// </remarks>
    ///
    /// <remarks>
    /// The scope is created and disposed here rather than reusing <c>HttpContext.RequestServices</c>, so the handler
    /// works whatever lifetime the resolver was registered with and needs no request scope of its own. Nothing taken
    /// from it outlives the call: only the resolved description, a string, is carried past the disposal, and a failure
    /// during that disposal is caught alongside the resolution and falls back to the key. The cost is that a consumer
    /// resolver depending on other per-request state gets a second, separately populated instance while an error is
    /// answered. Language negotiation is unaffected, because the default provider reads the request through
    /// <see cref="IHttpContextAccessor"/> on each call rather than from anything the scope holds.
    /// </remarks>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
            return false;

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
