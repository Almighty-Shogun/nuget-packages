using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Represents the authentication configuration used by ASP.NET JWT Auth.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public sealed record AuthSettings
{
    /// <summary>
    /// Gets the expected issuer value for incoming JWTs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Required]
    public required string Issuer { get; init; }

    /// <summary>
    /// Gets the symmetric signing secret used to validate JWT signatures.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Required]
    public required string Secret { get; init; }

    /// <summary>
    /// Gets the access token lifetime in hours.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Range(1, int.MaxValue)]
    public required int Hours { get; init; }

    /// <summary>
    /// Gets the refresh token lifetime in days.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Range(1, int.MaxValue)]
    public required int RefreshTokenDays { get; init; }

    /// <summary>
    /// Gets the application audience name used when requests arrive from plain localhost in development.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public string? LocalhostApp { get; init; }

    /// <summary>
    /// Gets the host-to-application mapping used for request host based audience validation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public Dictionary<string, string> Hosts { get; init; } = [];

    /// <summary>
    /// Gets the normalized audience values configured for localhost and host mappings.
    /// </summary>
    ///
    /// <remarks>
    /// The value is used by JWT bearer validation when host-based app scoping is enabled.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal IReadOnlyList<string> ValidAudiences
    {
        get
        {
            HashSet<string> audiences = new(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(LocalhostApp))
                audiences.Add(LocalhostApp);

            foreach (string audience in Hosts.Values.Where(audience => !string.IsNullOrWhiteSpace(audience)))
            {
                audiences.Add(audience);
            }

            return [.. audiences];
        }
    }

    /// <summary>
    /// Determines whether host-based app scoping is active for authentication and authorization.
    /// </summary>
    ///
    /// <returns><c>true</c> when host mappings are configured; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal bool IsScoped() => Hosts.Count > 0;
}
