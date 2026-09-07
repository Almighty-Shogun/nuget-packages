using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Opens, renews, and ends the refresh-token sessions behind a signed-in user. Each acts on the one session a refresh
/// token names rather than on the account, and a user may hold several at once.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, returned alongside the tokens a session yields.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IAuthSessionService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Renews a session and issues a new refresh token, invalidating the presented one. The session remembers the token it
    /// just replaced, so presenting that one again is refused, and once the rotation is more than thirty seconds old it is
    /// also treated as a replay and revokes every session the user holds.
    /// </summary>
    ///
    /// <param name="refreshToken">The token as the client holds it, matched by hash.</param>
    /// <param name="httpContext">
    /// The current request, read for the address and user agent recorded on the session. The application scope comes from
    /// the ambient request instead, through the host resolver's own accessor.
    /// </param>
    /// <param name="cancellationToken">Cancels the database work, rolling the rotation back with the transaction.</param>
    ///
    /// <returns>A new access token, the rotated refresh token, and the user they belong to.</returns>
    ///
    /// <exception cref="AlmightyShogun.AspNet.Auth.UnknownAppException">
    /// Application scoping is on and the request's host maps to no configured application, so the session cannot be
    /// matched against one. Thrown before the presented token is looked up.
    /// </exception>
    /// <exception cref="InvalidSessionException">
    /// The token matches no usable session, whether unknown, expired, revoked, or scoped to a different application. Also
    /// thrown when another request rotated the same session first, since only one of two concurrent refreshes may win.
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
    /// <remarks>
    /// Only the immediately previous token is remembered. Across a chain of rotations a client that replays the token
    /// before last is refused as an unknown token rather than recognised as a replay, so nothing is revoked in that case.
    /// Nothing is revoked either while the rotation is under thirty seconds old, which covers a client that retried before
    /// it had stored the new token: inside that window the replay is still refused, but the sessions stand.
    ///
    /// A replay that is acted on spends the session's record of the token it replayed, so that token revokes once and is
    /// refused as an unknown one from then on. A session already ended, by a sign-out or by a password or email change,
    /// still detects, which is what catches a token stolen shortly before that session ended.
    ///
    /// This opens a transaction of its own. A rotation lost to a concurrent one rolls back before the refusal, while the
    /// revocations a detected replay causes are committed before the exception is thrown, so they survive the failure. A
    /// rotation of another of the user's sessions landing at that moment is written over rather than allowed to discard
    /// them; on the repeated collision that defeats even that, the revocations are given up on and logged, and the refusal
    /// is unchanged.
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
    /// Ends one session, which is what a sign-out on a single device does. The row is kept rather than deleted, and the
    /// token the client was holding is refused afterwards exactly as an unknown token is. The token that session had
    /// already rotated away is not, so ending a session does not disarm theft detection for the token it last spent.
    /// </summary>
    ///
    /// <param name="refreshToken">The token as the client holds it. An unknown token is not an error.</param>
    /// <param name="cancellationToken">Cancels the database work, rolling the revocation back with the transaction.</param>
    ///
    /// <returns>A task that completes once the session can no longer be refreshed.</returns>
    ///
    /// <remarks>
    /// This opens a transaction of its own, and commits it even when the token matched nothing.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a session for a user and issues both of its tokens, pruning that user's already-expired sessions on the way
    /// so the table does not grow without bound. This is the sign-in path for a flow the package does not own, such as an
    /// SSO callback, where the credentials were established elsewhere and only the session still has to be made.
    /// </summary>
    ///
    /// <param name="user">The user the session belongs to, read for the claims the access token carries.</param>
    /// <param name="app">
    /// The application audience the session is scoped to, or <c>null</c> when the deployment is not app-scoped. It also
    /// narrows the permissions the access token carries, so passing the wrong one mints a token for the wrong audience.
    /// </param>
    /// <param name="context">
    /// The request's address and user agent, recorded on the session so a user can recognise their own devices.
    /// </param>
    /// <param name="cancellationToken">Cancels the database work.</param>
    ///
    /// <returns>
    /// The access token, the refresh token, and the user they were issued for. The refresh token is in plain text, which
    /// is the only time it exists in that form: only its hash is stored.
    /// </returns>
    ///
    /// <remarks>
    /// Nothing is checked here. The account being active, not locked out, and past whatever second factor it owes are all
    /// the caller's to establish first, because this mints the credential rather than deciding who may have one.
    ///
    /// The session expires one refresh window from now, or at the configured absolute lifetime when that falls sooner, so a
    /// client that never refreshes cannot hold a usable token past the ceiling.
    ///
    /// This saves but opens no transaction of its own, so a caller that wants the session and its own writes to land
    /// together must call it inside one.
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
