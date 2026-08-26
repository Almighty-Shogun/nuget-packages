using OtpNet;
using System.Web;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using AlmightyShogun.AspNet.JwtAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.DataProtection;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Enrols users in TOTP two-factor authentication and verifies their codes. The secret is protected at rest and the
/// recovery codes are hashed, so a database copy alone yields neither working codes nor a way to mint them.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, looked up to reach its enrolment.</typeparam>
/// <param name="databaseContext">The application's context, so auth writes join whatever transaction it is in.</param>
/// <param name="authOptions">The bound JWT settings, read for token and session lifetimes.</param>
/// <param name="credentialOptions">
/// The bound credential settings, read for the two-factor policy that decides the issuer shown, how many recovery codes
/// are issued, and the shape of a generated code.
/// </param>
/// <param name="appHostResolver">
/// The resolver deciding which application the current request belongs to, so what is issued is scoped to it.
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
    IOptions<AuthSettings> authOptions,
    IOptions<CredentialAuthSettings> credentialOptions,
    IAppHostResolver appHostResolver,
    IDataProtectionProvider dataProtectionProvider
) : AuthServiceBase<TUser>(databaseContext, authOptions, appHostResolver), IAuthTwoFactorService<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The protector the secret is encrypted with, created once from a fixed purpose string so a secret written by one
    /// instance can be read by another.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector("AlmightyShogun.CredentialAuth.TwoFactor");

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
        TUser user = await GetUserAsync(candidate => candidate.Identifier == identifier);

        byte[] secret = RandomNumberGenerator.GetBytes(20);
        var base32Secret = Base32Encoding.ToString(secret);

        UserTwoFactor enrolment = await GetOrCreateEnrolmentAsync(user, cancellationToken);

        enrolment.Secret = _protector.Protect(base32Secret);
        enrolment.LastWindow = null;
        enrolment.IsEnabled = false;

        DatabaseContext.TwoFactorRecoveryCodes.RemoveRange(enrolment.RecoveryCodes);

        await DatabaseContext.SaveChangesAsync(cancellationToken);

        string label = string.IsNullOrWhiteSpace(_policy.Issuer) ? issuer : _policy.Issuer;

        string uri = $"otpauth://totp/{HttpUtility.UrlEncode(label)}:{HttpUtility.UrlEncode(user.Email)}"
                     + $"?secret={base32Secret}&issuer={HttpUtility.UrlEncode(label)}"
                     + $"&digits={_policy.Digits}&period={_policy.PeriodSeconds}";

        return new AuthTwoFactorResult(base32Secret, uri);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> CompleteEnrolmentAsync(
        Guid identifier,
        string code,
        CancellationToken cancellationToken = default
    )
    {
        UserTwoFactor enrolment = await GetEnrolmentAsync(identifier, cancellationToken);

        if (!TryVerifyTotp(enrolment, code))
            throw new InvalidTwoFactorCodeException();

        DatabaseContext.TwoFactorRecoveryCodes.RemoveRange(enrolment.RecoveryCodes);

        List<string> recoveryCodes = [];

        for (var index = 0; index < _policy.RecoveryCodeCount; index++)
        {
            string recoveryCode = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(12));

            recoveryCodes.Add(recoveryCode);

            enrolment.RecoveryCodes.Add(new TwoFactorRecoveryCode { CodeHash = TokenHasher.Hash(recoveryCode) });
        }

        enrolment.IsEnabled = true;

        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return recoveryCodes;
    }

    /// <inheritdoc />
    public async Task<bool> VerifyAsync(Guid identifier, string code, CancellationToken cancellationToken = default)
    {
        UserTwoFactor enrolment = await GetEnrolmentAsync(identifier, cancellationToken);

        if (TryVerifyTotp(enrolment, code))
        {
            await DatabaseContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        string codeHash = TokenHasher.Hash(code);

        TwoFactorRecoveryCode? recoveryCode = enrolment.RecoveryCodes
            .FirstOrDefault(stored => stored.UsedAt is null && stored.CodeHash == codeHash);

        if (recoveryCode is null)
            return false;

        recoveryCode.UsedAt = DateTimeOffset.UtcNow;

        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task DisableAsync(Guid identifier, CancellationToken cancellationToken = default)
    {
        TUser user = await GetUserAsync(candidate => candidate.Identifier == identifier);

        UserTwoFactor? enrolment = await DatabaseContext.UserTwoFactors
            .Include(twoFactor => twoFactor.RecoveryCodes)
            .FirstOrDefaultAsync(twoFactor => twoFactor.UserId == user.Id, cancellationToken);

        if (enrolment is null)
            return;

        DatabaseContext.TwoFactorRecoveryCodes.RemoveRange(enrolment.RecoveryCodes);
        DatabaseContext.UserTwoFactors.Remove(enrolment);

        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Loads the enrolment for a user, refusing when there is none, so every caller past this point has a secret to
    /// verify against.
    /// </summary>
    ///
    /// <param name="identifier">The public identifier of the user whose enrolment is wanted.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>The enrolment, with its recovery codes loaded.</returns>
    ///
    /// <exception cref="InvalidTwoFactorCodeException">
    /// The user has no enrolment. Reported as a bad code rather than as a missing enrolment, so the response cannot be
    /// used to learn which accounts have a second factor.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<UserTwoFactor> GetEnrolmentAsync(Guid identifier, CancellationToken cancellationToken)
    {
        TUser user = await GetUserAsync(candidate => candidate.Identifier == identifier);

        return await DatabaseContext.UserTwoFactors
            .Include(twoFactor => twoFactor.RecoveryCodes)
            .FirstOrDefaultAsync(twoFactor => twoFactor.UserId == user.Id, cancellationToken) ?? throw new InvalidTwoFactorCodeException();
    }

    /// <summary>
    /// Loads the enrolment for a user, creating an unconfirmed one when there is none, so re-enrolling replaces the
    /// previous secret rather than failing.
    /// </summary>
    ///
    /// <param name="user">The user enrolling, already loaded so the enrolment can be attached to its key.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    ///
    /// <returns>The enrolment to write the new secret onto, with its recovery codes loaded.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<UserTwoFactor> GetOrCreateEnrolmentAsync(TUser user, CancellationToken cancellationToken)
    {
        UserTwoFactor? enrolment = await DatabaseContext.UserTwoFactors
            .Include(twoFactor => twoFactor.RecoveryCodes)
            .FirstOrDefaultAsync(twoFactor => twoFactor.UserId == user.Id, cancellationToken);

        if (enrolment is not null)
            return enrolment;

        enrolment = new UserTwoFactor { UserId = user.Id };

        await DatabaseContext.UserTwoFactors.AddAsync(enrolment, cancellationToken);

        return enrolment;
    }

    /// <summary>
    /// Verifies a TOTP code and records the accepted time step, so the same code cannot be presented twice.
    /// </summary>
    ///
    /// <param name="enrolment">The enrolment holding the protected secret, and the time step last accepted for it.</param>
    /// <param name="code">The submitted code, checked against the current time step and its immediate neighbours.</param>
    ///
    /// <returns>
    /// <c>true</c> when the code is valid for a step later than the last accepted one. An unreadable secret returns
    /// <c>false</c> rather than throwing, so a rotated protection key looks like a wrong code instead of a crash.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool TryVerifyTotp(UserTwoFactor enrolment, string code)
    {
        if (string.IsNullOrWhiteSpace(enrolment.Secret))
            return false;

        byte[] secret;

        try
        {
            secret = Base32Encoding.ToBytes(_protector.Unprotect(enrolment.Secret));
        }
        catch (CryptographicException)
        {
            return false;
        }

        Totp totp = new(secret, step: _policy.PeriodSeconds, totpSize: _policy.Digits);

        if (!totp.VerifyTotp(code, out long window, VerificationWindow.RfcSpecifiedNetworkDelay))
            return false;

        if (enrolment.LastWindow >= window)
            return false;

        enrolment.LastWindow = window;

        return true;
    }
}
