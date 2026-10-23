
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
/// Handles user registration, password login, and two-factor sign-in.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity.</typeparam>
/// <param name="databaseContext">The authentication database context.</param>
/// <param name="credentialOptions">The credential and two-factor settings.</param>
/// <param name="appHostResolver">Resolves the application associated with the request.</param>
/// <param name="lockoutGuard">Enforces the authentication failure limit.</param>
/// <param name="sessionService">Creates authenticated sessions.</param>
/// <param name="twoFactorService">Verifies two-factor authentication codes.</param>
///
/// <remarks>
/// Unknown users receive decoy password verification. Valid passwords create
/// sessions directly unless two-factor authentication is enabled, in which
/// case a challenge must be completed first.
/// </remarks>
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
    /// The password hasher used for hashing and verification.
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

        await using IDbContextTransaction transaction =
            await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        if (!await twoFactorService.VerifyAsync(user.Identifier, request.Code, cancellationToken))
        {
            await transaction.CommitAsync(cancellationToken);
            throw new InvalidTwoFactorCodeException();
        }

        await SpendChallengeAsync(pending.Id, DateTimeOffset.UtcNow, cancellationToken);

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
    /// Performs decoy password verification for an unknown user.
    /// </summary>
    ///
    /// <param name="password">The submitted password.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static void VerifyDecoy(string password) => AuthTimingDefence.SpendVerification(password);

    /// <summary>
    /// Issues a two-factor challenge and retires previous challenges.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="app">The application associated with the sign-in.</param>
    /// <param name="context">The client context used for auditing.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The plaintext challenge; only its hash is stored.</returns>
    ///
    /// <remarks>
    /// Expired challenges are removed within the caller's transaction.
    /// Concurrent sign-ins may still issue multiple challenges.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
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
    /// Marks the user's outstanding two-factor challenges as used.
    /// </summary>
    ///
    /// <param name="userId">The user's database identifier.</param>
    /// <param name="now">The timestamp assigned to retired challenges.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <remarks>
    /// Uses a direct database update within the caller's transaction.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task RetireActiveChallengesAsync(int userId, DateTimeOffset now, CancellationToken cancellationToken)
        => await databaseContext.TwoFactorChallenges
            .Where(challenge => challenge.UserId == userId && challenge.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(challenge => challenge.UsedAt, now), cancellationToken);

    /// <summary>
    /// Atomically redeems an unused, unexpired two-factor challenge.
    /// </summary>
    ///
    /// <param name="challengeId">The challenge's database identifier.</param>
    /// <param name="now">The redemption timestamp.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="InvalidTwoFactorChallengeException">
    /// The challenge expired or was already used or retired.
    /// </exception>
    ///
    /// <remarks>
    /// The guarded update prevents concurrent redemption of the same challenge.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    private async Task SpendChallengeAsync(int challengeId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        int affectedRows = await databaseContext.TwoFactorChallenges
            .Where(challenge => challenge.Id == challengeId && challenge.UsedAt == null && challenge.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters.SetProperty(challenge => challenge.UsedAt, now), cancellationToken);

        if (affectedRows != 1)
            throw new InvalidTwoFactorChallengeException();
    }

    /// <summary>
    /// Checks whether a username and email are available.
    /// </summary>
    ///
    /// <param name="username">The requested username.</param>
    /// <param name="email">The requested email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <exception cref="UsernameTakenException">The username is already registered.</exception>
    /// <exception cref="EmailTakenException">The email address is already registered.</exception>
    ///
    /// <remarks>
    /// Database unique indexes remain authoritative against concurrent registrations.
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
