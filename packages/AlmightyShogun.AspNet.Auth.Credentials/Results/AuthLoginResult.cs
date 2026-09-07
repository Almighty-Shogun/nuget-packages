using System.Diagnostics.CodeAnalysis;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// What a correct password earned: either the session it opened, or the challenge a second factor has to be presented
/// against before one is opened. Exactly one of the two is populated, and <see cref="RequiresTwoFactor"/> says which.
/// </summary>
///
/// <typeparam name="TUser">
/// The application's user entity, so a caller reads its own properties off <see cref="User"/> without casting.
/// </typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public sealed class AuthLoginResult<TUser> where TUser : AuthUser
{
    /// <summary>
    /// The user whose password verified, loaded without its two-factor enrolment, so no secret rides along with it.
    /// Populated on both outcomes, so a caller can name the account it is about to challenge without querying again.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public required TUser User { get; init; }

    /// <summary>
    /// The tokens the sign-in issued, or <c>null</c> when a second factor is owed and nothing was issued at all.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public AuthSessionResult<TUser>? Session { get; init; }

    /// <summary>
    /// The challenge in plain text, to hand back to the client and send to
    /// <see cref="IAuthUserService{TUser}.CompleteTwoFactorLoginAsync"/> with the code. This is the only place it appears
    /// in that form; only its hash is stored. <c>null</c> when no second factor was owed.
    /// </summary>
    ///
    /// <remarks>
    /// It names a sign-in whose password already verified, so it is a credential in its own right and belongs in the
    /// response body rather than in a log or a URL.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public string? Challenge { get; init; }

    /// <summary>
    /// Whether the sign-in is unfinished, which is what the caller branches on. <c>true</c> means
    /// <see cref="Session"/> is <c>null</c> and no token was minted.
    /// </summary>
    ///
    /// <remarks>
    /// Both properties are annotated against this one, so branching on it narrows the branch's own property to non-null
    /// and a caller reads it without a null check or a null-forgiving operator.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [MemberNotNullWhen(false, nameof(Session))]
    [MemberNotNullWhen(true, nameof(Challenge))]
    public bool RequiresTwoFactor => Challenge is not null;
}
