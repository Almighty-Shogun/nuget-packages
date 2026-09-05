using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Auth.Credentials;

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
    /// one. An unknown identifier still costs a password verification, so the hash is not what separates the two. With
    /// lockout enabled a known identifier additionally pays the lockout statements the unknown path never reaches.
    /// </summary>
    ///
    /// <param name="request">The submitted credentials, matched against username and email alike.</param>
    /// <param name="context">
    /// The current request, read for the address and user agent
    /// recorded on the session.
    /// </param>
    /// <param name="cancellationToken">Cancels the database work, rolling the session back with the transaction.</param>
    ///
    /// <returns>The access token, the refresh token, and the user they were issued for.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">
    /// The identifier matches no account, or the password is wrong. One exception covers both, so a caller cannot tell
    /// them apart and neither can whoever is calling the caller. A wrong password is counted towards the lockout before
    /// this is thrown, and that count is committed even though the sign-in fails.
    /// </exception>
    /// <exception cref="AccountLockedException">
    /// A lockout is in force. Carries the moment it lifts, and is only ever thrown while lockout is enabled. The check
    /// runs before the password is verified and is not repeated afterwards, so a lockout applied by concurrent failures
    /// while this sign-in was verifying does not stop it: a correct password deletes the whole run instead.
    /// </exception>
    /// <exception cref="AccountDisabledException">
    /// The account is deactivated. Thrown after the password is checked, so it cannot be used to discover which
    /// addresses are registered.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthSessionResult<TUser>> LoginAsync(LoginRequest request, HttpContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a user without signing them in, for an administrative flow or an import. Refuses a username or email address
    /// another account already holds.
    /// </summary>
    ///
    /// <param name="user">
    /// The user to insert, with whatever additional columns the application's own entity carries.
    /// </param>
    /// <param name="password">The initial password, hashed here and never stored as given.</param>
    /// <param name="cancellationToken">Cancels the database work.</param>
    ///
    /// <returns>The inserted user, with its generated key and public identifier populated.</returns>
    ///
    /// <exception cref="UsernameTakenException">Another account holds that username under the database's collation.</exception>
    /// <exception cref="EmailTakenException">Another account holds that address under the database's collation.</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    /// The insert failed at the database. A duplicate claimed between the availability check and the insert surfaces this
    /// way rather than as one of the two above, since the unique indexes on username and email are what settle that race.
    /// </exception>
    ///
    /// <remarks>
    /// This saves but opens no transaction of its own, so it commits on its own unless a caller has already started one.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<TUser> CreateUserAsync(TUser user, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a user and signs them in, which is what a public sign-up wants. The insert and the session are one
    /// transaction, so a failure part-way leaves no account that was never signed into.
    /// </summary>
    ///
    /// <param name="user">The user to insert, carrying no role or permissions a client supplied.</param>
    /// <param name="password">The initial password, hashed here and never stored as given.</param>
    /// <param name="context">
    /// The current request, read for the address and user agent
    /// recorded on the session.
    /// </param>
    /// <param name="cancellationToken">Cancels the database work, rolling both the account and the session back.</param>
    ///
    /// <returns>The access token, the refresh token, and the user they were issued for.</returns>
    ///
    /// <exception cref="UsernameTakenException">Another account holds that username under the database's collation.</exception>
    /// <exception cref="EmailTakenException">Another account holds that address under the database's collation.</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    /// The insert failed at the database, raised by the same unique indexes as in <see cref="CreateUserAsync"/> and left
    /// to escape here, so the transaction rolls back and no account is created.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthSessionResult<TUser>> RegisterAsync(
        TUser user,
        string password,
        HttpContext context,
        CancellationToken cancellationToken = default
    );
}
