using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Resolves configured application audience names from incoming request hosts.
/// </summary>
///
/// <param name="authSettings">The bound authentication settings that contain host mappings and the localhost fallback.</param>
/// <param name="httpContextAccessor">The HTTP context accessor used to inspect and cache request state.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class AppHostResolver(IOptions<AuthSettings> authSettings, IHttpContextAccessor httpContextAccessor) : IAppHostResolver
{
    private static readonly object _resolvedAppKey = new();
    private readonly AuthSettings _authSettings = authSettings.Value;

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
