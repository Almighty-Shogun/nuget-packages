using System.Security.Claims;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Mints signed access tokens using the configured issuer, secret, and lifetime. The caller supplies the claims, so the
/// user model stays out of this package.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IAuthTokenGenerator
{
    /// <summary>
    /// Mints a signed access token around the supplied claims, stamping the issuer, audience, and expiry from configuration
    /// so a caller cannot mint one that outlives what the application allows.
    /// </summary>
    ///
    /// <param name="claims">
    /// The claims to embed, normally the caller's identity and permissions. The issuer, audience, and expiry are added
    /// here, so supplying them is unnecessary.
    /// </param>
    /// <param name="audience">The audience to issue for, or <c>null</c> to resolve it from the current request host.</param>
    ///
    /// <returns>
    /// The encoded token and its absolute expiry, so a caller can hand one to the client and the other to a refresh
    /// schedule without decoding the token again.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    AuthToken Generate(IEnumerable<Claim> claims, string? audience = null);
}
