using System.Security.Claims;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Builds user identity and permission claims for access tokens.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class AuthClaimFactory
{
    /// <summary>
    /// Creates identity and application-scoped permission claims for a user.
    /// </summary>
    ///
    /// <param name="user">The user whose claims are generated.</param>
    /// <param name="app">
    /// The application used to filter permissions, or null to include all permissions.
    /// </param>
    ///
    /// <returns>The user's identity, role, and applicable permission claims.</returns>
    /// 
    /// <remarks>
    /// Application-scoped permissions are matched case-insensitively and
    /// have their application prefix removed. Without an application scope,
    /// all permissions are included unchanged.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static List<Claim> Create(AuthUser user, string? app)
    {
        List<Claim> claims =
        [
            new(AuthClaimTypes.UserId, user.Identifier.ToString()),
            new("username", user.Username),
            new(ClaimTypes.NameIdentifier, user.Identifier.ToString()),
            new(ClaimTypes.Role, user.Role)
        ];

        IEnumerable<string> permissions = app is null
            ? user.Permissions
            : user.Permissions
                .Where(permission => permission.StartsWith($"{app}:", StringComparison.OrdinalIgnoreCase))
                .Select(permission => permission[(app.Length + 1)..]);

        claims.AddRange(permissions.Select(permission => new Claim(AuthClaimTypes.Permission, permission)));

        return claims;
    }
}
