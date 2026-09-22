using System.Net;
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Provides client information for an HTTP request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.2</since>
public static class HttpContextExtensions
{
    private static readonly object _clientContextKey = new();

    /// <param name="httpContext">
    /// The HTTP context.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.2</since>
    extension(HttpContext httpContext)
    {
        /// <summary>
        /// Gets the client context for the current request.
        /// </summary>
        ///
        /// <returns>The cached or newly created client context.</returns>
        ///
        /// <remarks>
        /// The context is created on first access and cached in <see cref="HttpContext.Items"/> for the request.
        /// </remarks>
        /// 
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.2</since>
        public ClientContext GetClientContext()
        {
            if (httpContext.Items.TryGetValue(_clientContextKey, out object? value) && value is ClientContext clientContext)
                return clientContext;

            ClientContext created = new()
            {
                IpAddress = httpContext.GetIpAddress(),
                UserAgent = httpContext.Request.Headers.UserAgent.ToString()
            };

            httpContext.Items[_clientContextKey] = created;

            return created;
        }

        /// <summary>
        /// Sets the client context for the current request.
        /// </summary>
        ///
        /// <param name="clientContext">The client context to store.</param>
        ///
        /// <remarks>Replaces any context previously stored for the request.</remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public void SetClientContext(ClientContext clientContext) => httpContext.Items[_clientContextKey] = clientContext;

        /// <summary>
        /// Gets the client IP address for the current request.
        /// </summary>
        ///
        /// <returns>The client IP address, or <c>null</c> when unavailable.</returns>
        ///
        /// <remarks>
        /// IPv4-mapped IPv6 addresses are returned in IPv4 form.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public string? GetIpAddress()
        {
            if (httpContext.Connection.RemoteIpAddress is not { } remoteAddress)
                return null;

            IPAddress ipAddress = remoteAddress.IsIPv4MappedToIPv6 ? remoteAddress.MapToIPv4() : remoteAddress;

            return ipAddress.ToString();
        }

        /// <summary>
        /// Parses the current request's User-Agent header.
        /// </summary>
        ///
        /// <returns>The parsed User-Agent information.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public UserAgent GetUserAgent() => UserAgent.Parse(httpContext.Request.Headers.UserAgent.ToString());
    }
}
