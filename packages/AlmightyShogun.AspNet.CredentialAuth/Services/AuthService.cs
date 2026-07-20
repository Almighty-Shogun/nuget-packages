using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using AlmightyShogun.AspNet.JwtAuth;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides authentication, session, password, and token operations.
/// </summary>
///
/// <param name="databaseContext">The authentication database context.</param>
/// <param name="authOptions">The configured authentication options.</param>
/// <param name="appHostResolver">The app host resolver used to resolve the current JWT audience scope.</param>
///
/// <typeparam name="TUser">The authentication user entity type.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed partial class AuthService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthSettings> authOptions,
    IAppHostResolver appHostResolver) :
    IAuthUserService<TUser>,
    IAuthSessionService<TUser>,
    IAuthPasswordService,
    IAuthTokenService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The password hasher used for authentication users.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly PasswordHasher<TUser> _hasher = new();

    private AuthDbContext<TUser> DatabaseContext => databaseContext;

    private AuthSettings AuthSettings => authOptions.Value;

    /// <summary>
    /// Checks whether a password matches the supplied authentication user.
    /// </summary>
    ///
    /// <param name="user">The authentication user.</param>
    /// <param name="password">The plain-text password value.</param>
    ///
    /// <returns><c>true</c> when the password matches; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool PasswordMatches(TUser user, string password)
    {
        PasswordVerificationResult verificationResult = _hasher.VerifyHashedPassword(user, user.Password, password);

        return verificationResult is not PasswordVerificationResult.Failed;
    }

    /// <summary>
    /// Resolves the application scope for the current HTTP context.
    /// </summary>
    ///
    /// <returns>The resolved application scope, or <c>null</c> when host scoping is disabled.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string? ResolveApp() => appHostResolver.Resolve();
}
