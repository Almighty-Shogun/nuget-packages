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
    /// Gets the encoded JWT, ready to send as a bearer token without further processing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Token { get; init; }

    /// <summary>
    /// Gets the absolute expiry in UTC. Absolute rather than a duration, so a client that holds it does not have to
    /// remember when it was issued. Validation allows <c>Auth:ClockSkewSeconds</c> beyond this, so the token is still
    /// accepted for that long after it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required DateTimeOffset ExpiresAt { get; init; }
}
