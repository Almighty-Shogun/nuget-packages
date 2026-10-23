
namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Issues and redeems tokens for email verification and address changes.
/// </summary>
///
/// <remarks>
/// Verification records proof of email ownership; enforcing verified-email
/// requirements remains the application's responsibility.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public interface IAuthEmailService
{
    /// <summary>
    /// Issues a verification token for the user's current email address.
    /// </summary>
    ///
    /// <param name="identifier">The user's public identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The plaintext verification token.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">The user does not exist.</exception>
    ///
    /// <remarks>
    /// Previous unused registration tokens are retired transactionally.
    /// Concurrent requests may still produce multiple valid tokens.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Task<string> RequestVerificationAsync(Guid identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Issues a verification token for a proposed email address.
    /// </summary>
    ///
    /// <param name="identifier">The user's public identifier.</param>
    /// <param name="newEmail">The proposed address, applied only after verification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The plaintext verification token.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">The user does not exist.</exception>
    /// <exception cref="EmailTakenException">The email address is already registered.</exception>
    ///
    /// <remarks>
    /// Callers must authorize the change before requesting a token and send
    /// it to the proposed address. Previous change tokens are retired
    /// transactionally, though concurrent requests may both succeed.
    /// Email availability is checked again during redemption.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Task<string> RequestEmailChangeAsync(Guid identifier, string newEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Redeems a registration token and marks the user's email as verified.
    /// </summary>
    ///
    /// <param name="request">The email verification request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// The token is invalid, expired, already used, or no longer matches the user's email.
    /// </exception>
    /// <exception cref="InvalidCredentialsException">The user no longer exists.</exception>
    /// <exception cref="ConcurrentSessionUpdateException">
    /// All concurrent update retries failed.
    /// </exception>
    ///
    /// <remarks>
    /// Token redemption and verification are atomic, with bounded retries
    /// after concurrency conflicts. Tokens cannot be redeemed twice.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Task CompleteVerificationAsync(CompleteEmailVerificationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Redeems an email-change token, updates the address, and revokes other sessions.
    /// </summary>
    ///
    /// <param name="request">The email-change verification request.</param>
    /// <param name="currentRefreshToken">
    /// The session to preserve, or null to revoke all sessions.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="InvalidEmailVerificationTokenException">
    /// The token is invalid, expired, already used, or intended for registration.
    /// </exception>
    /// <exception cref="InvalidCredentialsException">The user no longer exists.</exception>
    /// <exception cref="EmailTakenException">The proposed email is already registered.</exception>
    /// <exception cref="ConcurrentSessionUpdateException">
    /// All concurrent update retries failed.
    /// </exception>
    /// <exception cref="System.Data.Common.DbException">
    /// A database write failed, including a possible concurrent uniqueness violation.
    /// </exception>
    ///
    /// <remarks>
    /// The address update, token redemption, registration-token retirement,
    /// and session revocations are atomic. Concurrent conflicts are retried
    /// within a fixed limit.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Task CompleteEmailChangeAsync(
        CompleteEmailVerificationRequest request,
        string? currentRefreshToken = null,
        CancellationToken cancellationToken = default
    );
}
