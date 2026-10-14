
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Resolves applications from request hosts using configured mappings and localhost fallback.
/// </summary>
/// <param name="authSettings">The authentication settings.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class AppHostResolver(IOptions<AuthSettings> authSettings, IHttpContextAccessor httpContextAccessor) : IAppHostResolver
{
    /// <summary>
    /// The key used to cache the resolved application in the HTTP context.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static readonly object _resolvedAppKey = new();

    /// <summary>
    /// The configured authentication settings.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    private readonly AuthSettings _authSettings = authSettings.Value;

    /// <summary>
    /// The case-insensitive host-to-application mappings.
    /// </summary>
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
    /// Checks whether the host is localhost or a loopback IP address.
    /// </summary>
    /// <param name="host">The host to check.</param>
    /// <returns>Whether the host represents a loopback address.</returns>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool IsLocalhost(string host)
        => host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
           || (IPAddress.TryParse(host, out IPAddress? address) && IPAddress.IsLoopback(address));
}
