namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when two-factor enrollment is invalid or a verification code is rejected.
/// </summary>
///
/// <remarks>
/// Includes expired enrollment secrets, replayed codes, and used recovery codes.
/// </remarks>
/// 
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class InvalidTwoFactorCodeException : Exception;
