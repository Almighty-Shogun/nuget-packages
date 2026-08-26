using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

using Microsoft.Extensions.Options;

using AlmightyShogun.AspNet.JwtAuth;

using Microsoft.Extensions.Logging;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Creates, renews, and ends refresh-token sessions. Renewal rotates the token every time, which is what turns a stolen
/// token into something detectable rather than something that quietly works until it expires.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthSessionService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthSettings> authOptions,
    IOptions<CredentialAuthSettings> credentialOptions,
    IAppHostResolver appHostResolver,
    IAuthTokenService<TUser> tokenService,
    ILogger<AuthSessionService<TUser>> logger
) : AuthServiceBase<TUser>(databaseContext, authOptions, appHostResolver), IAuthSessionService<TUser> where TUser : AuthUser
{
    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> RefreshSessionAsync(string refreshToken, HttpContext httpContext)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        string? app = ResolveApp();
        string refreshTokenHash = TokenHasher.Hash(refreshToken);

        SessionContext sessionContext = httpContext.GetSessionContext();

        DateTimeOffset now = DateTimeOffset.UtcNow;

        IQueryable<UserSession> query = DatabaseContext.UserSessions.Where(session => session.RefreshTokenHash == refreshTokenHash)
            .Where(session => !session.IsRevoked && session.ExpiresAt > now);

        if (app is not null)
            query = query.Where(session => session.App == app);

        UserSession? session = await query.FirstOrDefaultAsync();

        if (session is null || !session.IsActive)
        {
            await DetectTokenReuseAsync(refreshTokenHash);

            throw new InvalidSessionException();
        }

        TUser user = await GetUserAsync(user => user.Id == session.UserId);

        if (!user.IsActive)
            throw new AccountDisabledException();

        await EnsureNotLockedOutAsync(user.Id, credentialOptions.Value.Lockout);

        string newRefreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        UserAgent userAgent = UserAgent.Parse(sessionContext.UserAgent ?? string.Empty);

        session.Os = Truncate(userAgent.Os, 256);
        session.Device = Truncate(userAgent.Device, 256);
        session.Browser = Truncate(userAgent.Browser, 256);
        session.LastActiveAt = DateTimeOffset.UtcNow;
        session.IpAddress = Truncate(sessionContext.IpAddress, 45);
        session.UserAgent = Truncate(sessionContext.UserAgent, 512);
        session.PreviousRefreshTokenHash = session.RefreshTokenHash;
        session.RefreshTokenHash = TokenHasher.Hash(newRefreshToken);
        session.ExpiresAt = CapToAbsoluteLifetime(session, DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(AuthSettings.RefreshTokenDays)));

        DatabaseContext.UserSessions.Update(session);

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return new AuthSessionResult<TUser>
        {
            User = user,
            RefreshToken = newRefreshToken,
            AccessToken = tokenService.GenerateToken(user, app)
        };
    }

    /// <inheritdoc />
    public async Task RevokeSessionAsync(string refreshToken)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        string refreshTokenHash = TokenHasher.Hash(refreshToken);

        UserSession? session = await DatabaseContext.UserSessions
            .Where(session => session.RefreshTokenHash == refreshTokenHash && !session.IsRevoked)
            .FirstOrDefaultAsync();

        if (session is null)
        {
            await transaction.CommitAsync();

            return;
        }

        session.IsRevoked = true;

        DatabaseContext.UserSessions.Update(session);

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Treats a refresh token that was already rotated away as stolen, and revokes every session the user holds.
    /// </summary>
    ///
    /// <param name="refreshTokenHash">The hash of the token that was presented after already being rotated away.</param>
    ///
    /// <returns>
    /// A task that completes once every session the user holds is revoked. Revoking all of them is deliberate: the token
    /// is known to be in two places, and there is no way to tell which holder is the owner.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task DetectTokenReuseAsync(string refreshTokenHash)
    {
        UserSession? rotated = await DatabaseContext.UserSessions
            .FirstOrDefaultAsync(session => session.PreviousRefreshTokenHash == refreshTokenHash);

        if (rotated is null || DateTimeOffset.UtcNow - rotated.LastActiveAt <= AuthSessionDefaults.RotationGrace)
        {
            return;
        }

        logger.LogWarning(
            "Refresh token reuse detected for user {UserId}; revoking every session for that user", rotated.UserId
        );

        List<UserSession> live = await DatabaseContext.UserSessions
            .Where(session => session.UserId == rotated.UserId && !session.IsRevoked)
            .ToListAsync();

        foreach (UserSession active in live)
        {
            active.IsRevoked = true;
        }

        await DatabaseContext.SaveChangesAsync();
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
    /// Opens a session and stores only the token's hash, pruning that user's already-expired sessions on the way so the
    /// table does not grow with rows nothing will ever read.
    /// </summary>
    ///
    /// <param name="user">The user signing in on this device.</param>
    /// <param name="app">The application the session is scoped to, or <c>null</c> when the deployment is not scoped.</param>
    /// <param name="context">The request's address and user agent, recorded so a user can recognise their own devices.</param>
    ///
    /// <returns>The token in plain text, the only time it exists in that form.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task<string> CreateSessionAsync(TUser user, string? app, SessionContext context)
    {
        string refreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        DateTimeOffset now = DateTimeOffset.UtcNow;

        List<UserSession> expired = await DatabaseContext.UserSessions
            .Where(session => session.UserId == user.Id && session.ExpiresAt <= now)
            .ToListAsync();

        if (expired.Count > 0)
        {
            DatabaseContext.UserSessions.RemoveRange(expired);
        }

        UserAgent userAgent = UserAgent.Parse(context.UserAgent ?? string.Empty);

        await DatabaseContext.UserSessions.AddAsync(new UserSession
        {
            UserId = user.Id,
            App = app,
            Os = Truncate(userAgent.Os, 256),
            Device = Truncate(userAgent.Device, 256),
            Browser = Truncate(userAgent.Browser, 256),
            IpAddress = Truncate(context.IpAddress, 45),
            UserAgent = Truncate(context.UserAgent, 512),
            RefreshTokenHash = TokenHasher.Hash(refreshToken),
            ExpiresAt = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(AuthSettings.RefreshTokenDays))
        });

        await DatabaseContext.SaveChangesAsync();

        return refreshToken;
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
