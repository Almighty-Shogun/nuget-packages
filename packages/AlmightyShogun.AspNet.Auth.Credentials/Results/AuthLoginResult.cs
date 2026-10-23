using System.Diagnostics.CodeAnalysis;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a successful login or a pending two-factor challenge.
/// </summary>
///
/// <typeparam name="TUser">The application's user entity. </typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public sealed class AuthLoginResult<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The authenticated user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public required TUser User { get; init; }

    /// <summary>
    /// The issued session, or null when two-factor authentication is required.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public AuthSessionResult<TUser>? Session { get; init; }

    /// <summary>
    /// The two-factor challenge token, or null when no challenge is required.
    /// </summary>
    ///
    /// <remarks>
    /// Treat the challenge as a credential. Do not expose it in logs or URLs.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public string? Challenge { get; init; }

    /// <summary>
    /// Indicates whether two-factor authentication must be completed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [MemberNotNullWhen(false, nameof(Session))]
    [MemberNotNullWhen(true, nameof(Challenge))]
    public bool RequiresTwoFactor => Challenge is not null;
}
