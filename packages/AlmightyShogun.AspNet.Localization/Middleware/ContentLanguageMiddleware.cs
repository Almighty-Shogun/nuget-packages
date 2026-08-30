using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Reports which language the response was actually served in, by setting <c>Content-Language</c> from the language the
/// message resolver settled on rather than from what the request asked for.
/// </summary>
///
/// <param name="next">The rest of the pipeline, always invoked; this middleware never short-circuits a request.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ContentLanguageMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Registers the header callback, then hands the request on.
    /// </summary>
    ///
    /// <param name="context">The context the callback is registered on and whose response header is set.</param>
    /// <param name="messageResolver">
    /// The resolver asked which language was served. Middleware is constructed once, so taking it here rather than in
    /// the constructor is what keeps a replacement registered with a scoped lifetime resolvable.
    /// </param>
    ///
    /// <returns>A task that completes once the rest of the pipeline has finished.</returns>
    ///
    /// <remarks>
    /// The language is resolved in an <c>OnStarting</c> callback rather than up front, because the negotiated language
    /// is only settled once something has actually been resolved during the request. The callback runs as the response
    /// headers are about to be sent, so it sees whatever the pipeline set and leaves an existing header alone rather
    /// than defeating a deliberate <c>TrySetContentLanguage</c> call.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task InvokeAsync(HttpContext context, IMessageResolver messageResolver)
    {
        context.Response.OnStarting(static state =>
        {
            (HttpContext httpContext, IMessageResolver resolver) = ((HttpContext, IMessageResolver))state;

            if (httpContext.Response.GetContentLanguage() is null)
                httpContext.Response.TrySetContentLanguage(resolver.ResolveLanguage());

            return Task.CompletedTask;
        }, (context, messageResolver));

        await next(context);
    }
}
