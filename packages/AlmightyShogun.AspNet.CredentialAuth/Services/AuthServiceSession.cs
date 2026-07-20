using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Utils;
using Microsoft.AspNetCore.WebUtilities;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides authenticated session creation and refresh operations for the authentication service.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed partial class AuthService<TUser> where TUser : AuthUser
{
    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> RefreshSessionAsync(
        string refreshToken,
        HttpContext httpContext)
    {
        string? app = ResolveApp();
        SessionContext sessionContext = httpContext.GetSessionContext();

        IQueryable<UserSession> query = DatabaseContext.UserSessions
            .Where(session => session.RefreshToken == refreshToken)
            .Where(session => !session.IsRevoked && session.ExpiresAt > DateTime.UtcNow);

        if (app is not null)
            query = query.Where(session => session.App == app);

        UserSession? session = await query.FirstOrDefaultAsync();

        if (session is null || !session.IsActive)
            throw new HttpErrorException(StatusCodes.Status401Unauthorized);

        string newRefreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        UserAgent userAgent = UserAgent.Parse(sessionContext.UserAgent ?? string.Empty);

        session.Device = userAgent.Device;
        session.Browser = userAgent.Browser;
        session.RefreshToken = newRefreshToken;
        session.LastActiveAt = DateTime.UtcNow;
        session.IpAddress = sessionContext.IpAddress;
        session.UserAgent = sessionContext.UserAgent;
        session.ExpiresAt = DateTime.UtcNow.AddDays(AuthSettings.RefreshTokenDays);

        DatabaseContext.UserSessions.Update(session);
        await DatabaseContext.SaveChangesAsync();

        TUser? user = await DatabaseContext.Users.FindAsync(session.UserId);

        if (user is null)
            throw new HttpErrorException(StatusCodes.Status401Unauthorized);

        return new AuthSessionResult<TUser>
        {
            User = user,
            AccessToken = GenerateToken(user, app),
            RefreshToken = newRefreshToken
        };
    }

    /// <summary>
    /// Creates a new refresh-token session for a user.
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

        UserAgent userAgent = UserAgent.Parse(context.UserAgent ?? string.Empty);

        await DatabaseContext.UserSessions.AddAsync(new UserSession
        {
            App = app ?? string.Empty,
            UserId = user.Id,
            Device = userAgent.Device,
            Browser = userAgent.Browser,
            IpAddress = context.IpAddress,
            UserAgent = context.UserAgent,
            ExpiresAt = DateTime.UtcNow.AddDays(AuthSettings.RefreshTokenDays),
            RefreshToken = refreshToken
        });

        await DatabaseContext.SaveChangesAsync();

        return refreshToken;
    }
}
