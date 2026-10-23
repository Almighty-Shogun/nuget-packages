
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Handles password changes and password reset requests.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity.</typeparam>
/// <param name="databaseContext">The authentication database context.</param>
/// <param name="credentialOptions">The credential authentication settings.</param>
///
/// <remarks>
/// Password changes and resets revoke applicable sessions and invalidate
/// pending two-factor challenges to prevent authentication using old credentials.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class AuthPasswordService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthCredentialsSettings> credentialOptions
) : IAuthPasswordService where TUser : AuthUser
{
    /// <summary>
    /// The password hasher used for hashing and verification.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly PasswordHasher<TUser> _hasher = new();

    /// <inheritdoc />
    public async Task ChangePasswordAsync(
        Guid identifier,
        ChangePasswordRequest request,
        string? currentRefreshToken = null,
        CancellationToken cancellationToken = default
    ) => await ConflictRetry.RunAsync(async () =>
    {
        TUser user = await GetUserAsync(candidate => candidate.Identifier == identifier, cancellationToken);

        if (request.NewPassword != request.ConfirmPassword)
            throw new PasswordMismatchException();

        string storedPasswordHash = user.Password;

        if (_hasher.VerifyHashedPassword(user, storedPasswordHash, request.CurrentPassword) is PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

        if (_hasher.VerifyHashedPassword(user, storedPasswordHash, request.NewPassword) is not PasswordVerificationResult.Failed)
            throw new PasswordReusedException();

        string newPasswordHash = _hasher.HashPassword(user, request.NewPassword);

        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        if (!await SetPasswordAsync(user.Id, storedPasswordHash, newPasswordHash, cancellationToken))
            return false;

        await InvalidateActiveTokenAsync(user.Id, cancellationToken);
        await RevokeUserSessionsAsync(user.Id, cancellationToken, currentRefreshToken);
        await RetireTwoFactorChallengesAsync(user.Id, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return true;
    });

    /// <inheritdoc />
    public async Task<string?> RequestForgotPasswordAsync(
        ForgotPasswordRequest request,
        string? requestIpAddress = null,
        CancellationToken cancellationToken = default
    )
    {
        long startedAt = Stopwatch.GetTimestamp();

        TUser? user = await databaseContext.Users
            .FirstOrDefaultAsync(candidate => candidate.Email == request.Email, cancellationToken);

        string? token = user is null ? null : await CreatePasswordResetTokenAsync(user, requestIpAddress, cancellationToken);

        TimeSpan elapsed = Stopwatch.GetElapsedTime(startedAt);
        TimeSpan minimumDuration = TimeSpan.FromMilliseconds(credentialOptions.Value.ForgotPasswordMinimumMilliseconds);

        if (elapsed < minimumDuration)
            await Task.Delay(minimumDuration - elapsed, CancellationToken.None);

        return token;
    }

    /// <inheritdoc />
    public async Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request, CancellationToken cancellationToken = default)
        => await ConflictRetry.RunAsync(async () =>
        {
            PasswordResetToken passwordToken = await FindActiveTokenAsync(request.Token, cancellationToken);

            TUser user = await GetUserAsync(candidate => candidate.Id == passwordToken.UserId, cancellationToken);

            if (request.NewPassword != request.ConfirmPassword)
                throw new PasswordMismatchException();

            string storedPasswordHash = user.Password;

            if (_hasher.VerifyHashedPassword(user, storedPasswordHash, request.NewPassword) is not PasswordVerificationResult.Failed)
                throw new PasswordReusedException();

            string newPasswordHash = _hasher.HashPassword(user, request.NewPassword);

            await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

            await ClaimResetTokenAsync(passwordToken.Id, cancellationToken);

            if (!await SetPasswordAsync(user.Id, storedPasswordHash, newPasswordHash, cancellationToken))
                return false;

            await RevokeUserSessionsAsync(user.Id, cancellationToken);
            await RetireTwoFactorChallengesAsync(user.Id, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        });

    /// <summary>
    /// Retrieves a matching user without tracking.
    /// </summary>
    ///
    /// <param name="predicate">The user lookup predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The matching user.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">No matching user exists.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken)
    {
        TUser? user = await databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Updates the password only if its previously verified hash is unchanged.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="verifiedPasswordHash">The previously verified password hash.</param>
    /// <param name="newPasswordHash">The replacement password hash.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>True if updated; otherwise, false for a concurrent change or missing user.</returns>
    ///
    /// <remarks>
    /// Uses a guarded database update to prevent overwriting concurrent password changes.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<bool> SetPasswordAsync(
        int userId,
        string verifiedPasswordHash,
        string newPasswordHash,
        CancellationToken cancellationToken
    )
    {
        int affectedRows = await databaseContext.Users
            .Where(user => user.Id == userId && user.Password == verifiedPasswordHash)
            .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.Password, newPasswordHash), cancellationToken);

        return affectedRows == 1;
    }

    /// <summary>
    /// Atomically redeems an unused, unexpired password reset token.
    /// </summary>
    ///
    /// <param name="tokenId">The token's database identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="InvalidPasswordResetTokenException">
    /// The token has expired or was already redeemed.
    /// </exception>
    ///
    /// <remarks>
    /// The guarded update prevents concurrent redemption of the same token.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task ClaimResetTokenAsync(int tokenId, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        int affectedRows = await databaseContext.PasswordResetTokens
            .Where(token => token.Id == tokenId && token.UsedAt == null && token.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, now), cancellationToken);

        if (affectedRows != 1)
            throw new InvalidPasswordResetTokenException();
    }

    /// <summary>
    /// Creates or replaces the user's password reset token.
    /// </summary>
    ///
    /// <param name="user">The user requesting the reset.</param>
    /// <param name="requestIpAddress">The optional request IP address for auditing.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The plaintext token; only its hash is stored.</returns>
    ///
    /// <remarks>
    /// Uses a serializable transaction to coordinate concurrent token requests
    /// against the unique user constraint. Issuing a new token invalidates the previous one.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<string> CreatePasswordResetTokenAsync(TUser user, string? requestIpAddress, CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction =
            await databaseContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        PasswordResetToken? resetToken = await databaseContext.PasswordResetTokens
            .SingleOrDefaultAsync(token => token.UserId == user.Id, cancellationToken);

        string token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48));

        DateTimeOffset now = DateTimeOffset.UtcNow;
        DateTimeOffset expiresAt = now.AddMinutes(credentialOptions.Value.PasswordResetMinutes);

        if (resetToken is null)
        {
            resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                CreatedAt = now,
                ExpiresAt = expiresAt,
                TokenHash = TokenHasher.Hash(token),
                RequestedIpAddress = ColumnValue.Truncate(requestIpAddress, 45)
            };

            databaseContext.PasswordResetTokens.Add(resetToken);
        }
        else
        {
            resetToken.UsedAt = null;
            resetToken.CreatedAt = now;
            resetToken.ExpiresAt = expiresAt;
            resetToken.TokenHash = TokenHasher.Hash(token);
            resetToken.RequestedIpAddress = ColumnValue.Truncate(requestIpAddress, 45);
        }

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return token;
    }

    /// <summary>
    /// Revokes the user's active sessions, optionally preserving one.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="exceptToken">The refresh token of the session to preserve, if any.</param>
    ///
    /// <remarks>
    /// Uses a direct database update without modifying tracked entities.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task RevokeUserSessionsAsync(int userId, CancellationToken cancellationToken, string? exceptToken = null)
    {
        string? exceptTokenHash = exceptToken is null ? null : TokenHasher.Hash(exceptToken);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        await databaseContext.UserSessions.Where(session => session.ExpiresAt > now)
            .Where(session => !session.IsRevoked && session.UserId == userId)
            .Where(session => exceptTokenHash == null || session.RefreshTokenHash != exceptTokenHash)
            .ExecuteUpdateAsync(setters => setters.SetProperty(session => session.IsRevoked, true), cancellationToken);
    }

    /// <summary>
    /// Invalidates the user's outstanding password reset token.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task InvalidateActiveTokenAsync(int userId, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        await databaseContext.PasswordResetTokens
            .Where(token => token.UserId == userId && token.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, now), cancellationToken);
    }

    /// <summary>
    /// Invalidates pending two-factor challenges after a password change or reset.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <remarks>
    /// Prevents completing authentication initiated with an outdated password.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task RetireTwoFactorChallengesAsync(int userId, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        await databaseContext.TwoFactorChallenges
            .Where(challenge => challenge.UserId == userId && challenge.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(challenge => challenge.UsedAt, now), cancellationToken);
    }

    /// <summary>
    /// Retrieves an unused, unexpired password reset token.
    /// </summary>
    ///
    /// <param name="token">The plaintext reset token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The matching token without tracking.</returns>
    ///
    /// <exception cref="InvalidPasswordResetTokenException">
    /// The token is invalid, expired, or already used.
    /// </exception>
    ///
    /// <remarks>
    /// Retrieval does not reserve the token; redemption requires a guarded update.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<PasswordResetToken> FindActiveTokenAsync(string token, CancellationToken cancellationToken)
    {
        string tokenHash = TokenHasher.Hash(token);
        DateTimeOffset now = DateTimeOffset.UtcNow;

        PasswordResetToken? passwordResetToken = await databaseContext.PasswordResetTokens.AsNoTracking()
            .Where(passwordToken => passwordToken.ExpiresAt > now)
            .Where(passwordToken => passwordToken.UsedAt == null && passwordToken.TokenHash == tokenHash)
            .FirstOrDefaultAsync(cancellationToken);

        return passwordResetToken ?? throw new InvalidPasswordResetTokenException();
    }
}
