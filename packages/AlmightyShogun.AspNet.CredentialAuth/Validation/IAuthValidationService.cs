namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Defines database-backed authentication validation checks.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IAuthValidationService
{
    /// <summary>
    /// Checks whether the supplied password matches the current authenticated user's password.
    /// </summary>
    ///
    /// <param name="currentPassword">The current password value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns><c>true</c> when the password is valid or missing; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsCurrentPasswordAsync(
        string? currentPassword,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the supplied password differs from the current authenticated user's stored password.
    /// </summary>
    ///
    /// <param name="newPassword">The new password value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns><c>true</c> when the password is different or missing; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsDifferentFromCurrentPasswordAsync(
        string? newPassword,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the supplied login identifier matches a user.
    /// </summary>
    ///
    /// <param name="identifier">The username or email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns><c>true</c> when the identifier exists or is missing; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> LoginIdentifierExistsAsync(
        string? identifier,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the supplied username is available.
    /// </summary>
    ///
    /// <param name="username">The username value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns><c>true</c> when the username is unique or missing; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsUsernameUniqueAsync(
        string? username,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the supplied email address is available.
    /// </summary>
    ///
    /// <param name="email">The email address value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns><c>true</c> when the email address is unique or missing; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsEmailUniqueAsync(
        string? email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the supplied password matches the user identified by username or email address.
    /// </summary>
    ///
    /// <param name="identifier">The username or email address.</param>
    /// <param name="currentPassword">The current password value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>
    /// <c>true</c> when the password is valid or validation should be deferred until required fields exist;
    /// otherwise, <c>false</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsCurrentPasswordAsync(
        string? identifier,
        string? currentPassword,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a password reset token exists and is still active.
    /// </summary>
    ///
    /// <param name="token">The password reset token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns><c>true</c> when the token is active or missing; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsPasswordResetTokenActiveAsync(
        string? token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the supplied password differs from the password owned by an active reset token.
    /// </summary>
    ///
    /// <param name="token">The password reset token.</param>
    /// <param name="newPassword">The new password value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns><c>true</c> when the password is different or validation should be deferred; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsDifferentFromPasswordResetTokenUserAsync(
        string? token,
        string? newPassword,
        CancellationToken cancellationToken = default);
}
