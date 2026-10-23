namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Contains the details required to configure two-factor authentication.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record AuthTwoFactorResult
{
    /// <summary>
    /// The Base32 secret used to configure an authenticator application.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Secret { get; init; }

    /// <summary>
    /// The otpauth URI used to generate an authenticator QR code.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Uri { get; init; }
}
