using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The nested <c>AuthCredentials:TwoFactor</c> section. Optional, because two-factor authentication is: an application
/// that never calls the enrolment methods needs none of these values.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record TwoFactorPolicy
{
    /// <summary>
    /// The name shown beside the account in an authenticator app, which is how a user tells one code apart from
    /// another. Overrides the issuer passed to enrolment; leave it unset to let each call supply its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Issuer { get; init; }

    /// <summary>
    /// How many recovery codes are issued when an enrolment is confirmed. Each is single use, so this is how many
    /// times a user can sign in after losing their authenticator before they must enrol again.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, 50)]
    public int RecoveryCodeCount { get; init; } = 10;

    /// <summary>
    /// How many digits a generated code carries. Authenticator apps overwhelmingly expect six, so changing this
    /// requires the user's app to support it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(6, 8)]
    public int Digits { get; init; } = 6;

    /// <summary>
    /// How long one code stays valid, in seconds. Thirty is what authenticator apps assume, so a different value
    /// only works where the user's app can be told about it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(15, 120)]
    public int PeriodSeconds { get; init; } = 30;

    /// <summary>
    /// How long a secret offered by an enrolment stays confirmable, in minutes. Long enough to scan a code and wait
    /// out a time step, short enough that a code left open on a shared screen stops being redeemable. An enrolment that
    /// expires leaves any existing secret working, since it was never replaced.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, 60)]
    public int PendingSecretMinutes { get; init; } = 10;

    /// <summary>
    /// How long the challenge a sign-in hands back stays redeemable, in minutes. It is what bounds the code prompt: past
    /// it the user has to send their password again. Long enough to fetch a code from a phone, short enough that a
    /// half-finished sign-in left on a shared machine stops being completable.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Range(1, int.MaxValue)]
    public int ChallengeMinutes { get; init; } = 5;
}
