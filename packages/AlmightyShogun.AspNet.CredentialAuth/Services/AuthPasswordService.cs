using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Changes passwords and runs the reset flow. Both paths that set a password revoke the user's other sessions, because a
/// password that has changed should not leave access granted under the old one.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, whose password column these paths read and write.</typeparam>
/// <param name="databaseContext">The application's context, so auth writes join whatever transaction it is in.</param>
/// <param name="credentialOptions">
/// The bound credential settings, read for how long a reset token lives and for the floor a forgot-password request is
/// held to.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthPasswordService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<CredentialAuthSettings> credentialOptions
) : IAuthPasswordService where TUser : AuthUser
{
    /// <summary>
    /// The hasher used for every password read and write, so hashing and verification cannot end up using different
    /// parameters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly PasswordHasher<TUser> _hasher = new();

    /// <inheritdoc />
    public async Task ChangePasswordAsync(
        Guid identifier,
        ChangePasswordRequest request,
        string? currentRefreshToken = null,
        CancellationToken cancellationToken = default
    )
    {
        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        TUser user = await GetUserAsync(user => user.Identifier == identifier, cancellationToken);

        if (request.NewPassword != request.ConfirmPassword)
            throw new PasswordMismatchException();

        if (_hasher.VerifyHashedPassword(user, user.Password, request.CurrentPassword) is PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

        if (_hasher.VerifyHashedPassword(user, user.Password, request.NewPassword) is not PasswordVerificationResult.Failed)
            throw new PasswordReusedException();

        user.Password = _hasher.HashPassword(user, request.NewPassword);

        databaseContext.Users.Update(user);

        await InvalidateActiveTokenAsync(user.Id, cancellationToken);
        await RevokeUserSessionsAsync(user.Id, cancellationToken, currentRefreshToken);

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

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
    {
        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        PasswordResetToken passwordToken = await FindActiveTokenAsync(request.Token, cancellationToken);

        TUser user = await GetUserAsync(user => user.Id == passwordToken.UserId, cancellationToken);

        if (request.NewPassword != request.ConfirmPassword)
            throw new PasswordMismatchException();

        if (_hasher.VerifyHashedPassword(user, user.Password, request.NewPassword) is not PasswordVerificationResult.Failed)
            throw new PasswordReusedException();

        DateTimeOffset now = DateTimeOffset.UtcNow;

        int affectedRows = await databaseContext.PasswordResetTokens
            .Where(token => token.Id == passwordToken.Id && token.UsedAt == null && token.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, now), cancellationToken);

        if (affectedRows != 1)
            throw new InvalidPasswordResetTokenException();

        user.Password = _hasher.HashPassword(user, request.NewPassword);

        databaseContext.Users.Update(user);

        await RevokeUserSessionsAsync(passwordToken.UserId, cancellationToken);

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Loads the one user matching a predicate, refusing rather than returning null, so every caller past this point has a
    /// user to work with.
    /// </summary>
    ///
    /// <param name="predicate">The lookup, by public identifier or by the key a reset token carries.</param>
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
    /// Writes the user's one reset token, replacing whatever that row held, so requesting a second link kills the first.
    /// </summary>
    ///
    /// <param name="user">The user the reset was requested for, already loaded so the token can be attached to its key.</param>
    /// <param name="requestIpAddress">The address the request came from, stored for auditing an unexpected reset.</param>
    /// <param name="cancellationToken">Cancels the read and the write.</param>
    ///
    /// <returns>The token in plain text, to be emailed; only its hash is stored.</returns>
    ///
    /// <remarks>
    /// The transaction is serializable because the read and the insert are one decision: at read-committed, two requests
    /// for the same address can both find no row and both try to insert one, which the unique key on
    /// <see cref="PasswordResetToken.UserId"/> then rejects. Serializing them turns that into one insert followed by one
    /// update, so the loser issues a valid link instead of failing.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
                RequestedIpAddress = requestIpAddress
            };

            databaseContext.PasswordResetTokens.Add(resetToken);
        }
        else
        {
            resetToken.UsedAt = null;
            resetToken.CreatedAt = now;
            resetToken.ExpiresAt = expiresAt;
            resetToken.TokenHash = TokenHasher.Hash(token);
            resetToken.RequestedIpAddress = requestIpAddress;
        }

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return token;
    }

    /// <summary>
    /// Revokes the user's sessions, optionally sparing one, so a password change signs out every device except the one
    /// making it.
    /// </summary>
    ///
    /// <param name="userId">The user whose sessions are ending, given as the database key rather than the public identifier.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    /// <param name="exceptToken">The session to leave alone, or <c>null</c> to end every one.</param>
    ///
    /// <returns>
    /// A task that completes once the loaded sessions are marked revoked. Nothing is written until the caller saves, so
    /// the revocations land in the caller's transaction rather than in one of their own.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task RevokeUserSessionsAsync(int userId, CancellationToken cancellationToken, string? exceptToken = null)
    {
        string? exceptTokenHash = exceptToken is null ? null : TokenHasher.Hash(exceptToken);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        List<UserSession> sessions = await databaseContext.UserSessions.Where(session => session.ExpiresAt > now)
            .Where(session => !session.IsRevoked && session.UserId == userId)
            .Where(session => exceptTokenHash == null || session.RefreshTokenHash != exceptTokenHash)
            .ToListAsync(cancellationToken);

        foreach (UserSession session in sessions)
            session.IsRevoked = true;

        databaseContext.UserSessions.UpdateRange(sessions);
    }

    /// <summary>
    /// Spends the user's outstanding reset token, so a link issued before a password change cannot still be redeemed
    /// afterwards.
    /// </summary>
    ///
    /// <param name="userId">The user whose outstanding token is being spent.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>
    /// A task that completes once no unspent token remains for that user. Nothing is written until the caller saves.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task InvalidateActiveTokenAsync(int userId, CancellationToken cancellationToken)
    {
        List<PasswordResetToken> tokens = await databaseContext.PasswordResetTokens
            .Where(token => token.UserId == userId && token.UsedAt == null)
            .ToListAsync(cancellationToken);

        foreach (PasswordResetToken token in tokens)
            token.UsedAt = DateTimeOffset.UtcNow;

        databaseContext.PasswordResetTokens.UpdateRange(tokens);
    }

    /// <summary>
    /// Finds the token a reset request presented, refusing one that is unknown, already spent, or past its expiry, so every
    /// caller past this point holds a redeemable token.
    /// </summary>
    ///
    /// <param name="token">The token as it arrived from the reset link, matched by hash.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>The redeemable token, tracked so the caller can read the user it belongs to.</returns>
    ///
    /// <exception cref="InvalidPasswordResetTokenException">
    /// No live token matches the hash. Unknown, spent, and expired are not distinguished, so the response cannot be
    /// used to learn which tokens once existed.
    /// </exception>
    ///
    /// <remarks>
    /// Finding a token here does not reserve it. Another request may spend it before this one does, which is why the
    /// caller claims it with a guarded update rather than trusting what this read returned.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<PasswordResetToken> FindActiveTokenAsync(string token, CancellationToken cancellationToken)
    {
        string tokenHash = TokenHasher.Hash(token);
        DateTimeOffset now = DateTimeOffset.UtcNow;

        PasswordResetToken? passwordResetToken = await databaseContext.PasswordResetTokens
            .Where(passwordToken => passwordToken.ExpiresAt > now)
            .Where(passwordToken => passwordToken.UsedAt == null && passwordToken.TokenHash == tokenHash)
            .FirstOrDefaultAsync(cancellationToken);

        return passwordResetToken ?? throw new InvalidPasswordResetTokenException();
    }
}
