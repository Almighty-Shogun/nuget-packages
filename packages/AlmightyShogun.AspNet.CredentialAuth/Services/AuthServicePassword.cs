using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides password change and forgot-password operations for the authentication service.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed partial class AuthService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The default lifetime in minutes for password reset tokens.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const int PasswordLifetimeMinutes = 60;

    /// <inheritdoc />
    public async Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request,
        string? currentRefreshToken = null)
    {
        TUser? user = await DatabaseContext.Users.FindAsync(userId);

        if (user is null)
            throw new HttpErrorException(StatusCodes.Status401Unauthorized);

        if (!PasswordMatches(user, request.CurrentPassword))
            throw new HttpErrorException(StatusCodes.Status401Unauthorized, "passwords.current");

        if (PasswordMatches(user, request.NewPassword))
            throw new HttpErrorException(StatusCodes.Status422UnprocessableEntity, "passwords.reused");

        user.Password = _hasher.HashPassword(user, request.NewPassword);

        DatabaseContext.Users.Update(user);
        await DatabaseContext.SaveChangesAsync();

        await InvalidateActiveTokenAsync(user.Id);
        await RevokeUserSessionsAsync(user.Id, currentRefreshToken);
    }

    /// <inheritdoc />
    public async Task<string> RequestForgotPasswordAsync(
        ForgotPasswordRequest request,
        string? requestIpAddress = null)
    {
        TUser? user = await DatabaseContext.Users.FirstOrDefaultAsync(user => user.Email == request.Email);

        if (user is null)
            throw new HttpErrorException(StatusCodes.Status401Unauthorized);

        return await CreatePasswordResetTokenAsync(user, requestIpAddress);
    }

    /// <inheritdoc />
    public async Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request)
    {
        PasswordResetToken passwordToken = await FindActiveTokenAsync(request.Token);

        TUser? user = await DatabaseContext.Users.FindAsync(passwordToken.UserId);

        if (user is null)
            throw new HttpErrorException(StatusCodes.Status401Unauthorized);

        if (PasswordMatches(user, request.NewPassword))
            throw new HttpErrorException(StatusCodes.Status422UnprocessableEntity, "passwords.reused");

        passwordToken.UsedAt = DateTime.UtcNow;
        user.Password = _hasher.HashPassword(user, request.NewPassword);

        DatabaseContext.Users.Update(user);
        DatabaseContext.PasswordResetTokens.Update(passwordToken);
        await DatabaseContext.SaveChangesAsync();

        await RevokeUserSessionsAsync(passwordToken.UserId);
        await InvalidateActiveTokenAsync(passwordToken.UserId);
    }

    /// <summary>
    /// Creates a fresh password reset token and removes any existing active tokens for the user.
    /// </summary>
    ///
    /// <param name="user">The user that requested the password reset.</param>
    /// <param name="requestIpAddress">The IP address that requested the password reset, if available.</param>
    ///
    /// <returns>The plain reset token that should be sent to the user.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<string> CreatePasswordResetTokenAsync(TUser user, string? requestIpAddress)
    {
        List<PasswordResetToken> existingTokens = await DatabaseContext.PasswordResetTokens
            .Where(token => token.UserId == user.Id && token.UsedAt == null)
            .ToListAsync();

        if (existingTokens.Count > 0)
        {
            DatabaseContext.PasswordResetTokens.RemoveRange(existingTokens);
            await DatabaseContext.SaveChangesAsync();
        }

        string token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48));

        await DatabaseContext.PasswordResetTokens.AddAsync(new PasswordResetToken
        {
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(PasswordLifetimeMinutes),
            TokenHash = PasswordResetTokenHasher.Hash(token),
            RequestedIpAddress = requestIpAddress
        });

        await DatabaseContext.SaveChangesAsync();

        return token;
    }

    /// <summary>
    /// Revokes active sessions for a user after a password-sensitive operation.
    /// </summary>
    ///
    /// <param name="userId">The identifier of the user whose sessions should be revoked.</param>
    /// <param name="exceptToken">The refresh token that should remain active, if available.</param>
    ///
    /// <returns>A task representing the asynchronous revoke operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task RevokeUserSessionsAsync(int userId, string? exceptToken = null)
    {
        List<UserSession> sessions = await DatabaseContext.UserSessions
            .Where(session => session.UserId == userId && !session.IsRevoked && session.ExpiresAt > DateTime.UtcNow)
            .Where(session => exceptToken == null || session.RefreshToken != exceptToken)
            .ToListAsync();

        foreach (UserSession session in sessions)
            session.IsRevoked = true;

        DatabaseContext.UserSessions.UpdateRange(sessions);
        await DatabaseContext.SaveChangesAsync();
    }

    /// <summary>
    /// Marks all active password reset tokens for a user as used.
    /// </summary>
    ///
    /// <param name="userId">The identifier of the user whose tokens should be invalidated.</param>
    ///
    /// <returns>A task representing the asynchronous invalidation operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task InvalidateActiveTokenAsync(int userId)
    {
        List<PasswordResetToken> tokens = await DatabaseContext.PasswordResetTokens
            .Where(token => token.UserId == userId && token.UsedAt == null)
            .ToListAsync();

        foreach (PasswordResetToken token in tokens)
            token.UsedAt = DateTime.UtcNow;

        DatabaseContext.PasswordResetTokens.UpdateRange(tokens);
        await DatabaseContext.SaveChangesAsync();
    }

    /// <summary>
    /// Finds an unused and unexpired password reset token by hash.
    /// </summary>
    ///
    /// <param name="token">The plain password reset token.</param>
    ///
    /// <returns>The active password reset token.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<PasswordResetToken> FindActiveTokenAsync(string token)
    {
        string tokenHash = PasswordResetTokenHasher.Hash(token);

        PasswordResetToken? passwordResetToken = await DatabaseContext.PasswordResetTokens
            .FirstOrDefaultAsync(prt => prt.TokenHash == tokenHash && prt.UsedAt == null && prt.ExpiresAt > DateTime.UtcNow);

        return passwordResetToken ?? throw new HttpErrorException(StatusCodes.Status401Unauthorized, "passwords.token");
    }
}
