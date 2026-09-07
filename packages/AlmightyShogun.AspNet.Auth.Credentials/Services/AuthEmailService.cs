using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Issues and redeems email verification tokens for both purposes the table carries. Redemption writes the proof onto the
/// user and nothing here reads it back, so the package records verification without ever enforcing it.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, whose address and verification columns these paths write.</typeparam>
/// <param name="databaseContext">The application's context, which the credential tables live in.</param>
/// <param name="credentialOptions">The bound credential settings, read for how long a verification token lives.</param>
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
    public async Task<string> RequestEmailChangeAsync(
        Guid identifier,
        string newEmail,
        CancellationToken cancellationToken = default
    )
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
            await MarkEmailVerifiedAsync(user.Id, now, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        });

    /// <inheritdoc />
    public async Task CompleteEmailChangeAsync(
        CompleteEmailVerificationRequest request,
        string? currentRefreshToken = null,
        CancellationToken cancellationToken = default
    )
        => await ConflictRetry.RunAsync(async () =>
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
    /// Loads the one user matching a predicate, refusing rather than returning null, so every caller past this point has a
    /// user to work with.
    /// </summary>
    ///
    /// <param name="predicate">The lookup, by public identifier or by the key a verification token carries.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>
    /// The matching user, untracked. Every write onto a user here is a statement of its own rather than a saved entity,
    /// so nothing returned is modified, and the context the application shares with this service is left tracking what
    /// it was.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">Thrown when no user matches the predicate.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken)
    {
        TUser? user = await databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Stamps the user as having proved the address they already hold, which is what redeeming a registration token
    /// records.
    /// </summary>
    ///
    /// <param name="userId">The user being stamped, given as the database key rather than the public identifier.</param>
    /// <param name="verifiedAt">The instant the proof is recorded at, shared with the token the same call spends.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// A task that completes once the stamp is written, which the database has already done when it does. The statement
    /// runs inside the transaction the caller opened, so it is rolled back with the rest of that work.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task MarkEmailVerifiedAsync(int userId, DateTimeOffset verifiedAt, CancellationToken cancellationToken)
        => await databaseContext.Users
            .Where(user => user.Id == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.EmailVerifiedAt, verifiedAt), cancellationToken);

    /// <summary>
    /// Moves the user to the address a redeemed change token carries and stamps them as having proved it, since the
    /// address they now hold is the one the link just proved reachable.
    /// </summary>
    ///
    /// <param name="userId">The user being moved, given as the database key rather than the public identifier.</param>
    /// <param name="email">The address to write, taken from the token rather than from the request.</param>
    /// <param name="verifiedAt">The instant the proof is recorded at, shared with the token the same call spends.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// A task that completes once the address is written, which the database has already done when it does. The
    /// statement runs inside the transaction the caller opened, so it is rolled back with the rest of that work.
    /// </returns>
    ///
    /// <remarks>
    /// The unique index on the address is what settles a claim made after the availability check, and it rejects this
    /// statement rather than a save, so it surfaces as the provider's own exception instead of a wrapped one.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task MoveEmailAsync(int userId, string email, DateTimeOffset verifiedAt, CancellationToken cancellationToken)
        => await databaseContext.Users
            .Where(user => user.Id == userId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(user => user.Email, email).SetProperty(user => user.EmailVerifiedAt, verifiedAt),
                cancellationToken
            );

    /// <summary>
    /// Refuses an address another account already holds, so a change is turned away with a message rather than by the
    /// unique index.
    /// </summary>
    ///
    /// <param name="email">The address being claimed, compared under the column's own collation.</param>
    /// <param name="userId">The account doing the claiming, excluded so re-confirming an address a user already holds is allowed.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>A task that completes when no other account held that address at the moment it was checked.</returns>
    ///
    /// <exception cref="EmailTakenException">Another account already holds that address.</exception>
    ///
    /// <remarks>
    /// A check here cannot make the write safe on its own, because another request may claim the same address in between.
    /// The unique index remains the guarantee; this only turns the common case into a useful message.
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
    /// Refuses a token whose address is no longer the one the account holds, so a link issued before a change of address
    /// cannot stamp the account as having proved the address it moved to.
    /// </summary>
    ///
    /// <param name="userId">The account the token was issued for, given as the database key rather than the public identifier.</param>
    /// <param name="email">The address the token carries, compared against the stored one under the column's own collation.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>A task that completes when the account still holds the address the token names.</returns>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// The account holds a different address now. Refusing with the same exception the lookup raises keeps this
    /// indistinguishable from a token that is unknown, spent, expired, or of the other purpose.
    /// </exception>
    ///
    /// <remarks>
    /// The comparison is made in the database rather than over the loaded user, so it follows the column's collation the
    /// way every other address match in the package does.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task EnsureTokenAddressCurrentAsync(int userId, string email, CancellationToken cancellationToken)
    {
        if (!await databaseContext.Users.AnyAsync(candidate => candidate.Id == userId && candidate.Email == email, cancellationToken))
            throw new InvalidEmailVerificationTokenException();
    }

    /// <summary>
    /// Retires the unspent tokens the user held for this purpose and writes a fresh one, so the newest email is the one
    /// that works and, short of two requests arriving at once, it is the only one.
    /// </summary>
    ///
    /// <param name="user">The user the token is issued for, already loaded so it can be attached to their key.</param>
    /// <param name="email">
    /// The address the token confirms, which is the user's own for a registration and the pending one for a change.
    /// </param>
    /// <param name="purpose">The flow the token belongs to, which redemption then has to match.</param>
    /// <param name="cancellationToken">Cancels the retirement and the write.</param>
    ///
    /// <returns>The token in plain text, to be emailed; only its hash is stored.</returns>
    ///
    /// <remarks>
    /// The retirement and the insert share a transaction of this method's own, so a request never ends with the previous
    /// link killed and no new one written. The cost of retiring is that a user who returns to an older message is refused,
    /// including one that would have confirmed the same address.
    ///
    /// Two requests running at once can still both insert, because the retirement of the later one need not see a row the
    /// earlier one has not committed yet and no index in the schema holds the count down. Sequential requests, which is
    /// what a user resending a link produces, do leave one.
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

        databaseContext.EmailVerificationTokens.Add(new EmailVerificationToken
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
    /// Spends the user's unspent tokens of one purpose in a single statement, leaving the tokens of the other alone.
    /// Issuing a link calls it to kill the previous one of that purpose, and completing a change of address calls it to
    /// kill the registration links, neither touching a verification in flight for the other reason.
    /// </summary>
    ///
    /// <param name="userId">The user whose outstanding tokens are being retired.</param>
    /// <param name="purpose">The purpose to retire, so the two flows do not cancel each other.</param>
    /// <param name="now">The instant stamped onto the retired rows, shared with the write that runs alongside them.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// A task that completes once the matching rows are spent, which the database has already written when it does. The
    /// statement runs inside whatever transaction the caller opened, so it is rolled back with the rest of that work.
    /// </returns>
    ///
    /// <remarks>
    /// The rows are matched and stamped by one update rather than loaded and rewritten, so no row can be retired on the
    /// strength of a read another request has already invalidated. The update bypasses the change tracker, so the rows
    /// in the database are stamped while an instance already materialized through this context keeps the
    /// <see cref="EmailVerificationToken.UsedAt"/> it was loaded with.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task RetireActiveTokensAsync(
        int userId,
        EmailVerificationPurpose purpose,
        DateTimeOffset now,
        CancellationToken cancellationToken
    )
        => await databaseContext.EmailVerificationTokens
            .Where(token => token.UserId == userId && token.Purpose == purpose && token.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UsedAt, now), cancellationToken);

    /// <summary>
    /// Finds the token a verification presented, refusing one that is unknown, already spent, past its expiry, or issued
    /// for the other purpose, so every caller past this point holds a token that endpoint may redeem.
    /// </summary>
    ///
    /// <param name="token">The token as it arrived from the verification link, matched by hash.</param>
    /// <param name="purpose">The purpose the redeeming method serves, which the row has to carry.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>
    /// The redeemable token, untracked, read for the user it belongs to and for the address it carries. The claim is a
    /// statement of its own rather than a saved entity, so this leaves nothing on the shared context.
    /// </returns>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// No live token of that purpose matches the hash. Unknown, spent, expired, and wrong-purpose are not distinguished
    /// here, and the refusals raised past this point use the same exception, so the response cannot be used to learn which
    /// tokens once existed or which flow one belongs to.
    /// </exception>
    ///
    /// <remarks>
    /// Finding a token here does not reserve it. Another request may spend it before this one does, which is why the
    /// caller claims it with a guarded update rather than trusting what this read returned.
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
    /// Claims the token with a guarded update, so two redemptions racing for the same link produce one success and one
    /// refusal rather than two successes.
    /// </summary>
    ///
    /// <param name="tokenId">The row to spend, as read moments earlier.</param>
    /// <param name="now">The instant stamped onto the row, and the instant its expiry is judged against.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>A task that completes once the row is spent, which the database has already written when it does.</returns>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// The row was no longer unspent and unexpired, so another request claimed it in between or it expired between the
    /// read and here.
    /// </exception>
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
    /// Revokes the user's live sessions, optionally sparing one, so redeeming a change of address signs the account out
    /// everywhere except on the device making the change.
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
