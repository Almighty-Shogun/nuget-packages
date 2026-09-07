using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Creates, renews, and ends refresh-token sessions. Renewal rotates the token every time and remembers the one it
/// replaced, so presenting that spent token once the rotation grace has passed is recognised as a replay and ends every
/// session the user holds.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, returned alongside the tokens a session yields.</typeparam>
/// <param name="databaseContext">The application's context, which the credential tables live in.</param>
/// <param name="authOptions">The bound JWT settings, read for how long a refresh token lives.</param>
/// <param name="credentialOptions">
/// The bound credential settings, read for the absolute lifetime that caps the expiry written when a session opens and
/// again when it is renewed.
/// </param>
/// <param name="appHostResolver">
/// The resolver deciding which application the current request belongs to, so a session is scoped to the host the user
/// actually signed in through.
/// </param>
/// <param name="lockoutGuard">
/// The guard owning the failure budget, consulted so a renewal honours a lockout the sign-in paths applied. Nothing here
/// claims an attempt against it, since a refresh token is not guessed at.
/// </param>
/// <param name="tokenGenerator">
/// The JWT package's generator, which signs and stamps issuer, audience, and expiry over the claims built here.
/// </param>
/// <param name="logger">Where a detected token replay is recorded, since nothing is returned to the caller about it.</param>
///
/// <remarks>
/// Only one step of the chain is remembered. A session that has rotated <c>a</c> to <c>b</c> to <c>c</c> holds
/// <c>b</c> as its previous token, so replaying <c>b</c> is detected while replaying <c>a</c> reads as an unknown token
/// and is refused without revoking anything. Detection therefore covers the token most recently spent, and not every
/// token the session has ever issued, and it covers that one token once, since the record of it is cleared as the
/// detection fires.
///
/// It also covers only a replay arriving more than <see cref="AuthSessionDefaults.RotationGrace"/> after the session was
/// last refreshed. Inside that window the replay is refused like any unusable token and nothing is revoked, so a thief
/// presenting the spent token while the legitimate client is still rotating goes undetected.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class AuthSessionService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthSettings> authOptions,
    IOptions<AuthCredentialsSettings> credentialOptions,
    IAppHostResolver appHostResolver,
    AuthLockoutGuard<TUser> lockoutGuard,
    IAuthTokenGenerator tokenGenerator,
    ILogger<AuthSessionService<TUser>> logger
) : IAuthSessionService<TUser> where TUser : AuthUser
{
    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> RefreshSessionAsync(
        string refreshToken,
        HttpContext httpContext,
        CancellationToken cancellationToken = default
    )
    {
        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        string? app = appHostResolver.Resolve();
        string refreshTokenHash = TokenHasher.Hash(refreshToken);

        ClientContext clientContext = httpContext.GetClientContext();

        DateTimeOffset now = DateTimeOffset.UtcNow;

        IQueryable<UserSession> query = databaseContext.UserSessions.Where(session => session.RefreshTokenHash == refreshTokenHash)
            .Where(session => !session.IsRevoked && session.ExpiresAt > now);

        if (app is not null)
            query = query.Where(session => session.App == app);

        UserSession? session = await query.FirstOrDefaultAsync(cancellationToken);

        if (session is null || !session.IsActive)
        {
            if (await DetectTokenReuseAsync(refreshTokenHash, cancellationToken))
                await SaveRevocationsAsync(transaction, cancellationToken);

            throw new InvalidSessionException();
        }

        TUser user = await GetUserAsync(user => user.Id == session.UserId, cancellationToken);

        if (!user.IsActive)
            throw new AccountDisabledException();

        await lockoutGuard.EnsureNotLockedOutAsync(user.Id, cancellationToken);

        string newRefreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        UserAgent userAgent = UserAgent.Parse(clientContext.UserAgent ?? string.Empty);

        session.Os = ColumnValue.Truncate(userAgent.Os, 256);
        session.Device = ColumnValue.Truncate(userAgent.Device, 256);
        session.Browser = ColumnValue.Truncate(userAgent.Browser, 256);
        session.LastActiveAt = DateTimeOffset.UtcNow;
        session.IpAddress = ColumnValue.Truncate(clientContext.IpAddress, 45);
        session.UserAgent = ColumnValue.Truncate(clientContext.UserAgent, 512);
        session.PreviousRefreshTokenHash = session.RefreshTokenHash;
        session.RefreshTokenHash = TokenHasher.Hash(newRefreshToken);
        session.ConcurrencyToken = Guid.NewGuid();
        session.ExpiresAt = CapToAbsoluteLifetime(
            session,
            DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(authOptions.Value.RefreshTokenDays))
        );

        try
        {
            await databaseContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw new InvalidSessionException();
        }

        return new AuthSessionResult<TUser>
        {
            User = user,
            RefreshToken = newRefreshToken,
            AccessToken = tokenGenerator.Generate(AuthClaimFactory.Create(user, app), app).Token
        };
    }

    /// <inheritdoc />
    public async Task RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        string refreshTokenHash = TokenHasher.Hash(refreshToken);

        UserSession? session = await databaseContext.UserSessions
            .Where(session => session.RefreshTokenHash == refreshTokenHash && !session.IsRevoked)
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            await transaction.CommitAsync(cancellationToken);

            return;
        }

        session.IsRevoked = true;

        databaseContext.UserSessions.Update(session);

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> CreateSessionAsync(
        TUser user,
        string? app,
        ClientContext context,
        CancellationToken cancellationToken = default
    )
    {
        string refreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        DateTimeOffset now = DateTimeOffset.UtcNow;

        List<UserSession> expired = await databaseContext.UserSessions
            .Where(session => session.UserId == user.Id && session.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        if (expired.Count > 0)
            databaseContext.UserSessions.RemoveRange(expired);

        UserAgent userAgent = UserAgent.Parse(context.UserAgent ?? string.Empty);

        UserSession session = new()
        {
            UserId = user.Id,
            App = app,
            Os = ColumnValue.Truncate(userAgent.Os, 256),
            Device = ColumnValue.Truncate(userAgent.Device, 256),
            Browser = ColumnValue.Truncate(userAgent.Browser, 256),
            IpAddress = ColumnValue.Truncate(context.IpAddress, 45),
            UserAgent = ColumnValue.Truncate(context.UserAgent, 512),
            RefreshTokenHash = TokenHasher.Hash(refreshToken)
        };

        session.ExpiresAt = CapToAbsoluteLifetime(
            session,
            DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(authOptions.Value.RefreshTokenDays))
        );

        await databaseContext.UserSessions.AddAsync(session, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);

        return new AuthSessionResult<TUser>
        {
            User = user,
            RefreshToken = refreshToken,
            AccessToken = tokenGenerator.Generate(AuthClaimFactory.Create(user, app), app).Token
        };
    }

    /// <summary>
    /// Loads the one user matching a predicate, refusing rather than returning null, so every caller past this point has a
    /// user to work with.
    /// </summary>
    ///
    /// <param name="predicate">The lookup, by the key the session being renewed carries.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>The matching user, tracked so a caller can modify and save it.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">Thrown when no user matches the predicate.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken)
    {
        TUser? user = await databaseContext.Users.FirstOrDefaultAsync(predicate, cancellationToken);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Treats a refresh token that was already rotated away as stolen, marks every session the user holds revoked, and
    /// clears the retired hash that matched, so one replayed token fires the detection once rather than repeatedly.
    /// </summary>
    ///
    /// <param name="refreshTokenHash">The hash of the token that was presented after already being rotated away.</param>
    /// <param name="cancellationToken">Cancels the lookups.</param>
    ///
    /// <returns>
    /// <c>true</c> when a replay was found, the user's live sessions were marked revoked and the matched session's
    /// <see cref="UserSession.PreviousRefreshTokenHash"/> was cleared, <c>false</c> when the hash matches no rotated
    /// session or the rotation is still inside the grace window.
    /// </returns>
    ///
    /// <remarks>
    /// The match is on the retired hash alone, with no filter on the session being live, so a row already revoked, by a
    /// sign-out or by a password or email change, is still a source. Clearing the hash is what bounds that: revoked rows
    /// are kept until they expire, so a hash left standing would revoke every session opened afterwards, once per
    /// sign-in, for the rest of the refresh window.
    ///
    /// Nothing is saved or committed here. The caller runs inside a transaction of its own and is about to throw, so it
    /// alone decides whether the revocations are written; saving here would commit a partial write on a path that fails.
    /// A rotation of one of the rows read here committing before that write lands makes the write match nothing, and the
    /// caller reapplies the revocation over the row that rotation left rather than the one this read saw.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<bool> DetectTokenReuseAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        UserSession? rotated = await databaseContext.UserSessions
            .FirstOrDefaultAsync(session => session.PreviousRefreshTokenHash == refreshTokenHash, cancellationToken);

        if (rotated is null || DateTimeOffset.UtcNow - rotated.LastActiveAt <= AuthSessionDefaults.RotationGrace)
            return false;

        logger.LogWarning(
            "Refresh token reuse detected for user {UserId}; revoking every session for that user", rotated.UserId
        );

        List<UserSession> live = await databaseContext.UserSessions
            .Where(session => session.UserId == rotated.UserId && !session.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (UserSession active in live)
            active.IsRevoked = true;

        rotated.PreviousRefreshTokenHash = null;

        return true;
    }

    /// <summary>
    /// Writes the revocations a detected replay made, reapplying them over any rotation that commits in between, so a
    /// confirmed theft ends the user's sessions instead of being discarded by a refresh that raced it.
    /// </summary>
    ///
    /// <param name="transaction">
    /// The caller's open transaction, committed once the write lands and rolled back when it never does.
    /// </param>
    /// <param name="cancellationToken">Cancels the save, the reloads it retries against, and the commit.</param>
    ///
    /// <remarks>
    /// Every session row carries <see cref="UserSession.ConcurrencyToken"/> in the <c>WHERE</c> clause of its UPDATE, so a
    /// rotation of the user's other sessions landing between the detection's read and this write leaves that row
    /// matching nothing. Each rejected entry is reloaded and its guard alone refreshed to the value that rotation wrote,
    /// re-arming the same UPDATE against the current row. Only the guard is taken from the reloaded row on purpose:
    /// refreshing every original would leave the columns the rotation wrote differing from the values read before it,
    /// marking them modified, and the retry would write the rotated token and expiry back to what they were.
    ///
    /// <see cref="AuthSessionDefaults.RevocationSaveAttempts"/> bounds the saves rather than the colliding rows. A
    /// provider that batches the UPDATEs reports every collision on one save, so a single retry clears them all; one that
    /// sends them singly reports the first alone and spends an attempt per row.
    ///
    /// The revocation is meant to win that collision. <see cref="UserSession.IsRevoked"/> and the cleared
    /// <see cref="UserSession.PreviousRefreshTokenHash"/> are the only columns it sets, both to a constant and neither
    /// reading the value it replaces, so reapplying them over the row the rotation left loses nothing, and the rotation
    /// being overtaken may well be the thief's.
    ///
    /// The reload runs on the caller's connection inside its still-open transaction, so it depends on the isolation level
    /// admitting the row the other request committed. Read committed, which SQL Server and PostgreSQL default to, does.
    ///
    /// A row deleted underneath the write is dropped from the change tracker rather than retried, since a session that no
    /// longer exists needs no revoking. Exhausting the attempts logs the failure and rolls back, which leaves the user's
    /// sessions standing and the replayed session's retired hash uncleared, so that token fires detection again on its
    /// next presentation; the caller refuses the refresh either way.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task SaveRevocationsAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
    {
        for (var attempt = 1;; attempt++)
        {
            try
            {
                await databaseContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return;
            }
            catch (DbUpdateConcurrencyException exception)
            {
                if (attempt >= AuthSessionDefaults.RevocationSaveAttempts)
                {
                    logger.LogError(
                        "Refresh token reuse revocations abandoned after {Attempts} attempts; the user's sessions stand",
                        attempt
                    );

                    await transaction.RollbackAsync(cancellationToken);

                    return;
                }

                foreach (EntityEntry entry in exception.Entries)
                {
                    PropertyValues? stored = await entry.GetDatabaseValuesAsync(cancellationToken);

                    if (stored is null)
                    {
                        entry.State = EntityState.Detached;

                        continue;
                    }

                    PropertyEntry guard = entry.Property(nameof(UserSession.ConcurrencyToken));

                    guard.CurrentValue = stored[nameof(UserSession.ConcurrencyToken)];
                    guard.OriginalValue = guard.CurrentValue;
                    guard.IsModified = false;
                }
            }
        }
    }

    /// <summary>
    /// Caps a session expiry at the absolute lifetime measured from when the session was created, so neither the expiry
    /// written when a session opens nor the one a renewal proposes carries it past that limit.
    /// </summary>
    ///
    /// <param name="session">
    /// The session the expiry belongs to, read for when it was created. A row that has not been saved yet works, since
    /// <see cref="UserSession.CreatedAt"/> is stamped as the entity is constructed and nothing rewrites it on insert.
    /// </param>
    /// <param name="proposedExpiry">The expiry a plain sliding window would give, before the absolute cap is applied.</param>
    ///
    /// <returns>
    /// The earlier of the proposed expiry and the absolute limit, so continuous use cannot extend one sign-in forever.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private DateTimeOffset CapToAbsoluteLifetime(UserSession session, DateTimeOffset proposedExpiry)
    {
        if (credentialOptions.Value.AbsoluteSessionLifetimeDays is not { } days)
            return proposedExpiry;

        DateTimeOffset absoluteEnd = session.CreatedAt.AddDays(days);

        return proposedExpiry > absoluteEnd ? absoluteEnd : proposedExpiry;
    }
}
