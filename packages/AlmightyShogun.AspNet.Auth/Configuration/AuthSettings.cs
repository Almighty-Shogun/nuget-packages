
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Configures JWT authentication, token lifetimes, and application audiences.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public sealed record AuthSettings
{
    /// <summary>
    /// The issuer used to create and validate JWTs.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Required]
    public required string Issuer { get; init; }

    /// <summary>
    /// The symmetric JWT signing secret, requiring at least 32 characters.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Required]
    [MinLength(AuthSigningKey.MinimumSecretBytes)]
    public required string Secret { get; init; }

    /// <summary>
    /// The access token lifetime in minutes (1–1440).
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Range(1, 1440)]
    public int AccessTokenMinutes { get; init; } = 60;

    /// <summary>
    /// The refresh token lifetime in days (1–365).
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Range(1, 365)]
    public int RefreshTokenDays { get; init; } = 30;

    /// <summary>
    /// The JWT validation clock tolerance in seconds (0–300).
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(0, 300)]
    public int ClockSkewSeconds { get; init; } = 30;

    /// <summary>
    /// The default application audience, required when no hosts are configured.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? DefaultApp { get; init; }

    /// <summary>
    /// The SameSite mode for refresh token cookies.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public SameSiteMode SameSite { get; init; } = SameSiteMode.Lax;

    /// <summary>
    /// The application audience used for unmatched localhost requests.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public string? LocalhostApp { get; init; }

    /// <summary>
    /// Maps request hosts to application audiences.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public IReadOnlyDictionary<string, string> Hosts { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets the distinct configured application audiences.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a host has no audience or no default audience is configured without host mappings.
    /// </exception>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> ValidAudiences => BuildValidAudiences();

    /// <summary>
    /// Determines whether host-based application scoping is enabled.
    /// </summary>
    /// <returns>Whether any host mappings are configured.</returns>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool IsScoped() => Hosts.Count > 0;

    /// <summary>
    /// Builds and validates the distinct configured application audiences.
    /// </summary>
    /// <returns>The configured audiences, compared case-sensitively.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a host has no audience or no default audience is configured without host mappings.
    /// </exception>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private IReadOnlyList<string> BuildValidAudiences()
    {
        foreach (KeyValuePair<string, string> host in Hosts.Where(host => string.IsNullOrWhiteSpace(host.Value)))
            throw new InvalidOperationException($"Auth:Hosts entry '{host.Key}' has no audience value.");

        if (!IsScoped() && string.IsNullOrWhiteSpace(DefaultApp))
            throw new InvalidOperationException(
                "Auth:DefaultApp must be set when Auth:Hosts is empty, because every token carries an audience and it is always validated."
            );

        HashSet<string> audiences = new(StringComparer.Ordinal);

        if (!string.IsNullOrWhiteSpace(DefaultApp))
            audiences.Add(DefaultApp);

        if (!string.IsNullOrWhiteSpace(LocalhostApp))
            audiences.Add(LocalhostApp);

        foreach (string audience in Hosts.Values.Where(audience => !string.IsNullOrWhiteSpace(audience)))
            audiences.Add(audience);

        return [.. audiences];
    }
}
