namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Thrown when a two-factor sign-in is completed with a challenge that cannot be redeemed, whether it is unknown,
/// already spent, past its expiry, or bought through a different application. Answering the same way for all four stops
/// the endpoint confirming which challenges once existed. A challenge claimed by another request between the lookup and
/// the update raises it too.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class InvalidTwoFactorChallengeException : Exception;
