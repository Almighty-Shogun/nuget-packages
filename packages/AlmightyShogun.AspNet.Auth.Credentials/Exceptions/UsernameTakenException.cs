namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when a registration or user creation supplies a username that already exists. The uniqueness index would refuse it anyway; this
/// reports it as a client mistake instead of a database failure.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class UsernameTakenException : Exception;
