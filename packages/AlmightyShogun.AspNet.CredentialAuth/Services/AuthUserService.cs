using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.Auth;
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

        await ReserveAttemptAsync(user.Id, lockoutPolicy, cancellationToken);

        PasswordVerificationResult verification = _hasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (verification is PasswordVerificationResult.Failed)
        {
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
    /// Claims one attempt against the user's failure budget before any password is verified, and refuses the sign-in
    /// when the budget is gone. Does nothing when lockout is disabled, so an application that does not want it pays no
    /// write.
    /// </summary>
    ///
    /// <param name="userId">The user signing in, given as the database key rather than the public identifier.</param>
    /// <param name="policy">The configured policy, read for the failure limit and how long a lockout lasts.</param>
    /// <param name="cancellationToken">Cancels the database work.</param>
    ///
    /// <returns>A task that completes once one attempt has been claimed for this caller.</returns>
    ///
    /// <exception cref="AccountLockedException">
    /// A lockout is in force for this user, or the budget was exhausted by attempts that claimed before this one.
    /// </exception>
    ///
    /// <remarks>
    /// Counting before the verification rather than after it is what bounds a burst. Verification is deliberately slow,
    /// so a check made before it and a count made after it leave a window as wide as the hash: every request arriving
    /// inside it reads the same unspent budget and is allowed to guess, and the limit then bounds attempts made one
    /// after another while doing nothing about attempts made at once.
    ///
    /// The claim is one guarded statement rather than a read followed by a write, so the database decides who gets each
    /// attempt and no isolation level has to be raised to make that safe. Two callers racing for the last attempt both
    /// issue the same update; exactly one reports a row, and the other is refused. The row's unique key on the user
    /// settles the same race for the first attempt of a run, where there is no row to update yet.
    ///
    /// A successful sign-in deletes the row through <see cref="ClearLockoutAsync"/> , so the attempt claimed here costs
    /// the caller nothing once the password proves correct.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task ReserveAttemptAsync(int userId, LockoutPolicy policy, CancellationToken cancellationToken)
    {
        if (!policy.Enabled)
            return;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        DateTimeOffset lockoutEnd = now.AddMinutes(policy.DurationMinutes);
        int maximum = policy.MaxFailedAttempts;

        await databaseContext.UserLockouts
            .Where(candidate => candidate.UserId == userId && candidate.LockoutEnd != null && candidate.LockoutEnd <= now)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(candidate => candidate.AccessFailedCount, 0)
                    .SetProperty(candidate => candidate.LockoutEnd, (DateTimeOffset?)null),
                cancellationToken
            );

        if (await TryClaimAttemptAsync(userId, maximum, lockoutEnd, cancellationToken))
            return;

        if (await TryClaimFirstAttemptAsync(userId, maximum, lockoutEnd, cancellationToken))
            return;

        if (await TryClaimAttemptAsync(userId, maximum, lockoutEnd, cancellationToken))
            return;

        throw new AccountLockedException(await ResolveLockoutEndAsync(userId, lockoutEnd, cancellationToken));
    }

    /// <summary>
    /// Claims an attempt against an existing row, locking the account when the claim spends the last one.
    /// </summary>
    ///
    /// <param name="userId">The user signing in, given as the database key rather than the public identifier.</param>
    /// <param name="maximum">The failure limit the budget is measured against.</param>
    /// <param name="lockoutEnd">When a lockout started by this claim would run out.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// <c>true</c> when this caller took the attempt; <c>false</c> when no row matched, which means either that no row
    /// exists yet or that the budget was already gone.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<bool> TryClaimAttemptAsync(
        int userId,
        int maximum,
        DateTimeOffset lockoutEnd,
        CancellationToken cancellationToken
    )
    {
        int claimed = await databaseContext.UserLockouts
            .Where(candidate => candidate.UserId == userId
                                && candidate.LockoutEnd == null
                                && candidate.AccessFailedCount < maximum)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(candidate => candidate.AccessFailedCount, candidate => candidate.AccessFailedCount + 1)
                    .SetProperty(
                        candidate => candidate.LockoutEnd,
                        candidate => candidate.AccessFailedCount + 1 >= maximum ? lockoutEnd : null
                    ),
                cancellationToken
            );

        return claimed == 1;
    }

    /// <summary>
    /// Opens a failure run by inserting the row, for the attempt that finds none.
    /// </summary>
    ///
    /// <param name="userId">The user signing in, given as the database key rather than the public identifier.</param>
    /// <param name="maximum">The failure limit, which this attempt reaches on its own when the limit is one.</param>
    /// <param name="lockoutEnd">When a lockout started by this claim would run out.</param>
    /// <param name="cancellationToken">Cancels the insert.</param>
    ///
    /// <returns>
    /// <c>true</c> when this caller inserted the row and so took the attempt; <c>false</c> when a row already existed,
    /// either because one was there all along or because a concurrent attempt inserted it first.
    /// </returns>
    ///
    /// <remarks>
    /// The insert is attempted rather than guarded by a preceding read, because a read cannot stop a second caller
    /// inserting between the two statements. The unique key on the user is what actually settles it, so the losing
    /// caller is told by the database and goes back to claiming against the row that won.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<bool> TryClaimFirstAttemptAsync(
        int userId,
        int maximum,
        DateTimeOffset lockoutEnd,
        CancellationToken cancellationToken
    )
    {
        UserLockout lockout = new()
        {
            UserId = userId,
            AccessFailedCount = 1,
            LockoutEnd = maximum <= 1 ? lockoutEnd : null
        };

        databaseContext.UserLockouts.Add(lockout);

        try
        {
            await databaseContext.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (DbUpdateException)
        {
            databaseContext.Entry(lockout).State = EntityState.Detached;

            return false;
        }
    }

    /// <summary>
    /// Reads when a refused caller's lockout runs out, settling a row that spent its budget without recording an end.
    /// </summary>
    ///
    /// <param name="userId">The user signing in, given as the database key rather than the public identifier.</param>
    /// <param name="lockoutEnd">The end to record and report when the row carries none.</param>
    /// <param name="cancellationToken">Cancels the update and the read.</param>
    ///
    /// <returns>When the lockout runs out, which is what the refusal reports to the caller.</returns>
    ///
    /// <remarks>
    /// A row can hold an exhausted count and no end when the configured limit was lowered under a run that had already
    /// passed the new value. Recording an end here means such a row expires on its own rather than refusing that user
    /// for good.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<DateTimeOffset> ResolveLockoutEndAsync(
        int userId,
        DateTimeOffset lockoutEnd,
        CancellationToken cancellationToken
    )
    {
        await databaseContext.UserLockouts
            .Where(candidate => candidate.UserId == userId && candidate.LockoutEnd == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(candidate => candidate.LockoutEnd, lockoutEnd),
                cancellationToken
            );

        UserLockout? lockout = await databaseContext.UserLockouts
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.UserId == userId, cancellationToken);

        return lockout?.LockoutEnd ?? lockoutEnd;
    }

    /// <summary>
    /// Clears the failure run behind a successful sign-in, including the attempt this sign-in claimed for itself.
    /// </summary>
    ///
    /// <param name="userId">The user signing in, given as the database key rather than the public identifier.</param>
    /// <param name="cancellationToken">Cancels the delete.</param>
    ///
    /// <returns>A task that completes once no lockout row stands for that user.</returns>
    ///
    /// <remarks>
    /// The delete is unconditional, and reaching it means the password was correct. A lockout in force before this
    /// attempt cannot be here, because <see cref="ReserveAttemptAsync"/> refuses on one. A lockout applied while this
    /// sign-in was verifying belongs to concurrent guesses that were wrong, and a caller who has just proved the
    /// password should not be held by them.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task ClearLockoutAsync(int userId, CancellationToken cancellationToken)
        => await databaseContext.UserLockouts
            .Where(candidate => candidate.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

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
