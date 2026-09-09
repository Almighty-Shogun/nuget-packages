namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Issues and redeems the tokens that prove an address belongs to the account holding it, both for a sign-up and for a
/// change of address. Redemption records the proof on the user and never acts on it, so whether an unverified account may
/// sign in stays the application's decision.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public interface IAuthEmailService
{
    /// <summary>
    /// Issues a token for the address the user already holds, retiring the unspent registration tokens that user had. The
    /// caller emails it to that address.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user whose address should be confirmed.</param>
    /// <param name="cancellationToken">Cancels the lookup and the write.</param>
    ///
    /// <returns>
    /// The token in plain text, the only time it exists in that form. Never <c>null</c>, and never padded to a timing
    /// floor: the caller is acting for a user it already knows exists, so there is no account enumeration to resist.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">The identifier matches no account.</exception>
    ///
    /// <remarks>
    /// Issuing for an already verified user is allowed and writes a new token, so a resend needs no state check first. The
    /// retirement it performs is what makes an older link stop working, including one the user is still holding.
    ///
    /// The retirement and the insert share a transaction, so a resend that fails leaves the previous link alive. Requests
    /// made one after another therefore leave a single live token; two arriving at once can leave both, since nothing in
    /// the schema enforces the limit.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Task<string> RequestVerificationAsync(Guid identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Issues a token for an address the user wants to move to, retiring the unspent change tokens that user had. The caller
    /// emails it to the address the request names rather than to whatever the account holds now, since the point is to
    /// prove that address is reachable.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user asking to change address.</param>
    /// <param name="newEmail">
    /// The address to move to, stored on the token and written onto the user only once the token is redeemed. Whatever
    /// else an application demands before allowing a change, a current password, a second factor, or a cooling-off period,
    /// is the caller's to check before calling: this receives an address and trusts the request was already allowed.
    /// </param>
    /// <param name="cancellationToken">Cancels the lookups and the write.</param>
    ///
    /// <returns>The token in plain text, the only time it exists in that form.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">The identifier matches no account.</exception>
    /// <exception cref="EmailTakenException">Another account already holds that address under the database's collation.</exception>
    ///
    /// <remarks>
    /// The address is checked here only to refuse the obvious case early. It is checked again at redemption, because
    /// nothing holds it between the two.
    ///
    /// The retirement and the insert share a transaction, so a request that fails leaves the previous link alive. Requests
    /// made one after another therefore leave a single live token; two arriving at once can leave both, each carrying the
    /// address it was asked for.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Task<string> RequestEmailChangeAsync(Guid identifier, string newEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Spends a registration token and stamps the user as verified. The address is left alone, since for this purpose the
    /// token confirms the one the user already holds.
    /// </summary>
    ///
    /// <param name="request">The token from the verification email.</param>
    /// <param name="cancellationToken">Cancels the database work, rolling the verification back with the transaction.</param>
    ///
    /// <returns>A task that completes once the token is spent and the user is stamped.</returns>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// The token is unknown, already spent, past its expiry, was issued for a change of address, or names an address the
    /// account no longer holds. Also thrown when a concurrent request spent it after this one read it, because the token is
    /// claimed with a guarded update rather than on the strength of that read.
    /// </exception>
    /// <exception cref="InvalidCredentialsException">
    /// The token names a user the query no longer finds, which a deletion between the two reads can produce.
    /// </exception>
    /// <exception cref="ConcurrentSessionUpdateException">
    /// Every attempt lost a race with a concurrent write to the token's row or the user's, which another sign-in rehashing
    /// that password or a request issuing a fresh link is enough to cause. Nothing was changed and the token is still
    /// unspent, so the same link still works.
    /// </exception>
    ///
    /// <remarks>
    /// This opens a transaction of its own, so spending the token and stamping the user land together or not at all.
    /// Nothing else is inside it: the token, the user, and the address check are read before it opens, so the window a
    /// concurrent write has to land in is the two updates rather than the request.
    ///
    /// A write to either of those rows committing while that transaction is open makes it lose, and the whole thing is
    /// read and written again from the start a bounded number of times before it is given up on. A token spent or an
    /// address moved in the meantime is refused on the attempt that finds it, since each attempt re-reads.
    ///
    /// The token's address has to still be the account's, so a link issued before the address moved cannot stamp the
    /// account as having proved the address it moved to. That address is compared under the column's own collation.
    ///
    /// Redemption is not idempotent: a spent token is no longer active, so a second click on the same link fails the same
    /// way an expired one does.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Task CompleteVerificationAsync(CompleteEmailVerificationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Spends a change token, moves the user to the address it carries, and stamps them as verified, since the address they
    /// now hold is the one they just proved. The other sessions are revoked whenever a change is redeemed, since one can
    /// move the identifier the account signs in with, and a change writing the address already held is not special-cased.
    /// </summary>
    ///
    /// <param name="request">The token from the verification email sent to the new address.</param>
    /// <param name="currentRefreshToken">
    /// The session to spare, so the device making the change is not signed out of the action it just completed. Omit it to
    /// sign the user out everywhere.
    /// </param>
    /// <param name="cancellationToken">Cancels the database work, rolling the change back with the transaction.</param>
    ///
    /// <returns>
    /// A task that completes once the token is spent, the outstanding registration links retired, the address written, and
    /// the other sessions revoked.
    /// </returns>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// The token is unknown, already spent, past its expiry, or was issued for a registration. Also thrown when a
    /// concurrent request spent it after this one read it.
    /// </exception>
    /// <exception cref="InvalidCredentialsException">
    /// The token names a user the query no longer finds, which a deletion between the two reads can produce.
    /// </exception>
    /// <exception cref="EmailTakenException">
    /// Another account claimed the address between the request and this redemption, which the check at request time cannot
    /// prevent.
    /// </exception>
    /// <exception cref="ConcurrentSessionUpdateException">
    /// Every attempt lost a race with a concurrent write to the same rows, which a refresh on another of the user's
    /// devices is enough to cause. Nothing was changed and the token is still unspent, so the same link still works.
    /// </exception>
    /// <exception cref="System.Data.Common.DbException">
    /// The write failed at the database for a reason other than that race. An address claimed between the re-check and
    /// the write surfaces this way rather than as <see cref="EmailTakenException"/>, since the unique index on the column
    /// is what settles that one. The address is written by a statement rather than by a save, so the provider's own
    /// exception arrives with nothing wrapped around it.
    /// </exception>
    ///
    /// <remarks>
    /// This opens a transaction of its own, so spending the token, writing the address, and revoking the sessions land
    /// together or not at all. Nothing else is inside it: the token, the user, and the availability of the address are
    /// read before it opens, so the window a concurrent write has to land in is the four updates rather than the request.
    ///
    /// The user's unspent registration links are retired in that same transaction, so none of them is still redeemable
    /// afterwards, including one naming the address the account has just moved to.
    ///
    /// A refresh of one of the user's other sessions committing while that transaction is open makes it lose, and the
    /// whole thing is read and written again from the start a bounded number of times before it is given up on. An
    /// address taken in the meantime is refused on the attempt that finds it, since each attempt re-checks.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Task CompleteEmailChangeAsync(
        CompleteEmailVerificationRequest request,
        string? currentRefreshToken = null,
        CancellationToken cancellationToken = default
    );
}
