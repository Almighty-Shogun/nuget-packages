using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Matches a request host against the configured mapping, falling back to the configured localhost app for loopback
/// hosts so a development machine resolves without being listed. A successful resolution is cached on the request, so
/// repeated calls while one request is served cost a single lookup.
/// </summary>
///
/// <param name="authSettings">The bound authentication settings that contain host mappings and the localhost fallback.</param>
/// <param name="httpContextAccessor">The HTTP context accessor used to inspect and cache request state.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class AppHostResolver(IOptions<AuthSettings> authSettings, IHttpContextAccessor httpContextAccessor) : IAppHostResolver
{
    /// <summary>
    /// The key the resolved app is stored under in <c>HttpContext.Items</c>. A private object rather than a string, so
    /// nothing outside this class can address the entry or collide with it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly object _resolvedAppKey = new();

    /// <summary>
    /// The bound settings, read once at construction. The resolver is a singleton taking <c>IOptions</c> rather than
    /// <c>IOptionsMonitor</c>, so the value it sees never changes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    private readonly AuthSettings _authSettings = authSettings.Value;

    /// <summary>
    /// The configured host mapping, copied into a case-insensitive dictionary. Hosts arrive from the request rather than
    /// from configuration, so their casing is not the application's to dictate.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    private readonly Dictionary<string, string> _hosts = authSettings.Value.Hosts
        .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public bool TryResolve(out string? app)
    {
        app = null;

        if (!_authSettings.IsScoped())
            return true;

        HttpContext? httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
            return false;

        if (httpContext.Items.TryGetValue(_resolvedAppKey, out object? existingApp))
        {
            app = existingApp as string;

            return app is not null;
        }

        if (!TryResolveAppFromHost(httpContext.Request.Host.Host, out string resolvedApp))
            return false;

        httpContext.Items[_resolvedAppKey] = resolvedApp;
        app = resolvedApp;

        return true;
    }

    /// <inheritdoc />
    public string? Resolve() => TryResolve(out string? app)
        ? app
        : throw new UnknownAppException(httpContextAccessor.HttpContext?.Request.Host.Host);

    /// <inheritdoc />
    public bool TryResolveAppFromHost(string? host, out string app)
    {
        app = string.Empty;

        if (string.IsNullOrWhiteSpace(host))
            return false;

        if (_hosts.TryGetValue(host, out string? mappedApp))
        {
            app = mappedApp;

            return true;
        }

        if (!IsLocalhost(host) || string.IsNullOrWhiteSpace(_authSettings.LocalhostApp))
            return false;

        app = _authSettings.LocalhostApp;

        return true;
    }

    /// <inheritdoc />
    public string ResolveAppFromHost(string? host) => TryResolveAppFromHost(host, out string app)
        ? app
        : throw new UnknownAppException(host);

    /// <summary>
    /// Determines whether a normalized host value represents localhost.
    /// </summary>
    ///
    /// <param name="host">The normalized host value to inspect.</param>
    ///
    /// <returns><c>true</c> when the host is localhost, loopback IPv4, or loopback IPv6; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsLocalhost(string host)
        => host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
           || (IPAddress.TryParse(host, out IPAddress? address) && IPAddress.IsLoopback(address));
}
