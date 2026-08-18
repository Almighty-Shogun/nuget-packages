using System.Net;
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Reads the caller's identity off the current request: the address it connected from, the client it used, and the
/// session context those two are captured into. Every value comes from the live request, so none of it is cached.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.2</since>
public static class HttpContextExtensions
{
    /// <summary>
    /// Provides the request-metadata helpers as extensions on the context.
    /// </summary>
    ///
    /// <param name="httpContext">
    /// The context of the request being served. Nothing here mutates it, so the helpers are safe to call at any point
    /// in the pipeline, including after the response has started.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.2</since>
    extension(HttpContext httpContext)
    {
        /// <summary>
        /// Retrieves the current request's <see cref="SessionContext"/> from <see cref="HttpContext.Items"/>.
        /// </summary>
        ///
        /// <returns>
        /// The context stored by <c>AddSessionContextFilter</c>, or a freshly built one when the filter is not
        /// registered or the request never reached an action, as on a middleware short-circuit.
        /// </returns>
        ///
        /// <remarks>
        /// The fallback is not written back to <see cref="HttpContext.Items"/>, so without the filter every call parses
        /// the request again. Register the filter when more than one caller per request needs the context.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.2</since>
        public SessionContext GetSessionContext()
            => httpContext.Items.TryGetValue(SessionContext.ItemKey, out object? sessionContext)
               && sessionContext is SessionContext typedSessionContext
                ? typedSessionContext
                : httpContext.CreateSessionContext();

        /// <summary>
        /// Gets the client address for the current request, normalized so an IPv4 address tunneled as IPv4-mapped IPv6
        /// is returned in its IPv4 form.
        /// </summary>
        ///
        /// <returns>The client address, or <c>null</c> when the connection has none, as on an in-memory test server.</returns>
        ///
        /// <remarks>
        /// Read from the connection, never from a forwarded header, because a header-supplied address is chosen by the
        /// caller. Behind a proxy or CDN, call <c>UseForwardedHeaders</c> first so the connection address is the real
        /// client; see <c>AddCloudflareHeaders</c>.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string? GetIpAddress()
        {
            if (httpContext.Connection.RemoteIpAddress is not { } remoteAddress)
                return null;

            IPAddress ipAddress = remoteAddress.IsIPv4MappedToIPv6 ? remoteAddress.MapToIPv4() : remoteAddress;

            return ipAddress.ToString();
        }

        /// <summary>
        /// Captures the address and raw User-Agent header into a <see cref="SessionContext"/>, without parsing either.
        /// </summary>
        ///
        /// <returns>
        /// A context holding the current address and header value. Both may be blank, since a missing header stringifies
        /// to an empty value rather than to <c>null</c>.
        /// </returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        internal SessionContext CreateSessionContext() => new(
            httpContext.GetIpAddress(),
            httpContext.Request.Headers.UserAgent.ToString()
        );

        /// <summary>
        /// Parses the current request's User-Agent header into a simplified <see cref="UserAgent"/> value.
        /// </summary>
        ///
        /// <returns>
        /// The browser, operating system, and device the header names. An absent header yields <c>Unknown</c> for all
        /// three, and an unrecognized one yields <c>Other</c> for whichever part could not be matched.
        /// </returns>
        ///
        /// <remarks>
        /// Parsing runs a regular expression set per call and nothing is cached on the context, so a request that needs
        /// the value more than once should hold on to the result rather than calling again.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public UserAgent GetUserAgent() => UserAgent.Parse(httpContext.Request.Headers.UserAgent.ToString());
    }
}
