namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Contains the authentication tokens and user returned after a successful session operation.
/// </summary>
///
/// <typeparam name="TUser"> The application's user entity. </typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class AuthSessionResult<TUser> where TUser : AuthUser
{
    /// <summary>
    ///  The JWT access token issued to the client.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string AccessToken { get; init; }

    /// <summary>
    /// The refresh token used to renew the session.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// The user associated with the issued tokens.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required TUser User { get; init; }
}
