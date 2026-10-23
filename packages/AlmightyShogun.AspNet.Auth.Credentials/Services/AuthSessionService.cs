
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
/// Creates, refreshes, and revokes authentication sessions.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity.</typeparam>
/// <param name="databaseContext">The authentication database context.</param>
/// <param name="authOptions">The JWT authentication settings.</param>
/// <param name="credentialOptions">The credential and session lifetime settings.</param>
/// <param name="appHostResolver">Resolves the current application host.</param>
/// <param name="lockoutGuard">Checks account lockout status during renewal.</param>
/// <param name="tokenGenerator">Generates signed access tokens.</param>
/// <param name="logger">Logs refresh-token replay detection.</param>
///
/// <remarks>
/// Refresh tokens rotate on renewal. Reuse of the most recently replaced token
/// outside <see cref="AuthSessionDefaults.RotationGrace"/> triggers session revocation.
/// Older tokens are rejected without replay detection, and reuse within the grace
/// period is rejected without revocation.
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
    /// Retrieves a matching user.
    /// </summary>
    ///
    /// <param name="predicate">The user lookup predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The matching tracked user.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">No matching user exists.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken)
    {
        TUser? user = await databaseContext.Users.FirstOrDefaultAsync(predicate, cancellationToken);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Atomically rotates a session's refresh token.
    /// </summary>
    ///
    /// <param name="session">The session being renewed.</param>
    /// <param name="newRefreshToken">The replacement refresh token.</param>
    /// <param name="clientContext">The current client metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>True if rotation succeeded; otherwise, false.</returns>
    ///
    /// <remarks>
    /// Guards the update with the existing concurrency token, refresh-token hash,
    /// revocation status, and expiry. Stores the replaced hash for replay detection
    /// and caps the new expiry at the absolute session lifetime.
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
            .Where(candidate =>
                candidate.Id == session.Id &&
                candidate.ConcurrencyToken == session.ConcurrencyToken &&
                !candidate.IsRevoked &&
                candidate.ExpiresAt > rotatedAt &&
                candidate.RefreshTokenHash == previousRefreshTokenHash)
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
    /// Detects reuse of a rotated refresh token and revokes the user's sessions.
    /// </summary>
    ///
    /// <param name="refreshTokenHash">The presented refresh-token hash.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>True if replay was detected; otherwise, false.</returns>
    ///
    /// <remarks>
    /// Detection applies after the rotation grace period, even to revoked sessions.
    /// The previous-token hash is cleared after detection to prevent repeated revocations.
    /// Changes remain subject to the caller's transaction.
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
    /// Persists replay-triggered session revocations with bounded retries.
    /// </summary>
    ///
    /// <param name="refreshTokenHash">The rejected refresh-token hash.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <remarks>
    /// Retries the entire transaction after concurrency conflicts to observe
    /// newly rotated sessions. Stops after
    /// <see cref="AuthSessionDefaults.RevocationSaveAttempts"/> attempts and
    /// logs a failure if revocations cannot be committed.
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
    /// Checks whether a session has reached its absolute lifetime.
    /// </summary>
    ///
    /// <param name="session">The session to check.</param>
    /// <param name="now">The time used for the comparison.</param>
    ///
    /// <returns>True if the absolute lifetime has expired; otherwise, false.</returns>
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
    /// Limits a session's expiry to its absolute lifetime.
    /// </summary>
    ///
    /// <param name="session">The session whose lifetime is being capped.</param>
    /// <param name="proposedExpiry">The expiry before applying the absolute limit.</param>
    ///
    /// <returns>The earlier of the proposed expiry and the absolute lifetime.</returns>
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
