using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.JwtAuth.Configuration;

namespace AlmightyShogun.AspNet.JwtAuth;

public class AppHostResolver(IOptions<AuthSettings> authSettings) : IAppHostResolver
{
    private readonly AuthSettings _authSettings = authSettings.Value;
    private readonly Dictionary<string, string> _hosts = authSettings.Value.Hosts
        .ToDictionary(pair => pair.Key.ToLowerInvariant(), pair => pair.Value, StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
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
    public string ResolveAppFromHost(string? host)
    {
        return TryResolveAppFromHost(host, out string app) ? app : throw new UnauthorizedAccessException("Unknown application");
    }
}
