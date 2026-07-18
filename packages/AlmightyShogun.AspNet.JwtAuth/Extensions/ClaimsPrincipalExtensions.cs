using System.Security.Claims;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Provides helpers for reading package-specific authentication claims from the current principal.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        /// <summary>
        /// Gets the current authenticated user id from the <c>userId</c> claim.
        /// </summary>
        ///
        /// <exception cref="UnauthorizedAccessException">Thrown when the user id claim is missing or cannot be parsed as an integer.</exception>
        ///
        /// <returns>The parsed user id from the authenticated principal.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public int GetCurrentUserId()
        {
            string? value = principal.FindFirstValue("userId");

            return int.TryParse(value, out int userId) ? userId : throw new UnauthorizedAccessException("Missing userId claim");
        }
    }
}
