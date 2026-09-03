namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when a password change supplies a confirmation that does not match the new password. Checked in the service rather than by the
/// request model, so an application posting its own shape gets the same guarantee.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class PasswordMismatchException : Exception;
