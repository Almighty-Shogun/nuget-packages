using System.Data;
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
/// <typeparam name="TUser">The application's own user entity, both inserted on creation and returned on sign-in.</typeparam>
/// <param name="databaseContext">The application's context, so auth writes join whatever transaction it is in.</param>
/// <param name="credentialOptions">The bound credential settings, read for the lockout policy.</param>
/// <param name="appHostResolver">
/// The resolver deciding which application the current request belongs to, so a session and its token are scoped to the
/// host the user actually signed in through.
/// </param>
/// <param name="sessionService">The session service, which issues the refresh token a successful sign-in returns.</param>
/// <param name="tokenGenerator">
/// The JWT package's generator, which signs and stamps issuer, audience, and expiry over the claims built here.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthUserService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<CredentialAuthSettings> credentialOptions,
    IAppHostResolver appHostResolver,
    IAuthSessionService<TUser> sessionService,
    IAuthTokenGenerator tokenGenerator
) : IAuthUserService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The hasher used for every password read and write, so hashing and verification cannot end up using different
    /// parameters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly PasswordHasher<TUser> _hasher = new();

    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> LoginAsync(
        LoginRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        string? app = appHostResolver.Resolve();
        ClientContext clientContext = context.GetClientContext();

        TUser? user = await databaseContext.Users.FirstOrDefaultAsync(
            candidate => candidate.Username == request.Identifier || candidate.Email == request.Identifier, cancellationToken
        );

        if (user is null)
        {
            VerifyDecoy(request.Password);

            throw new InvalidCredentialsException();
        }

        LockoutPolicy lockoutPolicy = credentialOptions.Value.Lockout;

        await EnsureNotLockedOutAsync(user.Id, lockoutPolicy, cancellationToken);

        PasswordVerificationResult verification = _hasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (verification is PasswordVerificationResult.Failed)
        {
            await RecordFailedAttemptAsync(user.Id, lockoutPolicy, cancellationToken);

            throw new InvalidCredentialsException();
        }

        if (!user.IsActive)
        {
            throw new AccountDisabledException();
        }

        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        if (verification is PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.Password = _hasher.HashPassword(user, request.Password);
        }

        await ClearLockoutAsync(user.Id, cancellationToken);

        string refreshToken = await sessionService.CreateSessionAsync(user, app, clientContext, cancellationToken);

        AuthSessionResult<TUser> result = new()
        {
            User = user,
            RefreshToken = refreshToken,
            AccessToken = tokenGenerator.Generate(AuthClaimFactory.Create(user, app), app).Token
        };

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<TUser> CreateUserAsync(TUser user, string password, CancellationToken cancellationToken = default)
    {
        await EnsureCredentialsAvailableAsync(user.Username, user.Email, cancellationToken);

        user.Password = _hasher.HashPassword(user, password);

        await databaseContext.Users.AddAsync(user, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> RegisterAsync(
        TUser user,
        string password,
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        string? app = appHostResolver.Resolve();
        ClientContext clientContext = context.GetClientContext();

        TUser createdUser = await CreateUserAsync(user, password, cancellationToken);
        string refreshToken = await sessionService.CreateSessionAsync(createdUser, app, clientContext, cancellationToken);

        AuthSessionResult<TUser> result = new()
        {
            User = createdUser,
            RefreshToken = refreshToken,
            AccessToken = tokenGenerator.Generate(AuthClaimFactory.Create(createdUser, app), app).Token
        };

        await transaction.CommitAsync(cancellationToken);

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
    /// Refuses a sign-in while a lockout is in force. Does nothing at all when lockout is disabled, so a deployment that
    /// never uses it pays no query for the check.
    /// </summary>
    ///
    /// <param name="userId">The user being let in, given as the database key rather than the public identifier.</param>
    /// <param name="policy">The configured policy, read for whether the feature is on at all.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>A task that completes once the account is known not to be locked.</returns>
    ///
    /// <exception cref="AccountLockedException">
    /// A lockout is in force. Thrown before the password is checked, because an attempt made during a lockout must not
    /// be able to extend it or reveal whether the password was right.
    /// </exception>
    ///
    /// <remarks>
    /// The read is untracked deliberately. <see cref="ClearLockoutAsync"/> reads the same row again inside the sign-in
    /// transaction to see what it looks like by then, and a tracked instance from here would be handed back unchanged,
    /// hiding any lockout applied in between.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task EnsureNotLockedOutAsync(int userId, LockoutPolicy policy, CancellationToken cancellationToken)
    {
        if (!policy.Enabled)
            return;

        UserLockout? lockout = await databaseContext.UserLockouts
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.UserId == userId, cancellationToken);

        if (lockout is not null && lockout.IsLocked)
            throw new AccountLockedException(lockout.LockoutEnd!.Value);
    }

    /// <summary>
    /// Re-reads the lockout inside the sign-in transaction and clears the failure run behind a successful sign-in, giving
    /// up on the sign-in instead when a lockout landed after the check that let it through.
    /// </summary>
    ///
    /// <param name="userId">The user signing in, given as the database key rather than the public identifier.</param>
    /// <param name="cancellationToken">Cancels the read and the delete.</param>
    ///
    /// <returns>A task that completes once no unexpired lockout row stands for that user.</returns>
    ///
    /// <exception cref="AccountLockedException">
    /// A lockout was applied between the check at the top of the sign-in and this transaction, by failed attempts running
    /// concurrently with it. Refusing here rather than deleting the row is what stops a correct password wiping a lockout
    /// that had already taken effect.
    /// </exception>
    ///
    /// <remarks>
    /// The delete is guarded on the row still being unlocked rather than issued against the instance that was read, so a
    /// lockout applied in the gap between the two statements survives instead of being deleted by a stale decision. That
    /// gap cannot be closed from here: this sign-in commits without holding the row, so a lockout committed after the
    /// delete still stands for the next attempt rather than stopping this one.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task ClearLockoutAsync(int userId, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        UserLockout? lockout = await databaseContext.UserLockouts
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.UserId == userId, cancellationToken);

        if (lockout is null)
            return;

        if (lockout.IsLocked)
            throw new AccountLockedException(lockout.LockoutEnd!.Value);

        await databaseContext.UserLockouts
            .Where(candidate => candidate.Id == lockout.Id && (candidate.LockoutEnd == null || candidate.LockoutEnd <= now))
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <summary>
    /// Counts a failed attempt and locks the account once the configured maximum is reached. Does nothing when lockout is
    /// disabled, so an application that does not want it pays no write.
    /// </summary>
    ///
    /// <param name="userId">The user the attempt was made against, needed for its key when no row exists yet.</param>
    /// <param name="policy">The configured policy, read for the failure limit and how long a lockout lasts.</param>
    /// <param name="cancellationToken">Cancels the database work, rolling the count back with the transaction.</param>
    ///
    /// <returns>A task that completes once the count, and any resulting lockout, have been committed.</returns>
    ///
    /// <remarks>
    /// It owns its transaction rather than joining the caller's, because the caller is about to throw and a transaction
    /// abandoned that way would roll the count back, leaving a run of failures that never reaches the limit. The
    /// isolation is serializable because the read and the write are one decision: at read-committed, two failures can
    /// both find no row and both insert one, or both read the same count and each write the same increment.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task RecordFailedAttemptAsync(int userId, LockoutPolicy policy, CancellationToken cancellationToken)
    {
        if (!policy.Enabled)
        {
            return;
        }

        await using IDbContextTransaction transaction =
            await databaseContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        UserLockout? lockout = await databaseContext.UserLockouts
            .FirstOrDefaultAsync(candidate => candidate.UserId == userId, cancellationToken);

        if (lockout is null)
        {
            lockout = new UserLockout { UserId = userId, AccessFailedCount = 1 };

            await databaseContext.UserLockouts.AddAsync(lockout, cancellationToken);
        }
        else
        {
            lockout.AccessFailedCount++;
        }

        if (lockout.AccessFailedCount >= policy.MaxFailedAttempts)
        {
            lockout.AccessFailedCount = 0;
            lockout.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(policy.DurationMinutes);
        }

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
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
    /// <param name="cancellationToken">Cancels the lookups.</param>
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
    private async Task EnsureCredentialsAvailableAsync(string username, string email, CancellationToken cancellationToken)
    {
        if (await databaseContext.Users.AnyAsync(candidate => candidate.Username == username, cancellationToken))
            throw new UsernameTakenException();

        if (await databaseContext.Users.AnyAsync(candidate => candidate.Email == email, cancellationToken))
            throw new EmailTakenException();
    }
}
