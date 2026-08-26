namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Thrown when a login fails. Deliberately the single exception for every credential failure, so a caller cannot tell
/// an unknown identifier from a wrong password.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class InvalidCredentialsException : Exception;
