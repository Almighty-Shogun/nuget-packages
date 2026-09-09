namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when a credential flow or session refresh is refused because the account is locked after repeated failures.
/// Distinct from wrong credentials on purpose: the caller is told to wait rather than to try again.
/// </summary>
///
/// <param name="lockoutEnd">The moment the lockout expires.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class AccountLockedException(DateTimeOffset lockoutEnd) : Exception
{
    /// <summary>
    /// When the lockout lifts, so a client can say how long to wait rather than only that the account is locked.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset LockoutEnd { get; } = lockoutEnd;
}
