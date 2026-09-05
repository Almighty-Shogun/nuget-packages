using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// One issued email verification. The package defines the table and issues nothing into it, so whether a spent token is
/// kept and how a replay is answered are decided by the application's own verification flow.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[Table("email_verification_tokens")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class EmailVerificationToken
{
    /// <summary>
    /// Gets or sets the surrogate key. No package service reads or writes this table, so whether it ever reaches a client
    /// is the application's own decision.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user the verification was issued for. Cascades with the user, so removing an account takes its
    /// outstanding verifications with it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the hash of the token that was emailed, so the emailed value cannot be read back out.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the address being verified. Stored separately from the user's current address so the same flow
    /// covers verifying a new sign-up and confirming a change of email.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the verification was requested, defaulted to the moment the entity is constructed. Nothing in
    /// the package writes it afterward.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets when the token stops being usable, which is what <see cref="IsActive"/> measures against. The package
    /// sets no value here, so how long a verification lives is decided by the application's own flow.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the token was spent, or <c>null</c> while it is still usable. The package never sets it. A flow
    /// that stamps it leaves the row in the table, where <see cref="IsActive"/> reports it as no longer accepted.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? UsedAt { get; set; }

    /// <summary>
    /// Gets whether the token would still be accepted, which is unspent and not past its expiry.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [NotMapped]
    public bool IsActive => UsedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
