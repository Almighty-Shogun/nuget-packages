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
    /// Issues a new TOTP secret and returns what the user needs to add it to an authenticator app. Two-factor is not yet in
    /// force: it takes effect only once a code proves the app was set up, so an abandoned enrolment cannot lock anyone out.
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
    /// The secret and the <c>otpauth://</c> URI, returned once. Calling this again replaces the secret and invalidates any
    /// app already set up. On an enrolment that was already confirmed it also discards the recovery codes and turns the
    /// second factor back off until a new code confirms it, so an abandoned re-enrolment leaves the account without one.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">The identifier matches no account.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthTwoFactorResult> BeginEnrolmentAsync(Guid identifier, string issuer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms an enrolment with a code from the app, which is what actually turns two-factor on, and issues the recovery
    /// codes. Any codes from a previous enrolment are discarded.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user enrolling.</param>
    /// <param name="code">The current code from the authenticator app, proving it holds the right secret.</param>
    /// <param name="cancellationToken">Cancels the database work.</param>
    ///
    /// <returns>
    /// The recovery codes in plain text, the only time they exist in that form. Show them once and tell the user to keep
    /// them: each is single use, and only their hashes are stored.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">The identifier matches no account.</exception>
    /// <exception cref="InvalidTwoFactorCodeException">
    /// The code is wrong, or the user never began an enrolment for it to confirm. Both are reported the same way, so a
    /// caller cannot use this to discover whether an enrolment is under way.
    /// </exception>
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
    /// <c>true</c> when the code was accepted. <c>false</c> covers a wrong code, a code already used in this time step,
    /// a spent recovery code, and a secret that can no longer be decrypted, none of which are distinguished.
    /// </returns>
    ///
    /// <exception cref="InvalidCredentialsException">The identifier matches no account.</exception>
    /// <exception cref="InvalidTwoFactorCodeException">
    /// The user has no enrolment at all. Only a user known to be enrolled should reach this, so call it behind a check
    /// on <see cref="UserTwoFactor.IsEnabled"/> rather than treating it as a way to ask.
    /// </exception>
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
