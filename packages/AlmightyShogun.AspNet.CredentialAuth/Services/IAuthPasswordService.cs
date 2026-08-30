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
    Task ChangePasswordAsync(Guid identifier, ChangePasswordRequest request, string? currentRefreshToken = null);

    /// <summary>
    /// Issues a reset token for the address given, invalidating any the user already had. The caller emails it; it is never
    /// returned to the requester.
    /// </summary>
    ///
    /// <param name="request">The address to reset, matched exactly.</param>
    /// <param name="requestIpAddress">The address the request came from, recorded for auditing an unexpected reset.</param>
    ///
    /// <returns>
    /// The token in plain text, the only time it exists in that form, or <c>null</c> when no account holds that address.
    /// Answering the same way either way is what stops the endpoint being used to discover registered addresses.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<string?> RequestForgotPasswordAsync(ForgotPasswordRequest request, string? requestIpAddress = null);

    /// <summary>
    /// Spends a reset token and sets the new password, refusing a replacement that matches the current one or a confirmation
    /// that does not. Every session is revoked, since whoever held the old password may not be the one resetting it.
    /// </summary>
    ///
    /// <param name="request">The token from the email, the replacement password, and its confirmation.</param>
    ///
    /// <returns>A task that completes once the password is set, the token spent, and the sessions revoked.</returns>
    ///
    /// <exception cref="InvalidPasswordResetTokenException">
    /// The token is unknown, already spent, or past its expiry. Checked before the passwords, so a dead link is
    /// reported without the submitted password being looked at.
    /// </exception>
    /// <exception cref="PasswordMismatchException">The confirmation differs from the replacement.</exception>
    /// <exception cref="PasswordReusedException">
    /// The replacement verifies against the password already stored, so the reset would restore the same password.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request);
}
