
using OtpNet;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Manages TOTP enrolment, verification, and recovery codes.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity.</typeparam>
/// <param name="databaseContext">The authentication database context.</param>
/// <param name="credentialOptions">The two-factor authentication settings.</param>
/// <param name="dataProtectionProvider">Protects stored TOTP secrets.</param>
/// <param name="lockoutGuard">Enforces the shared authentication failure limit.</param>
///
/// <remarks>
/// TOTP secrets are encrypted, recovery codes are hashed, and new enrolments
/// remain pending until verified. Code verification shares the password lockout
/// budget, while enrolment operations do not.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class AuthTwoFactorService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthCredentialsSettings> credentialOptions,
    IDataProtectionProvider dataProtectionProvider,
    AuthLockoutGuard<TUser> lockoutGuard
) : IAuthTwoFactorService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Protects TOTP secrets using a fixed encryption purpose.
    /// </summary>
    ///
    /// <remarks>
    /// Changing the purpose or losing protection keys makes existing secrets unreadable.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly IDataProtector _protector =
        dataProtectionProvider.CreateProtector("AlmightyShogun.Auth.Credentials.TwoFactor");

    /// <summary>
    /// The configured two-factor authentication policy.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly TwoFactorPolicy _policy = credentialOptions.Value.TwoFactor;

    /// <inheritdoc />
    public async Task<AuthTwoFactorResult> BeginEnrolmentAsync(
        Guid identifier,
        string issuer,
        CancellationToken cancellationToken = default
    )
    {
        TUser user = await GetUserAsync(candidate => candidate.Identifier == identifier, cancellationToken);

        byte[] secret = RandomNumberGenerator.GetBytes(20);
        var base32Secret = Base32Encoding.ToString(secret);

        UserTwoFactor enrolment = await GetOrCreateEnrolmentAsync(user, cancellationToken);

        enrolment.PendingSecret = _protector.Protect(base32Secret);
        enrolment.PendingSecretExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_policy.PendingSecretMinutes);

        await databaseContext.SaveChangesAsync(cancellationToken);

        string label = string.IsNullOrWhiteSpace(_policy.Issuer) ? issuer : _policy.Issuer;

        string uri = $"otpauth://totp/{Uri.EscapeDataString(label)}:{Uri.EscapeDataString(user.Email)}"
                     + $"?secret={base32Secret}&issuer={Uri.EscapeDataString(label)}"
                     + $"&digits={_policy.Digits}&period={_policy.PeriodSeconds}";

        return new AuthTwoFactorResult
        {
            Secret = base32Secret,
            Uri = uri
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> CompleteEnrolmentAsync(
        Guid identifier,
        string code,
        CancellationToken cancellationToken = default
    )
    {
        await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(cancellationToken);

        UserTwoFactor enrolment = await GetEnrolmentAsync(identifier, tracked: true, cancellationToken);

        if (enrolment.PendingSecret is null || enrolment.PendingSecretExpiresAt is not { } expiresAt || expiresAt <= DateTimeOffset.UtcNow)
            throw new InvalidTwoFactorCodeException();

        string pendingSecret = enrolment.PendingSecret;

        if (!TryVerifyTotp(pendingSecret, code, out long window))
            throw new InvalidTwoFactorCodeException();

        databaseContext.TwoFactorRecoveryCodes.RemoveRange(enrolment.RecoveryCodes);

        List<string> recoveryCodes = [];

        for (var index = 0; index < _policy.RecoveryCodeCount; index++)
        {
            string recoveryCode = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(12));

            recoveryCodes.Add(recoveryCode);

            enrolment.RecoveryCodes.Add(new TwoFactorRecoveryCode { CodeHash = TokenHasher.Hash(recoveryCode) });
        }

        enrolment.Secret = pendingSecret;
        enrolment.PendingSecret = null;
        enrolment.PendingSecretExpiresAt = null;
        enrolment.LastWindow = window;
        enrolment.IsEnabled = true;

        await databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return recoveryCodes;
    }

    /// <inheritdoc />
    public async Task<bool> VerifyAsync(Guid identifier, string code, CancellationToken cancellationToken = default)
    {
        UserTwoFactor enrolment = await GetEnrolmentAsync(identifier, tracked: false, cancellationToken);

        if (!enrolment.IsEnabled || string.IsNullOrWhiteSpace(enrolment.Secret))
            return false;

        await lockoutGuard.ReserveAttemptAsync(enrolment.UserId, cancellationToken);

        if (TryVerifyTotp(enrolment.Secret, code, out long window))
        {
            int affected = await databaseContext.UserTwoFactors
                .Where(twoFactor => twoFactor.Id == enrolment.Id && (twoFactor.LastWindow == null || twoFactor.LastWindow < window))
                .ExecuteUpdateAsync(setters => setters.SetProperty(twoFactor => twoFactor.LastWindow, window), cancellationToken);

            if (affected != 1)
                return false;

            await lockoutGuard.ClearLockoutAsync(enrolment.UserId, cancellationToken);

            return true;
        }

        string codeHash = TokenHasher.Hash(code);
        DateTimeOffset now = DateTimeOffset.UtcNow;

        int recoveryCodeAffected = await databaseContext.TwoFactorRecoveryCodes
            .Where(recoveryCode => recoveryCode.UserTwoFactorId == enrolment.Id)
            .Where(recoveryCode => recoveryCode.CodeHash == codeHash && recoveryCode.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(recoveryCode => recoveryCode.UsedAt, now), cancellationToken);

        if (recoveryCodeAffected != 1)
            return false;

        await lockoutGuard.ClearLockoutAsync(enrolment.UserId, cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task DisableAsync(Guid identifier, CancellationToken cancellationToken = default)
    {
        TUser user = await GetUserAsync(candidate => candidate.Identifier == identifier, cancellationToken);

        UserTwoFactor? enrolment = await databaseContext.UserTwoFactors
            .Include(twoFactor => twoFactor.RecoveryCodes)
            .FirstOrDefaultAsync(twoFactor => twoFactor.UserId == user.Id, cancellationToken);

        if (enrolment is null)
            return;

        databaseContext.TwoFactorRecoveryCodes.RemoveRange(enrolment.RecoveryCodes);
        databaseContext.UserTwoFactors.Remove(enrolment);

        await databaseContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a matching user.
    /// </summary>
    ///
    /// <param name="predicate">The user lookup predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The matching tracked user.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">No matching user exists.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken)
    {
        TUser? user = await databaseContext.Users.FirstOrDefaultAsync(predicate, cancellationToken);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Retrieves a user's two-factor enrolment and recovery codes.
    /// </summary>
    ///
    /// <param name="identifier">The user's public identifier.</param>
    /// <param name="tracked">Whether to track the enrolment for updates.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The enrolment with its recovery codes.</returns>
    ///
    /// <exception cref="InvalidTwoFactorCodeException">No enrolment exists.</exception>
    ///
    /// <remarks>
    /// Untracked reads avoid relationship fixup on existing tracked users.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<UserTwoFactor> GetEnrolmentAsync(Guid identifier, bool tracked, CancellationToken cancellationToken)
    {
        TUser user = await GetUserAsync(candidate => candidate.Identifier == identifier, cancellationToken);

        IQueryable<UserTwoFactor> enrolments = databaseContext.UserTwoFactors.Include(twoFactor => twoFactor.RecoveryCodes);

        if (!tracked)
            enrolments = enrolments.AsNoTracking();

        return await enrolments.FirstOrDefaultAsync(twoFactor => twoFactor.UserId == user.Id, cancellationToken)
               ?? throw new InvalidTwoFactorCodeException();
    }

    /// <summary>
    /// Retrieves or creates a user's two-factor enrolment.
    /// </summary>
    ///
    /// <param name="user">The user enrolling in two-factor authentication.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The enrolment with its recovery codes.</returns>
    ///
    /// <remarks>
    /// Newly created enrolments remain disabled until confirmation.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<UserTwoFactor> GetOrCreateEnrolmentAsync(TUser user, CancellationToken cancellationToken)
    {
        UserTwoFactor? enrolment = await databaseContext.UserTwoFactors
            .Include(twoFactor => twoFactor.RecoveryCodes)
            .FirstOrDefaultAsync(twoFactor => twoFactor.UserId == user.Id, cancellationToken);

        if (enrolment is not null)
            return enrolment;

        enrolment = new UserTwoFactor { UserId = user.Id };

        await databaseContext.UserTwoFactors.AddAsync(enrolment, cancellationToken);

        return enrolment;
    }

    /// <summary>
    /// Verifies a TOTP code against a protected secret.
    /// </summary>
    ///
    /// <param name="protectedSecret">The encrypted TOTP secret.</param>
    /// <param name="code">The submitted authentication code.</param>
    /// <param name="window">The matching time window, or zero if none.</param>
    ///
    /// <returns>True if the code is valid; otherwise, false.</returns>
    ///
    /// <remarks>
    /// Accepts the configured adjacent time windows. Invalid protection data
    /// returns false; replay prevention is handled by the caller.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private bool TryVerifyTotp(string protectedSecret, string code, out long window)
    {
        window = 0;

        if (string.IsNullOrWhiteSpace(protectedSecret))
            return false;

        byte[] secret;

        try
        {
            secret = Base32Encoding.ToBytes(_protector.Unprotect(protectedSecret));
        }
        catch (CryptographicException)
        {
            return false;
        }

        Totp totp = new(secret, step: _policy.PeriodSeconds, totpSize: _policy.Digits);

        return totp.VerifyTotp(code, out window, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}
