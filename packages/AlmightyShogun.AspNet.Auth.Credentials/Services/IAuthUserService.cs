using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Signs users in and creates them. Every path here writes credentials, opens a session, or decides whether one may be
/// opened, so each one checks what it is given rather than trusting that a request model already did.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, both accepted on creation and returned on sign-in.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IAuthUserService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Verifies a password and either opens a session or, for a user whose two-factor enrolment is enabled, issues a
    /// challenge instead and opens nothing. Refuses a wrong identifier, a wrong password, a locked account, and a disabled
    /// one. An unknown identifier still costs a password verification, so the hash is not what separates the two. With
    /// lockout enabled a known identifier additionally pays the lockout statements the unknown path never reaches.
    /// </summary>
    ///
    /// <param name="request">The submitted credentials, matched against username and email alike.</param>
    /// <param name="context">
    /// The current request, read for the address and user agent recorded on the session, or for the address recorded on
    /// the challenge when one is issued instead.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the database work, rolling back the session or the challenge with the transaction.
    /// </param>
    ///
    /// <returns>
    /// The user, with either the tokens a completed sign-in issued or the challenge a second factor is owed against.
    /// Branch on <see cref="AuthLoginResult{TUser}.RequiresTwoFactor"/>: a challenge means no token was minted at all, and
    /// the sign-in finishes through <see cref="CompleteTwoFactorLoginAsync"/>.
    /// </returns>
    ///
    /// <exception cref="AlmightyShogun.AspNet.Auth.UnknownAppException">
    /// Application scoping is on and the request's host maps to no configured application. Resolved before the identifier
    /// is even looked up, so this reports a mapping or deployment problem rather than a failed sign-in.
    /// </exception>
    /// <exception cref="InvalidCredentialsException">
    /// The identifier matches no account, or the password is wrong. One exception covers both, so a caller cannot tell
    /// them apart and neither can whoever is calling the caller. A wrong password is counted towards the lockout before
    /// this is thrown, and that count is committed even though the sign-in fails.
    /// </exception>
    /// <exception cref="AccountLockedException">
    /// A lockout is in force. Carries the moment it lifts, and is only ever thrown while lockout is enabled. The check
    /// runs before the password is verified and is not repeated afterwards, so a lockout applied by concurrent failures
    /// while this sign-in was verifying does not stop it.
    /// </exception>
    /// <exception cref="AccountDisabledException">
    /// The account is deactivated. Thrown after the password is checked, so it cannot be used to discover which
    /// addresses are registered.
    /// </exception>
    ///
    /// <remarks>
    /// A stored hash the hasher reports as outdated is rewritten in place once the password verifies, so raising the work
    /// factor takes effect as users return rather than needing a migration.
    ///
    /// This opens a transaction of its own. It covers that rehash and then either the lockout clear and the session
    /// insert, or the retirement of the user's outstanding challenges, the removal of any that have expired, and the
    /// insert of the new one. The lockout attempt is claimed before the transaction opens, which is what leaves that
    /// count standing when the sign-in fails.
    ///
    /// The failure run is only cleared where the sign-in completed, so a user owing a second factor keeps their count
    /// until <see cref="CompleteTwoFactorLoginAsync"/> succeeds and the password attempts they have already spent still
    /// stand against them. The attempt this call claimed for itself is given back once the challenge is issued, so a code
    /// prompt that is abandoned or left to expire costs nothing and correct passwords cannot lock the account on their
    /// own.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<AuthLoginResult<TUser>> LoginAsync(LoginRequest request, HttpContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finishes a sign-in that <see cref="LoginAsync"/> stopped at the second factor, opening the session only once the
    /// code verifies. The challenge is spent on success, so it cannot be presented twice, and it is only accepted through
    /// the application it was issued through.
    /// </summary>
    ///
    /// <param name="request">
    /// The challenge as <see cref="LoginAsync"/> returned it, matched by hash, and the code it is owed against, tried as a
    /// TOTP code and then as a recovery code by the two-factor service.
    /// </param>
    /// <param name="context">The current request, read for the address and user agent recorded on the session.</param>
    /// <param name="cancellationToken">Cancels the database work, rolling the session back with the transaction.</param>
    ///
    /// <returns>The access token, the refresh token, and the user they were issued for.</returns>
    ///
    /// <exception cref="AlmightyShogun.AspNet.Auth.UnknownAppException">
    /// Application scoping is on and the request's host maps to no configured application. Resolved before the challenge
    /// is looked up, so this reports a mapping or deployment problem rather than a failed sign-in.
    /// </exception>
    /// <exception cref="InvalidTwoFactorChallengeException">
    /// The challenge is unknown, already spent, past its expiry, or was issued through a different application, none of
    /// which are distinguished. Also thrown when another request spent the same challenge between the lookup here and
    /// the claim.
    /// </exception>
    /// <exception cref="AccountDisabledException">
    /// The account was deactivated after the password was verified, so a challenge issued moments earlier does not carry
    /// a sign-in through a deactivation.
    /// </exception>
    /// <exception cref="InvalidTwoFactorCodeException">
    /// The code is wrong, or the enrolment was disabled after the challenge was issued. The challenge is left unspent
    /// either way, so a mistyped code can be corrected without starting again, though a wrong code still costs a lockout
    /// attempt where a disabled enrolment costs none.
    /// </exception>
    /// <exception cref="AccountLockedException">
    /// A lockout is in force, or the budget was exhausted by attempts that claimed before this one. Carries the moment it
    /// lifts, and is only ever thrown while lockout is enabled. Raised by the two-factor service before it looks at the
    /// code, so the challenge is left unspent.
    /// </exception>
    /// <exception cref="InvalidCredentialsException">
    /// The user the challenge names is gone, which the cascade on the row makes unreachable in practice.
    /// </exception>
    ///
    /// <remarks>
    /// The code is verified before the challenge is spent, and the two-factor service claims the time step or the recovery
    /// code as it verifies, outside the transaction opened here. A correct code therefore stays spent even if the session
    /// insert that follows it rolls back.
    ///
    /// This opens a transaction of its own, covering the claim on the challenge, the lockout clear, and the session
    /// insert. The application is resolved from this request's host and has to match the one recorded on the challenge,
    /// so a challenge bought through one application cannot be completed through another.
    ///
    /// The code prompt is metered by the two-factor service against the same budget <see cref="LoginAsync"/> uses, so
    /// wrong codes lock the account and a correct one clears the run, password failures included. The clear here is
    /// therefore the second one on that path and finds nothing left to delete; it stands for the case where a lockout
    /// applied by concurrent guesses arrived between the two.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<AuthSessionResult<TUser>> CompleteTwoFactorLoginAsync(
        CompleteTwoFactorLoginRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default
    );

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
    /// <since>4.0.0</since>
    Task<TUser> CreateUserAsync(TUser user, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a user and signs them in, which is what a public sign-up wants. The insert and the session are one
    /// transaction, so a failure part-way leaves no account that was never signed into.
    /// </summary>
    ///
    /// <param name="user">The user to insert, carrying no role or permissions a client supplied.</param>
    /// <param name="password">The initial password, hashed here and never stored as given.</param>
    /// <param name="context">The current request, read for the address and user agent recorded on the session.</param>
    /// <param name="cancellationToken">Cancels the database work, rolling both the account and the session back.</param>
    ///
    /// <returns>The access token, the refresh token, and the user they were issued for.</returns>
    ///
    /// <exception cref="AlmightyShogun.AspNet.Auth.UnknownAppException">
    /// Application scoping is on and the request's host maps to no configured application. Resolved before the user is
    /// inserted, so a deployment with an unmapped host creates no accounts at all.
    /// </exception>
    /// <exception cref="UsernameTakenException">Another account holds that username under the database's collation.</exception>
    /// <exception cref="EmailTakenException">Another account holds that address under the database's collation.</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    /// The insert failed at the database, raised by the same unique indexes as in <see cref="CreateUserAsync"/> and left
    /// to escape here, so the transaction rolls back and no account is created.
    /// </exception>
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
