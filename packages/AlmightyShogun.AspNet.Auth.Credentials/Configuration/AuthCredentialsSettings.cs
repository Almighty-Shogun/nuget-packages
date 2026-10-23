using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Configures credential authentication settings with optional configuration overrides.
/// </summary>
///
/// <remarks>
/// Bound from the <c>AuthCredentials</c> configuration section.
/// All settings have defaults.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record AuthCredentialsSettings
{
    /// <summary>
    /// The account lockout settings, disabled by default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public LockoutPolicy Lockout { get; init; } = new();

    /// <summary>
    /// The two-factor authentication settings.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public TwoFactorPolicy TwoFactor { get; init; } = new();

    /// <summary>
    /// The maximum session lifetime in days, or null for no absolute limit.
    /// </summary>
    ///
    /// <remarks>
    /// Refreshing a session cannot extend it beyond this lifetime.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int? AbsoluteSessionLifetimeDays { get; init; } = 30;

    /// <summary>
    /// The password reset token lifetime in minutes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int PasswordResetMinutes { get; init; } = 60;

    /// <summary>
    /// The email verification token lifetime in minutes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Range(1, int.MaxValue)]
    public int EmailVerificationMinutes { get; init; } = 1440;

    /// <summary>
    /// The minimum forgot-password request duration in milliseconds.
    /// </summary>
    ///
    /// Helps reduce account enumeration through response timing.
    /// Configure a value above the normal duration of both request paths.
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int ForgotPasswordMinimumMilliseconds { get; init; } = 200;
}
