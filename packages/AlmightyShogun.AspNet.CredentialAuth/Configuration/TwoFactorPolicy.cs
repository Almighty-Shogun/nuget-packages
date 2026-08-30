using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// The nested <c>CredentialAuth:TwoFactor</c> section. Optional, because two-factor authentication is: an application
/// that never calls the enrolment methods needs none of these values.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record TwoFactorPolicy
{
    /// <summary>
    /// Gets the name shown beside the account in an authenticator app, which is how a user tells one code apart from
    /// another. Overrides the issuer passed to enrolment; leave it unset to let each call supply its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Issuer { get; init; }

    /// <summary>
    /// Gets how many recovery codes are issued when an enrolment is confirmed. Each is single use, so this is how many
    /// times a user can sign in after losing their authenticator before they must enrol again.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, 50)]
    public int RecoveryCodeCount { get; init; } = 10;

    /// <summary>
    /// Gets how many digits a generated code carries. Authenticator apps overwhelmingly expect six, so changing this
    /// requires the user's app to support it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(6, 8)]
    public int Digits { get; init; } = 6;

    /// <summary>
    /// Gets how long one code stays valid, in seconds. Thirty is what authenticator apps assume, so a different value
    /// only works where the user's app can be told about it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(15, 120)]
    public int PeriodSeconds { get; init; } = 30;
}
