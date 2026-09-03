namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// A minted access token and the moment it stops being accepted. Returned together because a caller almost always needs
/// both: one to send, the other to decide when to refresh.
/// </summary>
///
/// <param name="Token">The encoded JWT, ready to send as a bearer token without further processing.</param>
/// <param name="ExpiresAt">
/// The absolute expiry in UTC. Absolute rather than a duration, so a client that holds it does not have to remember when
/// it was issued.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record AuthToken(string Token, DateTimeOffset ExpiresAt);
