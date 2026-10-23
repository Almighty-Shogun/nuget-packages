
namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Manages password changes and email-based password resets.
/// </summary>
///
/// <remarks>
/// Password updates revoke affected sessions and retire outstanding
/// two-factor challenges to invalidate access established with the old password.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IAuthPasswordService
{
    /// <summary>
    /// Changes a user's password after verifying the current password.
    /// </summary>
    ///
    /// <param name="identifier">The user's public identifier.</param>
    /// <param name="request">The current password, replacement, and confirmation.</param>
    /// <param name="currentRefreshToken">
    /// The session to preserve, or null to revoke all sessions.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="InvalidCredentialsException">
    /// The user does not exist or the current password is incorrect.
    /// </exception>
    /// <exception cref="PasswordMismatchException">The confirmation does not match.</exception>
    /// <exception cref="PasswordReusedException">The replacement matches the current password.</exception>
    /// <exception cref="ConcurrentSessionUpdateException">
    /// All concurrent update retries failed.
    /// </exception>
    ///
    /// <remarks>
    /// Password replacement, session revocation, and challenge retirement are
    /// atomic. Concurrent changes are retried with a guarded password update.
    /// Incorrect current passwords do not count toward account lockout.
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
    /// Issues a password reset token for an email address, if registered.
    /// </summary>
    ///
    /// <param name="request">The email address requesting a reset.</param>
    /// <param name="requestIpAddress">The requesting IP address for auditing.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The plaintext reset token, or null if no account matches.</returns>
    ///
    /// <remarks>
    /// Replaces previous reset tokens and pads response time to
    /// <see cref="AuthCredentialsSettings.ForgotPasswordMinimumMilliseconds"/>.
    /// Callers must send the token by email and return identical client responses
    /// for existing and unknown accounts to prevent account enumeration.
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
    /// Redeems a reset token and replaces the user's password.
    /// </summary>
    ///
    /// <param name="request">The reset token, replacement password, and confirmation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="InvalidPasswordResetTokenException">
    /// The token is invalid, expired, or already used.
    /// </exception>
    /// <exception cref="PasswordMismatchException">The confirmation does not match.</exception>
    /// <exception cref="PasswordReusedException">The replacement matches the current password.</exception>
    /// <exception cref="ConcurrentSessionUpdateException">
    /// All concurrent update retries failed.
    /// </exception>
    ///
    /// <remarks>
    /// Token redemption, password replacement, revocation of all sessions,
    /// and retirement of outstanding two-factor challenges are atomic.
    /// Concurrent updates are retried within a fixed limit.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request, CancellationToken cancellationToken = default);
}
