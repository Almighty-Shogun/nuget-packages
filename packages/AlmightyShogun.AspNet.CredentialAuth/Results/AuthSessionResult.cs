namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Represents the result of an authenticated session operation.
/// </summary>
///
/// <typeparam name="TUser">The authentication user entity type.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class AuthSessionResult<TUser> where TUser : AuthUser
{
    /// <summary>
    /// Gets the generated access token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Gets the generated refresh token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// Gets the authenticated user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required TUser User { get; init; }
}
