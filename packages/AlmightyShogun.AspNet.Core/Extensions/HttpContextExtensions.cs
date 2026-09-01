using System.Net;
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Reads what the current request says about the client behind it: the address it connected from, the client it names
/// itself as, and the <see cref="ClientContext"/> those two are captured into. The address and the client are read from
/// the live request on every call; the client context is built once and kept in <see cref="HttpContext.Items"/> for the
/// rest of the request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.2</since>
public static class HttpContextExtensions
{
    /// <summary>
    /// The <see cref="HttpContext.Items"/> key the client context is cached under. A private instance rather than a
    /// string, so no other middleware or library can pick the same key by accident.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly object _clientContextKey = new();

    /// <summary>
    /// Provides the request-metadata helpers as extensions on the context.
    /// </summary>
    ///
    /// <param name="httpContext">
    /// The context of the request being served. Only <see cref="HttpContext.Items"/> is written to, where the client
    /// context is cached; the request and the response are untouched, so these are safe to call at any point in the
    /// pipeline, including after the response has started.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.2</since>
    extension(HttpContext httpContext)
    {
        /// <summary>
        /// Retrieves the current request's <see cref="ClientContext"/>, building it from the live request when nothing
        /// has been stored yet.
        /// </summary>
        ///
        /// <returns>
        /// The context a previous call or <see cref="SetClientContext"/> stored, otherwise one built from the
        /// connection address and the User-Agent header.
        /// </returns>
        ///
        /// <remarks>
        /// A built context is written back to <see cref="HttpContext.Items"/>, so only the first call in a request
        /// reads the connection and every later one returns that same instance. Call
        /// <see cref="SetClientContext"/> beforehand to pin the values, as a test does when there is no real connection
        /// behind them.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.2</since>
        public ClientContext GetClientContext()
        {
            if (httpContext.Items.TryGetValue(_clientContextKey, out object? value) && value is ClientContext clientContext)
                return clientContext;

            ClientContext created = new(
                httpContext.GetIpAddress(),
                httpContext.Request.Headers.UserAgent.ToString()
            );

            httpContext.Items[_clientContextKey] = created;

            return created;
        }

        /// <summary>
        /// Stores the client context for the current request, so every later <see cref="GetClientContext"/> returns it
        /// instead of reading the connection.
        /// </summary>
        ///
        /// <param name="clientContext">
        /// The context to pin. Replaces whatever a previous call stored, including a context
        /// <see cref="GetClientContext"/> built itself.
        /// </param>
        ///
        /// <remarks>
        /// For middleware that captures the values once per request, and for a test that needs fixed values with no
        /// real connection behind them. The key is private, so this is the only supported way to seed the entry.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public void SetClientContext(ClientContext clientContext) => httpContext.Items[_clientContextKey] = clientContext;

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
