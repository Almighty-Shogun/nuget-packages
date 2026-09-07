namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Changes passwords, both for a signed-in user and through a reset link. Both paths that set a password revoke the
/// user's other sessions and retire the sign-ins waiting on a second factor, so a change actually ends access that used
/// the old one.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IAuthPasswordService
{
    /// <summary>
    /// Changes a signed-in user's password, refusing in that order a confirmation that does not match, a wrong current
    /// password, and a replacement that verifies against the one already stored.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user whose password should be changed.</param>
    /// <param name="request">The current password, the replacement, and its confirmation.</param>
    /// <param name="currentRefreshToken">
    /// The session to spare, so the device making the change stays signed in while every other is revoked. Omit it to
    /// sign the user out everywhere.
    /// </param>
    /// <param name="cancellationToken">Cancels the database work, rolling the change back with the transaction.</param>
    ///
    /// <returns>
    /// A task that completes once the password is changed, the other sessions are revoked, and any sign-in still waiting
    /// on a second factor is retired.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">
    /// The identifier matches no account, or the current password is wrong. The exception does not distinguish the two,
    /// but the work does: an unknown identifier is refused before any hash is verified, while a wrong password costs one
    /// verification and is refused before the replacement is hashed. Another request changing the password while this one
    /// is running ends here as well, since the attempt that follows verifies against the hash that is then stored.
    /// Nothing here counts a failure towards the lockout, so guesses against this route are unmetered.
    /// </exception>
    /// <exception cref="PasswordMismatchException">The confirmation differs from the replacement.</exception>
    /// <exception cref="PasswordReusedException">
    /// The replacement verifies against the password already stored, so the change would change nothing.
    /// </exception>
    /// <exception cref="ConcurrentSessionUpdateException">
    /// Every attempt lost a race with a concurrent write to the same rows, which a refresh on another of the user's
    /// devices is enough to cause. Nothing was changed, so the same request may simply be sent again.
    /// </exception>
    ///
    /// <remarks>
    /// This opens a transaction of its own, so the new hash, the revocation of the other sessions, and the retirement of
    /// any outstanding two-factor challenge land together or not at all. Nothing else is inside it: the user is read and
    /// both the verification and the hashing are done before it opens, so the window a concurrent write has to land in is
    /// the four updates rather than the request.
    ///
    /// The new hash is written by an update that matches the user only while the stored hash is still the one that was
    /// verified. Matching nothing means another request wrote a password in between, which counts as a collision, and so
    /// does a refresh of one of the user's other sessions committing while the transaction is open. Either way the whole
    /// thing is read, verified, and written again from the start a bounded number of times before it is given up on.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task ChangePasswordAsync(
        Guid identifier,
        ChangePasswordRequest request,
        string? currentRefreshToken = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Issues a reset token for the address given, replacing the one that address already had. The caller emails it; it is
    /// never returned to the requester.
    /// </summary>
    ///
    /// <param name="request">The address to reset, matched under the column's own collation.</param>
    /// <param name="requestIpAddress">
    /// The address the request came from, recorded for auditing an unexpected reset. Anything past
    /// <see cref="PasswordResetToken.RequestedIpAddress"/>'s 45 characters is stored trimmed rather than refused, so a
    /// forwarded-header chain does not fail the request.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the lookup and the write, both of which throw before the padding wait is reached. The wait itself ignores
    /// it, so a call that gets that far is held to the floor whether or not the token was signalled.
    /// </param>
    ///
    /// <returns>
    /// The token in plain text, the only time it exists in that form, or <c>null</c> when no account holds that address.
    /// The two are distinguishable here on purpose, so the caller has to answer its own client identically either way.
    /// </returns>
    ///
    /// <remarks>
    /// Both outcomes are held to <see cref="AuthCredentialsSettings.ForgotPasswordMinimumMilliseconds"/>, which pads a
    /// path that finished sooner and shortens none. Issuing a token also opens a serializable transaction of its own for a
    /// read and a write, which is the slower of the two paths the floor has to cover. It is wasted anyway unless the
    /// controller above answers identically too: returning a body, a status, or a header that differs between a token and
    /// <c>null</c> tells an attacker directly what the timing was hiding.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<string?> RequestForgotPasswordAsync(
        ForgotPasswordRequest request,
        string? requestIpAddress = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Spends a reset token and sets the new password, refusing a replacement that matches the current one or a confirmation
    /// that does not. Every session is revoked and every outstanding two-factor challenge retired, since whoever held the
    /// old password may not be the one resetting it.
    /// </summary>
    ///
    /// <param name="request">The token from the email, the replacement password, and its confirmation.</param>
    /// <param name="cancellationToken">Cancels the database work, rolling the reset back with the transaction.</param>
    ///
    /// <returns>
    /// A task that completes once the password is set, the token spent, the sessions revoked, and the outstanding
    /// challenges retired.
    /// </returns>
    ///
    /// <exception cref="InvalidPasswordResetTokenException">
    /// The token is unknown, already spent, or past its expiry. Also thrown when a concurrent request spent it after this
    /// one read it, because the token is claimed with a guarded update rather than on the strength of that read.
    /// </exception>
    /// <exception cref="PasswordMismatchException">The confirmation differs from the replacement.</exception>
    /// <exception cref="PasswordReusedException">
    /// The replacement verifies against the password already stored, so the reset would restore the same password.
    /// </exception>
    /// <exception cref="ConcurrentSessionUpdateException">
    /// Every attempt lost a race with a concurrent write to the same rows, which a refresh on any of the user's devices
    /// is enough to cause. Nothing was changed and the token is still unspent, so the same link still works.
    /// </exception>
    ///
    /// <remarks>
    /// This opens a transaction of its own, so spending the token, writing the new hash, revoking every session, and
    /// retiring every outstanding challenge land together or not at all. Nothing else is inside it: the token and the
    /// user are read and the replacement hashed before it opens, so the window a concurrent write has to land in is the
    /// four updates rather than the request.
    ///
    /// The new hash is written by an update that matches the user only while the stored hash is still the one the reuse
    /// check read. Matching nothing means another request wrote a password in between, which counts as a collision, and
    /// so does a refresh of one of the user's sessions committing while the transaction is open. Either way the whole
    /// thing is read and written again from the start a bounded number of times before it is given up on.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request, CancellationToken cancellationToken = default);
}
