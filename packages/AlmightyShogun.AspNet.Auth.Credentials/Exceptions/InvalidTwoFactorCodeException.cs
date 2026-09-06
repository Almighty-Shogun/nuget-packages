namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown while confirming an enrolment that has no offered secret or whose secret has expired, when the code presented
/// does not match that secret, and whenever a user turns out to have no enrolment row at all. It is also what completing
/// a two-factor sign-in reports for a code the verification refused, replayed codes and spent recovery codes included,
/// though <see cref="IAuthTwoFactorService{TUser}.VerifyAsync"/> itself returns <c>false</c> for those rather than
/// throwing.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class InvalidTwoFactorCodeException : Exception;
