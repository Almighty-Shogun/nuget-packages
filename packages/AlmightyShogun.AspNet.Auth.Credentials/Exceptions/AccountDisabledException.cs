namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when the account behind a request is deactivated: at sign-in once the password has verified, and at every
/// refresh of a session that was opened before the account was disabled, where no credentials are presented at all.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class AccountDisabledException : Exception;
