using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Meters the credential checks an attacker can guess at against one failure budget per account. A caller claims an
/// attempt before it checks anything, and then either gives that attempt back, clears the whole run, or leaves it
/// standing; a wrong answer is what leaving it standing means. Holding the four operations together is what lets the
/// password and the second factor draw on the same budget rather than one each.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, which fixes the context type the lockout rows are read through.</typeparam>
/// <param name="databaseContext">The application's context, which the lockout table lives in.</param>
/// <param name="credentialOptions">
/// The bound credential settings, read for the lockout policy: whether the feature is on at all, the failure limit, and
/// how long a lockout lasts.
/// </param>
///
/// <remarks>
/// Every statement runs through the caller's own context, which is scoped to the request, so a claim made while a caller
/// holds a transaction open is rolled back with it. The claim and the give-back are made by guarded update statements
/// rather than by reading a row and writing it back, so concurrent callers are settled by the database and no isolation
/// level has to be raised.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthLockoutGuard<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthCredentialsSettings> credentialOptions
) where TUser : AuthUser
{
    /// <summary>
    /// Claims one attempt against the user's failure budget before the credential is checked, and refuses when the
    /// budget is gone. Does nothing when lockout is disabled, so an application that does not want it pays no write.
    /// </summary>
    ///
    /// <param name="userId">The user being checked, given as the database key rather than the public identifier.</param>
    /// <param name="cancellationToken">Cancels the database work.</param>
    ///
    /// <returns>A task that completes once one attempt has been claimed for this caller.</returns>
    ///
    /// <exception cref="AccountLockedException">
    /// A lockout is in force for this user, or the budget was exhausted by attempts that claimed before this one.
    /// </exception>
    ///
    /// <remarks>
    /// Counting before the check rather than after it is what bounds a burst. Password verification is deliberately
    /// slow, so a check made before it and a count made after it leave a window as wide as the hash: every request
    /// arriving inside it reads the same unspent budget and is allowed to guess, and the limit then bounds attempts made
    /// one after another while doing nothing about attempts made at once.
    ///
    /// The claim is one guarded statement rather than a read followed by a write, so the database decides who gets each
    /// attempt. Two callers racing for the last attempt both issue the same update; exactly one reports a row, and the
    /// other is refused. The row's unique key on the user settles the same race for the first attempt of a run, where
    /// there is no row to update yet.
    ///
    /// A run that ends in a completed sign-in is deleted through <see cref="ClearLockoutAsync"/>, so the attempt claimed
    /// here costs that caller nothing. A correct password that owes a second factor gives the one attempt back through
    /// <see cref="ReleaseAttemptAsync"/> instead, leaving whatever failures already stood for the code prompt to clear
    /// and taking the row away where none did. A caller that is refused after claiming, as a deactivated account is,
    /// leaves the claimed attempt standing, because neither the delete nor the give-back is reached.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async Task ReserveAttemptAsync(int userId, CancellationToken cancellationToken)
    {
        LockoutPolicy policy = credentialOptions.Value.Lockout;

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

        if (await TryClaimAttemptAsync(userId, maximum, lockoutEnd, cancellationToken)) return;
        if (await TryClaimFirstAttemptAsync(userId, maximum, lockoutEnd, cancellationToken)) return;
        if (await TryClaimAttemptAsync(userId, maximum, lockoutEnd, cancellationToken)) return;

        throw new AccountLockedException(await ResolveLockoutEndAsync(userId, lockoutEnd, cancellationToken));
    }

    /// <summary>
    /// Gives back the one attempt <see cref="ReserveAttemptAsync"/> claimed, for a check that passed without finishing
    /// the sign-in. Without it an abandoned or expired code prompt would leave that attempt standing and a run of correct
    /// passwords would lock the account. Does nothing when lockout is disabled, matching the claim it undoes.
    /// </summary>
    ///
    /// <param name="userId">The user being checked, given as the database key rather than the public identifier.</param>
    /// <param name="cancellationToken">Cancels the delete and the update that follows it.</param>
    ///
    /// <returns>A task that completes once the attempt is back in the budget, or once no row was found holding it.</returns>
    ///
    /// <remarks>
    /// Only the one attempt is returned. Failures recorded before it stay on the row and are cleared by
    /// <see cref="ClearLockoutAsync"/> once the sign-in finishes, so a correct password neither ends the run nor adds to
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
    public async Task ReleaseAttemptAsync(int userId, CancellationToken cancellationToken)
    {
        LockoutPolicy policy = credentialOptions.Value.Lockout;

        if (!policy.Enabled) return;

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
    /// Clears the failure run behind a credential that has just been proved, including the attempt that check claimed
    /// for itself. Called only where the caller is satisfied, so it is the correct answer that ends a run.
    /// </summary>
    ///
    /// <param name="userId">The user being checked, given as the database key rather than the public identifier.</param>
    /// <param name="cancellationToken">Cancels the delete.</param>
    ///
    /// <returns>A task that completes once no lockout row stands for that user.</returns>
    ///
    /// <remarks>
    /// The delete is unconditional and is made whether or not lockout is enabled, which costs a statement that matches
    /// nothing where the feature was never on. A lockout in force goes with the count behind it, which is wanted in both
    /// cases where one can stand here: the caller's own claim through <see cref="ReserveAttemptAsync"/> applied it by
    /// spending the last attempt, which that claim records while still letting the check go ahead, or concurrent guesses
    /// that were wrong applied it in between. A lockout that already stood beforehand is neither, since
    /// <see cref="ReserveAttemptAsync"/> refuses on one. A caller that has just answered correctly should not be held by
    /// either of the two.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async Task ClearLockoutAsync(int userId, CancellationToken cancellationToken)
        => await databaseContext.UserLockouts
            .Where(candidate => candidate.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

    /// <summary>
    /// Loads the lockout row for a user and refuses when it is in force, without claiming anything. For a caller that
    /// has to honour a lockout but has no credential to meter. Does nothing at all when lockout is disabled, so a
    /// deployment that never uses it pays no query for the check.
    /// </summary>
    ///
    /// <param name="userId">The user being let in, given as the database key rather than the public identifier.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>A task that completes once the account is known not to be locked.</returns>
    ///
    /// <exception cref="AccountLockedException">
    /// A lockout is in force, so an account locked by failed sign-ins cannot go on renewing a session it opened before
    /// the lockout began.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async Task EnsureNotLockedOutAsync(int userId, CancellationToken cancellationToken)
    {
        if (!credentialOptions.Value.Lockout.Enabled) return;

        UserLockout? lockout = await databaseContext.UserLockouts
            .FirstOrDefaultAsync(candidate => candidate.UserId == userId, cancellationToken);

        if (lockout is not null && lockout.IsLocked)
            throw new AccountLockedException(lockout.LockoutEnd!.Value);
    }

    /// <summary>
    /// Claims an attempt against an existing row, locking the account when the claim spends the last one.
    /// </summary>
    ///
    /// <param name="userId">The user being checked, given as the database key rather than the public identifier.</param>
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
    /// <param name="userId">The user being checked, given as the database key rather than the public identifier.</param>
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
    /// <param name="userId">The user being checked, given as the database key rather than the public identifier.</param>
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
}
