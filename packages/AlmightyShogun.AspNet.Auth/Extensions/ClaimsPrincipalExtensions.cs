using System.Security.Claims;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Reads this package's claims off an authenticated principal, so endpoint and service code never spells a claim type or
/// parses one by hand.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Provides the claim readers on any principal, so they work the same on <c>HttpContext.User</c> and on one built
    /// by hand in a test.
    /// </summary>
    ///
    /// <param name="principal">The principal to read from, normally the authenticated caller of the current request.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(ClaimsPrincipal principal)
    {
        /// <summary>
        /// Reads the caller's public identifier from the <see cref="AuthClaimTypes.UserId"/> claim, falling back to the
        /// name identifier claim so a token minted by another issuer still resolves.
        /// </summary>
        ///
        /// <returns>The caller's public identifier, which is what every user-facing service accepts.</returns>
        ///
        /// <exception cref="MissingUserIdClaimException">
        /// Neither claim is present, or the value is not a well-formed identifier. Thrown rather than returning a default,
        /// so an unauthenticated request cannot be mistaken for one belonging to a real account. Use
        /// <see cref="TryGetCurrentUserId"/> where an anonymous caller is expected.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public Guid GetCurrentUserId() => principal.TryGetCurrentUserId() ?? throw new MissingUserIdClaimException();

        /// <summary>
        /// Reads the caller's public identifier without failing when there is none, for code that treats an anonymous
        /// caller as ordinary rather than exceptional, such as a handler that personalizes a public response.
        /// </summary>
        ///
        /// <returns>
        /// The caller's public identifier, or <c>null</c> when neither claim is present or the value is not a
        /// well-formed identifier. The two cases are not distinguished, because neither yields a usable caller.
        /// </returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public Guid? TryGetCurrentUserId()
        {
            string? value = principal.FindFirstValue(AuthClaimTypes.UserId) ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out Guid identifier) ? identifier : null;
        }
    }
}
