using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Opens, renews, and ends the refresh-token sessions behind a signed-in user. A session is per device, so these act on
/// one device rather than on the account.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, returned alongside the tokens a session yields.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthSessionService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Renews a session and issues a new refresh token, invalidating the presented one. Rotation is what makes a stolen
    /// token detectable: presenting a spent one outside the grace window is treated as a replay.
    /// </summary>
    ///
    /// <param name="refreshToken">The token as the client holds it, matched by hash.</param>
    /// <param name="httpContext">
    /// The current request, read for the application scope and for the address and user agent recorded on the session.
    /// </param>
    ///
    /// <returns>A new access token, the rotated refresh token, and the user they belong to.</returns>
    ///
    /// <exception cref="InvalidSessionException">
    /// The token matches no usable session, whether unknown, expired, revoked, or scoped to a different application.
    /// A token detected as a replay revokes every session for its user before this is thrown.
    /// </exception>
    /// <exception cref="AccountDisabledException">
    /// The account was deactivated after the session opened, so deactivating a user takes effect on their next refresh
    /// rather than only when their access token expires.
    /// </exception>
    /// <exception cref="AccountLockedException">
    /// A lockout is in force. Carries the moment it lifts, and is only ever thrown while lockout is enabled.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthSessionResult<TUser>> RefreshSessionAsync(string refreshToken, HttpContext httpContext);

    /// <summary>
    /// Ends one session, which is what a sign-out on a single device does. The row is kept rather than deleted, so a later
    /// replay of its token is recognised.
    /// </summary>
    ///
    /// <param name="refreshToken">The token as the client holds it. An unknown token is not an error.</param>
    ///
    /// <returns>A task that completes once the session can no longer be refreshed.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task RevokeSessionAsync(string refreshToken);

    /// <summary>
    /// Opens a session for a user and returns its refresh token, pruning that user's already-expired sessions on the way
    /// so the table does not grow without bound.
    /// </summary>
    ///
    /// <param name="user">The user the session belongs to.</param>
    /// <param name="app">
    /// The application audience the session is scoped to, or <c>null</c> when the deployment is not app-scoped.
    /// </param>
    /// <param name="context">
    /// The request's address and user agent, recorded on the session so a user can recognise their own devices.
    /// </param>
    ///
    /// <returns>
    /// The refresh token in plain text, which is the only time it exists in that form: only its hash is stored.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<string> CreateSessionAsync(TUser user, string? app, SessionContext context);
}
