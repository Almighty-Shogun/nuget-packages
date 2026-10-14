using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Validates token audiences against resolved application audiences.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class AppAudience
{
    /// <summary>
    /// Checks whether a principal contains the specified application audience.
    /// </summary>
    /// <param name="principal">The claims principal to check.</param>
    /// <param name="app">The required application audience.</param>
    /// <returns>Whether the principal contains a matching audience.</returns>
    /// <remarks>Audience values are compared case-sensitively.</remarks>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool Matches(ClaimsPrincipal? principal, string app) => principal is not null && principal.Claims
        .Where(IsAudienceClaim)
        .Any(claim => string.Equals(claim.Value, app, StringComparison.Ordinal));

    /// <summary>
    /// Checks whether a claim represents a JWT audience.
    /// </summary>
    /// <param name="claim">The claim to check.</param>
    /// <returns>Whether the claim uses a supported audience claim type.</returns>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool IsAudienceClaim(Claim claim)
        => claim.Type is JwtRegisteredClaimNames.Aud or "http://schemas.microsoft.com/identity/claims/audience";
}
