namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Changes passwords, both for a signed-in user and through a reset link. Both paths that set a password revoke the
/// user's other sessions, so a change actually ends access that used the old one.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthPasswordService
{
    /// <summary>
    /// Changes a signed-in user's password, refusing a confirmation that does not match, then a wrong current password, then a
    /// confirmation that does not.
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
    /// <returns>A task that completes once the password is changed and the other sessions are revoked.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">
    /// The identifier matches no account, or the current password is wrong. The exception does not distinguish the two,
    /// but the work does: an unknown identifier is refused before any hash is verified, while a wrong password costs a
    /// verification first. Nothing here counts a failure towards the lockout, so guesses against this route are unmetered.
    /// </exception>
    /// <exception cref="PasswordMismatchException">The confirmation differs from the replacement.</exception>
    /// <exception cref="PasswordReusedException">
    /// The replacement verifies against the password already stored, so the change would change nothing.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// <param name="requestIpAddress">The address the request came from, recorded for auditing an unexpected reset.</param>
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
    /// path that finished sooner and shortens none. Issuing a token also runs a serializable transaction with a read and
    /// a write, so the floor only hides the difference while it stays above what that path costs on the deployment's own
    /// hardware. It is wasted anyway unless the controller above answers identically too: returning a body, a status, or
    /// a header that differs between a token and <c>null</c> tells an attacker directly what the timing was hiding.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<string?> RequestForgotPasswordAsync(
        ForgotPasswordRequest request,
        string? requestIpAddress = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Spends a reset token and sets the new password, refusing a replacement that matches the current one or a confirmation
    /// that does not. Every session is revoked, since whoever held the old password may not be the one resetting it.
    /// </summary>
    ///
    /// <param name="request">The token from the email, the replacement password, and its confirmation.</param>
    /// <param name="cancellationToken">Cancels the database work, rolling the reset back with the transaction.</param>
    ///
    /// <returns>A task that completes once the password is set, the token spent, and the sessions revoked.</returns>
    ///
    /// <exception cref="InvalidPasswordResetTokenException">
    /// The token is unknown, already spent, or past its expiry. Also thrown when a concurrent request spent it after this
    /// one read it, because the token is claimed with a guarded update rather than on the strength of that read.
    /// </exception>
    /// <exception cref="PasswordMismatchException">The confirmation differs from the replacement.</exception>
    /// <exception cref="PasswordReusedException">
    /// The replacement verifies against the password already stored, so the reset would restore the same password.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request, CancellationToken cancellationToken = default);
}
