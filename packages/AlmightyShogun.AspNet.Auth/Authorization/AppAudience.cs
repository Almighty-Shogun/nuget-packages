using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Compares a token's audience against the application a request resolved to. The comparison lives here rather than on
/// the bearer events so the rule has one definition, since an audience that matches in one place and not another is the
/// failure this check exists to prevent.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class AppAudience
{
    /// <summary>
    /// Reports whether the principal carries the audience the request host resolved to, which is what stops a token minted
    /// for one application being replayed against another.
    /// </summary>
    ///
    /// <param name="principal">The caller the token produced, or <c>null</c> when validation yielded no principal.</param>
    /// <param name="app">The audience the request host resolved to, compared case-insensitively.</param>
    ///
    /// <returns>
    /// <c>true</c> when the principal carries that audience. A token may carry several, so one match is enough. A
    /// <c>null</c> principal never matches.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool Matches(ClaimsPrincipal? principal, string app) => principal is not null && principal.Claims
        .Where(IsAudienceClaim)
        .Any(claim => string.Equals(claim.Value, app, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Reports whether a claim is an audience, accepting both the short JWT name and the long identity URI form.
    /// </summary>
    ///
    /// <param name="claim">One claim from the principal.</param>
    ///
    /// <returns><c>true</c> when the claim type is a supported audience claim type; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool IsAudienceClaim(Claim claim)
        => claim.Type is JwtRegisteredClaimNames.Aud or "http://schemas.microsoft.com/identity/claims/audience";
}
