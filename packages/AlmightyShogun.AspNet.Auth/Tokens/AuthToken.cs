namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Represents a JWT access token and its expiration.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record AuthToken
{
    /// <summary>
    /// The encoded JWT access token.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Token { get; init; }

    /// <summary>
    /// The token expiration time in UTC.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required DateTimeOffset ExpiresAt { get; init; }
}
