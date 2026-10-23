namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when a refresh token is invalid, expired, revoked, or belongs to another application.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class InvalidSessionException : Exception;
