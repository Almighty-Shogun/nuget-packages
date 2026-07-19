using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Provides helpers for reading package-specific authentication claims from the current principal.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Provides claim-reading extension methods for the target claims principal.
    /// </summary>
    ///
    /// <param name="principal">The claims principal that supplies authentication claims.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(ClaimsPrincipal principal)
    {
        /// <summary>
        /// Gets the current authenticated user id from the <c>userId</c> or name identifier claim.
        /// </summary>
        ///
        /// <exception cref="HttpErrorException">Thrown with status code <c>401</c> when the user id claim is missing or cannot be parsed as an integer.</exception>
        ///
        /// <returns>The parsed user id from the authenticated principal.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public int GetCurrentUserId()
        {
            string? value = principal.FindFirstValue("userId")
                ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out int userId)
                ? userId : throw new HttpErrorException(StatusCodes.Status401Unauthorized);
        }
    }
}
