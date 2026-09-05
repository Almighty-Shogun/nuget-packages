namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown while confirming an enrolment that has no offered secret or whose secret has expired, when the code presented
/// does not match that secret, and whenever a user turns out to have no enrolment row at all. Verifying a code at
/// sign-in returns <c>false</c> instead of throwing, replayed codes and spent recovery codes included.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class InvalidTwoFactorCodeException : Exception;
