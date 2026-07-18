using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Provides helpers for reading request-scoped metadata from an ASP.NET Core HTTP context.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.2</since>
public static class HttpContextExtensions
{
    /// <param name="httpContext">The <see cref="HttpContext"/> used to register the functionalities.</param>
    extension(HttpContext httpContext)
    {
        /// <summary>
        /// Retrieves the current request's <see cref="SessionContext"/> from <see cref="HttpContext.Items"/>.
        /// </summary>
        ///
        /// <returns>The stored <see cref="SessionContext"/>, or a new context created from the connection IP address and User-Agent header when none is stored.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.2</since>
        public SessionContext GetSessionContext()
        {
            if (!httpContext.Items.TryGetValue(SessionContext.ItemKey, out object? sessionContext))
            {
                return new SessionContext(
                    httpContext.Connection.RemoteIpAddress?.ToString(),
                    httpContext.Request.Headers.UserAgent.ToString()
                );
            }

            if (sessionContext is SessionContext typedSessionContext)
            {
                return typedSessionContext;
            }

            return new SessionContext(
                httpContext.Connection.RemoteIpAddress?.ToString(),
                httpContext.Request.Headers.UserAgent.ToString()
            );
        }

        /// <summary>
        /// Parses the current request's User-Agent header into a simplified <see cref="UserAgent"/> value.
        /// </summary>
        ///
        /// <returns>The parsed <see cref="UserAgent"/> for the current request.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public UserAgent GetUserAgent()
            => UserAgent.Parse(httpContext.Request.Headers.UserAgent.ToString());
    }
}
