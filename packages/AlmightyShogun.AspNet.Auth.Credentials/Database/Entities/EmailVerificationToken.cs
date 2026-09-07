using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// One issued email verification. <see cref="IAuthEmailService"/> writes these rows and spends them, and a
/// spent row is kept rather than deleted, so a second click on the same link is refused exactly as an expired one is.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("email_verification_tokens")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class EmailVerificationToken
{
    /// <summary>
    /// The surrogate key, which redemption names when it claims the row it just read. Nothing hands it to a
    /// client: a link carries the token instead.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int Id { get; set; }

    /// <summary>
    /// The user the verification was issued for. Cascades with the user, so removing an account takes its
    /// outstanding verifications with it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int UserId { get; set; }

    /// <summary>
    /// The hash of the token that was emailed, so the emailed value cannot be read back out.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// The address being verified, which <see cref="Purpose"/> says how to read. For a registration it repeats
    /// the address the user already holds; for a change of email it is the address they asked to move to, which redemption
    /// then writes onto the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Which flow the token was issued for. Redemption requires it to match the method presenting the token, so
    /// a change-of-email link cannot be spent on the registration endpoint or the other way round.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public EmailVerificationPurpose Purpose { get; set; }

    /// <summary>
    /// When the verification was requested, set as the row is written and never changed afterwards.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the token stops being usable, which is what <see cref="IsActive"/> measures against. Set at issue
    /// from <see cref="AuthCredentialsSettings.EmailVerificationMinutes"/>, one lifetime covering both purposes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// When the token was spent, or <c>null</c> while it is still usable. Stamped when the token is redeemed, when a later
    /// request of the same purpose retires it, and, on a registration token, when a change of email is redeemed, whether
    /// or not that change moves the account off the address the token names. Every one of those leaves the row in the
    /// table where <see cref="IsActive"/> reports it as no longer accepted.
    /// </summary>
    ///
    /// <remarks>
    /// Each of those stamps is written by an update statement that bypasses the change tracker, so the row in the database
    /// carries the new value while an instance loaded beforehand keeps the one it was read with.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? UsedAt { get; set; }

    /// <summary>
    /// Whether the token would still be accepted, which is unspent and not past its expiry.
    /// </summary>
    ///
    /// <remarks>
    /// A redemption tests these two conditions in the database rather than reading this property, and requires
    /// <see cref="Purpose"/> to match as well, a registration also requiring <see cref="Email"/> to still be the account's,
    /// so an active row is not necessarily one the endpoint at hand will take. This also answers for the instance rather
    /// than for the row: <see cref="UsedAt"/> is stamped past the change tracker, so one loaded before that write still
    /// reports itself unspent until it is read again.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [NotMapped]
    public bool IsActive => UsedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
