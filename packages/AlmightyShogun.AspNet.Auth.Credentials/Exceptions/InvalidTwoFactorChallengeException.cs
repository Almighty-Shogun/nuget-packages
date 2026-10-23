namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when a two-factor challenge is invalid, expired, used, or belongs to another application.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public sealed class InvalidTwoFactorChallengeException : Exception;
