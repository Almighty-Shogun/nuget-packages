namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The details a user needs to add an account to an authenticator application. Returned once, when enrolment begins:
/// the secret is not readable afterwards, so a user who loses it must start again.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record AuthTwoFactorResult
{
    /// <summary>
    /// Gets the base32 shared secret, for an app that cannot scan a code. Show it only during enrolment, and never store
    /// it anywhere the user's account could be reached from.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Secret { get; init; }

    /// <summary>
    /// Gets the <c>otpauth://</c> URI to render as a QR code, carrying the same secret plus the configured issuer, digit
    /// count, and period, so the app agrees with the server about how codes are generated.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Uri { get; init; }
}
