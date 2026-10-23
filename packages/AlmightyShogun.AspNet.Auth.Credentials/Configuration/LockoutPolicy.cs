using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Configures account lockout behavior for failed authentication attempts.
/// </summary>
///
/// <remarks>
/// Password and two-factor failures share the same counter.
/// Lockout is disabled by default.
/// </remarks>
/// 
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record LockoutPolicy
{
    /// <summary>
    /// Indicates whether account lockout is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool Enabled { get; init; } = false;

    /// <summary>
    /// The maximum consecutive failed attempts before an account is locked.
    /// </summary>
    ///
    /// <remarks>
    /// The counter resets after successful authentication, including
    /// completion of two-factor authentication when required.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int MaxFailedAttempts { get; init; } = 5;

    /// <summary>
    /// The account lockout duration in minutes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; init; } = 15;
}
