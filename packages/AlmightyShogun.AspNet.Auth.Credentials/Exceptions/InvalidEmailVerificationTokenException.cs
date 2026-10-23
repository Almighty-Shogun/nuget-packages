namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when an email verification token is invalid, expired, used, or no longer applicable.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public sealed class InvalidEmailVerificationTokenException : Exception;
