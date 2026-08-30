using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.JwtAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Signs users in and creates them. An unknown identifier and a wrong password are refused identically, and an unknown one
/// still pays a decoy verification so the timing does not separate them. A locked and a disabled account are each refused
/// distinctly, and a lockout is answered before the password is checked at all.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthUserService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthSettings> authOptions,
    IOptions<CredentialAuthSettings> credentialOptions,
    IAppHostResolver appHostResolver,
    IAuthSessionService<TUser> sessionService,
    IAuthTokenService<TUser> tokenService
) : AuthServiceBase<TUser>(databaseContext, authOptions, appHostResolver), IAuthUserService<TUser> where TUser : AuthUser
{
    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> LoginAsync(LoginRequest request, HttpContext context)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        string? app = ResolveApp();
        SessionContext sessionContext = context.GetSessionContext();

        TUser? user = await DatabaseContext.Users
            .FirstOrDefaultAsync(candidate => candidate.Username == request.Identifier || candidate.Email == request.Identifier);

        if (user is null)
        {
            VerifyDecoy(request.Password);

            throw new InvalidCredentialsException();
        }

        LockoutPolicy lockoutPolicy = credentialOptions.Value.Lockout;
        UserLockout? lockout = await EnsureNotLockedOutAsync(user.Id, lockoutPolicy);

        PasswordVerificationResult verification = Hasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (verification is PasswordVerificationResult.Failed)
        {
            await RecordFailedAttemptAsync(user, lockout, lockoutPolicy);

            throw new InvalidCredentialsException();
        }

        if (!user.IsActive)
        {
            throw new AccountDisabledException();
        }

        if (verification is PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.Password = Hasher.HashPassword(user, request.Password);
        }

        if (lockout is not null)
        {
            DatabaseContext.UserLockouts.Remove(lockout);
        }

        string refreshToken = await sessionService.CreateSessionAsync(user, app, sessionContext);

        AuthSessionResult<TUser> result = new()
        {
            User = user,
            RefreshToken = refreshToken,
            AccessToken = tokenService.GenerateToken(user, app)
        };

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return result;
    }

    /// <summary>
    /// Verifies the supplied password against a throwaway hash, so the no-user path costs the same as a wrong password.
    /// </summary>
    ///
    /// <param name="password">The submitted password, verified against a throwaway hash and then discarded.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void VerifyDecoy(string password)
        => AuthTimingDefence.SpendVerification(password);

    /// <summary>
    /// Counts a failed attempt and locks the account once the configured maximum is reached. Does nothing when lockout is
    /// disabled, so an application that does not want it pays no write.
    /// </summary>
    ///
    /// <param name="user">The user the attempt was made against, needed for its key when no row exists yet.</param>
    /// <param name="lockout">
    /// The row already loaded for this attempt, or <c>null</c> when the account has none. A new one is inserted in that
    /// case, so the first failure is what creates it.
    /// </param>
    /// <param name="policy">The configured policy, read for the failure limit and how long a lockout lasts.</param>
    ///
    /// <returns>A task that completes once the count, and any resulting lockout, have been saved.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task RecordFailedAttemptAsync(TUser user, UserLockout? lockout, LockoutPolicy policy)
    {
        if (!policy.Enabled)
        {
            return;
        }

        if (lockout is null)
        {
            lockout = new UserLockout { UserId = user.Id };

            await DatabaseContext.UserLockouts.AddAsync(lockout);
        }

        lockout.AccessFailedCount++;

        if (lockout.AccessFailedCount >= policy.MaxFailedAttempts)
        {
            lockout.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(policy.DurationMinutes);
            lockout.AccessFailedCount = 0;
        }

        await DatabaseContext.SaveChangesAsync();
    }

    /// <summary>
    /// Refuses a username or email address that another account already holds, so a duplicate is reported as a client
    /// mistake rather than surfacing as a unique-index violation from the database.
    /// </summary>
    ///
    /// <param name="username">The username the new account wants, compared under the database's own collation.</param>
    /// <param name="email">
    /// The address the new account wants. Compared under the database's collation, so a case-sensitive one lets two
    /// accounts differ only in casing.
    /// </param>
    ///
    /// <returns>A task that completes when both values are free; the caller may then insert without a further check.</returns>
    ///
    /// <exception cref="UsernameTakenException">Another account already uses that username.</exception>
    /// <exception cref="EmailTakenException">Another account already uses that email address.</exception>
    ///
    /// <remarks>
    /// A check here cannot make the write safe on its own, because another request may claim the same value in between.
    /// The unique indexes remain the guarantee; this only turns the common case into a useful message.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task EnsureCredentialsAvailableAsync(string username, string email)
    {
        if (await DatabaseContext.Users.AnyAsync(candidate => candidate.Username == username))
            throw new UsernameTakenException();

        if (await DatabaseContext.Users.AnyAsync(candidate => candidate.Email == email))
            throw new EmailTakenException();
    }

    /// <inheritdoc />
    public async Task<TUser> CreateUserAsync(TUser user, string password)
    {
        await EnsureCredentialsAvailableAsync(user.Username, user.Email);

        user.Password = Hasher.HashPassword(user, password);

        await DatabaseContext.Users.AddAsync(user);
        await DatabaseContext.SaveChangesAsync();

        return user;
    }

    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> RegisterAsync(TUser user, string password, HttpContext context)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        string? app = ResolveApp();
        SessionContext sessionContext = context.GetSessionContext();

        TUser createdUser = await CreateUserAsync(user, password);
        string refreshToken = await sessionService.CreateSessionAsync(createdUser, app, sessionContext);

        AuthSessionResult<TUser> result = new()
        {
            User = createdUser,
            RefreshToken = refreshToken,
            AccessToken = tokenService.GenerateToken(createdUser, app)
        };

        await transaction.CommitAsync();

        return result;
    }
}
