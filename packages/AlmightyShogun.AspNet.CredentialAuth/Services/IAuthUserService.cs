using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Signs users in and creates them. Every path here writes credentials or opens a session, so each one checks what it is
/// given rather than trusting that a request model already did.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, both accepted on creation and returned on sign-in.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthUserService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Signs a user in and opens a session, refusing a wrong identifier, a wrong password, a locked account, and a disabled
    /// one. An unknown identifier still costs a password verification, so timing does not reveal which accounts exist.
    /// </summary>
    ///
    /// <param name="request">The submitted credentials, matched against username and email alike.</param>
    /// <param name="context">
    /// The current request, read for the host that decides the application scope and for the address and user agent
    /// recorded on the session.
    /// </param>
    ///
    /// <returns>The access token, the refresh token, and the user they were issued for.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">
    /// The identifier matches no account, or the password is wrong. One exception covers both, so a caller cannot tell
    /// them apart and neither can whoever is calling the caller.
    /// </exception>
    /// <exception cref="AccountLockedException">
    /// A lockout is in force. Carries the moment it lifts, and is only ever thrown while lockout is enabled.
    /// </exception>
    /// <exception cref="AccountDisabledException">
    /// The account is deactivated. Thrown after the password is checked, so it cannot be used to discover which
    /// addresses are registered.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthSessionResult<TUser>> LoginAsync(LoginRequest request, HttpContext context);

    /// <summary>
    /// Creates a user without signing them in, for an administrative flow or an import. Refuses a username or email address
    /// another account already holds.
    /// </summary>
    ///
    /// <param name="user">
    /// The user to insert, with whatever additional columns the application's own entity carries.
    /// </param>
    /// <param name="password">The initial password, hashed here and never stored as given.</param>
    ///
    /// <returns>The inserted user, with its generated key and public identifier populated.</returns>
    ///
    /// <exception cref="UsernameTakenException">Another account holds that username under the database's collation.</exception>
    /// <exception cref="EmailTakenException">Another account holds that address under the database's collation.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<TUser> CreateUserAsync(TUser user, string password);

    /// <summary>
    /// Creates a user and signs them in, which is what a public sign-up wants. The insert and the session are one
    /// transaction, so a failure part-way leaves no account that was never signed into.
    /// </summary>
    ///
    /// <param name="user">The user to insert, carrying no role or permissions a client supplied.</param>
    /// <param name="password">The initial password, hashed here and never stored as given.</param>
    /// <param name="context">
    /// The current request, read for the host that decides the application scope and for the address and user agent
    /// recorded on the session.
    /// </param>
    ///
    /// <returns>The access token, the refresh token, and the user they were issued for.</returns>
    ///
    /// <exception cref="UsernameTakenException">Another account holds that username under the database's collation.</exception>
    /// <exception cref="EmailTakenException">Another account holds that address under the database's collation.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthSessionResult<TUser>> RegisterAsync(TUser user, string password, HttpContext context);
}
