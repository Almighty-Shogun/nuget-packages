namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when sign-in is refused because the account is locked after repeated failures. Distinct from wrong credentials
/// on purpose: the caller is told to wait rather than to try again.
/// </summary>
///
/// <param name="lockoutEnd">The moment the lockout expires.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class AccountLockedException(DateTimeOffset lockoutEnd) : Exception
{
    /// <summary>
    /// Gets when the lockout lifts, so a client can say how long to wait rather than only that the account is locked.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset LockoutEnd { get; } = lockoutEnd;
}
