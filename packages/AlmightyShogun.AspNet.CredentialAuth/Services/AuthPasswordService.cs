using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.JwtAuth;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Changes passwords and runs the reset flow. Both paths that set a password revoke the user's other sessions, because a
/// password that has changed should not leave access granted under the old one.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthPasswordService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthSettings> authOptions,
    IOptions<CredentialAuthSettings> credentialOptions,
    IAppHostResolver appHostResolver
) : AuthServiceBase<TUser>(databaseContext, authOptions, appHostResolver), IAuthPasswordService where TUser : AuthUser
{
    /// <inheritdoc />
    public async Task ChangePasswordAsync(Guid identifier, ChangePasswordRequest request, string? currentRefreshToken = null)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        TUser user = await GetUserAsync(user => user.Identifier == identifier);

        if (request.NewPassword != request.ConfirmPassword)
            throw new PasswordMismatchException();

        if (Hasher.VerifyHashedPassword(user, user.Password, request.CurrentPassword) is PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

        if (Hasher.VerifyHashedPassword(user, user.Password, request.NewPassword) is not PasswordVerificationResult.Failed)
            throw new PasswordReusedException();

        user.Password = Hasher.HashPassword(user, request.NewPassword);

        DatabaseContext.Users.Update(user);

        await InvalidateActiveTokenAsync(user.Id);
        await RevokeUserSessionsAsync(user.Id, currentRefreshToken);

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <inheritdoc />
    public async Task<string?> RequestForgotPasswordAsync(ForgotPasswordRequest request, string? requestIpAddress = null)
    {
        TUser? user = await DatabaseContext.Users.FirstOrDefaultAsync(candidate => candidate.Email == request.Email);

        if (user is not null)
            return await CreatePasswordResetTokenAsync(user, requestIpAddress);

        await Task.Delay(Random.Shared.Next(80, 160));

        return null;
    }

    /// <inheritdoc />
    public async Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        PasswordResetToken passwordToken = await FindActiveTokenAsync(request.Token);

        TUser user = await GetUserAsync(user => user.Id == passwordToken.UserId);

        if (request.NewPassword != request.ConfirmPassword)
            throw new PasswordMismatchException();

        if (Hasher.VerifyHashedPassword(user, user.Password, request.NewPassword) is not PasswordVerificationResult.Failed)
            throw new PasswordReusedException();

        passwordToken.UsedAt = DateTimeOffset.UtcNow;
        user.Password = Hasher.HashPassword(user, request.NewPassword);

        DatabaseContext.Users.Update(user);
        DatabaseContext.PasswordResetTokens.Update(passwordToken);

        await RevokeUserSessionsAsync(passwordToken.UserId);
        await InvalidateActiveTokenAsync(passwordToken.UserId);

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Issues a reset token and deletes any unspent one the user already had, so a second request invalidates the first
    /// link rather than leaving two working at once.
    /// </summary>
    ///
    /// <param name="user">The user the reset was requested for, already loaded so the token can be attached to its key.</param>
    /// <param name="requestIpAddress">The address the request came from, stored for auditing an unexpected reset.</param>
    ///
    /// <returns>The token in plain text, to be emailed; only its hash is stored.</returns>
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
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(credentialOptions.Value.PasswordResetMinutes)
        });

        await DatabaseContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return token;
    }

    /// <summary>
    /// Revokes the user's sessions, optionally sparing one, so a password change signs out every device except the one
    /// making it.
    /// </summary>
    ///
    /// <param name="userId">The user whose sessions are ending, given as the database key rather than the public identifier.</param>
    /// <param name="exceptToken">The session to leave alone, or <c>null</c> to end every one.</param>
    ///
    /// <returns>A task that completes once the sessions can no longer be refreshed.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task RevokeUserSessionsAsync(int userId, string? exceptToken = null)
    {
        string? exceptTokenHash = exceptToken is null ? null : TokenHasher.Hash(exceptToken);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        List<UserSession> sessions = await DatabaseContext.UserSessions.Where(session => session.ExpiresAt > now)
            .Where(session => !session.IsRevoked && session.UserId == userId)
            .Where(session => exceptTokenHash == null || session.RefreshTokenHash != exceptTokenHash)
            .ToListAsync();

        foreach (UserSession session in sessions)
            session.IsRevoked = true;

        DatabaseContext.UserSessions.UpdateRange(sessions);
    }

    /// <summary>
    /// Spends every outstanding reset token for a user, so a link issued before a password change cannot still be redeemed
    /// afterwards.
    /// </summary>
    ///
    /// <param name="userId">The user whose outstanding tokens are being spent.</param>
    ///
    /// <returns>A task that completes once no unspent token remains for that user.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task InvalidateActiveTokenAsync(int userId)
    {
        List<PasswordResetToken> tokens = await DatabaseContext.PasswordResetTokens
            .Where(token => token.UserId == userId && token.UsedAt == null)
            .ToListAsync();

        foreach (PasswordResetToken token in tokens)
            token.UsedAt = DateTimeOffset.UtcNow;

        DatabaseContext.PasswordResetTokens.UpdateRange(tokens);
    }

    /// <summary>
    /// Finds the token a reset request presented, refusing one that is unknown, already spent, or past its expiry, so every
    /// caller past this point holds a redeemable token.
    /// </summary>
    ///
    /// <param name="token">The token as it arrived from the reset link, matched by hash.</param>
    ///
    /// <returns>The redeemable token, tracked so the caller can mark it spent.</returns>
    ///
    /// <exception cref="InvalidPasswordResetTokenException">
    /// No live token matches the hash. Unknown, spent, and expired are not distinguished, so the response cannot be
    /// used to learn which tokens once existed.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<PasswordResetToken> FindActiveTokenAsync(string token)
    {
        string tokenHash = TokenHasher.Hash(token);
        DateTimeOffset now = DateTimeOffset.UtcNow;

        PasswordResetToken? passwordResetToken = await DatabaseContext.PasswordResetTokens
            .Where(passwordToken => passwordToken.ExpiresAt > now)
            .Where(passwordToken => passwordToken.UsedAt == null && passwordToken.TokenHash == tokenHash)
            .FirstOrDefaultAsync();

        return passwordResetToken ?? throw new InvalidPasswordResetTokenException();
    }
}
