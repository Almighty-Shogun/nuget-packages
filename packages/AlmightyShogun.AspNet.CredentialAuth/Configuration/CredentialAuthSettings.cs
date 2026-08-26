using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Configures credential authentication behavior. Bound from the optional <c>CredentialAuth</c> configuration section;
/// every value has a default, so the section may be absent entirely.
/// </summary>
///
/// <remarks>
/// These settings are separate from <c>Auth</c>, which belongs to <c>AlmightyShogun.AspNet.JwtAuth</c> and describes
/// token minting and validation. Credential concerns such as lockout live with the package that owns credentials.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record CredentialAuthSettings
{
    /// <summary>
    /// Gets the nested <c>Lockout</c> object. Off by default, because locking on failure count alone lets anyone deny
    /// service to a known account by failing logins against it deliberately.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public LockoutPolicy Lockout { get; init; } = new();

    /// <summary>
    /// Gets the nested <c>TwoFactor</c> section. Present with its defaults when the section is absent, because the
    /// feature is opt-in: nothing here takes effect until an application calls the enrolment methods.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public TwoFactorPolicy TwoFactor { get; init; } = new();

    /// <summary>
    /// Gets the maximum age a session may reach regardless of refreshing, in days. An explicit <c>null</c> means a
    /// session can be refreshed indefinitely; an absent key uses the default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, int.MaxValue)]
    public int? AbsoluteSessionLifetimeDays { get; init; } = 30;

    /// <summary>
    /// Gets how long a reset token stays redeemable, in minutes. Shorter is safer: the token travels by email, and the
    /// window is how long a leaked message stays useful to whoever finds it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, int.MaxValue)]
    public int PasswordResetMinutes { get; init; } = 60;
}
