using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// One single-use recovery code. A row rather than an entry in a serialized list, so spending one is a guarded update
/// against that row instead of a rewrite of every remaining code.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[Table("two_factor_recovery_codes")]
public sealed class TwoFactorRecoveryCode
{
    /// <summary>
    /// Gets or sets the surrogate key. The codes are matched by hash rather than looked up by key, so this value never
    /// leaves the database.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the enrolment the code was issued under. Cascades with it, so disabling two-factor takes every
    /// outstanding code with it rather than leaving codes that match nothing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int UserTwoFactorId { get; set; }

    /// <summary>
    /// Gets or sets the hash of the code, produced by <see cref="TokenHasher"/>. A plain digest rather than a password
    /// hash, which suffices because a recovery code is long and random rather than something a person chose.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [MaxLength(64)]
    public string CodeHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the code was spent, or <c>null</c> while it is still usable. Kept rather than deleted, so a
    /// user can be shown that a recovery code was used and when.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? UsedAt { get; set; }
}
