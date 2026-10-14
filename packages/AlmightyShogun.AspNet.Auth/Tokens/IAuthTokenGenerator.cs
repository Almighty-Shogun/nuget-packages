using System.Security.Claims;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Provides JWT access token generation.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IAuthTokenGenerator
{
    /// <summary>
    /// Generates a signed JWT access token with the specified claims.
    /// </summary>
    /// <param name="claims">The claims to include in the token.</param>
    /// <param name="audience">
    /// The token audience. When null, resolves from the request host or configured default.
    /// </param>
    /// <returns>The generated token and its expiration.</returns>
    /// <exception cref="UnknownAppException">
    /// Thrown when the application cannot be resolved from the request host.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no audience can be determined.
    /// </exception>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    AuthToken Generate(IEnumerable<Claim> claims, string? audience = null);
}
