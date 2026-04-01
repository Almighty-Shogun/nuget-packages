namespace AlmightyShogun.AspNet.JwtAuth.Configuration;

public sealed record AuthSettings
{
    /// <summary>
    /// Gets the expected JWT issuer.
    /// </summary>
    public required string Issuer { get; init; }

    /// <summary>
    /// Gets the symmetric signing secret used to validate and issue JWTs.
    /// </summary>
    public required string Secret { get; init; }

    /// <summary>
    /// Gets the access token lifetime in hours.
    /// </summary>
    public required int Hours { get; init; }

    /// <summary>
    /// Gets the refresh token lifetime in days.
    /// </summary>
    public required int RefreshTokenDays { get; init; }

    /// <summary>
    /// Gets the application name to use when requests arrive on plain localhost in development.
    /// </summary>
    public string? LocalhostApp { get; init; }
    
    /// <summary>
    /// Gets the host-to-application mapping used for app-scoped audience validation.
    /// </summary>
    public Dictionary<string, string> Hosts { get; init; } = [];
}
