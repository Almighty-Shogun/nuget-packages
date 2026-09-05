namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when a login fails. Deliberately the single exception for every credential failure, so a caller cannot tell
/// an unknown identifier from a wrong password. Also thrown wherever a user lookup finds no row: changing a password,
/// completing a reset, refreshing a session, and every two-factor call raise it rather than reporting a missing user.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class InvalidCredentialsException : Exception;
