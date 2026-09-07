namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Which flow a verification token was issued for, which is what stops one being redeemed on the other's endpoint. The
/// address on the row alone cannot say, since a change may be requested to the address the account already holds.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public enum EmailVerificationPurpose
{
    /// <summary>
    /// Confirms the address the account already holds, as after a sign-up. Zero, so a row written before this property
    /// existed reads as this one after the column is added.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    Registration = 0,

    /// <summary>
    /// Confirms an address the account has asked to move to, which redemption then writes onto the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    EmailChange = 1
}
