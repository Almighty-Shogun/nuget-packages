using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Sets the response <c>Content-Language</c> header to the resolved message language.
/// </summary>
///
/// <param name="next">The next middleware in the pipeline.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class ContentLanguageMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    /// <param name="messageResolver">The message resolver used to determine the response language.</param>
    ///
    /// <returns>A task representing the middleware execution.</returns>
    ///
    /// <remarks>
    /// An existing <c>Content-Language</c> header is preserved.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
