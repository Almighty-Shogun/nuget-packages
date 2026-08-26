namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Thrown when a registration or user creation supplies an email address that already exists. The uniqueness index would refuse it anyway;
/// this reports it as a client mistake instead of a database failure.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class EmailTakenException : Exception;
