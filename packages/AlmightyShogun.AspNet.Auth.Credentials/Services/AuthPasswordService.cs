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
/// Changes passwords and runs the reset flow. Both paths that set a password revoke the user's other sessions and retire
/// the sign-ins still waiting on a second factor, because a password that has changed should not leave access granted
/// under the old one.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, whose password column these paths read and write.</typeparam>
/// <param name="databaseContext">The application's context, which the credential tables live in.</param>
/// <param name="credentialOptions">
/// The bound credential settings, read for how long a reset token lives and for the floor a forgot-password request is
/// held to.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class AuthPasswordService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthCredentialsSettings> credentialOptions
) : IAuthPasswordService where TUser : AuthUser
{
    /// <summary>
    /// The hasher used for every password read and write, so hashing and verification cannot end up using different
    /// parameters.
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
    )
        => await ConflictRetry.RunAsync(async () =>
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
    /// Loads the one user matching a predicate, refusing rather than returning null, so every caller past this point has a
    /// user to work with.
    /// </summary>
    ///
    /// <param name="predicate">The lookup, by public identifier or by the key a reset token carries.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>
    /// The matching user, untracked. Every write on these paths is a statement of its own rather than a saved entity, so
    /// nothing here is modified, and the context the application shares with this service is left tracking what it was.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">Thrown when no user matches the predicate.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken)
    {
        TUser? user = await databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Writes a new password hash onto the user, but only while the column still holds the hash the caller verified
    /// against, so a password that moved between that verification and here is not overwritten on the strength of a
    /// check made against what it used to be.
    /// </summary>
    ///
    /// <param name="userId">The user whose password is being replaced, given as the database key.</param>
    /// <param name="verifiedPasswordHash">The hash the caller read and verified against, which the update is guarded on.</param>
    /// <param name="newPasswordHash">The hash to store.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// <c>true</c> when the row was updated, <c>false</c> when it matched nothing, which means another request wrote a
    /// password in between, or deleted the user. The caller treats that as a collision and runs the whole operation
    /// again, so the second reading verifies against the password that is actually stored.
    /// </returns>
    ///
    /// <remarks>
    /// The row is matched and written by one statement rather than loaded and saved, so nothing is staged on the context
    /// that an abandoned attempt would leave behind for the application's own save to commit.
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
    /// Claims the reset token with a guarded update, so two redemptions racing for the same link produce one success and
    /// one refusal rather than two successes.
    /// </summary>
    ///
    /// <param name="tokenId">The row to spend, as read moments earlier.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>A task that completes once the row is spent, which the database has already written when it does.</returns>
    ///
    /// <exception cref="InvalidPasswordResetTokenException">
    /// The row was no longer unspent and unexpired, so another request claimed it in between or it expired between the
    /// read and here.
    /// </exception>
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
    /// Writes the user's one reset token, replacing whatever that row held, so requesting a second link kills the first.
    /// </summary>
    ///
    /// <param name="user">The user the reset was requested for, already loaded so the token can be attached to its key.</param>
    /// <param name="requestIpAddress">
    /// The address the request came from, stored for auditing an unexpected reset. Trimmed to the column width, since it
    /// reaches here as the caller passed it.
    /// </param>
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
    /// Revokes the user's sessions, optionally sparing one, so a password change signs out every device except the one
    /// making it.
    /// </summary>
    ///
    /// <param name="userId">The user whose sessions are ending, given as the database key rather than the public identifier.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    /// <param name="exceptToken">The session to leave alone, or <c>null</c> to end every one.</param>
    ///
    /// <returns>
    /// A task that completes once the matching rows are revoked, which the database has already written when it does.
    /// The statement runs inside the transaction the caller opened, so it is rolled back with the rest of that work.
    /// </returns>
    ///
    /// <remarks>
    /// The rows are matched and stamped by one update rather than loaded and rewritten, so no session is revoked on the
    /// strength of a read another request has already invalidated, and nothing is staged on the context for an abandoned
    /// attempt to leave behind. <see cref="UserSession.IsRevoked"/> is the only column written, and it is written to a
    /// constant, so a row a rotation touched a moment earlier is revoked as that rotation left it.
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
    /// Spends the user's outstanding reset token, so a link issued before a password change cannot still be redeemed
    /// afterwards.
    /// </summary>
    ///
    /// <param name="userId">The user whose outstanding token is being spent.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// A task that completes once no unspent token remains for that user, which the database has already written when it
    /// does. The statement runs inside the transaction the caller opened, so it is rolled back with the rest of that work.
    /// </returns>
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
    /// Spends the user's outstanding two-factor challenges, so a sign-in that got past the old password cannot be
    /// finished with a code once that password is gone.
    /// </summary>
    ///
    /// <param name="userId">The user whose outstanding challenges are being retired.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// A task that completes once no unspent challenge remains for that user, which the database has already written when
    /// it does. The statement runs inside the transaction the caller opened, so it is rolled back with the rest of that work.
    /// </returns>
    ///
    /// <remarks>
    /// Retiring them is what revoking a session cannot cover: a challenge names a password that has already verified, so
    /// leaving one live would let it buy a session under a password the account no longer has.
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
    /// Finds the token a reset request presented, refusing one that is unknown, already spent, or past its expiry, so every
    /// caller past this point holds a redeemable token.
    /// </summary>
    ///
    /// <param name="token">The token as it arrived from the reset link, matched by hash.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>
    /// The redeemable token, untracked, read for the user it belongs to and for the row the caller then claims. The
    /// claim is a statement of its own rather than a saved entity, so this leaves nothing on the shared context.
    /// </returns>
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
