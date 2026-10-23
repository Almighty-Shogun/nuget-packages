namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Defines the purpose of an email verification token.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public enum EmailVerificationPurpose
{
    /// <summary>
    /// Verifies the account's existing email address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Registration = 0,

    /// <summary>
    /// Verifies a requested new email address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    EmailChange = 1
}
