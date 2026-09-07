using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Signs users in and creates them. An unknown identifier and a wrong password are refused identically, and an unknown one
/// still pays a decoy verification, so the hash is not what separates them; with lockout enabled a known identifier also
/// pays the lockout statements the unknown path never reaches. A locked and a disabled account are each refused distinctly,
/// and a lockout is answered before the password is checked at all. A correct password opens a session only where no
/// enabled two-factor enrolment stands in the way; where one does, it buys a challenge and nothing else.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, both inserted on creation and returned on sign-in.</typeparam>
/// <param name="databaseContext">The application's context, which the credential tables live in.</param>
/// <param name="credentialOptions">
/// The bound credential settings, read for how long an issued challenge stays redeemable.
/// </param>
/// <param name="appHostResolver">
/// The resolver deciding which application the current request belongs to, so a session and its token are scoped to the
/// host the user actually signed in through, and so a challenge is only completed through the application that bought it.
/// </param>
/// <param name="lockoutGuard">
/// The guard owning the failure budget, which the password check is claimed against and which a completed sign-in clears.
/// </param>
/// <param name="sessionService">The session service, which issues the tokens a completed sign-in returns.</param>
/// <param name="twoFactorService">
/// The two-factor service, asked to verify the code a challenge is completed with. Nothing here reads a secret or a
/// recovery code itself.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class AuthUserService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthCredentialsSettings> credentialOptions,
    IAppHostResolver appHostResolver,
    AuthLockoutGuard<TUser> lockoutGuard,
    IAuthSessionService<TUser> sessionService,
    IAuthTwoFactorService<TUser> twoFactorService
) : IAuthUserService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The hasher used for every password read and write, so hashing and verification cannot end up using different
    /// parameters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly PasswordHasher<TUser> _hasher = new();

    /// <inheritdoc />
    public async Task<AuthLoginResult<TUser>> LoginAsync(
        LoginRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        string? app = appHostResolver.Resolve();
        ClientContext clientContext = context.GetClientContext();

        TUser? user = await databaseContext.Users
            .Where(candidate => candidate.Username == request.Identifier || candidate.Email == request.Identifier)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            VerifyDecoy(request.Password);

            throw new InvalidCredentialsException();
        }

        await lockoutGuard.ReserveAttemptAsync(user.Id, cancellationToken);

        PasswordVerificationResult verification = _hasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (verification is PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new AccountDisabledException();

        bool enrolled = await databaseContext.UserTwoFactors
            .AnyAsync(enrolment => enrolment.UserId == user.Id && enrolment.IsEnabled, cancellationToken);

        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        if (verification is PasswordVerificationResult.SuccessRehashNeeded)
            user.Password = _hasher.HashPassword(user, request.Password);

        if (enrolled)
        {
            await lockoutGuard.ReleaseAttemptAsync(user.Id, cancellationToken);

            string challenge = await IssueChallengeAsync(user.Id, app, clientContext, cancellationToken);

            await databaseContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new AuthLoginResult<TUser> { User = user, Challenge = challenge };
        }

        await lockoutGuard.ClearLockoutAsync(user.Id, cancellationToken);

        AuthSessionResult<TUser> session = await sessionService.CreateSessionAsync(user, app, clientContext, cancellationToken);

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new AuthLoginResult<TUser> { User = user, Session = session };
    }

    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> CompleteTwoFactorLoginAsync(
        CompleteTwoFactorLoginRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        string? app = appHostResolver.Resolve();
        ClientContext clientContext = context.GetClientContext();

        DateTimeOffset now = DateTimeOffset.UtcNow;
        string challengeHash = TokenHasher.Hash(request.Challenge);

        TwoFactorChallenge? pending = await databaseContext.TwoFactorChallenges
            .Where(candidate => candidate.TokenHash == challengeHash)
            .Where(candidate => candidate.UsedAt == null && candidate.ExpiresAt > now)
            .FirstOrDefaultAsync(cancellationToken);

        if (pending is null || pending.App != app)
            throw new InvalidTwoFactorChallengeException();

        TUser? user = await databaseContext.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == pending.UserId, cancellationToken);

        if (user is null)
            throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new AccountDisabledException();

        if (!await twoFactorService.VerifyAsync(user.Identifier, request.Code, cancellationToken))
            throw new InvalidTwoFactorCodeException();

        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        await SpendChallengeAsync(pending.Id, now, cancellationToken);
        await lockoutGuard.ClearLockoutAsync(user.Id, cancellationToken);

        AuthSessionResult<TUser> result =
            await sessionService.CreateSessionAsync(user, pending.App, clientContext, cancellationToken);

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<TUser> CreateUserAsync(TUser user, string password, CancellationToken cancellationToken = default)
    {
        await EnsureCredentialsAvailableAsync(user.Username, user.Email, cancellationToken);

        user.Password = _hasher.HashPassword(user, password);

        await databaseContext.Users.AddAsync(user, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> RegisterAsync(
        TUser user,
        string password,
        HttpContext context,
        CancellationToken cancellationToken = default
    )
    {
        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        string? app = appHostResolver.Resolve();
        ClientContext clientContext = context.GetClientContext();

        TUser createdUser = await CreateUserAsync(user, password, cancellationToken);

        AuthSessionResult<TUser> result =
            await sessionService.CreateSessionAsync(createdUser, app, clientContext, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return result;
    }

    /// <summary>
    /// Verifies the supplied password against a throwaway hash, so the no-user path costs the same as a wrong password.
    /// </summary>
    ///
    /// <param name="password">The submitted password, verified against a throwaway hash and then discarded.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static void VerifyDecoy(string password) => AuthTimingDefence.SpendVerification(password);

    /// <summary>
    /// Retires the challenges the user still had outstanding, clears out the ones that have expired, and writes a fresh
    /// one, so the newest sign-in is the one that can be completed and, short of two arriving at once, it is the only one.
    /// </summary>
    ///
    /// <param name="userId">The user whose password just verified, given as the database key rather than the public identifier.</param>
    /// <param name="app">The application the sign-in came through, recorded so completion scopes the session to it.</param>
    /// <param name="context">The request's address, recorded on the challenge for auditing.</param>
    /// <param name="cancellationToken">Cancels the retirement, the prune, and the insert.</param>
    ///
    /// <returns>The challenge in plain text, to hand back to the client; only its hash is stored.</returns>
    ///
    /// <remarks>
    /// The retirement is one update statement, so the database has already written it when this returns. The prune and
    /// the insert are not saved here: the caller runs inside a transaction of its own and saves them with the rehash that
    /// may sit beside them, so a sign-in never ends with the previous challenge killed and no new one written.
    ///
    /// The prune covers only the user being signed in, so a row belonging to an account that never signs in again is
    /// left where it is. Retiring before pruning means a row that is both unspent and expired is stamped and then
    /// deleted in the same call.
    ///
    /// Two sign-ins running at once can still both insert, because the retirement of the later one need not see a row the
    /// earlier one has not committed yet and no index in the schema holds the count down.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<string> IssueChallengeAsync(
        int userId,
        string? app,
        ClientContext context,
        CancellationToken cancellationToken
    )
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        await RetireActiveChallengesAsync(userId, now, cancellationToken);

        List<TwoFactorChallenge> expired = await databaseContext.TwoFactorChallenges
            .Where(candidate => candidate.UserId == userId && candidate.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        if (expired.Count > 0)
            databaseContext.TwoFactorChallenges.RemoveRange(expired);

        string challenge = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48));

        databaseContext.TwoFactorChallenges.Add(new TwoFactorChallenge
        {
            App = app,
            UserId = userId,
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(credentialOptions.Value.TwoFactor.ChallengeMinutes),
            TokenHash = TokenHasher.Hash(challenge),
            RequestedIpAddress = ColumnValue.Truncate(context.IpAddress, 45)
        });

        return challenge;
    }

    /// <summary>
    /// Spends the user's unspent challenges in a single statement, so signing in again abandons the code prompt that was
    /// still open rather than leaving two completable at once.
    /// </summary>
    ///
    /// <param name="userId">The user whose outstanding challenges are being retired.</param>
    /// <param name="now">The instant stamped onto the retired rows, shared with the insert that runs alongside them.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>
    /// A task that completes once the matching rows are spent, which the database has already written when it does. The
    /// statement runs inside the transaction the caller opened, so it is rolled back with the rest of that work.
    /// </returns>
    ///
    /// <remarks>
    /// The rows are matched and stamped by one update rather than loaded and rewritten, so no row can be retired on the
    /// strength of a read another request has already invalidated. The update bypasses the change tracker, so the rows in
    /// the database are stamped while an instance already materialized through this context keeps the
    /// <see cref="TwoFactorChallenge.UsedAt"/> it was loaded with.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task RetireActiveChallengesAsync(int userId, DateTimeOffset now, CancellationToken cancellationToken)
        => await databaseContext.TwoFactorChallenges
            .Where(challenge => challenge.UserId == userId && challenge.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(challenge => challenge.UsedAt, now), cancellationToken);

    /// <summary>
    /// Claims the challenge with a guarded update, so two completions racing for the same one produce one session and one
    /// refusal rather than two sessions.
    /// </summary>
    ///
    /// <param name="challengeId">The row to spend, as read moments earlier.</param>
    /// <param name="now">The instant stamped onto the row, and the instant its expiry is judged against.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    ///
    /// <returns>A task that completes once the row is spent, which the database has already written when it does.</returns>
    ///
    /// <exception cref="InvalidTwoFactorChallengeException">
    /// The row was no longer unspent and unexpired, so another request claimed it in between, or a sign-in made in the
    /// meantime retired it.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task SpendChallengeAsync(int challengeId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        int affectedRows = await databaseContext.TwoFactorChallenges
            .Where(challenge => challenge.Id == challengeId && challenge.UsedAt == null && challenge.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters.SetProperty(challenge => challenge.UsedAt, now), cancellationToken);

        if (affectedRows != 1)
            throw new InvalidTwoFactorChallengeException();
    }

    /// <summary>
    /// Refuses a username or email address that another account already holds, so a duplicate is reported as a client
    /// mistake rather than surfacing as a unique-index violation from the database.
    /// </summary>
    ///
    /// <param name="username">The username the new account wants, compared under the database's own collation.</param>
    /// <param name="email">
    /// The address the new account wants. Compared under the database's collation, so a case-sensitive one lets two
    /// accounts differ only in casing.
    /// </param>
    /// <param name="cancellationToken">Cancels the lookups.</param>
    ///
    /// <returns>A task that completes when neither value was in use at the moment it was checked.</returns>
    ///
    /// <exception cref="UsernameTakenException">Another account already uses that username.</exception>
    /// <exception cref="EmailTakenException">Another account already uses that email address.</exception>
    ///
    /// <remarks>
    /// A check here cannot make the write safe on its own, because another request may claim the same value in between.
    /// The unique indexes remain the guarantee; this only turns the common case into a useful message.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task EnsureCredentialsAvailableAsync(string username, string email, CancellationToken cancellationToken)
    {
        if (await databaseContext.Users.AnyAsync(candidate => candidate.Username == username, cancellationToken))
            throw new UsernameTakenException();

        if (await databaseContext.Users.AnyAsync(candidate => candidate.Email == email, cancellationToken))
            throw new EmailTakenException();
    }
}
