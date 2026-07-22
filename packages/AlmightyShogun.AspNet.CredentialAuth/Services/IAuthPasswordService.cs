namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Defines password change and forgot-password operations.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthPasswordService
{
    /// <summary>
    /// Changes a user's password after the password-change request has been validated.
    /// </summary>
    ///
    /// <param name="userId">The identifier of the user whose password should be changed.</param>
    /// <param name="request">The password change request.</param>
    /// <param name="currentRefreshToken">The refresh token that should remain active, if available.</param>
    ///
    /// <returns>A task representing the asynchronous password change operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request, string? currentRefreshToken = null);

    /// <summary>
    /// Creates a password reset token for the user matching the supplied forgot-password request.
    /// </summary>
    ///
    /// <param name="request">The forgot-password request containing the user email address.</param>
    /// <param name="requestIpAddress">The IP address that requested the reset token, if available.</param>
    ///
    /// <returns>The password reset token.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<string> RequestForgotPasswordAsync(ForgotPasswordRequest request, string? requestIpAddress = null);

    /// <summary>
    /// Completes a forgot-password request and updates the user's password.
    /// </summary>
    ///
    /// <param name="request">The complete forgot-password request containing the token and new password.</param>
    ///
    /// <returns>A task representing the asynchronous password reset operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request);
}
