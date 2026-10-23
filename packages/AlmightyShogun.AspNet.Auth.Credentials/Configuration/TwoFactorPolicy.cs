using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Configures two-factor authentication behavior.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record TwoFactorPolicy
{
    /// <summary>
    /// The issuer displayed in authenticator apps, overriding the enrollment issuer if set.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Issuer { get; init; }

    /// <summary>
    /// The number of single-use recovery codes generated during enrollment.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, 50)]
    public int RecoveryCodeCount { get; init; } = 10;

    /// <summary>
    /// The number of digits in generated TOTP codes.
    /// </summary>
    ///
    /// <remarks>
    /// Authenticator apps may not support values other than six.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(6, 8)]
    public int Digits { get; init; } = 6;

    /// <summary>
    /// The TOTP time step duration in seconds.
    /// </summary>
    ///
    /// <remarks>
    /// Authenticator apps may not support periods other than 30 seconds.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(15, 120)]
    public int PeriodSeconds { get; init; } = 30;

    /// <summary>
    /// The lifetime of a pending enrollment secret in minutes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, 60)]
    public int PendingSecretMinutes { get; init; } = 10;

    /// <summary>
    /// The lifetime of a two-factor login challenge in minutes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Range(1, int.MaxValue)]
    public int ChallengeMinutes { get; init; } = 5;
}
