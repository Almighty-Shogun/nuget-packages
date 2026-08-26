namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Thrown when a refresh presents a token that matches no usable session, whether it is unknown, expired, revoked, or
/// scoped to a different application. The cases are not distinguished, so the response cannot be probed.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class InvalidSessionException : Exception;
