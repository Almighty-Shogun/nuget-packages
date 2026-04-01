using System.Security.Claims;

namespace AlmightyShogun.AspNet.JwtAuth.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        /// <summary>
        /// Gets the current authenticated user id from the <c>userId</c> claim.
        /// </summary>
        /// 
        /// <exception cref="UnauthorizedAccessException">Thrown when the user id claim is missing or invalid. </exception>
        ///
        /// <returns>The user ID of the user.</returns>
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
