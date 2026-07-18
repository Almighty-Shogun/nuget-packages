using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Resolves configured application audience names from incoming request hosts.
/// </summary>
///
/// <param name="authSettings">The bound authentication settings that contain host mappings and the localhost fallback.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class AppHostResolver(IOptions<AuthSettings> authSettings) : IAppHostResolver
{
    /// <summary>
    /// Stores the current authentication settings snapshot used by the resolver.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    private readonly AuthSettings _authSettings = authSettings.Value;

    /// <summary>
    /// Stores normalized host-to-application mappings for case-insensitive host lookup.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    private readonly Dictionary<string, string> _hosts = authSettings.Value.Hosts
        .ToDictionary(pair => pair.Key.ToLowerInvariant(), pair => pair.Value, StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public bool TryResolveAppFromHost(string? host, out string app)
    {
        app = string.Empty;

        if (string.IsNullOrWhiteSpace(host))
        {
            return false;
        }

        string normalizedHost = host.ToLowerInvariant();

        if (_hosts.TryGetValue(normalizedHost, out app!))
        {
            return true;
        }

        if (!string.Equals(normalizedHost, "localhost", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(_authSettings.LocalhostApp))
            return false;

        app = _authSettings.LocalhostApp;

        return true;
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public string ResolveAppFromHost(string? host)
    {
        return TryResolveAppFromHost(host, out string app) ? app : throw new UnauthorizedAccessException("Unknown application");
    }
}
