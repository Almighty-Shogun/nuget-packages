using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

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
/// again when it is renewed, and that refuses a renewal arriving past it.
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

        IQueryable<UserSession> query = databaseContext.UserSessions.AsNoTracking()
            .Where(session => session.RefreshTokenHash == refreshTokenHash)
            .Where(session => !session.IsRevoked && session.ExpiresAt > now);

        if (app is not null)
            query = query.Where(session => session.App == app);

        UserSession? session = await query.FirstOrDefaultAsync(cancellationToken);

        if (session is null || !session.IsActive)
        {
            await transaction.RollbackAsync(cancellationToken);
            await SaveRevocationsAsync(refreshTokenHash, cancellationToken);

            throw new InvalidSessionException();
        }

        if (IsPastAbsoluteLifetime(session, now))
            throw new InvalidSessionException();

        TUser user = await GetUserAsync(user => user.Id == session.UserId, cancellationToken);

        if (!user.IsActive)
            throw new AccountDisabledException();

        await lockoutGuard.EnsureNotLockedOutAsync(user.Id, cancellationToken);

        string newRefreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        bool rotated;

        try
        {
            rotated = await RotateSessionAsync(session, newRefreshToken, clientContext, cancellationToken);

            if (rotated)
                await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception exception) when (ConcurrencyConflict.IsConflict(exception))
        {
            rotated = false;
        }

        if (rotated)
            return new AuthSessionResult<TUser>
            {
                User = user,
                RefreshToken = newRefreshToken,
                AccessToken = tokenGenerator.Generate(AuthClaimFactory.Create(user, app), app).Token
            };

        await transaction.RollbackAsync(cancellationToken);

        throw new InvalidSessionException();
    }

    /// <inheritdoc />
    public async Task RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        string refreshTokenHash = TokenHasher.Hash(refreshToken);

        await databaseContext.UserSessions
            .Where(session => session.RefreshTokenHash == refreshTokenHash && !session.IsRevoked)
            .ExecuteUpdateAsync(setters => setters.SetProperty(session => session.IsRevoked, true), cancellationToken);
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
    /// <returns>
    /// The matching user, tracked. Nothing on this path writes to it: the rotation is a statement of its own rather than a
    /// saved entity, so the context the application shares with this service gains a row to track and nothing to save.
    /// </returns>
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
    /// Writes a rotation onto the session with one statement, guarded on the concurrency token the row carried when it was
    /// read, so two refreshes presenting the same token leave one rotation standing rather than two.
    /// </summary>
    ///
    /// <param name="session">
    /// The session being renewed, read moments earlier. Read here for the row to match, the token value the guard compares,
    /// the hash the rotation retires, and the creation instant the absolute cap is measured from.
    /// </param>
    /// <param name="newRefreshToken">The token about to be handed to the client, stored as its hash.</param>
    /// <param name="clientContext">
    /// The current request's address and user agent, recorded on the row so a user can recognise their own devices. Both are
    /// trimmed to the widths their columns declare, since they arrive at whatever length the client sent.
    /// </param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// <c>true</c> when the row was rewritten, <c>false</c> when the guard matched nothing, which means another refresh
    /// rotated the same session between the read and here. The caller rolls back and refuses on <c>false</c>, so the token
    /// this call would have issued is discarded rather than left half written.
    /// </returns>
    ///
    /// <remarks>
    /// The row is matched and written by one statement rather than loaded and saved, so nothing is staged on the context
    /// the application shares with this service and a rotation that loses leaves no entry behind for the application's own
    /// save to commit. Entity Framework applies no concurrency check to a statement it runs directly, so the token is
    /// compared in the predicate and replaced in the same update rather than left to a save.
    ///
    /// The expiry is recomputed from the refresh window and capped, so a renewal slides the session forward without
    /// carrying it past the absolute lifetime, and the hash being replaced is recorded as the previous one, which is what
    /// a later replay of that token is detected against.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<bool> RotateSessionAsync(
        UserSession session,
        string newRefreshToken,
        ClientContext clientContext,
        CancellationToken cancellationToken
    )
    {
        UserAgent userAgent = UserAgent.Parse(clientContext.UserAgent ?? string.Empty);

        string? os = ColumnValue.Truncate(userAgent.Os, 256);
        string? device = ColumnValue.Truncate(userAgent.Device, 256);
        string? browser = ColumnValue.Truncate(userAgent.Browser, 256);
        string? ipAddress = ColumnValue.Truncate(clientContext.IpAddress, 45);
        string? rawUserAgent = ColumnValue.Truncate(clientContext.UserAgent, 512);

        string previousRefreshTokenHash = session.RefreshTokenHash;
        string newRefreshTokenHash = TokenHasher.Hash(newRefreshToken);

        var concurrencyToken = Guid.NewGuid();

        DateTimeOffset rotatedAt = DateTimeOffset.UtcNow;
        DateTimeOffset expiresAt = CapToAbsoluteLifetime(session, rotatedAt.Add(TimeSpan.FromDays(authOptions.Value.RefreshTokenDays)));

        int affectedRows = await databaseContext.UserSessions
            .Where(candidate => candidate.Id == session.Id && candidate.ConcurrencyToken == session.ConcurrencyToken)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(candidate => candidate.Os, os)
                    .SetProperty(candidate => candidate.Device, device)
                    .SetProperty(candidate => candidate.Browser, browser)
                    .SetProperty(candidate => candidate.LastActiveAt, rotatedAt)
                    .SetProperty(candidate => candidate.IpAddress, ipAddress)
                    .SetProperty(candidate => candidate.UserAgent, rawUserAgent)
                    .SetProperty(candidate => candidate.PreviousRefreshTokenHash, previousRefreshTokenHash)
                    .SetProperty(candidate => candidate.RefreshTokenHash, newRefreshTokenHash)
                    .SetProperty(candidate => candidate.ConcurrencyToken, concurrencyToken)
                    .SetProperty(candidate => candidate.ExpiresAt, expiresAt),
                cancellationToken
            );

        return affectedRows == 1;
    }

    /// <summary>
    /// Treats a refresh token that was already rotated away as stolen, revokes every session the user holds, and clears
    /// the retired hash that matched, so one replayed token fires the detection once rather than repeatedly.
    /// </summary>
    ///
    /// <param name="refreshTokenHash">The hash of the token that was presented after already being rotated away.</param>
    /// <param name="cancellationToken">Cancels the lookup and the two updates.</param>
    ///
    /// <returns>
    /// <c>true</c> when a replay was found, the user's live sessions were revoked and the matched session's
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
    /// Both writes are statements the database applies as they are issued rather than entities left for a save, so
    /// nothing is staged on the shared context that an abandoned attempt could leave behind. Neither is committed here:
    /// the caller owns the transaction this runs in, so a detection that cannot be committed leaves no change behind.
    /// The caller runs this again in a fresh transaction when an attempt loses a collision, which is why the row is read
    /// here rather than passed in: the second reading sees what the rotation that won left, and revokes that.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<bool> DetectTokenReuseAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        UserSession? rotated = await databaseContext.UserSessions.AsNoTracking()
            .FirstOrDefaultAsync(session => session.PreviousRefreshTokenHash == refreshTokenHash, cancellationToken);

        if (rotated is null || DateTimeOffset.UtcNow - rotated.LastActiveAt <= AuthSessionDefaults.RotationGrace)
            return false;

        logger.LogWarning(
            "Refresh token reuse detected for user {UserId}; revoking every session for that user", rotated.UserId
        );

        await databaseContext.UserSessions
            .Where(session => session.UserId == rotated.UserId && !session.IsRevoked)
            .ExecuteUpdateAsync(setters => setters.SetProperty(session => session.IsRevoked, true), cancellationToken);

        await databaseContext.UserSessions
            .Where(session => session.Id == rotated.Id)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(session => session.PreviousRefreshTokenHash, (string?)null),
                cancellationToken
            );

        return true;
    }

    /// <summary>
    /// Detects a replay and writes the revocations it makes in a transaction of its own, running the pair again over
    /// freshly read rows whenever a refresh that raced it wins, so a confirmed theft ends the user's sessions instead of
    /// being discarded.
    /// </summary>
    ///
    /// <param name="refreshTokenHash">The hash of the token that was refused, which the detection matches on.</param>
    /// <param name="cancellationToken">Cancels the reads, the updates, and the commit of every attempt.</param>
    ///
    /// <returns>
    /// A task that completes once an attempt has committed, once a detection has found nothing to revoke, or once the
    /// attempts are spent.
    /// </returns>
    ///
    /// <remarks>
    /// A whole attempt is redone rather than only its write, because a server enforcing snapshot isolation rejects the
    /// update outright once a row has moved since the transaction's first read. That cannot be answered inside the
    /// transaction at all, since every read it makes still returns the snapshot the rotation is not in, so the
    /// transaction is left behind and the next attempt opens its own and reads what the rotation left.
    ///
    /// <see cref="AuthSessionDefaults.RevocationSaveAttempts"/> bounds those attempts, so a steady stream of refreshes
    /// cannot hold one request open indefinitely. Nothing is done to the context between them: the revocations are
    /// statements the database applies as they are issued, so an abandoned attempt stages nothing to be sent again and
    /// disturbs nothing the application was tracking.
    ///
    /// The revocation is meant to win that collision. <see cref="UserSession.IsRevoked"/> and the cleared
    /// <see cref="UserSession.PreviousRefreshTokenHash"/> are the only columns it sets, both to a constant and neither
    /// reading the value it replaces, so reapplying them over the row the rotation left loses nothing, and the rotation
    /// being overtaken may well be the thief's.
    ///
    /// A later attempt whose detection finds nothing returns without writing, which is what happens once another request
    /// has already cleared the retired hash and revoked the sessions this one was going to. Exhausting the attempts logs
    /// the failure and leaves the user's sessions standing with that hash uncleared, so the token fires detection again
    /// on its next presentation. The caller refuses the refresh with <see cref="InvalidSessionException"/> either way,
    /// so nothing about the answer tells the presenter which of the two happened.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task SaveRevocationsAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        for (var attempt = 1;; attempt++)
        {
            try
            {
                await using IDbContextTransaction transaction =
                    await databaseContext.Database.BeginTransactionAsync(cancellationToken);

                if (!await DetectTokenReuseAsync(refreshTokenHash, cancellationToken))
                    return;

                await transaction.CommitAsync(cancellationToken);

                return;
            }
            catch (Exception exception) when (ConcurrencyConflict.IsConflict(exception))
            {
                if (attempt < AuthSessionDefaults.RevocationSaveAttempts)
                    continue;

                logger.LogError(
                    exception,
                    "Refresh token reuse revocations abandoned after {Attempts} attempts; the user's sessions stand",
                    attempt
                );

                return;
            }
        }
    }

    /// <summary>
    /// Reports whether a session has already reached the absolute lifetime measured from its creation, so a refresh
    /// presented against it is refused rather than capped to an expiry at or before <paramref name="now"/> that no later
    /// refresh would match. It draws that line and nothing more: a refresh admitted just under the limit is still capped to
    /// it, so the refresh token handed back can be milliseconds from dead.
    /// </summary>
    ///
    /// <param name="session">The session a refresh was presented against, read for when it was created.</param>
    /// <param name="now">
    /// The moment the refresh is judged against, read before the session was looked up. The expiry a renewal goes on to
    /// write is computed from a later reading of the clock, so a session crossing the limit in between is admitted here and
    /// then capped to an expiry already past.
    /// </param>
    ///
    /// <returns>
    /// <c>false</c> when no absolute lifetime is configured, otherwise whether that lifetime ends at or before
    /// <paramref name="now"/>. Reaching it exactly counts as past, matching <see cref="UserSession.IsExpired"/> treating an
    /// expiry equal to the current moment as reached.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private bool IsPastAbsoluteLifetime(UserSession session, DateTimeOffset now)
    {
        if (credentialOptions.Value.AbsoluteSessionLifetimeDays is not { } days)
            return false;

        return session.CreatedAt.AddDays(days) <= now;
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
