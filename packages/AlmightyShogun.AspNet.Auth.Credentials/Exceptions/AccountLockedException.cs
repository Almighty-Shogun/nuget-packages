namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when authentication is denied due to an account lockout.
/// </summary>
///
/// <param name="lockoutEnd">The lockout expiration time.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class AccountLockedException(DateTimeOffset lockoutEnd) : Exception
{
    /// <summary>
    /// The time when the account lockout expires.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset LockoutEnd { get; } = lockoutEnd;
}
