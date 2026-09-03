using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using AlmightyShogun.AspNet.JwtAuth;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Creates, renews, and ends refresh-token sessions. Renewal rotates the token every time and remembers the one it
/// replaced, so presenting that spent token afterwards is recognised as a replay and ends every session the user holds.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, returned alongside the tokens a session yields.</typeparam>
/// <param name="databaseContext">The application's context, so auth writes join whatever transaction it is in.</param>
/// <param name="authOptions">The bound JWT settings, read for how long a refresh token lives.</param>
/// <param name="credentialOptions">
/// The bound credential settings, read for the lockout policy and for the absolute lifetime a renewal is capped at.
/// </param>
/// <param name="appHostResolver">
/// The resolver deciding which application the current request belongs to, so a session is scoped to the host the user
/// actually signed in through.
/// </param>
/// <param name="tokenGenerator">
/// The JWT package's generator, which signs and stamps issuer, audience, and expiry over the claims built here.
/// </param>
/// <param name="logger">Where a detected token replay is recorded, since nothing is returned to the caller about it.</param>
///
/// <remarks>
/// Only one step of the chain is remembered. A session that has rotated <c>a</c> to <c>b</c> to <c>c</c> holds
/// <c>b</c> as its previous token, so replaying <c>b</c> is detected while replaying <c>a</c> reads as an unknown token
/// and is refused without revoking anything. Detection therefore covers the token most recently spent, which is the one
/// a thief racing the legitimate client would hold, and not every token the session has ever issued.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthSessionService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthSettings> authOptions,
    IOptions<CredentialAuthSettings> credentialOptions,
    IAppHostResolver appHostResolver,
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
            {
                await databaseContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }

            throw new InvalidSessionException();
        }

        TUser user = await GetUserAsync(user => user.Id == session.UserId, cancellationToken);

        if (!user.IsActive)
            throw new AccountDisabledException();

        await EnsureNotLockedOutAsync(user.Id, credentialOptions.Value.Lockout, cancellationToken);

        string newRefreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        UserAgent userAgent = UserAgent.Parse(clientContext.UserAgent ?? string.Empty);

        session.Os = Truncate(userAgent.Os, 256);
        session.Device = Truncate(userAgent.Device, 256);
        session.Browser = Truncate(userAgent.Browser, 256);
        session.LastActiveAt = DateTimeOffset.UtcNow;
        session.IpAddress = Truncate(clientContext.IpAddress, 45);
        session.UserAgent = Truncate(clientContext.UserAgent, 512);
        session.PreviousRefreshTokenHash = session.RefreshTokenHash;
        session.RefreshTokenHash = TokenHasher.Hash(newRefreshToken);
        session.ConcurrencyToken = Guid.NewGuid();
        session.ExpiresAt = CapToAbsoluteLifetime(
            session, DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(authOptions.Value.RefreshTokenDays))
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
    public async Task<string> CreateSessionAsync(
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
        {
            databaseContext.UserSessions.RemoveRange(expired);
        }

        UserAgent userAgent = UserAgent.Parse(context.UserAgent ?? string.Empty);

        await databaseContext.UserSessions.AddAsync(new UserSession
        {
            UserId = user.Id,
            App = app,
            Os = Truncate(userAgent.Os, 256),
            Device = Truncate(userAgent.Device, 256),
            Browser = Truncate(userAgent.Browser, 256),
            IpAddress = Truncate(context.IpAddress, 45),
            UserAgent = Truncate(context.UserAgent, 512),
            RefreshTokenHash = TokenHasher.Hash(refreshToken),
            ExpiresAt = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(authOptions.Value.RefreshTokenDays))
        }, cancellationToken);

        await databaseContext.SaveChangesAsync(cancellationToken);

        return refreshToken;
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
    /// <since>Unreleased</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken)
    {
        TUser? user = await databaseContext.Users.FirstOrDefaultAsync(predicate, cancellationToken);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Loads the lockout row for a user and refuses when it is in force. Does nothing at all when lockout is disabled,
    /// so a deployment that never uses it pays no query for the check.
    /// </summary>
    ///
    /// <param name="userId">The user being let in, given as the database key rather than the public identifier.</param>
    /// <param name="policy">The configured policy, read for whether the feature is on at all.</param>
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
    /// <since>Unreleased</since>
    private async Task EnsureNotLockedOutAsync(int userId, LockoutPolicy policy, CancellationToken cancellationToken)
    {
        if (!policy.Enabled)
            return;

        UserLockout? lockout = await databaseContext.UserLockouts
            .FirstOrDefaultAsync(candidate => candidate.UserId == userId, cancellationToken);

        if (lockout is not null && lockout.IsLocked)
            throw new AccountLockedException(lockout.LockoutEnd!.Value);
    }

    /// <summary>
    /// Treats a refresh token that was already rotated away as stolen, and marks every session the user holds revoked.
    /// </summary>
    ///
    /// <param name="refreshTokenHash">The hash of the token that was presented after already being rotated away.</param>
    /// <param name="cancellationToken">Cancels the lookups.</param>
    ///
    /// <returns>
    /// <c>true</c> when a replay was found and the user's live sessions were marked revoked, <c>false</c> when the hash
    /// matches no rotated session or the rotation is still inside the grace window.
    /// </returns>
    ///
    /// <remarks>
    /// Nothing is saved or committed here. The caller runs inside a transaction of its own and is about to throw, so it
    /// alone decides whether the revocations are written; saving here would commit a partial write on a path that fails.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<bool> DetectTokenReuseAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        UserSession? rotated = await databaseContext.UserSessions
            .FirstOrDefaultAsync(session => session.PreviousRefreshTokenHash == refreshTokenHash, cancellationToken);

        if (rotated is null || DateTimeOffset.UtcNow - rotated.LastActiveAt <= AuthSessionDefaults.RotationGrace)
        {
            return false;
        }

        logger.LogWarning(
            "Refresh token reuse detected for user {UserId}; revoking every session for that user", rotated.UserId
        );

        List<UserSession> live = await databaseContext.UserSessions
            .Where(session => session.UserId == rotated.UserId && !session.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (UserSession active in live)
        {
            active.IsRevoked = true;
        }

        return true;
    }

    /// <summary>
    /// Caps a session expiry at the absolute lifetime measured from when the session was created, so refreshing cannot
    /// extend a session indefinitely.
    /// </summary>
    ///
    /// <param name="session">The session being renewed, read for when it was originally created.</param>
    /// <param name="proposedExpiry">The expiry a plain sliding window would give, before the absolute cap is applied.</param>
    ///
    /// <returns>
    /// The earlier of the proposed expiry and the absolute limit, so continuous use cannot extend one sign-in forever.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private DateTimeOffset CapToAbsoluteLifetime(UserSession session, DateTimeOffset proposedExpiry)
    {
        if (credentialOptions.Value.AbsoluteSessionLifetimeDays is not { } days)
        {
            return proposedExpiry;
        }

        DateTimeOffset absoluteEnd = session.CreatedAt.AddDays(days);

        return proposedExpiry > absoluteEnd ? absoluteEnd : proposedExpiry;
    }

    /// <summary>
    /// Trims a value to the column length. A header longer than its column otherwise fails the insert with a database
    /// error, and both the IP address and the User-Agent come straight from the request.
    /// </summary>
    ///
    /// <param name="value">The value as it arrived, normally straight off a request header.</param>
    /// <param name="maxLength">The column width, which is what the value has to fit.</param>
    ///
    /// <returns>The value, trimmed when it exceeds the column length.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string? Truncate(string? value, int maxLength)
        => value is null || value.Length <= maxLength ? value : value[..maxLength];
}
