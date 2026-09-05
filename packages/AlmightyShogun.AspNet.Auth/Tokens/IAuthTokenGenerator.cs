using System.Security.Claims;

namespace AlmightyShogun.AspNet.Auth;

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
    /// Mints a signed access token around the supplied claims, stamping the issuer and the expiry from configuration so a
    /// caller cannot mint one that outlives what the application allows. The audience is the caller's to choose and is
    /// never checked against the configured list.
    /// </summary>
    ///
    /// <param name="claims">
    /// The claims to embed, normally the caller's identity and permissions. The issuer, audience, and expiry are added
    /// here, so supplying them is unnecessary.
    /// </param>
    /// <param name="audience">
    /// The audience to issue for. When <c>null</c>, the current request host resolves it if host scoping is on, and the
    /// configured default app is used otherwise.
    /// </param>
    ///
    /// <returns>
    /// The encoded token and its absolute expiry, so a caller can hand one to the client and the other to a refresh
    /// schedule without decoding the token again.
    /// </returns>
    ///
    /// <exception cref="UnknownAppException">
    /// <paramref name="audience"/> is <c>null</c>, host scoping is on, and the request host maps to no configured
    /// application, which includes there being no request in flight at all.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    AuthToken Generate(IEnumerable<Claim> claims, string? audience = null);
}
