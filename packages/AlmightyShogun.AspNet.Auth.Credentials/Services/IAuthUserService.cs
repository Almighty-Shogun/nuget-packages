
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Handles user creation, registration, and authentication.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IAuthUserService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Verifies credentials and creates a session or two-factor challenge.
    /// </summary>
    ///
    /// <param name="request">The submitted username or email and password.</param>
    /// <param name="context">The request context used for client information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>
    /// The user and either session tokens or a two-factor challenge.
    /// Check <see cref="AuthLoginResult{TUser}.RequiresTwoFactor"/> to determine the next step.
    /// </returns>
    ///
    /// <exception cref="AlmightyShogun.AspNet.Auth.UnknownAppException">
    /// The request does not resolve to a configured application.
    /// </exception>
    /// <exception cref="InvalidCredentialsException">
    /// The identifier or password is incorrect.
    /// </exception>
    /// <exception cref="AccountLockedException">The account is locked.</exception>
    /// <exception cref="AccountDisabledException">The account is disabled.</exception>
    ///
    /// <remarks>
    /// Unknown users receive decoy password verification. Failed password
    /// attempts count toward lockout, while successful verification may
    /// update outdated password hashes.
    ///
    /// Session or challenge creation is transactional. Accounts requiring
    /// two-factor authentication retain their failure count until verification,
    /// but the successful password attempt itself is released.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<AuthLoginResult<TUser>> LoginAsync(LoginRequest request, HttpContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes a two-factor challenge and creates an authenticated session.
    /// </summary>
    ///
    /// <param name="request">The challenge and submitted authentication code.</param>
    /// <param name="context">The request context used for client information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The issued session tokens and their associated user.</returns>
    ///
    /// <exception cref="AlmightyShogun.AspNet.Auth.UnknownAppException">
    /// The request does not resolve to a configured application.
    /// </exception>
    /// <exception cref="InvalidTwoFactorChallengeException">
    /// The challenge is invalid, expired, already used, or belongs to another application.
    /// </exception>
    /// <exception cref="AccountDisabledException">The account is disabled.</exception>
    /// <exception cref="InvalidTwoFactorCodeException">
    /// The code is invalid or two-factor authentication was disabled.
    /// </exception>
    /// <exception cref="AccountLockedException">The account is locked.</exception>
    /// <exception cref="InvalidCredentialsException">The user no longer exists.</exception>
    ///
    /// <remarks>
    /// Challenges are single-use and application-scoped. Invalid codes leave
    /// the challenge available for retry but count toward the shared lockout
    /// limit. A verified TOTP step or recovery code remains spent even if
    /// subsequent session creation fails.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Task<AuthSessionResult<TUser>> CompleteTwoFactorLoginAsync(
        CompleteTwoFactorLoginRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a user without starting an authenticated session.
    /// </summary>
    ///
    /// <param name="user">The user entity to create.</param>
    /// <param name="password">The initial password, stored as a hash.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The created user with generated identifiers.</returns>
    ///
    /// <exception cref="UsernameTakenException">The username is already registered.</exception>
    /// <exception cref="EmailTakenException">The email address is already registered.</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    /// The database insert failed, including a possible concurrent uniqueness violation.
    /// </exception>
    ///
    /// <remarks>
    /// Saves changes without starting its own transaction.
    /// Database unique indexes remain authoritative against concurrent registrations.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<TUser> CreateUserAsync(TUser user, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a user and creates an authenticated session.
    /// </summary>
    ///
    /// <param name="user">The user entity to register.</param>
    /// <param name="password">The initial password, stored as a hash.</param>
    /// <param name="context">The request context used for client information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The issued session tokens and newly registered user.</returns>
    ///
    /// <exception cref="AlmightyShogun.AspNet.Auth.UnknownAppException">
    /// The request does not resolve to a configured application.
    /// </exception>
    /// <exception cref="UsernameTakenException">The username is already registered.</exception>
    /// <exception cref="EmailTakenException">The email address is already registered.</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    /// The database insert failed, including a possible concurrent uniqueness violation.
    /// </exception>
    ///
    /// <remarks>
    /// User creation and session issuance occur in a single transaction.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<AuthSessionResult<TUser>> RegisterAsync(
        TUser user,
        string password,
        HttpContext context,
        CancellationToken cancellationToken = default
    );
}
