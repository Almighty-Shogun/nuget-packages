using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

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
    private const int _passwordLifetimeMinutes = 60;

    /// <inheritdoc />
    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request, string? currentRefreshToken = null)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        TUser user = await GetUserAsync(user => user.Id == userId);

        user.Password = _hasher.HashPassword(user, request.NewPassword);

        DatabaseContext.Users.Update(user);

        await InvalidateActiveTokenAsync(user.Id);
        await RevokeUserSessionsAsync(user.Id, currentRefreshToken);

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <inheritdoc />
    public async Task<string> RequestForgotPasswordAsync(ForgotPasswordRequest request, string? requestIpAddress = null)
    {
        TUser user = await GetUserAsync(user => user.Email == request.Email, "auth.failed");

        return await CreatePasswordResetTokenAsync(user, requestIpAddress);
    }

    /// <inheritdoc />
    public async Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        PasswordResetToken passwordToken = await FindActiveTokenAsync(request.Token);

        TUser user = await GetUserAsync(user => user.Id == passwordToken.UserId, "passwords.token");

        passwordToken.UsedAt = DateTime.UtcNow;
        user.Password = _hasher.HashPassword(user, request.NewPassword);

        DatabaseContext.Users.Update(user);
        DatabaseContext.PasswordResetTokens.Update(passwordToken);

        await RevokeUserSessionsAsync(passwordToken.UserId);
        await InvalidateActiveTokenAsync(passwordToken.UserId);

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();
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
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        List<PasswordResetToken> existingTokens = await DatabaseContext.PasswordResetTokens
            .Where(token => token.UserId == user.Id && token.UsedAt == null)
            .ToListAsync();

        if (existingTokens.Count > 0)
            DatabaseContext.PasswordResetTokens.RemoveRange(existingTokens);

        string token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48));

        await DatabaseContext.PasswordResetTokens.AddAsync(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = TokenHasher.Hash(token),
            RequestedIpAddress = requestIpAddress,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_passwordLifetimeMinutes)
        });

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();

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
        string? exceptTokenHash = exceptToken is null ? null : TokenHasher.Hash(exceptToken);

        List<UserSession> sessions = await DatabaseContext.UserSessions
            .Where(session => session.ExpiresAt > DateTime.UtcNow)
            .Where(session => !session.IsRevoked && session.UserId == userId)
            .Where(session => exceptTokenHash == null || session.RefreshTokenHash != exceptTokenHash)
            .ToListAsync();

        foreach (UserSession session in sessions)
            session.IsRevoked = true;

        DatabaseContext.UserSessions.UpdateRange(sessions);
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
        string tokenHash = TokenHasher.Hash(token);

        PasswordResetToken? passwordResetToken = await DatabaseContext.PasswordResetTokens
            .Where(passwordToken => passwordToken.ExpiresAt > DateTime.UtcNow)
            .Where(passwordToken => passwordToken.UsedAt == null && passwordToken.TokenHash == tokenHash)
            .FirstOrDefaultAsync();

        return passwordResetToken ?? throw new HttpErrorException(StatusCodes.Status401Unauthorized, "passwords.token");
    }
}
