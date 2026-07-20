using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Defines login, user creation, and registration operations.
/// </summary>
///
/// <typeparam name="TUser">The authentication user entity type.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthUserService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Authenticates a user by username or email and creates a new authenticated session.
    /// </summary>
    ///
    /// <param name="request">The login request containing the username or email and password.</param>
    /// <param name="context">The HTTP context used to resolve host and session details.</param>
    ///
    /// <returns>The authenticated session result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthSessionResult<TUser>> LoginAsync(
        LoginRequest request,
        HttpContext context);

    /// <summary>
    /// Creates a new user without creating an authenticated session.
    /// </summary>
    ///
    /// <param name="user">The user entity to create.</param>
    /// <param name="password">The plain-text password to hash before saving the user.</param>
    ///
    /// <returns>The created user.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<TUser> CreateUserAsync(TUser user, string password);

    /// <summary>
    /// Creates a new user and immediately creates an authenticated session for that user.
    /// </summary>
    ///
    /// <param name="user">The user entity to register.</param>
    /// <param name="password">The plain-text password to hash before saving the user.</param>
    /// <param name="context">The HTTP context used to resolve host and session details.</param>
    ///
    /// <returns>The authenticated session result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthSessionResult<TUser>> RegisterAsync(
        TUser user,
        string password,
        HttpContext context);
}
