namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Defines access token generation operations.
/// </summary>
///
/// <typeparam name="TUser">The authentication user entity type.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthTokenService<in TUser> where TUser : AuthUser
{
    /// <summary>
    /// Generates a signed access token for a user.
    /// </summary>
    ///
    /// <param name="user">The user for whom the access token should be generated.</param>
    /// <param name="app">The application scope used to filter permissions and set the token audience, if available.</param>
    ///
    /// <returns>The serialized access token.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string GenerateToken(TUser user, string? app = null);
}
