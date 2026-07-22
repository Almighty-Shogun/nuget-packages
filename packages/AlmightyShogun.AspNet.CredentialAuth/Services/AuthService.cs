using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;
using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.JwtAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

    /// <summary>
    /// The authentication database context used by service operations.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private AuthDbContext<TUser> DatabaseContext => databaseContext;

    /// <summary>
    /// The current authentication settings resolved from options.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private AuthSettings AuthSettings => authOptions.Value;

    /// <summary>
    /// Gets a user by predicate and throws when no matching user exists.
    /// </summary>
    ///
    /// <param name="predicate">The user lookup predicate.</param>
    /// <param name="messageKey">The optional message key used when no matching user exists.</param>
    ///
    /// <returns>The resolved authentication user.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, string? messageKey = null)
    {
        TUser? user = await DatabaseContext.Users.FirstOrDefaultAsync(predicate);

        return user ?? throw new HttpErrorException(StatusCodes.Status401Unauthorized, messageKey);
    }

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
        => _hasher.VerifyHashedPassword(user, user.Password, password) is not PasswordVerificationResult.Failed;

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
