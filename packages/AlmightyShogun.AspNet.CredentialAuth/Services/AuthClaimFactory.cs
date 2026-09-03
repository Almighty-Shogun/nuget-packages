using System.Security.Claims;
using AlmightyShogun.AspNet.Auth;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Builds the claim set an access token carries for a user, so this package decides what a token says and
/// <see cref="IAuthTokenGenerator"/> decides only how it is signed.
/// </summary>
///
/// <remarks>
/// Kept here rather than in the JWT package because it reads <see cref="AuthUser"/>, which this package owns and which
/// the JWT package cannot see without a reference back.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class AuthClaimFactory
{
    /// <summary>
    /// Builds the claims for one user, narrowing the permission list to the application in scope and stripping the
    /// prefix that scoped it, so a token issued for one application never carries another's permissions.
    /// </summary>
    ///
    /// <param name="user">
    /// The user the token speaks for, read for its public identifier, username, role, and permission list.
    /// </param>
    /// <param name="app">
    /// The application the permissions are filtered against, matched case-insensitively against each permission's
    /// <c>app:</c> prefix. Pass <c>null</c> on a deployment that is not app-scoped, which carries every permission
    /// through unchanged, prefixes included.
    /// </param>
    ///
    /// <returns>
    /// The identifier, username, name identifier, and role claims, followed by one permission claim per permission the
    /// user holds for that application. A user holding none for it yields only the first four.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
