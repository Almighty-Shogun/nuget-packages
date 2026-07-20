using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Defines authenticated session operations.
/// </summary>
///
/// <typeparam name="TUser">The authentication user entity type.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthSessionService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Refreshes an existing authenticated session and rotates the refresh token.
    /// </summary>
    ///
    /// <param name="refreshToken">The refresh token for the session to refresh.</param>
    /// <param name="httpContext">The HTTP context used to resolve host and session details.</param>
    ///
    /// <returns>The refreshed session result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthSessionResult<TUser>> RefreshSessionAsync(
        string refreshToken,
        HttpContext httpContext);
}
