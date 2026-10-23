
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Issues and redeems tokens for email verification and email changes.
/// </summary>
///
/// <remarks>
/// Records email verification without enforcing it during authentication.
/// </remarks>
///
/// <typeparam name="TUser">The application's user entity.</typeparam>
/// <param name="databaseContext">The authentication database context.</param>
/// <param name="credentialOptions">The credential authentication settings.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
internal sealed class AuthEmailService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthCredentialsSettings> credentialOptions
) : IAuthEmailService where TUser : AuthUser
{
    /// <inheritdoc />
    public async Task<string> RequestVerificationAsync(Guid identifier, CancellationToken cancellationToken = default)
    {
        TUser user = await GetUserAsync(user => user.Identifier == identifier, cancellationToken);

        return await IssueTokenAsync(user, user.Email, EmailVerificationPurpose.Registration, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> RequestEmailChangeAsync(Guid identifier, string newEmail, CancellationToken cancellationToken = default)
    {
        TUser user = await GetUserAsync(user => user.Identifier == identifier, cancellationToken);

        await EnsureEmailAvailableAsync(newEmail, user.Id, cancellationToken);

        return await IssueTokenAsync(user, newEmail, EmailVerificationPurpose.EmailChange, cancellationToken);
    }

    /// <inheritdoc />
    public async Task CompleteVerificationAsync(CompleteEmailVerificationRequest request, CancellationToken cancellationToken = default)
        => await ConflictRetry.RunAsync(async () =>
        {
            EmailVerificationToken verificationToken =
                await FindActiveTokenAsync(request.Token, EmailVerificationPurpose.Registration, cancellationToken);

            TUser user = await GetUserAsync(user => user.Id == verificationToken.UserId, cancellationToken);

            await EnsureTokenAddressCurrentAsync(user.Id, verificationToken.Email, cancellationToken);

            await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

            DateTimeOffset now = DateTimeOffset.UtcNow;

            await SpendTokenAsync(verificationToken.Id, now, cancellationToken);
            await MarkEmailVerifiedAsync(user.Id, verificationToken.Email, now, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        });

    /// <inheritdoc />
    public async Task CompleteEmailChangeAsync(
        CompleteEmailVerificationRequest request,
        string? currentRefreshToken = null,
        CancellationToken cancellationToken = default
    ) => await ConflictRetry.RunAsync(async () =>
    {
        EmailVerificationToken verificationToken =
            await FindActiveTokenAsync(request.Token, EmailVerificationPurpose.EmailChange, cancellationToken);

        TUser user = await GetUserAsync(user => user.Id == verificationToken.UserId, cancellationToken);

        await EnsureEmailAvailableAsync(verificationToken.Email, user.Id, cancellationToken);

        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        await SpendTokenAsync(verificationToken.Id, now, cancellationToken);
        await RetireActiveTokensAsync(user.Id, EmailVerificationPurpose.Registration, now, cancellationToken);
        await MoveEmailAsync(user.Id, verificationToken.Email, now, cancellationToken);
        await RevokeUserSessionsAsync(user.Id, cancellationToken, currentRefreshToken);

        await transaction.CommitAsync(cancellationToken);

        return true;
    });

    /// <summary>
    /// Retrieves a user matching the specified predicate without tracking.
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
    /// <since>4.1.0</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken)
    {
        TUser? user = await databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Marks the user's current email address as verified.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="email">The email address being verified.</param>
    /// <param name="verifiedAt">The verification timestamp.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <remarks>
    /// Rejects verification if the user's email has changed.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task MarkEmailVerifiedAsync(int userId, string email, DateTimeOffset verifiedAt, CancellationToken cancellationToken)
    {
        int affectedRows = await databaseContext.Users
                .Where(user => user.Id == userId && user.Email == email)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(user => user.EmailVerifiedAt, verifiedAt),
                    cancellationToken
                );
        
            if (affectedRows != 1)
                throw new InvalidEmailVerificationTokenException();
    }

    /// <summary>
    /// Updates the user's email address and marks it as verified.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="email">The verified email address.</param>
    /// <param name="verifiedAt">The verification timestamp.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <remarks>
    /// The database's unique email index protects against concurrent claims.
    /// A constraint violation may surface as a provider-specific exception.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task MoveEmailAsync(int userId, string email, DateTimeOffset verifiedAt, CancellationToken cancellationToken)
        => await databaseContext.Users
            .Where(user => user.Id == userId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(user => user.Email, email).SetProperty(user => user.EmailVerifiedAt, verifiedAt),
                cancellationToken);

    /// <summary>
    /// Ensures another user does not already own the email address.
    /// </summary>
    ///
    /// <param name="email">The email address to check.</param>
    /// <param name="userId">The user excluded from the check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="EmailTakenException">Another user owns the address.</exception>
    ///
    /// <remarks>
    /// This preliminary check does not replace the database's unique constraint.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task EnsureEmailAvailableAsync(string email, int userId, CancellationToken cancellationToken)
    {
        if (await databaseContext.Users.AnyAsync(candidate => candidate.Email == email && candidate.Id != userId, cancellationToken))
            throw new EmailTakenException();
    }

    /// <summary>
    /// Ensures the verification token matches the user's current email.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="email">The email address stored in the token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// The user's email no longer matches the token.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task EnsureTokenAddressCurrentAsync(int userId, string email, CancellationToken cancellationToken)
    {
        if (!await databaseContext.Users.AnyAsync(candidate => candidate.Id == userId && candidate.Email == email, cancellationToken))
            throw new InvalidEmailVerificationTokenException();
    }

    /// <summary>
    /// Issues a new verification token and retires previous tokens of the same purpose.
    /// </summary>
    ///
    /// <param name="user">The user receiving the token.</param>
    /// <param name="email">The email address to verify.</param>
    /// <param name="purpose">The verification purpose.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The plaintext token; only its hash is stored.</returns>
    ///
    /// <remarks>
    /// Token retirement and issuance share a transaction.
    /// Concurrent requests may still issue multiple active tokens.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task<string> IssueTokenAsync(
        TUser user,
        string email,
        EmailVerificationPurpose purpose,
        CancellationToken cancellationToken
    )
    {
        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        await RetireActiveTokensAsync(user.Id, purpose, now, cancellationToken);

        string token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48));

        databaseContext.EmailVerificationTokens.Add(
            new EmailVerificationToken
            {
                Email = email,
                UserId = user.Id,
                Purpose = purpose,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(credentialOptions.Value.EmailVerificationMinutes),
                TokenHash = TokenHasher.Hash(token)
            });

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return token;
    }

    /// <summary>
    /// Invalidates unused verification tokens for a user and purpose.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="purpose">The token purpose to invalidate.</param>
    /// <param name="now">The invalidation timestamp.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <remarks>
    /// Uses a direct database update that bypasses EF Core change tracking.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task RetireActiveTokensAsync(
        int userId,
        EmailVerificationPurpose purpose,
        DateTimeOffset now,
        CancellationToken cancellationToken
    ) => await databaseContext.EmailVerificationTokens
        .Where(token => token.UserId == userId && token.Purpose == purpose && token.UsedAt == null)
        .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, now), cancellationToken);

    /// <summary>
    /// Retrieves an unused, unexpired verification token of the specified purpose.
    /// </summary>
    ///
    /// <param name="token">The plaintext verification token.</param>
    /// <param name="purpose">The required verification purpose.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The matching token without tracking.</returns>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// The token is invalid, expired, used, or belongs to another purpose.
    /// </exception>
    ///
    /// <remarks>
    /// Retrieval does not reserve the token; redemption requires a guarded update.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task<EmailVerificationToken> FindActiveTokenAsync(
        string token,
        EmailVerificationPurpose purpose,
        CancellationToken cancellationToken
    )
    {
        string tokenHash = TokenHasher.Hash(token);
        DateTimeOffset now = DateTimeOffset.UtcNow;

        EmailVerificationToken? verificationToken = await databaseContext.EmailVerificationTokens.AsNoTracking()
            .Where(candidate => candidate.ExpiresAt > now && candidate.Purpose == purpose)
            .Where(candidate => candidate.UsedAt == null && candidate.TokenHash == tokenHash)
            .FirstOrDefaultAsync(cancellationToken);

        return verificationToken ?? throw new InvalidEmailVerificationTokenException();
    }

    /// <summary>
    /// Atomically redeems an unused, unexpired verification token.
    /// </summary>
    ///
    /// <param name="tokenId">The token's database identifier.</param>
    /// <param name="now">The redemption timestamp.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// The token has expired or was already redeemed.
    /// </exception>
    ///
    /// <remarks>
    /// The guarded update prevents concurrent redemption of the same token.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task SpendTokenAsync(int tokenId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        int affectedRows = await databaseContext.EmailVerificationTokens
            .Where(token => token.Id == tokenId && token.UsedAt == null && token.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, now), cancellationToken);

        if (affectedRows != 1)
            throw new InvalidEmailVerificationTokenException();
    }

    /// <summary>
    /// Revokes a user's active sessions, optionally preserving one.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="exceptToken">The refresh token of the session to preserve, if any.</param>
    ///
    /// <remarks>
    /// Uses a direct database update and does not modify tracked entities.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task RevokeUserSessionsAsync(int userId, CancellationToken cancellationToken, string? exceptToken = null)
    {
        string? exceptTokenHash = exceptToken is null ? null : TokenHasher.Hash(exceptToken);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        await databaseContext.UserSessions.Where(session => session.ExpiresAt > now)
            .Where(session => !session.IsRevoked && session.UserId == userId)
            .Where(session => exceptTokenHash == null || session.RefreshTokenHash != exceptTokenHash)
            .ExecuteUpdateAsync(setters => setters.SetProperty(session => session.IsRevoked, true), cancellationToken);
    }
}
