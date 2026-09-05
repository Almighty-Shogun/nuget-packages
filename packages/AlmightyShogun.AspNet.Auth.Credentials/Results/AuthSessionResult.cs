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
/// <since>4.0.0</since>
public sealed class AuthSessionResult<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The signed access token to return to the client, ready to send as a bearer token. Its lifetime, issuer, and
    /// audience come from <see cref="AlmightyShogun.AspNet.Auth.AuthSettings"/>, which is where its validation lives too.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string AccessToken { get; init; }

    /// <summary>
    /// The refresh token in plain text, which the client presents to renew the session. This is the only place it
    /// appears in that form; see <see cref="UserSession.RefreshTokenHash"/> for what the row holds instead.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// The user the tokens were issued for, already loaded, so a caller does not query again for the details it is
    /// about to return.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required TUser User { get; init; }
}
