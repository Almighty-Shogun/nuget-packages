
namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Manages TOTP enrolment, verification, and recovery codes.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity.</typeparam>
///
/// <remarks>
/// TOTP secrets are encrypted and recovery codes are hashed.
/// Once enrolment is confirmed, password login requires a second-factor code.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IAuthTwoFactorService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Creates a pending TOTP enrolment for an authenticator app.
    /// </summary>
    ///
    /// <param name="identifier">The user's public identifier.</param>
    /// <param name="issuer">The issuer name, overridden by <see cref="TwoFactorPolicy.Issuer"/> when configured.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The TOTP secret and authenticator URI.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">The user does not exist.</exception>
    ///
    /// <remarks>
    /// A new request replaces the previous pending secret without affecting
    /// an active enrolment. Pending secrets expire after
    /// <see cref="TwoFactorPolicy.PendingSecretMinutes"/>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<AuthTwoFactorResult> BeginEnrolmentAsync(Guid identifier, string issuer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms a pending enrolment and generates recovery codes.
    /// </summary>
    ///
    /// <param name="identifier">The user's public identifier.</param>
    /// <param name="code">A valid code from the authenticator app.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The plaintext, single-use recovery codes.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">The user does not exist.</exception>
    /// <exception cref="InvalidTwoFactorCodeException">
    /// The code is invalid or the pending enrolment is missing or expired.
    /// </exception>
    ///
    /// <remarks>
    /// Enabling two-factor authentication and replacing existing secrets and
    /// recovery codes occur in a single transaction. Only recovery-code hashes
    /// are stored.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<IReadOnlyList<string>> CompleteEnrolmentAsync(Guid identifier, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a TOTP or unused recovery code during sign-in.
    /// </summary>
    ///
    /// <param name="identifier">The user's public identifier.</param>
    /// <param name="code">The submitted TOTP or recovery code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>True if accepted; otherwise, false.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">The user does not exist.</exception>
    /// <exception cref="InvalidTwoFactorCodeException">The user has no enrolment.</exception>
    /// <exception cref="AccountLockedException">The account is locked.</exception>
    ///
    /// <remarks>
    /// Successful verification clears the shared authentication failure count.
    /// Failed attempts count toward lockout when enabled. TOTP time steps and
    /// recovery codes are claimed atomically to prevent reuse.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<bool> VerifyAsync(Guid identifier, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables two-factor authentication and deletes the enrolment.
    /// </summary>
    ///
    /// <param name="identifier">The user's public identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="InvalidCredentialsException">The user does not exist.</exception>
    ///
    /// <remarks>
    /// Deletes the secret and recovery codes. Re-enabling requires a new enrolment.
    /// Disabling an account without an enrolment has no effect.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task DisableAsync(Guid identifier, CancellationToken cancellationToken = default);
}
