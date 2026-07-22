using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides authenticated session creation, refresh, and revoke operations for the authentication service.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed partial class AuthService<TUser> where TUser : AuthUser
{
    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> RefreshSessionAsync(string refreshToken, HttpContext httpContext)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        string? app = ResolveApp();
        string refreshTokenHash = TokenHasher.Hash(refreshToken);

        SessionContext sessionContext = httpContext.GetSessionContext();

        IQueryable<UserSession> query = DatabaseContext.UserSessions
            .Where(session => session.RefreshTokenHash == refreshTokenHash)
            .Where(session => !session.IsRevoked && session.ExpiresAt > DateTime.UtcNow);

        if (app is not null)
            query = query.Where(session => session.App == app);

        UserSession? session = await query.FirstOrDefaultAsync();

        if (session is null || !session.IsActive)
            throw new HttpErrorException(StatusCodes.Status401Unauthorized);

        TUser user = await GetUserAsync(user => user.Id == session.UserId);

        string newRefreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        var userAgent = UserAgent.Parse(sessionContext.UserAgent ?? string.Empty);

        session.Device = userAgent.Device;
        session.Browser = userAgent.Browser;
        session.LastActiveAt = DateTime.UtcNow;
        session.IpAddress = sessionContext.IpAddress;
        session.UserAgent = sessionContext.UserAgent;
        session.RefreshTokenHash = TokenHasher.Hash(newRefreshToken);
        session.ExpiresAt = DateTime.UtcNow.AddDays(AuthSettings.RefreshTokenDays);

        DatabaseContext.UserSessions.Update(session);

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return new AuthSessionResult<TUser>
        {
            User = user,
            RefreshToken = newRefreshToken,
            AccessToken = GenerateToken(user, app)
        };
    }

    /// <inheritdoc />
    public async Task RevokeSessionAsync(string refreshToken)
    {
        string refreshTokenHash = TokenHasher.Hash(refreshToken);

        UserSession? session = await DatabaseContext.UserSessions
            .Where(session => session.RefreshTokenHash == refreshTokenHash)
            .Where(session => !session.IsRevoked && session.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (session is null)
            return;

        session.IsRevoked = true;

        DatabaseContext.UserSessions.Update(session);

        await DatabaseContext.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new refresh-token session for a user and stores the token hash.
    /// </summary>
    ///
    /// <param name="user">The user that owns the session.</param>
    /// <param name="app">The resolved application name for the session, if available.</param>
    /// <param name="context">The session context containing device and request metadata.</param>
    ///
    /// <returns>The generated refresh token.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<string> CreateSessionAsync(TUser user, string? app, SessionContext context)
    {
        string refreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        var userAgent = UserAgent.Parse(context.UserAgent ?? string.Empty);

        await DatabaseContext.UserSessions.AddAsync(new UserSession
        {
            UserId = user.Id,
            App = app ?? string.Empty,
            Device = userAgent.Device,
            Browser = userAgent.Browser,
            IpAddress = context.IpAddress,
            UserAgent = context.UserAgent,
            RefreshTokenHash = TokenHasher.Hash(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(AuthSettings.RefreshTokenDays)
        });

        await DatabaseContext.SaveChangesAsync();

        return refreshToken;
    }
}
