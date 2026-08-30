using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.JwtAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// The state and lookups every credential service needs: the database, the hasher, the bound settings, and the current
/// application scope. Shared through a base rather than injected four times into each service.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, so a derived service reads and writes its columns.</typeparam>
/// <param name="databaseContext">The application's context, so auth writes join whatever transaction it is in.</param>
/// <param name="authOptions">The bound JWT settings, read for token and session lifetimes.</param>
/// <param name="appHostResolver">
/// The resolver deciding which application the current request belongs to, so sessions and tokens are scoped to it.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal abstract class AuthServiceBase<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthSettings> authOptions,
    IAppHostResolver appHostResolver
) where TUser : AuthUser
{
    /// <summary>
    /// The hasher used for every password read and write, so hashing and verification cannot end up using different
    /// parameters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected readonly PasswordHasher<TUser> Hasher = new();

    /// <summary>
    /// The context every service operation reads and writes through.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected AuthDbContext<TUser> DatabaseContext => databaseContext;

    /// <summary>
    /// The bound JWT settings, read through <see cref="IOptions{TOptions}"/> on each access. That returns the instance
    /// bound once for the process, so the value is stable without this property caching anything itself.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected AuthSettings AuthSettings => authOptions.Value;

    /// <summary>
    /// Loads the one user matching a predicate, refusing rather than returning null, so every caller past this point has a
    /// user to work with.
    /// </summary>
    ///
    /// <param name="predicate">The lookup, normally by public identifier or by username and email together.</param>
    ///
    /// <returns>The matching user, tracked so a caller can modify and save it.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">Thrown when no user matches the predicate.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate)
    {
        TUser? user = await DatabaseContext.Users.FirstOrDefaultAsync(predicate);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Resolves which application the current request belongs to, so a session and its token are scoped to the host the
    /// user actually signed in through.
    /// </summary>
    ///
    /// <returns>The application audience, or <c>null</c> when the deployment is not app-scoped.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected string? ResolveApp() => appHostResolver.Resolve();

    /// <summary>
    /// Loads the lockout row for a user and refuses when it is in force. Does nothing at all when lockout is disabled,
    /// so a deployment that never uses it pays no query for the check.
    /// </summary>
    ///
    /// <param name="userId">The user being let in, given as the database key rather than the public identifier.</param>
    /// <param name="policy">The configured policy, read for whether the feature is on at all.</param>
    ///
    /// <returns>The lockout row, or <c>null</c> when the feature is off or the account has nothing against it.</returns>
    ///
    /// <exception cref="AccountLockedException">
    /// A lockout is in force. Thrown before the password is checked, because an attempt made during a lockout must not
    /// be able to extend it or reveal whether the password was right.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected async Task<UserLockout?> EnsureNotLockedOutAsync(int userId, LockoutPolicy policy)
    {
        if (!policy.Enabled)
            return null;

        UserLockout? lockout = await DatabaseContext.UserLockouts
            .FirstOrDefaultAsync(candidate => candidate.UserId == userId);

        if (lockout is not null && lockout.IsLocked)
            throw new AccountLockedException(lockout.LockoutEnd!.Value);

        return lockout;
    }
}
