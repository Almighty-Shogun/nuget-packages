namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// A minted access token and its expiry. Returned together because a caller almost always needs both: one to send, the
/// other to decide when to refresh.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record AuthToken
{
    /// <summary>
    /// The encoded JWT, ready to send as a bearer token without further processing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Token { get; init; }

    /// <summary>
    /// The absolute expiry in UTC. Absolute rather than a duration, so a client that holds it does not have to
    /// remember when it was issued. See <see cref="AuthSettings.ClockSkewSeconds"/> for the tolerance this package's own
    /// validation allows beyond it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required DateTimeOffset ExpiresAt { get; init; }
}
