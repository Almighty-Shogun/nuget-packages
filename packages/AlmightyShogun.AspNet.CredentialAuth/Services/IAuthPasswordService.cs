namespace AlmightyShogun.AspNet.CredentialAuth;

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
    /// Changes a signed-in user's password, verifying the current one first and refusing a replacement that matches it or a
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
    /// The identifier matches no account, or the current password is wrong. The same exception a failed sign-in raises,
    /// so a caller cannot use this route to test passwords and learn more than login would tell it.
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
    /// <param name="request">The address to reset, matched exactly.</param>
    /// <param name="requestIpAddress">The address the request came from, recorded for auditing an unexpected reset.</param>
    /// <param name="cancellationToken">
    /// Cancels the lookup and the write. It does not cancel the minimum-duration wait, so a cancelled call still cannot
    /// return faster than an uncancelled one.
    /// </param>
    ///
    /// <returns>
    /// The token in plain text, the only time it exists in that form, or <c>null</c> when no account holds that address.
    /// The two are distinguishable here on purpose, so the caller has to answer its own client identically either way.
    /// </returns>
    ///
    /// <remarks>
    /// Both outcomes are held to <see cref="CredentialAuthSettings.ForgotPasswordMinimumMilliseconds"/>, so the time this
    /// takes says nothing about whether the address exists. That work is wasted unless the controller above it answers
    /// identically too: returning a body, a status, or a header that differs between a token and <c>null</c> tells an
    /// attacker directly what the timing was hiding.
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
