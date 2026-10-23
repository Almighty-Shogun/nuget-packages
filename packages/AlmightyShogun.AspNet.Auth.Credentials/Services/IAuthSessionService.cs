
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Creates, refreshes, and revokes individual authentication sessions.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity.</typeparam>
///
/// <remarks>
/// Users may maintain multiple independent refresh-token sessions.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IAuthSessionService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Rotates a session's refresh token and issues new authentication tokens.
    /// </summary>
    ///
    /// <param name="refreshToken">The plaintext refresh token, matched by hash.</param>
    /// <param name="httpContext">The request context used for client information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The renewed tokens and their associated user.</returns>
    ///
    /// <exception cref="AlmightyShogun.AspNet.Auth.UnknownAppException">
    /// The request does not resolve to a configured application.
    /// </exception>
    /// <exception cref="InvalidSessionException">
    /// The session is invalid, expired, revoked, outside its application scope,
    /// or has exceeded its absolute lifetime. Also thrown for concurrent
    /// rotation conflicts or detected token replay.
    /// </exception>
    /// <exception cref="AccountDisabledException">The account is disabled.</exception>
    /// <exception cref="AccountLockedException">The account is locked.</exception>
    ///
    /// <remarks>
    /// Only the immediately previous refresh token is tracked for replay detection.
    /// Reuse after 30 seconds revokes all user sessions; earlier reuse is refused
    /// without revocation. Older tokens are treated as unknown.
    ///
    /// Rotation is transactional. Detected replay revocations are committed
    /// before refusal, with concurrency handling and logging if revocation fails.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<AuthSessionResult<TUser>> RefreshSessionAsync(
        string refreshToken,
        HttpContext httpContext,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Revokes the session associated with a refresh token.
    /// </summary>
    ///
    /// <param name="refreshToken">The refresh token; unknown tokens are ignored.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <remarks>
    /// Revocation uses an atomic database update and preserves the previous
    /// token's replay-detection record.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an authenticated session and issues access and refresh tokens.
    /// </summary>
    ///
    /// <param name="user">The user receiving the session.</param>
    /// <param name="app">The application scope, or null when unscoped.</param>
    /// <param name="context">The client information recorded for the session.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The issued tokens and their associated user.</returns>
    ///
    /// <remarks>
    /// Callers must validate account status and authentication requirements
    /// before creating a session. Only the refresh-token hash is stored.
    ///
    /// Session expiry respects both the refresh window and absolute lifetime.
    /// This method saves changes but does not start its own transaction.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<AuthSessionResult<TUser>> CreateSessionAsync(
        TUser user,
        string? app,
        ClientContext context,
        CancellationToken cancellationToken = default
    );
}
