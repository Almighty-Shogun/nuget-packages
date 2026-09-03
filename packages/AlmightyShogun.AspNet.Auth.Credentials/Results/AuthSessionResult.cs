namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Everything a caller needs after a successful sign-in, refresh, or registration: the token to send back, the token to
/// renew with, and the user the two belong to.
/// </summary>
///
/// <typeparam name="TUser">
/// The application's user entity, so a caller reads its own properties off <see cref="User"/> without casting.
/// </typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class AuthSessionResult<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Gets the signed access token to return to the client. Short-lived and not revocable once issued, so it is meant to
    /// be held in memory rather than stored.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Gets the refresh token in plain text, the only point at which it exists in that form. Only its hash is stored, so a
    /// caller that discards this cannot recover it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// Gets the user the tokens were issued for, already loaded, so a caller does not query again for the details it is
    /// about to return.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required TUser User { get; init; }
}
