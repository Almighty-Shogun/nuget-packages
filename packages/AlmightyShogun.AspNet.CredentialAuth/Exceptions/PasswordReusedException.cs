namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Thrown when a password change supplies the password already in use. Refused so a forced rotation actually rotates, rather than appearing
/// to succeed while leaving the credential unchanged.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class PasswordReusedException : Exception;
