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
    public required string Issuer { get; init; }

    /// <summary>
    /// Gets the symmetric signing secret used to validate JWT signatures.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public required string Secret { get; init; }

    /// <summary>
    /// Gets the access token lifetime in hours.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public required int Hours { get; init; }

    /// <summary>
    /// Gets the refresh token lifetime in days.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
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
}
