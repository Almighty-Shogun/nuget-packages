namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Builds the access token a signed-in user carries. Separate from the JWT package's own generator because the claim set
/// is this package's concern, while signing is not.
/// </summary>
///
/// <typeparam name="TUser">
/// The application's own user entity, read for the claims a token carries. Contravariant, so a service typed to a base
/// user still accepts a derived one.
/// </typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthTokenService<in TUser> where TUser : AuthUser
{
    /// <summary>
    /// Builds the claim set for a user and returns a signed token carrying it, including the role and the permissions the
    /// user holds for the resolved application.
    /// </summary>
    ///
    /// <param name="user">
    /// The user the token speaks for, read for its public identifier, username, role, and permission list.
    /// </param>
    /// <param name="app">The application scope used to filter permissions and set the token audience, if available.</param>
    ///
    /// <returns>The signed token, ready to be returned to the client; the refresh token is issued separately.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// No audience could be resolved: <paramref name="app"/> was omitted, the request host matches no configured one,
    /// and no default application is set. Pass an explicit value, or configure one, rather than catching this.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string GenerateToken(TUser user, string? app = null);
}
