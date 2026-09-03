namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when a two-factor code is not valid, has expired, or has already been used.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class InvalidTwoFactorCodeException : Exception;
