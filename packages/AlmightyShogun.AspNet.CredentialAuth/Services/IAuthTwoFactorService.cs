namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Enrols a user in TOTP two-factor authentication and checks the codes they present. The shared secret is encrypted
/// at rest and the recovery codes are hashed, so a database copy yields neither working codes nor a way to mint them.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, which the public identifier is resolved against.</typeparam>
///
/// <remarks>
/// The package stores and verifies the second factor but never requires it. Deciding when a login must present one is
/// the application's job, because that policy differs per product.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthTwoFactorService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Issues a new TOTP secret and returns what the user needs to add it to an authenticator app. The secret is held
    /// aside rather than put in force, so nothing about the account changes until a code proves the app was set up.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user enrolling.</param>
    /// <param name="issuer">
    /// The name shown beside the account in the authenticator app. Ignored when the configured
    /// <see cref="TwoFactorPolicy.Issuer"/> is set, which is the better place for a value that never varies per call.
    /// </param>
    /// <param name="cancellationToken">Cancels the database work.</param>
    ///
    /// <returns>
    /// The secret and the <c>otpauth://</c> URI, returned once. Calling this again discards the previous unconfirmed
    /// secret and offers a fresh one, so only the most recent QR can be confirmed.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">The identifier matches no account.</exception>
    ///
    /// <remarks>
    /// A user who already has a working second factor keeps it, codes and all, until this enrolment is confirmed.
    /// Abandoning the enrolment therefore costs them nothing, and the offered secret stops being confirmable on its own
    /// after ten minutes.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthTwoFactorResult> BeginEnrolmentAsync(Guid identifier, string issuer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms an enrolment with a code from the app, which is what actually turns two-factor on, and issues the recovery
    /// codes. The offered secret replaces whatever was in force, and any codes from a previous enrolment are discarded.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user enrolling.</param>
    /// <param name="code">The current code from the authenticator app, proving it holds the right secret.</param>
    /// <param name="cancellationToken">Cancels the database work, rolling the whole promotion back with the transaction.</param>
    ///
    /// <returns>
    /// The recovery codes in plain text, the only time they exist in that form. Show them once and tell the user to keep
    /// them: each is single use, and only their hashes are stored.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">The identifier matches no account.</exception>
    /// <exception cref="InvalidTwoFactorCodeException">
    /// The code is wrong, the user never began an enrolment for it to confirm, or the one they began has expired. All are
    /// reported the same way, so a caller cannot use this to discover whether an enrolment is under way.
    /// </exception>
    ///
    /// <remarks>
    /// Promotion, code replacement, and enabling happen in one transaction, so a failure part-way cannot leave the old
    /// secret gone with the new one not yet in force.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<IReadOnlyList<string>> CompleteEnrolmentAsync(Guid identifier, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a code at sign-in, accepting either a current TOTP code or one unspent recovery code. A recovery code is
    /// spent on success, so it cannot be presented twice.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user.</param>
    /// <param name="code">
    /// The submitted value, tried as a TOTP code first and then as a recovery code, so the caller need not say which it is.
    /// </param>
    /// <param name="cancellationToken">Cancels the database work.</param>
    ///
    /// <returns>
    /// <c>true</c> when the code was accepted. <c>false</c> covers a wrong code, a code already used in this time step or
    /// an earlier one, a spent recovery code, a secret that can no longer be decrypted, and an enrolment that was begun
    /// but never confirmed, none of which are distinguished.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">The identifier matches no account.</exception>
    /// <exception cref="InvalidTwoFactorCodeException">
    /// The user has no enrolment at all. Only a user known to be enrolled should reach this, so call it behind a check
    /// on <see cref="UserTwoFactor.IsEnabled"/> rather than treating it as a way to ask.
    /// </exception>
    ///
    /// <remarks>
    /// The time step and the recovery code are both claimed with a guarded update rather than read and then written, so
    /// two requests presenting the same code at once cannot both be accepted.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> VerifyAsync(Guid identifier, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Turns two-factor off and deletes the enrolment outright, secret and recovery codes with it. Re-enabling means
    /// enrolling again, so this is not a pause.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user.</param>
    /// <param name="cancellationToken">Cancels the database work.</param>
    ///
    /// <returns>
    /// A task that completes once the enrolment is gone. Disabling a user who never enrolled is not an error.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">The identifier matches no account.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task DisableAsync(Guid identifier, CancellationToken cancellationToken = default);
}
