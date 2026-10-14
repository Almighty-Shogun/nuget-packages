using System.Security.Claims;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Provides extensions for retrieving user claims.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Provides claim retrieval extensions for principals.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(ClaimsPrincipal principal)
    {
        /// <summary>
        /// Retrieves the user's GUID from the user ID or name identifier claim.
        /// </summary>
        /// <returns>The user's identifier.</returns>
        /// <exception cref="MissingUserIdClaimException">
        /// Thrown when no valid user identifier is found.
        /// </exception>
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public Guid GetCurrentUserId() => principal.TryGetCurrentUserId() ?? throw new MissingUserIdClaimException();

        /// <summary>
        /// Attempts to retrieve the user's GUID from their claims.
        /// </summary>
        /// <returns>The user's identifier, or null if unavailable or invalid.</returns>
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public Guid? TryGetCurrentUserId()
        {
            string? value = principal.FindFirstValue(AuthClaimTypes.UserId) ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out Guid identifier) ? identifier : null;
        }
    }
}
