namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when an account exists and the credentials are correct, but the account has been disabled.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class AccountDisabledException : Exception;
