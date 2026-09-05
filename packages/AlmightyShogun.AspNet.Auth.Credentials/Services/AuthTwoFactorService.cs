using OtpNet;
using System.Web;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Enrols users in TOTP two-factor authentication and verifies their codes. The secret is protected at rest and the
/// recovery codes are hashed, so a database copy alone yields neither working codes nor a way to mint them. A new
/// enrolment is held aside until a code proves it, so starting one and abandoning it leaves a working second factor
/// exactly as it was.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, looked up to reach its enrolment.</typeparam>
/// <param name="databaseContext">The application's context, so auth writes join whatever transaction it is in.</param>
/// <param name="credentialOptions">
/// The bound credential settings, read for the two-factor policy that decides the issuer shown, how many recovery codes
/// are issued, and the shape of a generated code.
/// </param>
/// <param name="dataProtectionProvider">
/// The provider that encrypts the shared secret before it is stored. Its keys must outlive the enrolments, or every
/// stored secret becomes unreadable and users have to enrol again.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthTwoFactorService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthCredentialsSettings> credentialOptions,
    IDataProtectionProvider dataProtectionProvider
) : IAuthTwoFactorService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The protector the secret is encrypted with, created once from a fixed purpose string so a secret written by one
    /// instance can be read by another.
    /// </summary>
    ///
    /// <remarks>
    /// The purpose string is part of the key derivation, so a secret protected under one string cannot be read under
    /// any other. It is fixed for that reason, and changing it makes every enrolment already stored undecryptable.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IDataProtector _protector =
        dataProtectionProvider.CreateProtector("AlmightyShogun.Auth.Credentials.TwoFactor");

    /// <summary>
    /// The configured two-factor policy, read once rather than per call.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
        string base32Secret = Base32Encoding.ToString(secret);

        UserTwoFactor enrolment = await GetOrCreateEnrolmentAsync(user, cancellationToken);

        enrolment.PendingSecret = _protector.Protect(base32Secret);
        enrolment.PendingSecretExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_policy.PendingSecretMinutes);

        await databaseContext.SaveChangesAsync(cancellationToken);

        string label = string.IsNullOrWhiteSpace(_policy.Issuer) ? issuer : _policy.Issuer;

        string uri = $"otpauth://totp/{HttpUtility.UrlEncode(label)}:{HttpUtility.UrlEncode(user.Email)}"
                     + $"?secret={base32Secret}&issuer={HttpUtility.UrlEncode(label)}"
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

        UserTwoFactor enrolment = await GetEnrolmentAsync(identifier, cancellationToken);

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
        UserTwoFactor enrolment = await GetEnrolmentAsync(identifier, cancellationToken);

        if (!enrolment.IsEnabled || string.IsNullOrWhiteSpace(enrolment.Secret))
            return false;

        if (TryVerifyTotp(enrolment.Secret, code, out long window))
        {
            int affected = await databaseContext.UserTwoFactors
                .Where(twoFactor => twoFactor.Id == enrolment.Id && (twoFactor.LastWindow == null || twoFactor.LastWindow < window))
                .ExecuteUpdateAsync(setters => setters.SetProperty(twoFactor => twoFactor.LastWindow, window), cancellationToken);

            return affected == 1;
        }

        string codeHash = TokenHasher.Hash(code);
        DateTimeOffset now = DateTimeOffset.UtcNow;

        int recoveryCodeAffected = await databaseContext.TwoFactorRecoveryCodes
            .Where(recoveryCode => recoveryCode.UserTwoFactorId == enrolment.Id)
            .Where(recoveryCode => recoveryCode.CodeHash == codeHash && recoveryCode.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(recoveryCode => recoveryCode.UsedAt, now), cancellationToken);

        return recoveryCodeAffected == 1;
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
    /// Loads the one user matching a predicate, refusing rather than returning null, so every caller past this point has a
    /// user to work with.
    /// </summary>
    ///
    /// <param name="predicate">The lookup, by public identifier.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>The matching user, tracked so a caller can modify and save it.</returns>
    ///
    /// <exception cref="InvalidCredentialsException">Thrown when no user matches the predicate.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<TUser> GetUserAsync(Expression<Func<TUser, bool>> predicate, CancellationToken cancellationToken)
    {
        TUser? user = await databaseContext.Users.FirstOrDefaultAsync(predicate, cancellationToken);

        return user ?? throw new InvalidCredentialsException();
    }

    /// <summary>
    /// Loads the enrolment for a user, refusing when there is none, so every caller past this point has a row to
    /// verify against.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user whose enrolment is wanted.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>The enrolment, with its recovery codes loaded.</returns>
    ///
    /// <exception cref="InvalidTwoFactorCodeException">
    /// The user has no enrolment. During enrolment confirmation that is indistinguishable from a wrong code, since both
    /// raise this. Verification is not: it throws here while every wrong-code case returns <c>false</c>, so a caller that
    /// surfaces the two differently lets a probe learn which accounts have ever enrolled.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<UserTwoFactor> GetEnrolmentAsync(Guid identifier, CancellationToken cancellationToken)
    {
        TUser user = await GetUserAsync(candidate => candidate.Identifier == identifier, cancellationToken);

        return await databaseContext.UserTwoFactors
            .Include(twoFactor => twoFactor.RecoveryCodes)
            .FirstOrDefaultAsync(twoFactor => twoFactor.UserId == user.Id, cancellationToken) ?? throw new InvalidTwoFactorCodeException();
    }

    /// <summary>
    /// Loads the enrolment for a user, creating an empty one when there is none, so a first enrolment and a re-enrolment
    /// both have a row to write the pending secret onto.
    /// </summary>
    ///
    /// <param name="user">The user enrolling, already loaded so the enrolment can be attached to its key.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>
    /// The enrolment, with its recovery codes loaded. A row created here carries no secret and is not enabled, so it
    /// grants nothing until an enrolment is confirmed against it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// Verifies a TOTP code against one protected secret and reports the time step it matched, so a caller can check
    /// either the secret in force or the one an enrolment is offering.
    /// </summary>
    ///
    /// <param name="protectedSecret">The secret in its stored, encrypted form, unprotected here rather than by the caller.</param>
    /// <param name="code">The submitted code, checked against the current time step and its immediate neighbours.</param>
    /// <param name="window">
    /// The time step the code matched, or <c>0</c> when it matched none. Only meaningful when this returns <c>true</c>.
    /// </param>
    ///
    /// <returns>
    /// <c>true</c> when the code is valid for some step in the accepted window. An unreadable secret returns <c>false</c>
    /// rather than throwing, so a rotated protection key looks like a wrong code instead of a crash.
    /// </returns>
    ///
    /// <remarks>
    /// Nothing here records that the step was spent. Replay is refused by the caller's guarded update instead, because a
    /// check made here and a write made later leave a gap two concurrent requests can both pass through.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool TryVerifyTotp(string protectedSecret, string code, out long window)
    {
        window = default;

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
