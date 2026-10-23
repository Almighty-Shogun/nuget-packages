
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Manages account lockout attempts across password and two-factor authentication.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity.</typeparam>
/// <param name="databaseContext">The authentication database context.</param>
/// <param name="credentialOptions">The credential authentication settings.</param>
///
/// <remarks>
/// Attempts are reserved before credential checks to limit concurrent guesses.
/// Guarded database updates coordinate competing requests without raising isolation levels.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
internal sealed class AuthLockoutGuard<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthCredentialsSettings> credentialOptions
) where TUser : AuthUser
{
    /// <summary>
    /// Reserves an authentication attempt or rejects an exhausted failure budget.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="AccountLockedException">
    /// The account is locked or has exhausted its available attempts.
    /// </exception>
    ///
    /// <remarks>
    /// Reserving before verification prevents concurrent guesses from bypassing
    /// the failure limit. Successful authentication clears the counter, while
    /// a successful password check awaiting 2FA releases only its own attempt.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async Task ReserveAttemptAsync(int userId, CancellationToken cancellationToken)
    {
        LockoutPolicy policy = credentialOptions.Value.Lockout;

        if (!policy.Enabled) return;

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
    /// Releases a reserved attempt without clearing previous failures.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <remarks>
    /// Used when a password check succeeds but two-factor authentication
    /// is still required. Deletes the row if no failures remain.
    /// Concurrent releases may leave an attempt counted until the next
    /// successful authentication.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
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
    /// Clears the user's failed authentication attempts and lockout.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <remarks>
    /// Called after successful authentication, including completion of 2FA.
    /// Deletes the lockout row even when lockout is disabled.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async Task ClearLockoutAsync(int userId, CancellationToken cancellationToken)
        => await databaseContext.UserLockouts
            .Where(candidate => candidate.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

    /// <summary>
    /// Rejects an account with an active lockout without reserving an attempt.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="AccountLockedException">The account is currently locked.</exception>
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
    /// Atomically reserves an attempt against an existing lockout record.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="maximum">The maximum failed attempts.</param>
    /// <param name="lockoutEnd">The lockout expiration timestamp.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>True if an attempt was reserved; otherwise, false.</returns>
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
    /// Creates the first lockout record when no failure history exists.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="maximum">The maximum failed attempts.</param>
    /// <param name="lockoutEnd">The lockout expiration timestamp.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>True if the record was inserted; otherwise, false.</returns>
    ///
    /// <remarks>
    /// The unique user constraint resolves competing inserts.
    /// Any <see cref="DbUpdateException"/> is treated as an unsuccessful claim.
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
    /// Retrieves or establishes the expiration of an exhausted lockout.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="lockoutEnd">The fallback lockout expiration timestamp.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The lockout expiration timestamp.</returns>
    ///
    /// <remarks>
    /// Handles records without an expiration, including those affected
    /// by a reduced maximum failure limit.
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
