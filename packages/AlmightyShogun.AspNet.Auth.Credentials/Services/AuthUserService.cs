using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Signs users in and creates them. An unknown identifier and a wrong password are refused identically, and an unknown one
/// still pays a decoy verification, so the hash is not what separates them; with lockout enabled a known identifier also
/// pays the lockout statements the unknown path never reaches. A locked and a disabled account are each refused distinctly,
/// and a lockout is answered before the password is checked at all. A correct password opens a session only where no
/// enabled two-factor enrolment stands in the way; where one does, it buys a challenge and nothing else.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, both inserted on creation and returned on sign-in.</typeparam>
/// <param name="databaseContext">The application's context, which the credential tables live in.</param>
/// <param name="credentialOptions">
/// The bound credential settings, read for the lockout policy and for how long an issued challenge stays redeemable.
/// </param>
/// <param name="appHostResolver">
/// The resolver deciding which application the current request belongs to, so a session and its token are scoped to the
/// host the user actually signed in through, and so a challenge is only completed through the application that bought it.
/// </param>
/// <param name="sessionService">The session service, which issues the tokens a completed sign-in returns.</param>
/// <param name="twoFactorService">
/// The two-factor service, asked to verify the code a challenge is completed with. Nothing here reads a secret or a
/// recovery code itself.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class AuthUserService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthCredentialsSettings> credentialOptions,
    IAppHostResolver appHostResolver,
    IAuthSessionService<TUser> sessionService,
    IAuthTwoFactorService<TUser> twoFactorService
) : IAuthUserService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The hasher used for every password read and write, so hashing and verification cannot end up using different
    /// parameters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly PasswordHasher<TUser> _hasher = new();

    /// <inheritdoc />
    public async Task<AuthLoginResult<TUser>> LoginAsync(
        LoginRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        string? app = appHostResolver.Resolve();
        ClientContext clientContext = context.GetClientContext();

        TUser? user = await databaseContext.Users
            .Where(candidate => candidate.Username == request.Identifier || candidate.Email == request.Identifier)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            VerifyDecoy(request.Password);

            throw new InvalidCredentialsException();
        }

        LockoutPolicy lockoutPolicy = credentialOptions.Value.Lockout;

        await ReserveAttemptAsync(user.Id, lockoutPolicy, cancellationToken);

        PasswordVerificationResult verification = _hasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (verification is PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new AccountDisabledException();

        bool enrolled = await databaseContext.UserTwoFactors
            .AnyAsync(enrolment => enrolment.UserId == user.Id && enrolment.IsEnabled, cancellationToken);

        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        if (verification is PasswordVerificationResult.SuccessRehashNeeded)
            user.Password = _hasher.HashPassword(user, request.Password);

        if (enrolled)
        {
            await ReleaseAttemptAsync(user.Id, lockoutPolicy, cancellationToken);

            string challenge = await IssueChallengeAsync(user.Id, app, clientContext, cancellationToken);

            await databaseContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new AuthLoginResult<TUser> { User = user, Challenge = challenge };
        }

        await ClearLockoutAsync(user.Id, cancellationToken);

        AuthSessionResult<TUser> session = await sessionService.CreateSessionAsync(user, app, clientContext, cancellationToken);

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new AuthLoginResult<TUser> { User = user, Session = session };
    }

    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> CompleteTwoFactorLoginAsync(
        CompleteTwoFactorLoginRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        string? app = appHostResolver.Resolve();
        ClientContext clientContext = context.GetClientContext();

        DateTimeOffset now = DateTimeOffset.UtcNow;
        string challengeHash = TokenHasher.Hash(request.Challenge);

        TwoFactorChallenge? pending = await databaseContext.TwoFactorChallenges
            .Where(candidate => candidate.TokenHash == challengeHash)
            .Where(candidate => candidate.UsedAt == null && candidate.ExpiresAt > now)
            .FirstOrDefaultAsync(cancellationToken);

        if (pending is null || pending.App != app)
            throw new InvalidTwoFactorChallengeException();

        TUser? user = await databaseContext.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == pending.UserId, cancellationToken);

        if (user is null)
            throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new AccountDisabledException();

        if (!await twoFactorService.VerifyAsync(user.Identifier, request.Code, cancellationToken))
            throw new InvalidTwoFactorCodeException();

        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        await SpendChallengeAsync(pending.Id, now, cancellationToken);
        await ClearLockoutAsync(user.Id, cancellationToken);

        AuthSessionResult<TUser> result =
            await sessionService.CreateSessionAsync(user, pending.App, clientContext, cancellationToken);

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

        AuthSessionResult<TUser> result =
            await sessionService.CreateSessionAsync(createdUser, app, clientContext, cancellationToken);

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
    /// <since>4.0.0</since>
    private static void VerifyDecoy(string password) => AuthTimingDefence.SpendVerification(password);

    /// <summary>
    /// Retires the challenges the user still had outstanding, clears out the ones that have expired, and writes a fresh
    /// one, so the newest sign-in is the one that can be completed and, short of two arriving at once, it is the only one.
    /// </summary>
    ///
    /// <param name="userId">The user whose password just verified, given as the database key rather than the public identifier.</param>
    /// <param name="app">The application the sign-in came through, recorded so completion scopes the session to it.</param>
    /// <param name="context">The request's address, recorded on the challenge for auditing.</param>
    /// <param name="cancellationToken">Cancels the retirement, the prune, and the insert.</param>
    ///
    /// <returns>The challenge in plain text, to hand back to the client; only its hash is stored.</returns>
    ///
    /// <remarks>
    /// The retirement is one update statement, so the database has already written it when this returns. The prune and
    /// the insert are not saved here: the caller runs inside a transaction of its own and saves them with the rehash that
    /// may sit beside them, so a sign-in never ends with the previous challenge killed and no new one written.
    ///
    /// The prune covers only the user being signed in, so a row belonging to an account that never signs in again is
    /// left where it is. Retiring before pruning means a row that is both unspent and expired is stamped and then
    /// deleted in the same call.
    ///
    /// Two sign-ins running at once can still both insert, because the retirement of the later one need not see a row the
    /// earlier one has not committed yet and no index in the schema holds the count down.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<string> IssueChallengeAsync(
        int userId,
        string? app,
        ClientContext context,
        CancellationToken cancellationToken
    )
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        await RetireActiveChallengesAsync(userId, now, cancellationToken);

        List<TwoFactorChallenge> expired = await databaseContext.TwoFactorChallenges
            .Where(candidate => candidate.UserId == userId && candidate.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        if (expired.Count > 0)
            databaseContext.TwoFactorChallenges.RemoveRange(expired);

        string challenge = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48));

        databaseContext.TwoFactorChallenges.Add(new TwoFactorChallenge
        {
            App = app,
            UserId = userId,
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(credentialOptions.Value.TwoFactor.ChallengeMinutes),
            TokenHash = TokenHasher.Hash(challenge),
            RequestedIpAddress = context.IpAddress is { Length: > 45 } address ? address[..45] : context.IpAddress
        });

        return challenge;
    }

    /// <summary>
    /// Spends the user's unspent challenges in a single statement, so signing in again abandons the code prompt that was
    /// still open rather than leaving two completable at once.
    /// </summary>
    ///
    /// <param name="userId">The user whose outstanding challenges are being retired.</param>
    /// <param name="now">The instant stamped onto the retired rows, shared with the insert that runs alongside them.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// A task that completes once the matching rows are spent, which the database has already written when it does. The
    /// statement runs inside the transaction the caller opened, so it is rolled back with the rest of that work.
    /// </returns>
    ///
    /// <remarks>
    /// The rows are matched and stamped by one update rather than loaded and rewritten, so no row can be retired on the
    /// strength of a read another request has already invalidated. The update bypasses the change tracker, so the rows in
    /// the database are stamped while an instance already materialized through this context keeps the
    /// <see cref="TwoFactorChallenge.UsedAt"/> it was loaded with.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task RetireActiveChallengesAsync(int userId, DateTimeOffset now, CancellationToken cancellationToken)
        => await databaseContext.TwoFactorChallenges
            .Where(challenge => challenge.UserId == userId && challenge.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(challenge => challenge.UsedAt, now), cancellationToken);

    /// <summary>
    /// Claims the challenge with a guarded update, so two completions racing for the same one produce one session and one
    /// refusal rather than two sessions.
    /// </summary>
    ///
    /// <param name="challengeId">The row to spend, as read moments earlier.</param>
    /// <param name="now">The instant stamped onto the row, and the instant its expiry is judged against.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>A task that completes once the row is spent, which the database has already written when it does.</returns>
    ///
    /// <exception cref="InvalidTwoFactorChallengeException">
    /// The row was no longer unspent and unexpired, so another request claimed it in between, or a sign-in made in the
    /// meantime retired it.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task SpendChallengeAsync(int challengeId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        int affectedRows = await databaseContext.TwoFactorChallenges
            .Where(challenge => challenge.Id == challengeId && challenge.UsedAt == null && challenge.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters.SetProperty(challenge => challenge.UsedAt, now), cancellationToken);

        if (affectedRows != 1)
            throw new InvalidTwoFactorChallengeException();
    }

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
    /// A sign-in that completes deletes the row through <see cref="ClearLockoutAsync"/>, so the attempt claimed here costs
    /// the caller nothing. A correct password that owes a second factor gives the one attempt back through
    /// <see cref="ReleaseAttemptAsync"/> instead, leaving whatever failures already stood for
    /// <see cref="CompleteTwoFactorLoginAsync"/> to clear and taking the row away where none did. A correct password that
    /// is then refused, as a deactivated account is, leaves the claimed attempt standing, because neither the delete nor
    /// the give-back is reached.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    private async Task<bool> TryClaimAttemptAsync(
        int userId,
        int maximum,
        DateTimeOffset lockoutEnd,
        CancellationToken cancellationToken
    )
    {
        int claimed = await databaseContext.UserLockouts
            .Where(candidate => candidate.UserId == userId)
            .Where(candidate => candidate.LockoutEnd == null && candidate.AccessFailedCount < maximum)
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
    /// <c>true</c> when the save succeeded and this caller therefore took the attempt; <c>false</c> when it raised
    /// <see cref="DbUpdateException"/>, which the unique key on the user does when a row already exists, and which any
    /// other failed write does too, since the save flushes everything the context is tracking rather than this row alone.
    /// </returns>
    ///
    /// <remarks>
    /// The insert is attempted rather than guarded by a preceding read, because a read cannot stop a second caller
    /// inserting between the two statements. The unique key on the user is what actually settles it, so the losing
    /// caller is told by the database and goes back to claiming against the row that won.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
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
    /// Gives back the one attempt <see cref="ReserveAttemptAsync"/> claimed, for a password that verified but bought a
    /// challenge rather than a session. Without it an abandoned or expired code prompt would leave that attempt standing
    /// and a run of correct passwords would lock the account. Does nothing when lockout is disabled, matching the claim
    /// it undoes.
    /// </summary>
    ///
    /// <param name="userId">The user signing in, given as the database key rather than the public identifier.</param>
    /// <param name="policy">The configured policy, read for whether the feature is on at all and for the failure limit.</param>
    /// <param name="cancellationToken">Cancels the delete and the update that follows it.</param>
    ///
    /// <returns>A task that completes once the attempt is back in the budget, or once no row was found holding it.</returns>
    ///
    /// <remarks>
    /// Only the one attempt is returned. Failures recorded before this sign-in stay on the row and are cleared by
    /// <see cref="ClearLockoutAsync"/> once the code is presented, so a correct password neither ends the run nor adds to
    /// it. Where the attempt being given back is the only one on the row, the row goes rather than being left counting
    /// nothing, so a prompt that is never answered leaves the table as it found it.
    ///
    /// Both statements are guarded on the count they expect, in keeping with the claim they reverse. Neither matches
    /// where the row has been deleted by a completion running alongside this one, or where its count has already been
    /// zeroed by an attempt that found the previous lockout expired, so no case drives the count below zero. Two
    /// sign-ins releasing at once can both miss the delete and leave one attempt standing, which the next completed
    /// sign-in clears.
    ///
    /// The end is cleared alongside a decremented count that falls under the limit, and goes with the row where the row
    /// is deleted, because a claim that spent the last attempt recorded one. That also lifts a lockout applied by
    /// concurrent guesses in the meantime; the next failure against the restored attempt applies it again.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task ReleaseAttemptAsync(int userId, LockoutPolicy policy, CancellationToken cancellationToken)
    {
        if (!policy.Enabled)
            return;

        int maximum = policy.MaxFailedAttempts;

        int released = await databaseContext.UserLockouts
            .Where(candidate => candidate.UserId == userId && candidate.AccessFailedCount == 1)
            .ExecuteDeleteAsync(cancellationToken);

        if (released == 1)
            return;

        await databaseContext.UserLockouts
            .Where(candidate => candidate.UserId == userId && candidate.AccessFailedCount > 1)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(candidate => candidate.AccessFailedCount, candidate => candidate.AccessFailedCount - 1)
                    .SetProperty(
                        candidate => candidate.LockoutEnd,
                        candidate => candidate.AccessFailedCount - 1 >= maximum ? candidate.LockoutEnd : null
                    ),
                cancellationToken
            );
    }

    /// <summary>
    /// Clears the failure run behind a sign-in that has just completed, including the attempt that sign-in claimed for
    /// itself. Called only once every factor the account owes has been presented.
    /// </summary>
    ///
    /// <param name="userId">The user signing in, given as the database key rather than the public identifier.</param>
    /// <param name="cancellationToken">Cancels the delete.</param>
    ///
    /// <returns>A task that completes once no lockout row stands for that user.</returns>
    ///
    /// <remarks>
    /// The delete is unconditional, and reaching it means the sign-in is finished. Reached from
    /// <see cref="LoginAsync"/> a lockout in force beforehand cannot be here, because <see cref="ReserveAttemptAsync"/>
    /// refuses on one; reached from <see cref="CompleteTwoFactorLoginAsync"/> it can, since nothing rechecks the run
    /// between the password and the code. Either way a lockout applied in between belongs to concurrent guesses that were
    /// wrong, and a caller who has just proved every factor should not be held by them.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// <returns>A task that completes when neither value was in use at the moment it was checked.</returns>
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
    /// <since>4.0.0</since>
    private async Task EnsureCredentialsAvailableAsync(string username, string email, CancellationToken cancellationToken)
    {
        if (await databaseContext.Users.AnyAsync(candidate => candidate.Username == username, cancellationToken))
            throw new UsernameTakenException();

        if (await databaseContext.Users.AnyAsync(candidate => candidate.Email == email, cancellationToken))
            throw new EmailTakenException();
    }
}
