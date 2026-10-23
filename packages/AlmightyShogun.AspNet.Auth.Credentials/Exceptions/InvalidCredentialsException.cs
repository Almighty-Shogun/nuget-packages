namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when authentication credentials are invalid or a user cannot be found.
/// </summary>
///
/// <remarks>
/// Does not distinguish unknown users from incorrect passwords.
/// </remarks>
/// 
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class InvalidCredentialsException : Exception;
